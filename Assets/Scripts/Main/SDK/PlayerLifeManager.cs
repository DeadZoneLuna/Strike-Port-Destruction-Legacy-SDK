using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeManager : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Life")]
#endif
    public int Life;

#if UNITY_EDITOR
    [Header("Armour")]
#endif
    public int Armour;
#if UNITY_EDITOR
    [Header("Speed Player")]
#endif
    public float Speed;
}
