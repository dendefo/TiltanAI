using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckForEnemies", story: "[SensorySystem] checks for [Target] at [Position] with [Flag]", category: "Action", id: "9bfddf4e3b20a7b3dd2a20880b0c9f4b")]
public partial class CheckForEnemiesAction : Action
{
    [SerializeReference] public BlackboardVariable<SensorySystem> SensorySystem;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<Vector3> Position;
    [SerializeReference] public BlackboardVariable<bool> Flag;

    protected override Status OnStart()
    {
        Target.Value = null;
        if (SensorySystem.Value == null)
        {
            Debug.Log("SensorySystem is not set.");
            return Status.Failure;
        }
        return Status.Running;

    }
    protected override Status OnUpdate()
    {
        if (SensorySystem.Value.GetAllStimuli().Count == 0)
        {
            Target.Value = null;
            Debug.Log("No stimuli detected by the SensorySystem.");
        }
        else
        {
            var target = SensorySystem.Value.GetStrongestStimulus();
            if (target != null)
            {
                Target.Value = target.Source;
                Position.Value = target.Source.transform.position;
                Flag.Value = true;
                Debug.Log($"Strongest stimulus detected: {target.Source.name} with intensity {target.Intensity}");
            }
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {

    }
}

