#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CreateLightProbes : EditorWindow
{
    float Spacing = 2.0f;
    float SecondHeight = 4.0f;

    [MenuItem("SDK/Lighting/Create LightProbes from selected GameObjects (Outdated)")]

    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CreateLightProbes window = (CreateLightProbes)GetWindow(typeof(CreateLightProbes), true, "Create Light Probes");
        window.position = new Rect((Screen.width / 2) - 125, Screen.height / 2 + 85, 300, 80);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Creation:", EditorStyles.boldLabel);
        Spacing = EditorGUILayout.FloatField("Spacing:", Spacing);
        SecondHeight = EditorGUILayout.FloatField("Secondary Height:", SecondHeight);

        // Clamp values
        if (Spacing < 0.1f) Spacing = 0.1f;
        if (SecondHeight < 0.1f) SecondHeight = 0.1f;

        if (GUILayout.Button("Create Light Probes On Selected Objects"))
        {
            Bounds();
        }
    }

    void Bounds()
    {
        // Get selection
        GameObject[] Select = Selection.gameObjects;
        if (Select.Length < 1) return;

        // Get total bounds for selected objects
        float minX = 0.0f;
        float minY = 0.0f;
        float minZ = 0.0f;
        float maxX = 0.0f;
        float maxY = 0.0f;
        float maxZ = 0.0f;
        for (int i = 0; i < Select.Length; i++)
        {
            // First check that mesh has a collider attached to it
            // if it doesn't then we can't raycast so we should ignore this object
            Collider col = Select[i].GetComponent<Collider>();
            if (col == null) continue;

            // Get renderer component attached to object
            // If there is no renderer attached then we ignore this object
            Renderer renderer = Select[i].GetComponent<Renderer>();
            if (renderer == null) continue;

            // Get the renderer bounds
            Bounds bbox = renderer.bounds;

            // Update total bounds
            if (bbox.min.x < minX) minX = bbox.min.x;
            if (bbox.min.y < minY) minY = bbox.min.y;
            if (bbox.min.z < minZ) minZ = bbox.min.z;
            if (bbox.max.x > maxX) maxX = bbox.max.x;
            if (bbox.max.y > maxY) maxY = bbox.max.y;
            if (bbox.max.z > maxZ) maxZ = bbox.max.z;
        }

        // Now go through in a grid and attempt to place a light probe using raycasting
        float xCount = minX;
        float zCount = minZ;

        List<Vector3> VertPositions = new List<Vector3>();

        for (int z = 0; z < maxZ; z++)
        {
            for (int x = 0; x < maxX; x++)
            {
                // Raycast downwards through each selected object and
                // attempt to find one that we are over
                for (int j = 0; j < Select.Length; j++)
                {
                    Collider col = Select[j].GetComponent<Collider>();

                    RaycastHit hit;
                    Ray ray = new Ray();
                    ray.origin = new Vector3(xCount, maxY, zCount);
                    ray.direction = -Vector3.up;
                    if (col.Raycast(ray, out hit, (maxY - minY) * 2))
                    {
                        VertPositions.Add(hit.point + new Vector3(0, 0f, 0));
                        VertPositions.Add(hit.point + new Vector3(0, SecondHeight, 0));
                    }
                }

                xCount += Spacing;
            }

            // Reset X Counter
            xCount = minX;

            zCount += Spacing;
        }

        // Check if we have any hits
        if (VertPositions.Count < 1) return;

        // Get _LightProbes game object
        GameObject LightProbeGameObj = GameObject.Find("_LightProbes");
        if (LightProbeGameObj == null) return;

        // Get light probe group component
        LightProbeGroup LPGroup = LightProbeGameObj.GetComponent<LightProbeGroup>();
        if (LPGroup == null) return;

        // Create lightprobe positions
        Vector3[] ProbePos = new Vector3[VertPositions.Count];
        for (int i = 0; i < VertPositions.Count; i++)
        {
            ProbePos[i] = VertPositions[i];
        }

        // Set new light probes
        LPGroup.probePositions = ProbePos;

        Debug.Log("Finished Probe Calculations with: " + ProbePos.Length + "Probes.");
    }
}
#endif
