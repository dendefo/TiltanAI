using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/AllClear")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "AllClear", message: "All Clear", category: "Events", id: "dabd8282e082bf77045fa4060dfde40f")]
public partial class AllClear : EventChannelBase
{
    public delegate void AllClearEventHandler();
    public event AllClearEventHandler Event; 

    public void SendEventMessage()
    {
        Event?.Invoke();
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        Event?.Invoke();
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        AllClearEventHandler del = () =>
        {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as AllClearEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as AllClearEventHandler;
    }
}

