using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Fleing (Agent)",
    story: "[Self] running away from [Enemy] using Agent.cs",
    category: "Action",
    id: "b2d2b2f6c3e54c5a9f5a1e2a0d7f3e11")]
public partial class FleingAgentAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;

    // Initial flee distance
    [SerializeReference] public BlackboardVariable<float> FleeDistance = new BlackboardVariable<float>(10f);
    // Completion radius to pass into Agent/AgentMovementController
    [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);

    // Retarget control: correct the flee destination occasionally, not every frame
    [SerializeReference] public BlackboardVariable<float> RepathInterval = new BlackboardVariable<float>(0.25f);
    [SerializeReference] public BlackboardVariable<float> MinRetargetDistance = new BlackboardVariable<float>(0.75f);
    [SerializeReference] public BlackboardVariable<float> MinDirectionChangeDegrees = new BlackboardVariable<float>(10f);

    private global::Agent m_AgentComponent;

    private Vector3 m_LastFleeTarget;
    private Vector3 m_LastAwayDir;
    private float m_NextRepathTime;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            LogFailure("Self GameObject is not set.");
            return Status.Failure;
        }

        m_AgentComponent = Self.Value.GetComponent<global::Agent>();
        if (m_AgentComponent == null)
        {
            LogFailure("Agent component is missing on the Self GameObject.");
            return Status.Failure;
        }

        if (Enemy.Value == null)
        {
            // Nothing to flee from
            return Status.Success;
        }

        // Compute initial flee destination and set it once
        Vector3 selfPos = Self.Value.transform.position;
        Vector3 away = ComputeAwayDir(selfPos, Enemy.Value.transform.position);
        Vector3 fleeTarget = selfPos + away * Mathf.Max(0.1f, FleeDistance.Value);

        m_LastAwayDir = away;
        m_LastFleeTarget = fleeTarget;
        m_AgentComponent.SetDestination(m_LastFleeTarget, Mathf.Max(0f, DistanceThreshold.Value));

        // schedule next correction
        m_NextRepathTime = Time.time + Mathf.Max(0.02f, RepathInterval.Value);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // Finish condition 1: enemy no longer perceived
        if (Enemy.Value == null)
            return Status.Success;

        if (Self.Value == null || m_AgentComponent == null)
        {
            LogFailure("Self or Agent component missing.");
            return Status.Failure;
        }

        // Finish condition 2: reached the current flee target within threshold
        if (m_AgentComponent.IsNavigationComplete())
            return Status.Success;

        // Occasional correction: only when timer elapses AND target changed enough
        if (Time.time >= m_NextRepathTime)
        {
            Vector3 selfPos = Self.Value.transform.position;
            Vector3 awayNow = ComputeAwayDir(selfPos, Enemy.Value.transform.position);
            Vector3 newFleeTarget = selfPos + awayNow * Mathf.Max(0.1f, FleeDistance.Value);

            float distDelta = Vector3.Distance(m_LastFleeTarget, newFleeTarget);
            float angleDelta = Vector3.Angle(m_LastAwayDir, awayNow);

            if (distDelta >= MinRetargetDistance.Value || angleDelta >= MinDirectionChangeDegrees.Value)
            {
                m_LastAwayDir = awayNow;
                m_LastFleeTarget = newFleeTarget;
                m_AgentComponent.SetDestination(m_LastFleeTarget, Mathf.Max(0f, DistanceThreshold.Value));
            }

            m_NextRepathTime = Time.time + Mathf.Max(0.02f, RepathInterval.Value);
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (m_AgentComponent != null)
            m_AgentComponent.Stop();

        m_AgentComponent = null;
    }

    private static Vector3 ComputeAwayDir(Vector3 selfPos, Vector3 enemyPos)
    {
        Vector3 away = selfPos - enemyPos;
        away.y = 0f;
        if (away.sqrMagnitude < 0.0001f)
            away = Vector3.forward; // fallback if overlapping
        return away.normalized;
    }
}