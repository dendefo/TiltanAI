using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GridTagManager", menuName = "Grid/Tag Manager", order = 1)]
public class GridTagManager : ScriptableObject
{
    public List<SurfaceTag> SurfaceTags = new();
    public List<AgentTag> AgentTags = new();
    private void OnValidate()
    {
        foreach (var agentTag in AgentTags)
        {
            if (agentTag.Surfaces == null)
            {
                agentTag.Surfaces = new List<SurfaceBoolPair>();
            }

            // Ensure each surface appears once and only once
            var existingSurfaces = new HashSet<SurfaceTag>();
            foreach (var pair in agentTag.Surfaces)
            {
                if (pair.Surface != null)
                    existingSurfaces.Add(pair.Surface);
            }

            // Add missing surfaces
            foreach (var surfaceTag in SurfaceTags)
            {
                if (!existingSurfaces.Contains(surfaceTag))
                {
                    agentTag.Surfaces.Add(new SurfaceBoolPair
                    {
                        Surface = surfaceTag,
                        IsActive = false
                    });
                }
            }

            // Remove duplicates and surfaces not in SurfaceTags
            agentTag.Surfaces.RemoveAll(pair =>
                pair.Surface == null ||
                !SurfaceTags.Contains(pair.Surface) ||
                agentTag.Surfaces.FindAll(p => p.Surface == pair.Surface).Count > 1 && agentTag.Surfaces.IndexOf(pair) != agentTag.Surfaces.FindIndex(p => p.Surface == pair.Surface)
            );
        }
    }
}
[System.Serializable]
public class SurfaceTag
{
    public string Name;
    public float Price;
}
[System.Serializable]
public class AgentTag
{
    public string Name;
    [SerializeField] public List<SurfaceBoolPair> Surfaces;
}
[System.Serializable]
public class SurfaceBoolPair
{
    public SurfaceTag Surface;
    public bool IsActive;
}