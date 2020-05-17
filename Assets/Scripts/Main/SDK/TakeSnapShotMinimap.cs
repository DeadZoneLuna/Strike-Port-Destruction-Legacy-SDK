﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using SPD.EditorAttributes;
#endif

namespace UGUIMiniMap
{
    [ExecuteInEditMode, RequireComponent(typeof(Camera))]
    public class TakeSnapShotMinimap : MonoBehaviour
    {
#if UNITY_EDITOR
        /// <summary>
        /// The width of the output snapshot in pixels (1 - 4096 px)
        /// </summary>
        public int resWidth = 2048;
        /// <summary>
        /// The height of the output snapshot in pixels  (1 - 4096 px)
        /// </summary>
        public int resHeight = 2048;
        /// <summary>
        /// The applied MSAA, possible values are 1,2,4 nad 8
        /// </summary>
        public int msaa = 1;
        [Space(7)]
        [InspectorButton("TakeSnapshot")]
        public string m_TakeSnapShot = "";

        private static string _folderPath = "/GameRes/SDK/Minimaps/";
        public static string FolderPath { get { return TakeSnapShotMinimap._folderPath; } }

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

        private string SnapshotName(int width, int height)
        {
            string levelName = SceneManager.GetActiveScene().name;//Application.loadedLevelName;

            //if in editor, we have to get the name through editor
            if (!Application.isPlaying)
            {
                string[] path = currentScene.Split(char.Parse("/"));
                string[] fileName = path[path.Length - 1].Split(char.Parse("."));
                levelName = fileName[0];
            }

            return string.Format(GetFullFolderPath() + "MapSnapshot_{0}_{1}x{2}.png", levelName, width, height);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetFullFolderPath()
        {
            return Application.dataPath + _folderPath;
        }

        /// <summary>
        /// Takes a map snapshot and saves it
        /// </summary>
        [ContextMenu("Take Snap Shot")]
        public void TakeSnapshot()
        {
            //setup rendertexture
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            rt.antiAliasing = msaa;
            rt.filterMode = FilterMode.Trilinear;
            GetComponent<Camera>().targetTexture = rt;

            //render the texture
            Texture2D snapshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            snapshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            GetComponent<Camera>().targetTexture = null;
            RenderTexture.active = null;
            snapshot.alphaIsTransparency = true;
            byte[] bytes = snapshot.EncodeToPNG();
            DestroyImmediate(rt);
            DestroyImmediate(snapshot);

            //save to the file
            if (!System.IO.Directory.Exists(GetFullFolderPath()))
            {
                Debug.LogError("File path: " + GetFullFolderPath() + " doesn't exist! Create it.");
                System.IO.Directory.CreateDirectory(GetFullFolderPath());
                //return;
            }

            string _fileName = SnapshotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(_fileName, bytes);
            Debug.Log(string.Format("Saved snapshot to: {0}", _fileName), this);
            _fileName = "";
            AssetDatabase.Refresh();
        }
#endif
    }
}