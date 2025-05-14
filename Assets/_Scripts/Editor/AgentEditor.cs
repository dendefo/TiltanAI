#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Agent))]
public class AgentEditor : Editor
{
    private Agent agent;
    private bool showDebugInfo = false;

    private void OnEnable()
    {
        agent = (Agent)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        // Group Selection Area
        EditorGUILayout.Space();
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Agent Group Settings", EditorStyles.boldLabel);
            
            // Draw group selection with custom styling
            var newGroup = (AgentGroup)EditorGUILayout.EnumPopup(new GUIContent("Agent Group", 
                "Select the group for this agent. This will determine its appearance."), 
                agent.Group);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(agent, "Change Agent Group");
                agent.Group = newGroup;
                EditorUtility.SetDirty(agent);
            }
        }

        EditorGUILayout.Space();

        // Debug Information
        showDebugInfo = EditorGUILayout.Foldout(showDebugInfo, "Debug Information");
        if (showDebugInfo)
        {
            EditorGUI.indentLevel++;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                var skinnedMesh = agent.GetComponentInChildren<SkinnedMeshRenderer>();
                EditorGUILayout.LabelField("Has SkinnedMeshRenderer:", skinnedMesh != null ? "Yes" : "No");
                if (skinnedMesh != null)
                {
                    EditorGUILayout.LabelField("Current Material:", 
                        skinnedMesh.sharedMaterial != null ? skinnedMesh.sharedMaterial.name : "None");
                }

                if (GUILayout.Button("Refresh Appearance"))
                {
                    // Force update the appearance
                    agent.Group = agent.Group;
                }
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // Draw the rest of the inspector
        DrawDefaultInspector();
    }

    // Add scene view helpers
    private void OnSceneGUI()
    {
        if (!showDebugInfo) return;

        var skinnedMesh = agent.GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMesh != null)
        {
            // Draw bounds of the skinned mesh
            Handles.color = Color.green;
            Handles.DrawWireCube(skinnedMesh.bounds.center, skinnedMesh.bounds.size);
        }
    }
}
#endif