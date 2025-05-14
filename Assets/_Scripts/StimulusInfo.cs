using UnityEngine;

public class StimulusInfo
{
    public GameObject Source { get; set; }
    public StimulusType Type { get; set; }
    public float Intensity { get; set; }
    public Vector3 Location { get; set; }
    public float TimeDetected { get; set; }

    public StimulusInfo(GameObject source, StimulusType type, float intensity, Vector3 location)
    {
        Source = source;
        Type = type;
        Intensity = intensity;
        Location = location;
        TimeDetected = Time.time;
    }
}