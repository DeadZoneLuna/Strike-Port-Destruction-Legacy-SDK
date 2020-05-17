using UnityEngine;
using UnityEditor;

namespace LightingTools.LightProbesVolumes
{
    public class RefreshLightProbesVolumes
    {
        [MenuItem("SDK/Lighting/Refresh lightprobes volumes")]
        static void Refresh()
        {
            var volumes = GameObject.FindObjectsOfType<LightProbesVolumeSettings>();
            foreach (var volume in volumes)
            {
                volume.Populate();
            }
        }
    }
}