using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Navigate To Location (Agent)",
        description: "Navigates a GameObject to a specified position using custom Agent navigation (AgentMovementController). " +
                     "Falls back to transform movement if Agent component is not present.",
        story: "[Agent] navigates to [Location] using Agent.cs",
        category: "Action/Navigation",
        id: "f6b2b1a7b0d34fd3b1e3c9b8a1c5b0a2")]
    internal partial class NavigateToLocationAgentAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<Vector3> Location;
        [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
        [SerializeReference] public BlackboardVariable<string> AnimatorSpeedParam = new BlackboardVariable<string>("SpeedMagnitude");

        // Used only for transform-based fallback movement (no Agent component).
        [SerializeReference] public BlackboardVariable<float> SlowDownDistance = new BlackboardVariable<float>(1.0f);

        private Animator m_Animator;
        private global::Agent m_AgentComponent;

        protected override Status OnStart()
        {
            if (Agent.Value == null)
            {
                return Status.Failure;
            }

            return Initialize();
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null)
            {
                return Status.Failure;
            }

            if (m_AgentComponent == null)
            {
                // Transform-based fallback (no Agent component available)
                Vector3 agentPosition, locationPosition;
                float distance = GetDistanceToLocation(out agentPosition, out locationPosition);
                if (distance <= DistanceThreshold.Value)
                {
                    return Status.Success;
                }

                float speed = Speed.Value;

                if (SlowDownDistance.Value > 0.0f && distance < SlowDownDistance.Value)
                {
                    float ratio = distance / SlowDownDistance.Value;
                    speed = Mathf.Max(0.1f, Speed.Value * ratio);
                }

                Vector3 toDestination = locationPosition - agentPosition;
                toDestination.y = 0.0f;
                toDestination.Normalize();
                agentPosition += toDestination * (speed * Time.deltaTime);
                Agent.Value.transform.position = agentPosition;

                // Look at the target.
                Agent.Value.transform.forward = toDestination;
            }
            else
            {
                // Keep controller speed aligned with desired Speed
                var controller = Agent.Value.GetComponent<AgentMovementController>();
                if (controller != null)
                {
                    controller.maxSpeed = Speed.Value;
                }

                if (m_AgentComponent.IsNavigationComplete())
                {
                    return Status.Success;
                }

                // Optional: drive animator while moving
                if (m_Animator != null)
                {
                    m_Animator.SetFloat(AnimatorSpeedParam, Speed.Value);
                }
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (m_Animator != null)
            {
                m_Animator.SetFloat(AnimatorSpeedParam, 0);
            }

            if (m_AgentComponent != null)
            {
                m_AgentComponent.Stop();
            }

            m_AgentComponent = null;
            m_Animator = null;
        }

        protected override void OnDeserialize()
        {
            Initialize();
        }

        private Status Initialize()
        {
            if (GetDistanceToLocation(out Vector3 agentPosition, out Vector3 locationPosition) <= DistanceThreshold.Value)
            {
                return Status.Success;
            }

            // Animator setup
            m_Animator = Agent.Value.GetComponentInChildren<Animator>();
            if (m_Animator != null)
            {
                m_Animator.SetFloat(AnimatorSpeedParam, Speed.Value);
            }

            // Use custom Agent navigation if available
            m_AgentComponent = Agent.Value.GetComponent<global::Agent>();
            if (m_AgentComponent != null)
            {
                // Align controller speed similar to NavMeshAgent.speed
                var controller = Agent.Value.GetComponent<AgentMovementController>();
                if (controller != null)
                {
                    controller.maxSpeed = Speed.Value;
                }

                m_AgentComponent.SetDestination(locationPosition, DistanceThreshold.Value);
            }

            return Status.Running;
        }

        private float GetDistanceToLocation(out Vector3 agentPosition, out Vector3 locationPosition)
        {
            agentPosition = Agent.Value.transform.position;
            locationPosition = Location.Value;
            return Vector3.Distance(new Vector3(agentPosition.x, locationPosition.y, agentPosition.z), locationPosition);
        }
    }
}