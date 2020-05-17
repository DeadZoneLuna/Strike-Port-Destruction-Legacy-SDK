using Sabresaurus.SabreCSG;
using System;
using System.Linq;
using UnityEngine;

public class SabreCustomVolumeLayer : Volume
{
    public enum Layer
    {
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2,
        None3 = 3,
        Water = 4,
        UI = 5,
        None6 = 6,
        None7 = 7,
        IgnoreColl = 8,
        C4 = 9,
        Level = 10,
        Player = 11,
        Dead = 12,
        MiniMap = 13,
        PlayerClip = 14,
        Effect = 15,
        Cull = 16,
        Ladder = 17,
        Glass = 18,
        Weapon = 19,
        IgnoreColl2 = 20,
        Grenade = 21,
        hands = 22,
        BombClip = 23,
        Physics = 24,
        GrenadeClip = 25,
        Destroyable = 26,
        PlayerMine = 27,
        Door = 28,
        Hitbox = 29,
        Clip = 30,
        Entity = 31
    }

    public Layer layer = Layer.Default;

#if UNITY_EDITOR

    /// <summary>
    /// Gets the brush preview material shown in the editor.
    /// </summary>
    public override Material BrushPreviewMaterial
    {
        get
        {
            return (Material)SabreCSGResources.LoadObject("Materials/scsg_volume_trigger.mat");
        }
    }

    public override bool OnInspectorGUI(Volume[] selectedVolumes)
    {
        var testVolumes = selectedVolumes.Cast<SabreCustomVolumeLayer>();
        bool invalidate = false;

        Layer previousLayer = layer;
        layer = (Layer)UnityEditor.EditorGUILayout.EnumPopup(layer);
        if (layer != previousLayer)
        {
            foreach (SabreCustomVolumeLayer volume in testVolumes)
                volume.layer = layer;

            invalidate = true;
        }

        return invalidate;
    }
#endif
    public override void OnCreateVolume(GameObject volume)
    {
        volume.layer = (int)layer;
    }
}
