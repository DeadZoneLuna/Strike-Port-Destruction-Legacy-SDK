using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Disable all weapons except the knife.")]
#endif
    public bool DisableWeapons;
#if UNITY_EDITOR
    [Header("List of approved weapons")]
#endif
    public string[] ApprovedWeapons;
#if UNITY_EDITOR
    [Header("The list with which weapon the player will be respawn and these weapons.")]
#endif
    public WeaponSpawn[] SpawnWeapons;

    [System.Serializable]
    public class WeaponSpawn
    {
#if UNITY_EDITOR
        [Header("Alias weapon name")]
#endif
        public string WeaponAliasName;
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
}
