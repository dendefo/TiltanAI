// File: Library/Runtime/Execution/Nodes/Actions/Navigation/Navigate To Target (Agent) - moved into project scripts folder
using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Navigate To Target (Agent)",
        description: "Navigates a GameObject towards another GameObject using custom Agent navigation.",
        story: "[Agent] navigates to [Target] using Agent.cs",
        category: "Action/Navigation",
        id: "b7e1c2d4e8f94b8e9a1c2d4e8f94b8e9")]
    internal partial class NavigateToTargetAgentAction : Action
    {
        public enum TargetPositionMode
        {
            ClosestPointOnAnyCollider,
            ClosestPointOnTargetCollider,
            ExactTargetPosition
        }

        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
        [SerializeReference] public BlackboardVariable<float> SlowDownDistance = new BlackboardVariable<float>(1.0f);
        [SerializeReference] public BlackboardVariable<TargetPositionMode> m_TargetPositionMode = new(TargetPositionMode.ClosestPointOnAnyCollider);

        private global::Agent m_AgentComponent;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_ColliderAdjustedTargetPosition;
        private float m_ColliderOffset;

        protected override Status OnStart()
        {
            if (Agent.Value == null || Target.Value == null)
                return Status.Failure;

            return Initialize();
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null || Target.Value == null)
                return Status.Failure;

            bool updateTarget = !Mathf.Approximately(m_LastTargetPosition.x, Target.Value.transform.position.x)
                || !Mathf.Approximately(m_LastTargetPosition.y, Target.Value.transform.position.y)
                || !Mathf.Approximately(m_LastTargetPosition.z, Target.Value.transform.position.z);

            if (updateTarget)
            {
                m_LastTargetPosition = Target.Value.transform.position;
                m_ColliderAdjustedTargetPosition = GetPositionColliderAdjusted();
                m_AgentComponent.SetDestination(m_ColliderAdjustedTargetPosition, DistanceThreshold.Value + m_ColliderOffset);
            }

            if (m_AgentComponent.IsNavigationComplete())
                return Status.Success;

            // Keep controller speed aligned to desired Speed
            var controller = Agent.Value.GetComponent<AgentMovementController>();
            if (controller != null)
            {
                controller.maxSpeed = Speed.Value;
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (m_AgentComponent != null)
                m_AgentComponent.Stop();

            m_AgentComponent = null;
        }

        protected override void OnDeserialize()
        {
            Initialize();
        }

        private Status Initialize()
        {
            m_LastTargetPosition = Target.Value.transform.position;
            m_ColliderAdjustedTargetPosition = GetPositionColliderAdjusted();

            m_ColliderOffset = 0.0f;
            Collider agentCollider = Agent.Value.GetComponentInChildren<Collider>();
            if (agentCollider != null)
            {
                Vector3 colliderExtents = agentCollider.bounds.extents;
                m_ColliderOffset += Mathf.Max(colliderExtents.x, colliderExtents.z);
            }

            m_AgentComponent = Agent.Value.GetComponent<global::Agent>();
            if (m_AgentComponent == null)
                m_AgentComponent = Agent.Value.AddComponent<global::Agent>();

            // Align controller speed to action Speed (similar to NavMeshAgent.speed)
            var controller = Agent.Value.GetComponent<AgentMovementController>();
            if (controller != null)
            {
                controller.maxSpeed = Speed.Value;
            }

            m_AgentComponent.SetDestination(m_ColliderAdjustedTargetPosition, DistanceThreshold.Value + m_ColliderOffset);

            if (GetDistanceXZ() <= (DistanceThreshold.Value + m_ColliderOffset))
                return Status.Success;

            return Status.Running;
        }

        private Vector3 GetPositionColliderAdjusted()
        {
            switch (m_TargetPositionMode.Value)
            {
                case TargetPositionMode.ClosestPointOnAnyCollider:
                    Collider anyCollider = Target.Value.GetComponentInChildren<Collider>(includeInactive: false);
                    if (anyCollider == null || !anyCollider.enabled)
                        break;
                    return anyCollider.ClosestPoint(Agent.Value.transform.position);
                case TargetPositionMode.ClosestPointOnTargetCollider:
                    Collider targetCollider = Target.Value.GetComponent<Collider>();
                    if (targetCollider == null || !targetCollider.enabled)
                        break;
                    return targetCollider.ClosestPoint(Agent.Value.transform.position);
            }
            return Target.Value.transform.position;
        }

        private float GetDistanceXZ()
        {
            Vector3 agentPosition = new Vector3(Agent.Value.transform.position.x, m_ColliderAdjustedTargetPosition.y, Agent.Value.transform.position.z);
            return Vector3.Distance(agentPosition, m_ColliderAdjustedTargetPosition);
        }
    }
}
