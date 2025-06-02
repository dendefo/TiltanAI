using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/SensorySystemSpottedEnemy")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "SensorySystemSpottedEnemy", message: "[Self] has spotted [Enemy] by [SensorySystem]", category: "Events", id: "15cf7dd2dc98fc3cd5d694097a4b5cfa")]
public partial class SensorySystemSpottedEnemy : EventChannelBase
{
    public delegate void NewEventChannelEventHandler(GameObject Self,GameObject Enemy,SensorySystem sensors);
    public event NewEventChannelEventHandler Event;
    public BlackboardVariable<GameObject> Enemy;

    public void SendEventMessage(GameObject Self,SensorySystem sensors)
    {
        Event?.Invoke(Self, sensors.GetStrongestStimulus().Source,sensors);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<GameObject> SelfBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
        var Self = SelfBlackboardVariable != null ? SelfBlackboardVariable.Value : default(GameObject);
        var sensors = messageData[2] as BlackboardVariable<SensorySystem>;


        Event?.Invoke(Self,sensors.Value.GetStrongestStimulus().Source, sensors.Value);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        NewEventChannelEventHandler del = (Self,_enemy,sensors) =>
        {
            BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
            if(var0 != null)
                var0.Value = Self;

            BlackboardVariable<GameObject> var1 = vars[1] as BlackboardVariable<GameObject>;
            if (var1 != null)
                var1.Value = _enemy;
            BlackboardVariable<SensorySystem> var2 = vars[2] as BlackboardVariable<SensorySystem>;
            if (var2 != null)
                var2.Value = sensors;
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as NewEventChannelEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as NewEventChannelEventHandler;
    }
}

