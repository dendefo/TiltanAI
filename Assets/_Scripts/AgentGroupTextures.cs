using UnityEngine;

[CreateAssetMenu(fileName = "AgentGroupTextures", menuName = "Game/Agent Group Textures")]
public class AgentGroupTextures : ScriptableObject
{
    [System.Serializable]
    public struct GroupTextureSet
    {
        public AgentGroup group;
        public Material material;
    }

    [SerializeField]
    private GroupTextureSet[] groupTextures;

    public Material GetMaterialForGroup(AgentGroup group)
    {
        foreach (var textureSet in groupTextures)
        {
            if (textureSet.group == group)
                return textureSet.material;
        }
        return null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Validate that we don't have duplicate groups
        if (groupTextures != null)
        {
            for (int i = 0; i < groupTextures.Length; i++)
            {
                for (int j = i + 1; j < groupTextures.Length; j++)
                {
                    if (groupTextures[i].group == groupTextures[j].group)
                    {
                        Debug.LogWarning($"Duplicate group {groupTextures[i].group} found in {name}", this);
                    }
                }
            }
        }
    }
#endif
}