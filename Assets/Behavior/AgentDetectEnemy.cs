using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Agent Detect Enemy")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Agent Detect Enemy", message: "[Agent] has spotted [Enemy]", category: "Events", id: "ce3aaf9b135ac2e7f2dd1d6273756cd4")]
public partial class AgentDetectEnemy : EventChannelBase
{
    public delegate void AgentDetectEnemyEventHandler(GameObject Agent, GameObject Enemy);
    public event AgentDetectEnemyEventHandler Event; 

    public void SendEventMessage(GameObject Agent, GameObject Enemy)
    {
        Event?.Invoke(Agent, Enemy);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<GameObject> AgentBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
        var Agent = AgentBlackboardVariable != null ? AgentBlackboardVariable.Value : default(GameObject);

        BlackboardVariable<GameObject> EnemyBlackboardVariable = messageData[1] as BlackboardVariable<GameObject>;
        var Enemy = EnemyBlackboardVariable != null ? EnemyBlackboardVariable.Value : default(GameObject);

        Event?.Invoke(Agent, Enemy);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        AgentDetectEnemyEventHandler del = (Agent, Enemy) =>
        {
            BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
            if(var0 != null)
                var0.Value = Agent;

            BlackboardVariable<GameObject> var1 = vars[1] as BlackboardVariable<GameObject>;
            if(var1 != null)
                var1.Value = Enemy;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as AgentDetectEnemyEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as AgentDetectEnemyEventHandler;
    }
}

