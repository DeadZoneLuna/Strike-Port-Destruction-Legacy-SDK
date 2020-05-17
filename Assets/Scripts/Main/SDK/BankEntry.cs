using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using GUIEditor = UnityEditor.EditorGUILayout;
#endif

namespace SPD.Audio
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "SDK/Audio/AudioClipBank")]
#endif
    [System.Serializable]
    public class BankEntry : ScriptableObject
    {
        public float Volume = 1.0f;
        public float Pitch = 1.0f;
        public bool RandomClip;
        public AudioClip Clip;
        public AudioClip[] Clips;

        public AudioClip GetClip()
        {
            return RandomClip ? Clips.Random() : Clip;
        }

        public AudioClip GetClipByIndex(int i)
        {
            return RandomClip ? Clips[i] : Clip;
        }
    }
}