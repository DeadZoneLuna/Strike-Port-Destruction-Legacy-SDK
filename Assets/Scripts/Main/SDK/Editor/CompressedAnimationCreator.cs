using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;

/// <summary>
/// Nash
/// </summary>
public class CompressedAnimationCreator : EditorWindow
{
    [MenuItem("Tools/Compressed Animation Creator")]
    static void MakeWindow()
    {
        window = GetWindow(typeof(CompressedAnimationCreator)) as CompressedAnimationCreator;
        window.oColor = GUI.contentColor;
    }

    private static CompressedAnimationCreator window;
    private Color oColor;
    private Vector2 scrollpos;
    private Dictionary<string, int> frameSkips = new Dictionary<string, int>();
    private Dictionary<string, bool> bakeAnims = new Dictionary<string, bool>();

    [SerializeField]
    private Vector2 scroll;
    [SerializeField]
    private int fps = 30;
    [SerializeField]
    private int previousGlobalBake = 1;
    [SerializeField]
    private int globalBake = 1;
    [SerializeField]
    private int smoothMeshAngle = -1;
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private List<AnimationClip> customClips = new List<AnimationClip>();
    [SerializeField]
    private List<MeshFilter> meshFilters = new List<MeshFilter>();
    [SerializeField]
    private List<SkinnedMeshRenderer> skinnedRenderers = new List<SkinnedMeshRenderer>();
    [SerializeField]
    private GameObject previousPrefab;
    [SerializeField]
    private bool customCompression = false;
    [SerializeField]
    private GameObject spawnedAsset;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private RuntimeAnimatorController animController;
    [SerializeField]
    private Avatar animAvatar;

    private void OnEnable()
    {
        if (prefab == null && Selection.activeGameObject)
        {
            prefab = Selection.activeGameObject;
            OnPrefabChanged();
        }
    }

    private void OnDisable()
    {
        if (spawnedAsset)
        {
            DestroyImmediate(spawnedAsset.gameObject);
        }
    }

