using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
#pragma warning disable 0649 // field is never assigned

namespace TiltShift.EditorUtility.Tools
{
    public class GroupSelected
    {
        static bool ToolEnable;
        static List<Hotkey> Hotkeys;
        static Hotkey group;

        static void Initialize()
        {
            group = Hotkeys[0];
            SceneView.onSceneGUIDelegate += Update;
        }

        // Update is called once per frame
        static void Update(SceneView scene)
        {
            if (!group.Equals(Event.current) || !ToolEnable || Selection.gameObjects.Length == 0) return;

            var firstSelectedObj = Selection.gameObjects.First();
            var parent = new GameObject("Group: " + firstSelectedObj.name).transform;
            parent.parent = firstSelectedObj.transform.parent;

            foreach (var item in Selection.gameObjects)
            {
                Undo.SetTransformParent(item.transform, parent, "Group objects");
                item.transform.parent = parent;
            }

            Debug.Log(string.Format("Grouping. Objects count: {0}. Group name - Group: {1}",
                Selection.gameObjects.Length, firstSelectedObj.name));
        }
    }
}