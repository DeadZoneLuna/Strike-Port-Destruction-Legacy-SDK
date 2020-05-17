using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponDropSpecial
{
#if UNITY_EDITOR
    [Header("Weapon Name")]
#endif
    public string Name;
#if UNITY_EDITOR
    [Header("Amount ammo in magazine")]
#endif
    public int Ammo;
#if UNITY_EDITOR
    [Header("Silencer mode (only for M4A1-S & USP-S)")]
#endif
    public bool Silencer;
#if UNITY_EDITOR
    [Header("Current ammo in magazine")]
#endif
    public int Clips;
}

public class WeaponMap : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Weapon Name")]
#endif
    public string Name;
#if UNITY_EDITOR
    [Header("Amount ammo in magazine")]
#endif
    public int Ammo;
#if UNITY_EDITOR
    [Header("Silencer mode (only for M4A1-S & USP-S)")]
#endif
    public bool Silencer;
#if UNITY_EDITOR
    [Header("Current ammo in magazine")]
#endif
    public int Clips;
#if UNITY_EDITOR
    [Header("Random Drop with special settings")]
#endif
    public bool RandomDropSpecial;
    public WeaponDropSpecial[] RandomDrop;

#if UNITY_EDITOR
    public Mesh Mesh;
    public bool ShowGizmos = true;

    void OnDrawGizmos()
    {
        if (!RandomDropSpecial)
        {
            if (ShowGizmos)
            {
                Gizmos.DrawMesh(Mesh, transform.position, transform.rotation);
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, -transform.right);
                Gizmos.DrawRay(transform.position, transform.forward);
            }
        }
    }
#endif
}
