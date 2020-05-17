using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SPD.Triggers
{
    public class BuyTrigger : MonoBehaviour
    {
        public enum Team
        {
            Any,
            T,
            CT
        }

        public Team team;

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (team == Team.T)
                Gizmos.color = new Color32(255, 0, 0, 150);
            else if (team == Team.CT)
                Gizmos.color = new Color32(0, 0, 255, 150);
            else
                Gizmos.color = new Color32(0, 255, 0, 150);

            Gizmos.DrawCube(GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.size);
        }
#endif
    }
}
