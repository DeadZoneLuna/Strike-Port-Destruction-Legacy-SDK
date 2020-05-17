using System;
using System.Collections.Specialized;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Object = UnityEngine.Object;
using System.Collections;
using System.Text.RegularExpressions;
#if (UNITY_EDITOR)
using UnityEditor;
#endif
public class Base : MonoBehaviour
{
    public bool selectedGameObject
    {
        get
        {
#if (UNITY_EDITOR)
            return Selection.activeGameObject == this.gameObject;
#else
            return false;
#endif
        }
    }

    public virtual void OnSelectionChanged()
    {
    }
    //public List<string> sbLog = new List<string>();
    public virtual void OnEditorGui()
    {
    }
#if (UNITY_EDITOR)
    public virtual void OnSceneGUI(SceneView scene, ref bool repaint)
    {
    }
#endif
    public virtual void Init()
    {
    }
    public void SetDirty()
    {
#if (UNITY_EDITOR)
        EditorUtility.SetDirty(this);
#endif
    }
}