using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
#pragma warning disable 0649 // field is never assigned

namespace TiltShift.EditorUtility.Tools
{
    public class RASnapToFloor : MonoBehaviour
    {
        static bool ToolEnable;
        static List<Hotkey> Hotkeys;
        static Hotkey Snap;

        static void Initialize()
        {
            Snap = Hotkeys[0];
            SceneView.onSceneGUIDelegate += Update;
        }

        // Update is called once per frame
        static void Update(SceneView scene)
        {
            if (!ToolEnable || !Snap.Equals(Event.current) || Selection.activeTransform == null) return;

            var transform = Selection.activeTransform;

            Ray ray = new Ray(transform.position, transform.up * -1);
            RaycastHit hitResult;
            if (Physics.Raycast(ray, out hitResult))
            {
                Undo.RecordObject(transform, "Snap to floor");

                transform.position = hitResult.point;

                var normal = hitResult.normal;
                var forward = transform.forward;

                float diff = Mathf.Abs((normal - forward).magnitude - 1);
                if (diff > 0.99)
                    forward = transform.right;

                transform.rotation = Quaternion.LookRotation(forward, normal);
            }
            else
                Debug.Log("RASnapToFloor raycast failed.");
        }
    }
}