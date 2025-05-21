using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MultiOutline : Shadow
{
    [SerializeField]
    private List<Color> m_OutlineColors = new List<Color>();

    [SerializeField]
    private List<Vector2> m_OutlineDistances = new List<Vector2>();

    [SerializeField]
    private List<bool> m_UseGraphicAlphas = new List<bool>();

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        // If no custom outline data is defined, use the base shadow implementation
        if (m_OutlineColors.Count == 0 || m_OutlineDistances.Count == 0 || m_UseGraphicAlphas.Count == 0)
        {
            base.ModifyMesh(vh);
            return;
        }

        // Make sure all lists have the same length
        int count = Mathf.Min(m_OutlineColors.Count, m_OutlineDistances.Count, m_UseGraphicAlphas.Count);

        var list = new List<UIVertex>();
        vh.GetUIVertexStream(list);

        int initialVertexCount = list.Count;

        if (initialVertexCount > 0)
        {
            // Apply each outline
            for (int i = 0; i < count; i++)
            {
                var originalColor = effectColor;
                var originalDistance = effectDistance;
                var originalUseGraphic = useGraphicAlpha;

                // Temporarily set the effect properties for this outline
                effectColor = m_OutlineColors[i];
                effectDistance = m_OutlineDistances[i];
                useGraphicAlpha = m_UseGraphicAlphas[i];

                // Apply the current shadow/outline
                var verts = new List<UIVertex>(list);
                int neededCapacity = verts.Count * 5;
                if (verts.Capacity < neededCapacity)
                    verts.Capacity = neededCapacity;

                //ApplyShadowForOffset(verts, effectColor, 0, verts.Count, effectDistance.x, effectDistance.y);

                // Restore original properties
                effectColor = originalColor;
                effectDistance = originalDistance;
                useGraphicAlpha = originalUseGraphic;

                // Add new vertices for this outline
                for (int j = initialVertexCount; j < verts.Count; j++)
                {
                    list.Add(verts[j]);
                }
            }
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(list);
    }

    // Method to add or update an outline
    public void SetOutline(int index, Color color, Vector2 distance, bool useGraphicAlpha)
    {
        // Ensure lists have enough capacity
        while (m_OutlineColors.Count <= index)
            m_OutlineColors.Add(Color.black);

        while (m_OutlineDistances.Count <= index)
            m_OutlineDistances.Add(Vector2.one);

        while (m_UseGraphicAlphas.Count <= index)
            m_UseGraphicAlphas.Add(true);

        // Set the values
        m_OutlineColors[index] = color;
        m_OutlineDistances[index] = distance;
        m_UseGraphicAlphas[index] = useGraphicAlpha;
    }

    // Method to clear all outlines
    public void ClearOutlines()
    {
        m_OutlineColors.Clear();
        m_OutlineDistances.Clear();
        m_UseGraphicAlphas.Clear();
    }

    // Method to get outline count
    public int GetOutlineCount()
    {
        return Mathf.Min(m_OutlineColors.Count, m_OutlineDistances.Count, m_UseGraphicAlphas.Count);
    }
}