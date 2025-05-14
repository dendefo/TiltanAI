using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/NewEventChannel")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "NewEventChannel", message: "[Self] has spotted Enemy", category: "Events", id: "15cf7dd2dc98fc3cd5d694097a4b5cfa")]
public partial class NewEventChannel : EventChannelBase
{
    public delegate void NewEventChannelEventHandler(GameObject Self);
    public event NewEventChannelEventHandler Event; 

    public void SendEventMessage(GameObject Self)
    {
        Event?.Invoke(Self);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<GameObject> SelfBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
        var Self = SelfBlackboardVariable != null ? SelfBlackboardVariable.Value : default(GameObject);

        Event?.Invoke(Self);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        NewEventChannelEventHandler del = (Self) =>
        {
            BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
            if(var0 != null)
                var0.Value = Self;

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

