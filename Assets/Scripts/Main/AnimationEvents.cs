using System.Linq;
using UnityEngine;
using System.Collections;
using SPD.Audio;
#if UNITY_EDITOR
using UnityEditor;
using SPD.EditorAttributes;
using Sirenix.OdinInspector;
#endif

public class AnimationEvents : Bs
{
    [System.Serializable]
    public class CustomParticle
    {
        public ParticleSystem Particle;
        public enum EmitType
        {
            Emit,
            Play,
            PlayWithChildrens
        }
        public EmitType EmitMode;
        public int CountEmit = 1;
    }
    //Default
    [Header("Muzzle Flash Parent")]
    public Transform MuzzleFlashParent;
    public Vector3 MuzzleFlashRotation;
    public Vector3 MuzzleFlashScale;
    [Header("Muzzle Flash Parent Second")]
    public Transform MuzzleFlashParentSecond;
    public Vector3 MuzzleFlashRotationSecond;
    public Vector3 MuzzleFlashScaleSecond;
    [Header("Shell Eject Parent")]
    public Transform ShellEjectParent;
    public Vector3 ShellEjectRotation;
    public Vector3 ShellEjectScale;
    [Header("Shell Eject Parent Second")]
    public Transform ShellEjectParentSecond;
    public Vector3 ShellEjectRotationSecond;
    public Vector3 ShellEjectScaleSecond;

    public string MuzzleFlashEffect = "MuzzleFlashEffect";
    public string ShellEjectEffect = "shell_9mm";

    [Header("Custom Particles for Animation Event")]
    public CustomParticle[] CustomParticles;

    [Header("Silencer model (Only for USP-S or M4A1-S)")]
    public GameObject Silencer;
    [Header("Code on panel C4 (It's can be used on any objects)")]
    public TextMesh PanelCode;
    [Header("Special weapon model for Counter-Terrorists")]
    public GameObject CTWeaponModel;
    [Header("Special weapon model for Terrorists")]
    public GameObject TWeaponModel;
    [Header("Special hands model for Counter-Terrorists")]
    public string CTHandsPath = "ct_arms";
    [Header("Special hands model for Terrorists")]
    public string THandsPath = "t_arms";
    internal GameObject CTHandsCache;
    internal GameObject THandsCache;
#if UNITY_EDITOR
    public string HandsPlaceholder_Path = "l:Weapons/t_arms";
    bool showPlaceHolderNull
    {
        get
        {
            return EditorLoadedHands == null;
        }
    }
    bool showPlaceHolderNotNull
    {
        get
        {
            return EditorLoadedHands != null;
        }
    }
    [ShowIf("showPlaceHolderNotNull")]
    [Header("Don't forget to delete 'EditorLoadedHands' object before saving the prefab!")]
    [SerializeField]
    private Transform EditorLoadedHands;
    [ShowIf("showPlaceHolderNull")]
    [InspectorButton("EditorHandsInit")]
    public int LoadHandsPlaceholder;

    public void EditorHandsInit()
    {
        EditorLoadedHands = CreateModelEditor(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(HandsPlaceholder_Path)[0])), GetGameObject.transform).transform;
    }

    public GameObject CreateModelEditor(GameObject GO, Transform Parent = null)
    {
        GameObject Mesh = null;

        foreach (SkinnedMeshRenderer SkinnedMesh in GO.GetComponentsInChildren<SkinnedMeshRenderer>())
            Mesh = Parent.CreateSubModel(SkinnedMesh, "EditorLoadedHands");

        return Mesh;
    }
#endif

    public void CreateHands()
    {

    }

    public void EmitSound(AudioClip Audio)
    {

    }

    public void EmitSoundByBank(BankEntry Audio)
    {

    }

    public void EmitByID(int ID)
    {
        //Not implemented :p
    }

    public void EmitCode(string Value)
    {

    }

    public void EmitMuzzleFlashMain()
    {

    }

    public void EmitMuzzleFlashSecond()
    {

    }

    public void EmitShellParentMain()
    {

    }

    public void EmitShellParentSecond()
    {

    }

    public void EmitCustomParticle(int IndexParticle)
    {

    }

    public void OnReloaded()
    {
        //Not implemented :p
    }

    public void Walk()
    {

    }

    public void Hit()
    {

    }
}
