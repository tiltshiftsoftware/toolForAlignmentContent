using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#pragma warning disable 0649 // field is never assigned

namespace TiltShift.EditorUtility.Tools
{
    public class RAMeshTransformator
    {
        static bool ToolEnable;
        static List<Hotkey> Hotkeys;

        static void Initialize()
        {
            SceneView.onSceneGUIDelegate += Update;
        }

        // Update is called once per frame
        static void Update(SceneView scene)
        {
            if (!ToolEnable) return;

            Hotkey hkey = null;

            foreach (var item in Hotkeys)
            {
                if (item.Equals(Event.current))
                {
                    hkey = item;
                    break;
                }
            }

            var transform = Selection.activeTransform;
            if (transform == null || hkey == null) return;

            var subStrings = hkey.Title.Split(' ');
            var command = subStrings[0];
            float numParam = subStrings.Length > 1 ? float.Parse(subStrings[1]) : 0;

            Undo.RecordObject(transform, "Mesh transformator");
            switch (command)
            {
                case "Move_X":
                    transform.localPosition += new Vector3(numParam, 0, 0);
                    break;

                case "Move_Y":
                    transform.localPosition += new Vector3(0, numParam, 0);
                    break;

                case "Move_Z":
                    transform.localPosition += new Vector3(0, 0, numParam);
                    break;

                case "Transform_reset":
                    transform.localPosition = new Vector3();
                    break;

                case "Rotate_X":
                    transform.Rotate(new Vector3(numParam, 0, 0));
                    break;

                case "Rotate_Y":
                    transform.Rotate(new Vector3(0, numParam, 0));
                    break;

                case "Rotate_Z":
                    transform.Rotate(new Vector3(0, 0, numParam));
                    break;

                case "Rotation_reset":
                    transform.rotation = Quaternion.identity;
                    break;

                default:
                    break;
            }
        }
    }
}