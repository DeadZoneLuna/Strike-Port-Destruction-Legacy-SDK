using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamSpawn
{
    CT,
    T,
    ZM,
    Any
}

public class SpawnPlayer : MonoBehaviour 
{
    public TeamSpawn Team;

#if UNITY_EDITOR
    public bool ShowGizmos = true;
    private bool Selected;

    void OnDrawGizmos()
    {
        if (ShowGizmos)
        {
            Vector3 FixerPisition = new Vector3(0, 0.915f, 0);
            Vector3 FixerScale = new Vector3(0.85f, 1.83f, 0.85f);
            Gizmos.color = Team == TeamSpawn.CT ? Color.blue : Team == TeamSpawn.T ? Color.red : Team == TeamSpawn.Any ? Color.yellow : Color.gray;//UnityEditor.Selection.activeGameObject == gameObject ? Color.white : Team == TeamSpawn.CT ? Color.blue : Team == TeamSpawn.T ? Color.red : Team == TeamSpawn.Any ? Color.yellow : Color.gray;
            Gizmos.DrawCube(transform.position + FixerPisition, FixerScale);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(new Ray(transform.position + FixerPisition, transform.forward));
        }
    }

    void OnDrawGizmosSelected()
    {
        if (ShowGizmos)
        {
            Vector3 FixerPisition = new Vector3(0, 0.915f, 0);
            Vector3 FixerScale = new Vector3(0.85f, 1.83f, 0.85f);
            Gizmos.color = Color.white;
            Gizmos.DrawCube(transform.position + FixerPisition, FixerScale);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(new Ray(transform.position + FixerPisition, transform.forward));
        }
    }
#endif
}
