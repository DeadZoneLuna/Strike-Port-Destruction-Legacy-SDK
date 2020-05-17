#pragma warning disable 1587
/// \file
/// <summary>Reimplements a RPC Attribute, as it's no longer in all versions of the UnityEngine assembly.</summary>
#pragma warning restore 1587

using UnityEngine;
#pragma warning disable 0618

public class Bs : Base
{

#if UNITY_EDITOR
    internal GameObject GetGameObject
    {
        get
        {
            return base.gameObject;
        }
    }
#endif

    public virtual void Awake()
    {
    }

    public virtual void OnPlConnected()
    {
    }
}