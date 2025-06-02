using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SensorySystem : MonoBehaviour
{
    private List<BaseSensor> sensors = new List<BaseSensor>();

    private void Awake()
    {
        // Automatically gather all sensors attached to this GameObject
        sensors.AddRange(GetComponents<BaseSensor>());
    }

    public List<StimulusInfo> GetAllStimuli()
    {
        List<StimulusInfo> allStimuli = new List<StimulusInfo>();
        foreach (var sensor in sensors)
        {
            allStimuli.AddRange(sensor.GetCurrentStimuli());
        }
        return allStimuli;
    }
    public StimulusInfo GetStrongestStimulus()
    {
        return GetAllStimuli()
            .OrderByDescending(stimulus => stimulus.Intensity)
            .FirstOrDefault();
    }

    public List<StimulusInfo> GetStimuliByType(StimulusType type)
    {
        return GetAllStimuli().FindAll(stimulus => stimulus.Type == type);
    }

    public StimulusInfo GetStrongestStimulus(StimulusType type)
    {
        return GetStimuliByType(type)
            .OrderByDescending(stimulus => stimulus.Intensity)
            .FirstOrDefault();
    }
}