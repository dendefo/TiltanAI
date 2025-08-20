using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Patrol (Agent)",
        description: "Moves a GameObject along waypoints using custom Agent navigation (AgentMovementController).",
        category: "Action/Navigation",
        story: "[Agent] patrols along [Waypoints] using Agent.cs",
        id: "c8b8f9b2a7a9448a9f1dc9c41f4f7b9a")]
    internal partial class PatrolAgentAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;
        [SerializeReference] public BlackboardVariable<float> Speed;
        [SerializeReference] public BlackboardVariable<float> WaypointWaitTime = new BlackboardVariable<float>(1.0f);
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
        [SerializeReference] public BlackboardVariable<string> AnimatorSpeedParam = new BlackboardVariable<string>("SpeedMagnitude");
        [Tooltip("Should patrol restart from the latest point?")]
        [SerializeReference] public BlackboardVariable<bool> PreserveLatestPatrolPoint = new(false);

        private Agent m_AgentComponent;
        private Animator m_Animator;

        [CreateProperty]
        private Vector3 m_CurrentTarget;
        [CreateProperty]
        private int m_CurrentPatrolPoint = 0;
        [CreateProperty]
        private bool m_Waiting;
        [CreateProperty]
        private float m_WaypointWaitTimer;

        protected override Status OnStart()
        {
            if (Agent.Value == null)
            {
                LogFailure("No agent assigned.");
                return Status.Failure;
            }

            if (Waypoints.Value == null || Waypoints.Value.Count == 0)
            {
                LogFailure("No waypoints to patrol assigned.");
                return Status.Failure;
            }

            Initialize();

            m_Waiting = false;
            m_WaypointWaitTimer = 0.0f;

            MoveToNextWaypoint();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null || Waypoints.Value == null)
            {
                return Status.Failure;
            }

            if (m_Waiting)
            {
                if (m_WaypointWaitTimer > 0.0f)
                {
                    m_WaypointWaitTimer -= Time.deltaTime;
                }
                else
                {
                    m_WaypointWaitTimer = 0f;
                    m_Waiting = false;
                    MoveToNextWaypoint();
                }
            }
            else
            {
                float distance = GetDistanceToWaypoint();

                if (m_Animator != null)
                {
                    // For custom agent we don't have runtime velocity; keep animator speed aligned to desired Speed.
                    m_Animator.SetFloat(AnimatorSpeedParam, Speed.Value);
                }

                if (distance <= DistanceThreshold.Value || (m_AgentComponent != null && m_AgentComponent.IsNavigationComplete()))
                {
                    if (m_Animator != null)
                    {
                        m_Animator.SetFloat(AnimatorSpeedParam, 0);
                    }

                    m_WaypointWaitTimer = WaypointWaitTime.Value;
                    m_Waiting = true;
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
        }

        protected override void OnDeserialize()
        {
            Initialize();
        }

        private void Initialize()
        {
            m_Animator = Agent.Value.GetComponentInChildren<Animator>();
            if (m_Animator != null)
            {
                m_Animator.SetFloat(AnimatorSpeedParam, 0);
            }

            m_AgentComponent = Agent.Value.GetComponent<global::Agent>();
            if (m_AgentComponent == null)
            {
                Debug.LogError("Agent component not found on GameObject.");
            }

            // Align controller speed to action Speed (similar to NavMeshAgent.speed)
            var controller = Agent.Value.GetComponent<AgentMovementController>();
            if (controller != null)
            {
                controller.maxSpeed = Speed.Value;
            }

            m_CurrentPatrolPoint = PreserveLatestPatrolPoint.Value ? m_CurrentPatrolPoint - 1 : -1;
        }

        private float GetDistanceToWaypoint()
        {
            Vector3 targetPosition = m_CurrentTarget;
            Vector3 agentPosition = Agent.Value.transform.position;
            agentPosition.y = targetPosition.y; // Ignore y for distance check.
            return Vector3.Distance(agentPosition, targetPosition);
        }

        private void MoveToNextWaypoint()
        {
            m_CurrentPatrolPoint = (m_CurrentPatrolPoint + 1) % Waypoints.Value.Count;

            m_CurrentTarget = Waypoints.Value[m_CurrentPatrolPoint].transform.position;

            if (m_AgentComponent != null)
            {
                m_AgentComponent.SetDestination(m_CurrentTarget, DistanceThreshold.Value);
            }

            if (m_Animator != null)
            {
                // Set animator speed when starting movement towards a waypoint
                m_Animator.SetFloat(AnimatorSpeedParam, Speed.Value);
            }
        }
    }
}