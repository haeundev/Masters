﻿using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kamgam.MeshExtractor
{
    public static class UtilsEditor
    {
		public static bool IsNotInScene(GameObject go)
		{
			return go == null || go.scene == null || !go.scene.isLoaded;
		}

		public static bool IsInScene(GameObject go)
		{
			return !IsNotInScene(go);
		}

        public static bool IsInPrefabStage()
        {
            try
            {
                // This uses UnityEditor.Experimental.SceneManagement.PrefabStageUtility.
#if UNITY_2021_2_OR_NEWER
                var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                return stage != null;
#elif UNITY_2018_3_OR_NEWER
                var stage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                return stage != null;
#else
                return false;
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogError("Prefab Stage object not found. Maybe Unity has changed the namespace of 'PrefabStageUtility' (thrown error: " + e.Message + "). Please let us know of this error (Tools > Smart Ui Selection > Feedback and Support). Assuming we are not in PrefabStage to continue.");
                return false;
            }
        }

        public static GameObject GetPrefabStageRoot()
        {
            try
            {
#if UNITY_2021_2_OR_NEWER
                var root = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;
                return root;
#elif UNITY_2018_3_OR_NEWER
                var root = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;
                return root;
#else
                return null;
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogError("Stage root object not found. Maybe Unity has changed the namespace of 'PrefabStageUtility' (thrown error: " + e.Message + "). Please let us know of this error (Tools > Smart Ui Selection > Feedback and Support).");
                return null;
            }
        }

        public static void SmartDestroy(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                GameObject.DestroyImmediate(obj);
            }
            else
#endif
            {
                GameObject.Destroy(obj);
            }
        }

        public static bool IsLightTheme()
        {
            return !EditorGUIUtility.isProSkin;
        }

        public static string GetProjectDirWithEndingSlash()
        {
            string projectDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "../")).Replace("\\", "/");
            if (projectDir.EndsWith("/"))
                return projectDir;
            else
                return projectDir + "/";
        }

        /// <summary>
        /// Is the files locked or read-only or does not exists at all?
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFileLocked(string path)
        {
            try
            {
                var fileInfo = new FileInfo(path);
                using (FileStream stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }

        public static void DrawBounds(Bounds b, float duration = 0)
        {
            // bottom
            var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
            var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
            var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
            var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

            Debug.DrawLine(p1, p2, Color.blue, duration);
            Debug.DrawLine(p2, p3, Color.red, duration);
            Debug.DrawLine(p3, p4, Color.yellow, duration);
            Debug.DrawLine(p4, p1, Color.magenta, duration);

            // top
            var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
            var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
            var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
            var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

            Debug.DrawLine(p5, p6, Color.blue, duration);
            Debug.DrawLine(p6, p7, Color.red, duration);
            Debug.DrawLine(p7, p8, Color.yellow, duration);
            Debug.DrawLine(p8, p5, Color.magenta, duration);

            // sides
            Debug.DrawLine(p1, p5, Color.white, duration);
            Debug.DrawLine(p2, p6, Color.gray, duration);
            Debug.DrawLine(p3, p7, Color.green, duration);
            Debug.DrawLine(p4, p8, Color.cyan, duration);
        }

        public static void DrawPoints(Vector3[] points, Color color, float duration = 0)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Debug.DrawLine(points[i], points[(i + 1) % points.Length], color, duration);
            }
        }
    }
}