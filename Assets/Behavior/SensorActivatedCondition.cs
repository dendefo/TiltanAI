using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Sensor Activated", story: "[Sensor] stimultiList Count is [number]", category: "Sensor condition", id: "28a1f6a80762bc681b7102ca8b1bdb73")]
public partial class SensorActivatedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<SensorySystem> Sensor;
    [Comparison(comparisonType: ComparisonType.Boolean)]
    [SerializeReference] public BlackboardVariable<int> Number;

    public override bool IsTrue()
    {
        if (!Sensor.Value) return false;
        var stimuli = Sensor.Value.GetAllStimuli();
        if (stimuli == null) return false;
        if (stimuli.Count >=Number.Value) return true;
        if (stimuli.Count < Number.Value) return false;
        // If the sensor has detected stimuli and the count is greater than or equal to the specified number
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
