using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
public class SDKTools : Editor 
{
    //MAPS & CONTENT
    public static string ContentPathStandalone = Path.Combine(Application.dataPath, "GameContent/Standalone/Custom");
    public static string ContentPathAndroid = Path.Combine(Application.dataPath, "GameContent/Android/Custom");
    public static bool SaveScene()
    {
        return EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "", false);
    }

    public static string currentScene
    {
        get
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            if (scene.IsValid())
                return scene.path;

            return "";
        }
        set
        {
        }
    }

    public static string BuildPlayer(string[] levels, string locationPathName, BuildTarget target, BuildOptions options)
    {
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = levels;
        buildPlayerOptions.locationPathName = locationPathName;
        buildPlayerOptions.targetGroup = buildTargetGroup;
        buildPlayerOptions.target = target;
        buildPlayerOptions.options = options;
        return BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    [MenuItem("Build/Build GameContent (Weapons, Sounds and etc...) for any platform")]
    static void AssetBundleBuild()
    {
        AssetBundleBuildStandalone();
        AssetBundleBuildAndroid();
    }

    [MenuItem("Build/Build GameContent (Weapons, Sounds and etc...) for Standalone")]
    static void AssetBundleBuildStandalone()
    {
        if (!Directory.Exists(ContentPathStandalone))
            Directory.CreateDirectory(ContentPathStandalone);

        BuildPipeline.BuildAssetBundles(ContentPathStandalone, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Build/Build GameContent (Weapons, Sounds and etc...) for Android")]
    static void AssetBundleBuildAndroid()
    {
        if (!Directory.Exists(ContentPathAndroid))
            Directory.CreateDirectory(ContentPathAndroid);

        BuildPipeline.BuildAssetBundles(ContentPathAndroid, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
    }

    [MenuItem("Build/Build GameContent (Weapons, Sounds and etc...) for Current Platform")]
    static void AssetBundleBuildCurPlatform()
    {
        var Path = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ? ContentPathAndroid : ContentPathStandalone;
        if (!Directory.Exists(Path))
            Directory.CreateDirectory(Path);

        BuildPipeline.BuildAssetBundles(Path, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
    }

    [MenuItem("Build/Build Map for any platforms (Not recommended)")]
    public static void ExportResource()
    {
        Debug.Log("building" + Selection.activeObject.name);
        SaveScene();//EditorApplication.SaveScene();
        var obs = Selection.objects;
        Build(BuildTarget.Android, obs);
        Build(BuildTarget.StandaloneWindows, obs);
    }

    [MenuItem("Build/Build Map for Android")]
    private static void BuildAndroid()
    {
        SaveScene();//EditorApplication.SaveScene();
        Build(BuildTarget.Android, Selection.objects);
    }

    [MenuItem("Build/Build Map for Windows")]
    private static void BuildWindows()
    {
        SaveScene();//EditorApplication.SaveScene();
        Build(BuildTarget.StandaloneWindows, Selection.objects);
    }

    private static void Build(BuildTarget bt, Object[] objects)
    {
        Directory.CreateDirectory("maps");
        if (objects == null || objects.Length == 0)
        {
            if (string.IsNullOrEmpty(currentScene)) throw new Exception("Scene is null");
            objects = new Object[] { AssetDatabase.LoadAssetAtPath(currentScene, typeof(Object)) };
        }
        foreach (var a in objects)
        {
            try
            {
                Debug.Log(a.name + ": " + BuildPlayer(new[] { AssetDatabase.GetAssetPath(a) }, "maps/" + a.name + ".unity3d" + bt + "u4", bt, BuildOptions.BuildAdditionalStreamedScenes));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    [MenuItem("SDK/Maps/Replace old spawns to new")]
    static void ReplaceOldSpawns()
    {
        Debug.Log("INIT");
        EditorUtility.ClearProgressBar();
        foreach (var a in GameObject.FindGameObjectsWithTag("SpawnT"))
        {
            var obj = a.AddComponent<SpawnPlayer>();
            obj.Team = TeamSpawn.T;
        }
        foreach (var b in GameObject.FindGameObjectsWithTag("SpawnCT"))
        {
            var obj = b.AddComponent<SpawnPlayer>();
            obj.Team = TeamSpawn.CT;
        }
        foreach (var c in GameObject.FindGameObjectsWithTag("DeathMatch"))
        {
            var obj = c.AddComponent<SpawnPlayer>();
            obj.Team = TeamSpawn.Any;
        }
        foreach (var d in GameObject.FindGameObjectsWithTag("ZSpawn"))
        {
            var obj = d.AddComponent<SpawnPlayer>();
            obj.Team = TeamSpawn.ZM;
        }
        EditorUtility.ClearProgressBar();
    }
    //MAPS & CONTENT

    [MenuItem("SDK/Show Mesh Info %#i")]
    public static void ShowCount()
    {
        int triangles = 0;
        int vertices = 0;
        int meshCount = 0;

        foreach (GameObject go in Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel))
        {
            Component[] skinnedMeshes = go.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
            Component[] meshFilters = go.GetComponentsInChildren(typeof(MeshFilter));

            ArrayList totalMeshes = new ArrayList(meshFilters.Length + skinnedMeshes.Length);

            for (int meshFiltersIndex = 0; meshFiltersIndex < meshFilters.Length; meshFiltersIndex++)
            {
                MeshFilter meshFilter = (MeshFilter)meshFilters[meshFiltersIndex];
                totalMeshes.Add(meshFilter.sharedMesh);
            }

            for (int skinnedMeshIndex = 0; skinnedMeshIndex < skinnedMeshes.Length; skinnedMeshIndex++)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)skinnedMeshes[skinnedMeshIndex];
                totalMeshes.Add(skinnedMeshRenderer.sharedMesh);
            }

            for (int i = 0; i < totalMeshes.Count; i++)
            {
                Mesh mesh = totalMeshes[i] as Mesh;
                if (mesh == null)
                {
                    Debug.LogWarning("You have a missing mesh in your scene.");
                    continue;
                }
                vertices += mesh.vertexCount;
                triangles += mesh.triangles.Length / 3;
                meshCount++;
            }
        }

        EditorUtility.DisplayDialog("Vertex and Triangle Count", vertices
            + " vertices in selection.  " + triangles + " triangles in selection.  "
            + meshCount + " meshes in selection." + (meshCount > 0 ? ("  Average of " + vertices / meshCount
            + " vertices and " + triangles / meshCount + " triangles per mesh.") : ""), "OK", "");
    }

    [MenuItem("SDK/Show Mesh Info %i", true)]
    public static bool ValidateShowCount()
    {
        return Selection.activeGameObject;
    }

    [MenuItem("SDK/ModelTools/World Model Mesh Set")]
    static void WorldModelMeshSet()
    {
        Debug.Log("INIT");
        EditorUtility.ClearProgressBar();
        var OBJS = Selection.GetFiltered<GameObject>(SelectionMode.Editable | SelectionMode.TopLevel);
        foreach (var objGet in OBJS)
        {
            if (objGet.GetComponent<WorldModel>())
            {
                WorldModel World = objGet.GetComponent<WorldModel>();
                World.MeshFilter = World.gameObject.GetComponent<MeshFilter>();
                World.MeshRenderer = World.gameObject.GetComponent<MeshRenderer>();
                //EditorUtility.SetDirty(objGet);
            }
            else
                EditorUtility.DisplayDialog("WorldModel not found!", string.Format("The object - {0} isn't a WorldModel!", objGet.name), "Close");
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("SDK/ModelTools/World Model Extract Dropped Mesh")]
    static void WorldModelDroppedMesh()
    {
        Debug.Log("INIT");
        EditorUtility.ClearProgressBar();
        var OBJS = Selection.GetFiltered<GameObject>(SelectionMode.Editable | SelectionMode.TopLevel);
        foreach (var objGet in OBJS)
        {
            var path = @"Assets\GameRes\SDK\Models\Weapons\Meshes\" + objGet.name + ".asset";
            Mesh sharedMesh = Object.Instantiate(objGet.GetComponent<MeshFilter>().sharedMesh);
            AssetDatabase.CreateAsset(sharedMesh, path);
            AssetDatabase.SaveAssets();
        }
        EditorUtility.ClearProgressBar();
    }

    //Weapon SDK
    [MenuItem("SDK/ModelTools/Weapons/Set world 'Name' var from selected object(s)")]
    static void FixWeaponNamePrefab()
    {
        Debug.Log("INIT");
        EditorUtility.ClearProgressBar();
        var OBJS = Selection.GetFiltered<GameObject>(SelectionMode.Editable | SelectionMode.TopLevel);
        foreach (var objGet in OBJS)
        {
            if (objGet.GetComponent<WeaponMap>())
            {
                WeaponMap wep = objGet.GetComponent<WeaponMap>();
                wep.Name = objGet.name;
                EditorUtility.SetDirty(objGet);
            }
            else
                EditorUtility.DisplayDialog("WeaponMap not found!", string.Format("The object - {0} isn't a WeaponMap!", objGet.name), "Close");
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("SDK/ModelTools/Weapons/Set world 'Mesh' var from selected object(s)")]
    static void FixWeaponMeshPrefab()
    {
        Debug.Log("INIT");
        EditorUtility.ClearProgressBar();
        var OBJS = Selection.GetFiltered<GameObject>(SelectionMode.Editable | SelectionMode.TopLevel);
        foreach (var objGet in OBJS)
        {
            if (objGet.GetComponent<WeaponMap>())
            {
                WeaponMap wep = objGet.GetComponent<WeaponMap>();
                wep.Mesh = objGet.GetComponent<MeshFilter>().sharedMesh;
                EditorUtility.SetDirty(objGet);
            }
            else
                EditorUtility.DisplayDialog("WeaponMap not found!", string.Format("The object - {0} isn't a WeaponMap!", objGet.name), "Close");
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("SDK/ModelTools/Weapons/Add MuzzleFlash and ShellEject event from selected AnimationClips")]
    static void SetupWeaponEmitEffect()
    {
        Debug.Log("INIT");
        EditorUtility.ClearProgressBar();
        var OBJS = Selection.GetFiltered<AnimationClip>(SelectionMode.Editable | SelectionMode.TopLevel);
        foreach (var objGet in OBJS)
        {
            var ClipDump = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GetAssetPath(objGet));

            //Copy Events from original clip
            List<AnimationEvent> Events = new List<AnimationEvent>();
            foreach (var Event in ClipDump.events)
            {
                Events.Add(Event);
            }

            //Adding new events (In our case, these are 'EmitMuzzleFlashMain' and 'EmitShellParentMain')
            Events.Add(new AnimationEvent
            {
                functionName = "EmitMuzzleFlashMain",
                time = 0
            });
            Events.Add(new AnimationEvent
            {
                functionName = "EmitShellParentMain",
                time = 0
            });

            //Final
            AnimationUtility.SetAnimationEvents(ClipDump, Events.ToArray());
            AssetDatabase.SaveAssets();
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("SDK/ModelTools/Weapons/Add MuzzleFlashSecond and ShellEjectSecond event from selected AnimationClips")]
    static void SetupWeaponEmitEffectSecond()
    {
        Debug.Log("INIT");
        EditorUtility.ClearProgressBar();
        var OBJS = Selection.GetFiltered<AnimationClip>(SelectionMode.Editable | SelectionMode.TopLevel);
        foreach (var objGet in OBJS)
        {
            var ClipDump = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GetAssetPath(objGet));

            //Copy Events from original clip
            List<AnimationEvent> Events = new List<AnimationEvent>();
            foreach (var Event in ClipDump.events)
            {
                Events.Add(Event);
            }

            //Adding new events (In our case, these are 'EmitMuzzleFlashSecond' and 'EmitShellParentSecond')
            Events.Add(new AnimationEvent
            {
                functionName = "EmitMuzzleFlashSecond",
                time = 0
            });
            Events.Add(new AnimationEvent
            {
                functionName = "EmitShellParentSecond",
                time = 0
            });

            //Final
            AnimationUtility.SetAnimationEvents(ClipDump, Events.ToArray());
            AssetDatabase.SaveAssets();
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("SDK/ModelTools/Weapons/Label Set")]
    static void LabelSet()
    {
        EditorUtility.ClearProgressBar();
        var OBJS = Selection.GetFiltered<Object>(SelectionMode.Assets);
        float progressBar = 0.0f;
        int hundredPercent = OBJS.Length;
        int amountDone = 0;
        foreach (var objGet in OBJS)
        {
            amountDone++;
            progressBar = amountDone / hundredPercent;
            EditorUtility.DisplayProgressBar("Progress", "Object: " + objGet, progressBar);
            var path = AssetDatabase.GetAssetPath(objGet);
            Debug.Log(objGet.name);
            AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant("weapons/" + objGet.name, "");
            AssetDatabase.SetLabels(objGet, new string[] { "weapons/" + objGet.name });
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("SDK/ModelTools/Weapons/Label Set Remove")]
    static void LabelSetRemove()
    {
        EditorUtility.ClearProgressBar();
        var OBJS = Selection.GetFiltered<Object>(SelectionMode.Assets);
        float progressBar = 0.0f;
        int hundredPercent = OBJS.Length;
        int amountDone = 0;
        foreach (var objGet in OBJS)
        {
            amountDone++;
            progressBar = amountDone / hundredPercent;
            EditorUtility.DisplayProgressBar("Progress", "Object: " + objGet, progressBar);
            var path = AssetDatabase.GetAssetPath(objGet);
            Debug.Log(objGet.name);
            AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant("", "");
            AssetDatabase.ClearLabels(objGet);
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("SDK/ModelTools/Players/Label Set")]
    static void LabelSet2()
    {
        EditorUtility.ClearProgressBar();
        var OBJS = Selection.GetFiltered<Object>(SelectionMode.Assets);
        float progressBar = 0.0f;
        int hundredPercent = OBJS.Length;
        int amountDone = 0;
        foreach (var objGet in OBJS)
        {
            amountDone++;
            progressBar = amountDone / hundredPercent;
            EditorUtility.DisplayProgressBar("Progress", "Object: " + objGet, progressBar);
            var path = AssetDatabase.GetAssetPath(objGet);
            Debug.Log(objGet.name);
            AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant("players/" + objGet.name, "");
            AssetDatabase.SetLabels(objGet, new string[] { "players/" + objGet.name });
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("SDK/Particles/Label Set")]
    static void LabelSet3()
    {
        EditorUtility.ClearProgressBar();
        var OBJS = Selection.GetFiltered<Object>(SelectionMode.Assets);
        float progressBar = 0.0f;
        int hundredPercent = OBJS.Length;
        int amountDone = 0;
        foreach (var objGet in OBJS)
        {
            amountDone++;
            progressBar = amountDone / hundredPercent;
            EditorUtility.DisplayProgressBar("Progress", "Object: " + objGet, progressBar);
            var path = AssetDatabase.GetAssetPath(objGet);
            Debug.Log(objGet.name);
            AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant("particles/" + objGet.name, "");
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("SDK/ModelTools/Weapons/Create prefab from selected weapon")]
    static void CreatePrefab3()
    {
        GameObject[] objs = Selection.gameObjects;

        foreach (GameObject go in objs)
        {
            string localPath = "Assets/Resources/SDK/Prefabs/Weapons/" + go.name + ".prefab";
            CreateNew(go, localPath);
        }
    }
    //Weapon SDK

    // Disable the menu item if no selection is in place
    [MenuItem("SDK/ModelTools/Create Prefab From Selected", true)]
    static bool ValidateCreatePrefab()
    {
        return Selection.activeGameObject != null;
    }

    static void CreateNew(GameObject obj, string localPath)
    {
        Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
        PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
    }

    [MenuItem("SDK/ModelTools/Rename Drop")]
    static void RenameDrop()
    {
        Debug.Log("INIT");
        EditorUtility.ClearProgressBar();
        var OBJS = Selection.GetFiltered<GameObject>(SelectionMode.Editable | SelectionMode.TopLevel);
        foreach (var objGet in OBJS)
        {
            objGet.name = objGet.name.Replace("w_eq", "weapon");
            objGet.name = objGet.name.Replace("w_rif", "weapon");
            objGet.name = objGet.name.Replace("w_pist", "weapon");
            objGet.name = objGet.name.Replace("w_snip", "weapon");
            objGet.name = objGet.name.Replace("w_mach", "weapon");
            objGet.name = objGet.name.Replace("w_smg", "weapon");
            objGet.name = objGet.name.Replace("w_shot", "weapon");
            objGet.name = objGet.name.Replace("w_knife", "weapon_knife");
            objGet.name = objGet.name.Replace("w_ied", "weapon_c4");
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("SDK/ModelTools/Weapons/Set parents effects (Only for CS:GO Models)")]
    static void SetParentEffects()
    {
        Debug.Log("INIT");
        EditorUtility.ClearProgressBar();
        var OBJS = Selection.GetFiltered<GameObject>(SelectionMode.Editable | SelectionMode.TopLevel);
        int i = 0;
        foreach (var objGet in OBJS)
        {
            var path = AssetDatabase.GetAssetPath(objGet);
            var obj = (GameObject)PrefabUtility.InstantiatePrefab(objGet);
            PrefabUtility.DisconnectPrefabInstance(obj);
            EditorUtility.DisplayProgressBar("Progress", "Object: " + obj.name, 0);
            var Events = obj.transform.Find(obj.name).GetComponent<AnimationEvents>();
            var PlaneOBJ = obj.transform.FirstOrDefault(
               x => x.name == "v_weapon.flash"
            || x.name == "v_weapon.muzzleflash"
            || x.name == "v_weapon.weapon_flash"
            || x.name == "v_weapon.weapon_muzzleflash"
            || x.name == string.Format("v_weapon.{0}_flash", obj.name.Replace("mach_", ""))
            || x.name == string.Format("v_weapon.{0}_flash", obj.name.Replace("shot_", ""))
            || x.name == string.Format("v_weapon.{0}_flash", obj.name.Replace("pist_", ""))
            || x.name == string.Format("v_weapon.{0}_flash", obj.name.Replace("rif_", ""))
            || x.name == string.Format("v_weapon.{0}_flash", obj.name.Replace("snip_", ""))
            || x.name == string.Format("v_weapon.{0}_flash", obj.name.Replace("smg_", ""))

            || x.name == string.Format("v_weapon.{0}_flash", obj.name.Replace("mach_", "").ToUpper())
            || x.name == string.Format("v_weapon.{0}_flash", obj.name.Replace("shot_", "").ToUpper())
            || x.name == string.Format("v_weapon.{0}_flash", obj.name.Replace("pist_", "").ToUpper())
            || x.name == string.Format("v_weapon.{0}_flash", obj.name.Replace("rif_", "").ToUpper())
            || x.name == string.Format("v_weapon.{0}_flash", obj.name.Replace("snip_", "").ToUpper())
            || x.name == string.Format("v_weapon.{0}_flash", obj.name.Replace("smg_", "")).ToUpper());

            var capsulasOBJ = obj.transform.FirstOrDefault(
               x => x.name == "v_weapon.shelleject"
            || x.name == "v_weapon.weapon_shelleject"
            || x.name == "v_weapon.shellEject"
            || x.name == string.Format("v_weapon.{0}_shelleject", obj.name.Replace("mach_", ""))
            || x.name == string.Format("v_weapon.{0}_shelleject", obj.name.Replace("shot_", ""))
            || x.name == string.Format("v_weapon.{0}_shelleject", obj.name.Replace("pist_", ""))
            || x.name == string.Format("v_weapon.{0}_shelleject", obj.name.Replace("rif_", ""))
            || x.name == string.Format("v_weapon.{0}_shelleject", obj.name.Replace("snip_", ""))
            || x.name == string.Format("v_weapon.{0}_shelleject", obj.name.Replace("smg_", ""))

            || x.name == string.Format("v_weapon.{0}_shelleject", obj.name.Replace("mach_", "")).ToUpper()
            || x.name == string.Format("v_weapon.{0}_shelleject", obj.name.Replace("shot_", "")).ToUpper()
            || x.name == string.Format("v_weapon.{0}_shelleject", obj.name.Replace("pist_", "")).ToUpper()
            || x.name == string.Format("v_weapon.{0}_shelleject", obj.name.Replace("rif_", "")).ToUpper()
            || x.name == string.Format("v_weapon.{0}_shelleject", obj.name.Replace("snip_", "")).ToUpper()
            || x.name == string.Format("v_weapon.{0}_shelleject", obj.name.Replace("smg_", "")).ToUpper()

            || x.name == string.Format("v_weapon.{0}_shellEject", obj.name.Replace("mach_", ""))
            || x.name == string.Format("v_weapon.{0}_shellEject", obj.name.Replace("shot_", ""))
            || x.name == string.Format("v_weapon.{0}_shellEject", obj.name.Replace("pist_", ""))
            || x.name == string.Format("v_weapon.{0}_shellEject", obj.name.Replace("rif_", ""))
            || x.name == string.Format("v_weapon.{0}_shellEject", obj.name.Replace("snip_", ""))
            || x.name == string.Format("v_weapon.{0}_shellEject", obj.name.Replace("smg_", ""))

            || x.name == string.Format("v_weapon.{0}_shellEject", obj.name.Replace("mach_", "")).ToUpper()
            || x.name == string.Format("v_weapon.{0}_shellEject", obj.name.Replace("shot_", "")).ToUpper()
            || x.name == string.Format("v_weapon.{0}_shellEject", obj.name.Replace("pist_", "")).ToUpper()
            || x.name == string.Format("v_weapon.{0}_shellEject", obj.name.Replace("rif_", "")).ToUpper()
            || x.name == string.Format("v_weapon.{0}_shellEject", obj.name.Replace("snip_", "")).ToUpper()
            || x.name == string.Format("v_weapon.{0}_shellEject", obj.name.Replace("smg_", "")).ToUpper());

            var PlaneOBJLEFT = obj.transform.FirstOrDefault(x => x.name == "v_weapon.muzzleflash_left");
            var capsulasOBJLEFT = obj.transform.FirstOrDefault(x => x.name == "v_weapon.shelleject_left");

            var PlaneOBJRIGHT = obj.transform.FirstOrDefault(x => x.name == "v_weapon.muzzleflash_right");
            var capsulasOBJRIGHT = obj.transform.FirstOrDefault(x => x.name == "v_weapon.shelleject_right");

            Debug.LogFormat("{0} | Index: {1}", obj.name, i);

            if (PlaneOBJ != null && PlaneOBJLEFT == null)
            {
                Events.MuzzleFlashParent = PlaneOBJ;
                Events.MuzzleFlashRotation = new Vector3(0, 0, 180);
                Events.MuzzleFlashScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
            if (capsulasOBJ != null && capsulasOBJLEFT == null)
            {
                Events.ShellEjectParent = capsulasOBJ;
                Events.ShellEjectRotation = new Vector3(0, -90, 0);
                Events.ShellEjectScale = new Vector3(0.5f, 0.5f, 0.5f);
            }

            if (PlaneOBJLEFT != null)
            {
                Events.MuzzleFlashParent = PlaneOBJLEFT;
                Events.MuzzleFlashRotation = new Vector3(0, 0, 180);
                Events.MuzzleFlashScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
            if (capsulasOBJLEFT != null)
            {
                Events.ShellEjectParent = capsulasOBJLEFT;
                Events.ShellEjectRotation = new Vector3(90, 0, 0);
                Events.ShellEjectScale = new Vector3(0.5f, 0.5f, 0.5f);
            }

            if (PlaneOBJRIGHT != null)
            {
                Events.MuzzleFlashParentSecond = PlaneOBJRIGHT;
                Events.MuzzleFlashRotationSecond = new Vector3(0, 0, 180);
                Events.MuzzleFlashScaleSecond = new Vector3(1.5f, 1.5f, 1.5f);
            }
            if (capsulasOBJRIGHT != null)
            {
                Events.ShellEjectParentSecond = capsulasOBJRIGHT;
                Events.ShellEjectRotationSecond = new Vector3(0, -90, 0);
                Events.ShellEjectScaleSecond = new Vector3(0.5f, 0.5f, 0.5f);
            }

            if ((PlaneOBJ != null && capsulasOBJ != null) || ((PlaneOBJLEFT != null && capsulasOBJLEFT != null) && (PlaneOBJRIGHT != null && capsulasOBJRIGHT != null)))
                PrefabUtility.ReplacePrefab(obj, AssetDatabase.LoadAssetAtPath<GameObject>(path), ReplacePrefabOptions.Default);
            Object.DestroyImmediate(obj);
            i++;
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("SDK/ModelTools/Weapons/Parent effects setup check")]
    static void ParentEffectsCheck()
    {
        Debug.Log("INIT");
        EditorUtility.ClearProgressBar();
        var OBJS = Selection.GetFiltered<GameObject>(SelectionMode.Editable | SelectionMode.TopLevel);
        int i = 0;
        foreach (var objGet in OBJS)
        {
            //var path = AssetDatabase.GetAssetPath(objGet);
            var obj = (GameObject)PrefabUtility.InstantiatePrefab(objGet);
            EditorUtility.DisplayProgressBar("Progress", "Object: " + obj.name, 0);
            var Events = obj.transform.Find(obj.name).GetComponent<AnimationEvents>();

            var PlaneOBJ = Events.MuzzleFlashParent;
            var capsulasOBJ = Events.ShellEjectParent;

            var PlaneOBJRIGHT = Events.MuzzleFlashParentSecond;
            var capsulasOBJRIGHT = Events.ShellEjectParentSecond;

            Debug.LogFormat("{0} | Index: {1}", obj.name, i);

            if (PlaneOBJ != null)
            {
                Debug.Log(obj.name + " / " + PlaneOBJ.name);
            }
            if (capsulasOBJ != null)
            {
                Debug.Log(obj.name + " / " + capsulasOBJ.name);
            }

            if (PlaneOBJRIGHT != null)
            {
                Debug.Log(obj.name + " / " + PlaneOBJRIGHT.name);
            }
            if (capsulasOBJRIGHT != null)
            {
                Debug.Log(obj.name + " / " + capsulasOBJRIGHT.name);
            }

            Object.DestroyImmediate(obj);

            i++;
        }
        EditorUtility.ClearProgressBar();
    }
}
