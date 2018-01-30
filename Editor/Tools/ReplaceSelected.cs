using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
#pragma warning disable 0649 // field is never assigned

namespace TiltShift.EditorUtility.Tools
{
    public class ReplaceSelected : EditorWindow
    {
        static bool ToolEnable;
        static List<Hotkey> Hotkeys;
        static Hotkey ShowWindow;

        bool MoveToEntities;
        GameObject replaceObj;

        static void Initialize()
        {
            ShowWindow = Hotkeys[0];
            SceneView.onSceneGUIDelegate += Update;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Replace by:");
            replaceObj = EditorGUILayout.ObjectField(replaceObj, typeof(GameObject)) as GameObject;
            MoveToEntities = EditorGUILayout.Toggle("Move to entities", MoveToEntities);
            EditorGUILayout.Space();

            if (GUILayout.Button("Replace"))
            {
                if (replaceObj != null || Selection.gameObjects.Length > 0)
                {
                    int totalCount = Selection.gameObjects.Length;
                    Transform parentEntities = null;
                    if (MoveToEntities)
                        parentEntities = Selection.gameObjects.First().transform.GetParentEntities();

                    foreach (var item in Selection.gameObjects)
                    {
                        GameObject spawnObj;
                        var obj = PrefabUtility.GetPrefabParent(replaceObj);

                        if (obj != null)
                            spawnObj = PrefabUtility.InstantiatePrefab(obj) as GameObject;
                        else
                            spawnObj = Instantiate(replaceObj);

                        spawnObj.transform.position = item.transform.position;
                        spawnObj.transform.rotation = item.transform.rotation;
                        spawnObj.transform.localScale = Vector3.one;
                        if (MoveToEntities)
                            spawnObj.transform.parent = parentEntities;
                        else
                            spawnObj.transform.parent = item.transform.parent;

                        Undo.RegisterCreatedObjectUndo(spawnObj, "Replace selected: spawn");
                        Undo.DestroyObjectImmediate(item);
                    }
                    replaceObj = null;
                    Close();

                    Debug.Log(string.Format("Replacing. Objects count: {0}.", totalCount));
                }
                else
                    Debug.Log("Replace selected warning: select substitutable object!");
            }
        }

        static void Update(SceneView scene)
        {
            if (!ShowWindow.Equals(Event.current) || !ToolEnable) return;

            var window = GetWindow<ReplaceSelected>();

            int width = 300, height = 50;
            float x = scene.position.x + scene.position.width - width;
            float y = scene.position.y + height;

            window.position = new Rect(x, y, width, height);
        }
    }

    public static partial class Extentions
    {
        public static Transform GetParentEntities(this Transform transform)
        {
            Transform entities;
            var entitiesObj = GameObject.Find("Entities");

            if (entitiesObj != null)
                entities = entitiesObj.transform;
            else
                entities = new GameObject("Entities").transform;


            var category = entities.Find(transform.name + "'s");
            if (category == null)
            {
                category = new GameObject(transform.name + "'s").transform;
                category.parent = entities;
            }

            var parentObj = new GameObject(transform.name);
            parentObj.transform.parent = category;

            return parentObj.transform;
        }
    }
}
