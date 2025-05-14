using UnityEngine;

public class AudioSensor : BaseSensor
{
    [Header("Audio Settings")]
    [SerializeField] private float hearingRadius = 15f;
    [SerializeField] private float minAudioIntensity = 0.1f;
    [SerializeField] private bool useDirectionalHearing = true;
    [SerializeField] private float directionalityFactor = 0.5f;

    protected override void Sense()
    {
        DetectedStimuli.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, hearingRadius, detectionLayers);

        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject == gameObject) continue;

            // Check for audio source component
            AudioSource audioSource = hit.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float baseIntensity = 1f - (distance / hearingRadius);

                if (useDirectionalHearing)
                {
                    Vector3 directionToSound = (hit.transform.position - transform.position).normalized;
                    float directionFactor = Vector3.Dot(transform.forward, directionToSound);
                    baseIntensity *= Mathf.Lerp(1f, directionFactor, directionalityFactor);
                }

                if (baseIntensity >= minAudioIntensity)
                {
                    DetectedStimuli.Add(new StimulusInfo(
                        hit.gameObject,
                        StimulusType.Audio,
                        baseIntensity,
                        hit.transform.position
                    ));
                }
            }
        }
    }

    protected override void DrawDebugVisualization()
    {
        Gizmos.color = debugColor;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);

        if (useDirectionalHearing)
        {
            // Draw directional indicator
            Vector3 forward = transform.forward * hearingRadius * 0.5f;
            Gizmos.DrawRay(transform.position, forward);
        }
    }
}