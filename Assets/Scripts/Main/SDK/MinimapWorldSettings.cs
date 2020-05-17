using UnityEngine;
using System.Collections;

public class MinimapWorldSettings : MonoBehaviour 
{
    [Header("Use UI editor Tool for scale the wordSpace")]
    public Color GizmoColor = new Color(1, 1, 1, 0.75f);
    [Header("Texture Minimap")]
    public Texture MinimapTexture;
    public RectTransform Rect;
#if UNITY_EDITOR
    /// <summary>
    /// Debuging world space of map
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Vector3 v = Rect.sizeDelta;
        Vector3 pivot = Rect.localPosition;

        Gizmos.color = GizmoColor;
        Gizmos.DrawCube(pivot, new Vector3(v.x, 2, v.y));
        Gizmos.DrawWireCube(pivot, new Vector3(v.x, 2, v.y));
    }
#endif
}