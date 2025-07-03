using UnityEngine;

public class AgentMetricsDisplay : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 2, 0);
    private Agent agent;

    void Start()
    {
        agent = GetComponent<Agent>();
    }

    void OnGUI()
    {
        if (agent == null) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + offset);
        if (screenPos.z > 0)
        {
            var m = agent.metrics;
            string text = $"Alg: {m.lastAlgorithm}\n" +
                          $"Time: {m.lastPathTime:F1}ms\n" +
                          $"Nodes: {m.lastNodesProcessed}\n" +
                          $"Dist: {m.lastPathDistance:F1}\n" +
                          $"Repaths: {m.rePathCount}";
            GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 120, 80), text);
        }
    }
}