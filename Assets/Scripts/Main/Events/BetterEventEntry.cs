using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

[Serializable]
public class BetterEventEntry : ISerializationCallbackReceiver
{
    [NonSerialized, HideInInspector]
    public Delegate Delegate;

    public float DelayInvoke = .0f;
    public bool InvokeOnce = false;
    public bool ResetEvent = true;

    [NonSerialized, HideInInspector]
    public object[] ParameterValues;
    internal bool OnceEnded;

    public BetterEventEntry(Delegate del) 
    {
        if (del != null && del.Method != null)
        {
            this.Delegate = del;
            this.ParameterValues = new object[del.Method.GetParameters().Length];
        }
    }

    public void Init()
    {

    }

    public void Reset()
    {

    }

    public async void Invoke()
    {
        if (this.Delegate != null && this.ParameterValues != null)
        {
            await Task.Delay(TimeSpan.FromSeconds(DelayInvoke));
            // This is faster than Dynamic Invoke.
            /*this.Result = */
            this.Delegate.Method.Invoke(this.Delegate.Target, this.ParameterValues);
        }
    }

    #region OdinSerialization
    [SerializeField, HideInInspector]
    private List<UnityEngine.Object> unityReferences;

    [SerializeField, HideInInspector]
    private byte[] bytes;

    public void OnAfterDeserialize()
    {
        var val = SerializationUtility.DeserializeValue<OdinSerializedData>(this.bytes, DataFormat.Binary, this.unityReferences);
        this.Delegate = val.Delegate;
        this.ParameterValues = val.ParameterValues;
    }

    public void OnBeforeSerialize()
    {
        var val = new OdinSerializedData() { Delegate = this.Delegate, ParameterValues = this.ParameterValues };
        this.bytes = SerializationUtility.SerializeValue(val, DataFormat.Binary, out this.unityReferences);
    }

    private struct OdinSerializedData
    {
        public Delegate Delegate;
        public object[] ParameterValues;
    }
    #endregion
}