    private string GetAssetPath(string s)
    {
        string path = s;
        string[] split = path.Split('\\');
        path = string.Empty;
        int startIndex = 0;
        for (int i = 0; i < split.Length; i++)
        {
            if (split[i] == "Assets")
                break;
            startIndex++;
        }
        for (int i = startIndex; i < split.Length; i++)
            path += split[i] + "\\";
        path = path.TrimEnd("\\".ToCharArray());
        path = path.Replace("\\", "/");
        return path;
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Batch Bake Selected Objects"))
        {
            previousPrefab = null;
            foreach (var obj in Selection.gameObjects)
            {
                try
                {
                    prefab = obj;
                    OnPrefabChanged();
                    var toBakeClips = GetClips();
                    foreach (var clip in toBakeClips)
                    {
                        frameSkips[clip.name] = 1;
                    }
                    CreateSnapshots();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        GUI.skin.label.richText = true;
        scroll = GUILayout.BeginScrollView(scroll);
        {
            EditorGUI.BeginChangeCheck();
            prefab = EditorGUILayout.ObjectField("Asset to Bake", prefab, typeof(GameObject), true) as GameObject;
            if (prefab)
            {
                if (string.IsNullOrEmpty(GetPrefabPath()))
                {
                    DrawText("Cannot find asset path, are you sure this object is a prefab?", Color.red + Color.white * 0.5f);
                    return;
                }
                if (previousPrefab != prefab)
                {
                    OnPrefabChanged();
                }
                if (spawnedAsset == null)
                {
                    OnPrefabChanged();
                }
                animController = EditorGUILayout.ObjectField("Animation Controller", animController, typeof(RuntimeAnimatorController), true) as RuntimeAnimatorController;
                if (animController == null)
                {
                    GUI.skin.label.richText = true;
                    GUILayout.Label("<b>Specify a Animation Controller to auto-populate animation clips</b>");
                }
                
                fps = EditorGUILayout.IntSlider("Bake FPS", fps, 1, 500);

                globalBake = EditorGUILayout.IntSlider("Global Frame Skip", globalBake, 1, fps);

                bool bChange = globalBake != previousGlobalBake;
                previousGlobalBake = globalBake;

                EditorGUILayout.LabelField("Custom Clips");
                for (int i = 0; i < customClips.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        customClips[i] = (AnimationClip)EditorGUILayout.ObjectField(customClips[i], typeof(AnimationClip), false);
                        if (GUILayout.Button("X", GUILayout.Width(32)))
                        {
                            customClips.RemoveAt(i);
                            GUILayout.EndHorizontal();
                            break;
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add Custom Animation Clip"))
                {
                    customClips.Add(null);
                }
                if (GUILayout.Button("Add Selected Animation Clips"))
                {
                    foreach (var o in Selection.objects)
                    {
                        string p = AssetDatabase.GetAssetPath(o);
                        if (string.IsNullOrEmpty(p) == false)
                        {
                            AnimationClip[] clipsToAdd = AssetDatabase.LoadAllAssetRepresentationsAtPath(p).Where(q => q is AnimationClip).Cast<AnimationClip>().ToArray();
                            customClips.AddRange(clipsToAdd);
                        }
                    }
                }
                var clips = GetClips();
                string[] clipNames = bakeAnims.Keys.ToArray();

                bool modified = false;
                scrollpos = GUILayout.BeginScrollView(scrollpos, GUILayout.MinHeight(100), GUILayout.MaxHeight(1000));
                try
                {
                    EditorGUI.indentLevel++;
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Select All", GUILayout.Width(100)))
                        {
                            foreach (var clipName in clipNames)
                                bakeAnims[clipName] = true;
                        }
                        if (GUILayout.Button("Deselect All", GUILayout.Width(100)))
                        {
                            foreach (var clipName in clipNames)
                                bakeAnims[clipName] = false;
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Bake Animation");
                        GUILayout.Label("Frame Skip");
                    }
                    GUILayout.EndHorizontal();
                    foreach (var clipName in clipNames)
                    {
                        if (frameSkips.ContainsKey(clipName) == false)
                            frameSkips.Add(clipName, globalBake);
                        AnimationClip clip = clips.Find(q => q.name == clipName);
                        int framesToBake = clip ? (int)(clip.length * fps / frameSkips[clipName]) : 0;
                        GUILayout.BeginHorizontal();
                        {
                            bakeAnims[clipName] = EditorGUILayout.Toggle(string.Format("{0} ({1} frames)", clipName, framesToBake), bakeAnims[clipName]);
                            GUI.enabled = bakeAnims[clipName];
                            frameSkips[clipName] = Mathf.Clamp(EditorGUILayout.IntField(frameSkips[clipName]), 1, fps);
                            GUI.enabled = true;
                        }
                        GUILayout.EndHorizontal();
                        if (framesToBake > 500)
                        {
                            GUI.skin.label.richText = true;
                            EditorGUILayout.LabelField("<color=red>Long animations degrade performance, consider using a higher frame skip value.</color>", GUI.skin.label);
                        }
                        if (bChange) frameSkips[clipName] = globalBake;
                        if (frameSkips[clipName] != 1)
                            modified = true;
                    }
                    EditorGUI.indentLevel--;
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
                GUILayout.EndScrollView();
                if (modified)
                    DrawText("Skipping more frames during baking will result in a smaller asset size, but potentially degrade animation quality.", Color.yellow);

                GUILayout.Space(10);
                int bakeCount = bakeAnims.Count(q => q.Value);
                GUI.enabled = bakeCount > 0;
                if (GUILayout.Button(string.Format("Generate Snapshots for {0} animation{1}", bakeCount, bakeCount > 1 ? "s" : string.Empty)))
                {
                    CreateSnapshots();
                }
                GUI.enabled = true;
                //SavePreferencesForAsset();
            }
            else // end if valid prefab
            {
                DrawText("Specify a asset to bake.", Color.red + Color.white * 0.5f);
            }
            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                Repaint();
            }
        }
        GUILayout.EndScrollView();
    }

    private void CreateSnapshots()
    {
        UnityEditor.Animations.AnimatorController bakeController = null;
        string assetPath = GetPrefabPath();
        if (string.IsNullOrEmpty(assetPath))
        {
            EditorUtility.DisplayDialog("Mesh Animator", "Unable to locate the asset path for prefab: " + prefab.name, "OK");
            return;
        }

        HashSet<string> allAssets = new HashSet<string>();

        List<AnimationClip> clips = GetClips();
        foreach (var clip in clips)
        {
            allAssets.Add(AssetDatabase.GetAssetPath(clip));
        }

        string[] split = assetPath.Split("/".ToCharArray());

        string assetFolder = string.Empty;
        for (int s = 0; s < split.Length - 1; s++)
        {
            assetFolder += split[s] + "/";
        }

        var sampleGO = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
        if (meshFilters.Count(q => q) == 0 && skinnedRenderers.Count(q => q) == 0)
        {
            throw new System.Exception("Bake Error! No MeshFilter's or SkinnedMeshRender's found to bake!");
        }
        else
        {
            for (int j = 0; j < clips.Count; j++)
            {
                AnimationClip animClip = clips[j];
                EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(animClip);
                foreach (EditorCurveBinding item in curveBindings)
                {
                    string propertyName = item.propertyName.ToLower();
                    if (propertyName.Contains("scale"))
                    {
                        AnimationUtility.SetEditorCurve(animClip, item, null);
                        Debug.Log("animClip " + animClip.name + " remove scale " + propertyName);
                    }
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(animClip, item);
                    if (curve == null || curve.keys == null)
                    {
                        continue;
                    }
                    var keyFrames = curve.keys;
                    for (var i = 0; i < keyFrames.Length; i++)
                    {
                        var key = keyFrames[i];
                        float last = key.value;
                        key.value = float.Parse(key.value.ToString("f3"));                
                        key.inTangent = float.Parse(key.inTangent.ToString("f3"));
                        key.outTangent = float.Parse(key.outTangent.ToString("f3"));
                        keyFrames[i] = key;

                        Debug.Log("keyFrames Reduce accuracy last " + last + " new " + key.value);
                    }
                    curve.keys = keyFrames;
                    animClip.SetCurve(item.path, item.type, item.propertyName, curve);
                }
            }
        }
        AssetDatabase.SaveAssets();
        GameObject.DestroyImmediate(sampleGO);
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Compress Animation", string.Format("Compressed {0} animation{1} successfully!", clips.Count
            , clips.Count > 1 ? "s" : string.Empty), "OK");
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(bakeController));
    }

    private Avatar GetAvatar()
    {
        if (animAvatar)
            return animAvatar;
        var objs = EditorUtility.CollectDependencies(new Object[] { prefab }).ToList();
        foreach (var obj in objs.ToArray())
            objs.AddRange(AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(obj)));
        objs.RemoveAll(q => q is Avatar == false || q == null);
        if (objs.Count > 0)
            animAvatar = objs[0] as Avatar;
        return animAvatar;
    }
    private List<AnimationClip> GetClips()
    {
        var clips = EditorUtility.CollectDependencies(new Object[] { prefab }).ToList();
        foreach (var obj in clips.ToArray())
            clips.AddRange(AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(obj)));
        clips.AddRange(customClips.Select(q => (Object)q));
        clips.RemoveAll(q => q is AnimationClip == false || q == null);
        foreach (AnimationClip clip in clips)
        {
            if (bakeAnims.ContainsKey(clip.name) == false)
                bakeAnims.Add(clip.name, true);
        }
        clips.RemoveAll(q => bakeAnims.ContainsKey(q.name) == false);
        clips.RemoveAll(q => bakeAnims[q.name] == false);

        var distinctClips = clips.Select(q => (AnimationClip)q).Distinct().ToList();

        var humanoidCheck = new List<AnimationClip>(distinctClips);
        if (animController)
        {
            humanoidCheck.AddRange(animController.animationClips);
            distinctClips.AddRange(animController.animationClips);
            distinctClips = distinctClips.Distinct().ToList();
        }

        for (int i = 0; i < distinctClips.Count; i++)
        {
            if (bakeAnims.ContainsKey(distinctClips[i].name) == false)
                bakeAnims.Add(distinctClips[i].name, true);
        }
        return distinctClips;
    }
    private void DrawText(string text, Color color)
    {
        GUI.contentColor = color;
        GUILayout.TextArea(text);
        GUI.contentColor = oColor;
    }
    private string GetPrefabPath()
    {
        string assetPath = AssetDatabase.GetAssetPath(prefab);
        if (string.IsNullOrEmpty(assetPath))
        {
            Object parentObject = PrefabUtility.GetPrefabParent(prefab);
            assetPath = AssetDatabase.GetAssetPath(parentObject);
        }
        return assetPath;
    }

