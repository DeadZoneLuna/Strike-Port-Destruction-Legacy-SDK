using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SPD.Triggers
{
    public class BombPlace : MonoBehaviour
    {
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = new Color32(255, 64, 64, 150);
            Gizmos.DrawCube(GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.size);
        }
#endif
    }
}