    private void OnPrefabChanged()
    {
        if (spawnedAsset)
            GameObject.DestroyImmediate(spawnedAsset.gameObject);
        if (Application.isPlaying)
        {
            return;
        }
        animator = null;
        animAvatar = null;
        if (prefab)
        {
            if (spawnedAsset == null)
            {
                spawnedAsset = GameObject.Instantiate(prefab) as GameObject;
                SetChildFlags(spawnedAsset.transform, HideFlags.HideAndDontSave);
            }
            bakeAnims.Clear();
            frameSkips.Clear();
            AutoPopulateFiltersAndRenderers();
            AutoPopulateAnimatorAndController();

            //LoadPreferencesForAsset();
        }
        previousPrefab = prefab;
    }

    private void AutoPopulateFiltersAndRenderers()
    {
        meshFilters.Clear();
        skinnedRenderers.Clear();
        MeshFilter[] filtersInPrefab = spawnedAsset.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < filtersInPrefab.Length; i++)
        {
            if (meshFilters.Contains(filtersInPrefab[i]) == false)
                meshFilters.Add(filtersInPrefab[i]);
            if (filtersInPrefab[i].GetComponent<MeshRenderer>())
                filtersInPrefab[i].GetComponent<MeshRenderer>().enabled = false;
        }
        SkinnedMeshRenderer[] renderers = spawnedAsset.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (skinnedRenderers.Contains(renderers[i]) == false)
                skinnedRenderers.Add(renderers[i]);
            renderers[i].enabled = false;
        }
    }
    private void AutoPopulateAnimatorAndController()
    {
        animator = spawnedAsset.GetComponent<Animator>();
        if (animator == null)
            animator = spawnedAsset.GetComponentInChildren<Animator>();
        if (animator && animController == null)
            animController = animator.runtimeAnimatorController;
    }

    private bool IsOptimizedAnimator()
    {
        var i = GetAllImporters();
        if (i.Count > 0)
            return i.Any(q => q.optimizeGameObjects);
        return false;
    }

    private ModelImporter GetImporter(string p)
    {
        return ModelImporter.GetAtPath(p) as ModelImporter;
    }

    private List<ModelImporter> GetAllImporters()
    {
        List<ModelImporter> importers = new List<ModelImporter>();
        importers.Add(GetImporter(GetPrefabPath()));
        foreach (var mf in meshFilters)
        {
            if (mf && mf.sharedMesh)
            {
                importers.Add(GetImporter(AssetDatabase.GetAssetPath(mf.sharedMesh)));
            }
        }
        foreach (var sr in skinnedRenderers)
        {
            if (sr && sr.sharedMesh)
            {
                importers.Add(GetImporter(AssetDatabase.GetAssetPath(sr.sharedMesh)));
            }
        }
        importers.RemoveAll(q => q == null);
        importers = importers.Distinct().ToList();
        return importers;
    }

    private void SetChildFlags(Transform t, HideFlags flags)
    {
        Queue<Transform> q = new Queue<Transform>();
        q.Enqueue(t);
        for (int i = 0; i < t.childCount; i++)
        {
            Transform c = t.GetChild(i);
            q.Enqueue(c);
            SetChildFlags(c, flags);
        }
        while (q.Count > 0)
        {
            q.Dequeue().gameObject.hideFlags = flags;
        }
    }

    private UnityEditor.Animations.AnimatorController CreateBakeController()
    {
        // Creates the controller automatically containing all animation clips
        string tempPath = "Assets/TempBakeController.controller";
        var bakeName = AssetDatabase.GenerateUniqueAssetPath(tempPath);
        var controller = AnimatorController.CreateAnimatorControllerAtPath(bakeName);
        var baseStateMachine = controller.layers[0].stateMachine;
        var clips = GetClips();
        foreach (var clip in clips)
        {
            var state = baseStateMachine.AddState(clip.name);
            state.motion = clip;
        }
        return controller;
    }

    private string FormatClipName(string name)
    {
        string badChars = "!@#$%%^&*()=+}{[]'\";:|";
        for (int i = 0; i < badChars.Length; i++)
        {
            name = name.Replace(badChars[i], '_');
        }
        return name;
    }
}