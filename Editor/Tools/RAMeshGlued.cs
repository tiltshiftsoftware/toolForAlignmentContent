using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
#pragma warning disable 0649 // field is never assigned

namespace TiltShift.EditorUtility.Tools
{
    public class RAMeshGlued
    {
        static GameObject gObject;
        static Vector3 lastPosition;
        static int lastLayer;

        static bool ToolEnable;
        static List<Hotkey> Hotkeys;
        static Hotkey Select;

        static void Initialize()
        {
            Select = Hotkeys[0];
            SceneView.onSceneGUIDelegate += Update;
        }


        static void Update(SceneView scene)
        {
            if (!ToolEnable) return;

            if (gObject == null && Select.Equals(Event.current) && Selection.activeObject != null)
            {
                gObject = Selection.activeGameObject;
                lastLayer = gObject.layer;
                lastPosition = gObject.transform.position;

                SetIndicesDFS(gObject.transform, 2);
                Undo.RecordObject(gObject.transform, "Mesh glued");
            }
            else if (gObject != null && Hotkey.LeftMouse.Equals(Event.current))
            {
                SetIndicesDFS(gObject.transform, lastLayer);
                gObject = null;
            }
            else if (gObject != null && Select.Equals(Event.current))
            {
                SetIndicesDFS(gObject.transform, lastLayer);
                gObject.transform.position = lastPosition;
                gObject = null;
            }
            else if (gObject != null) // Moving
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitResult;

                if (Physics.Raycast(ray, out hitResult))
                {
                    var normal = hitResult.normal;
                    var forward = gObject.transform.forward;

                    float diff = Mathf.Abs((normal - forward).magnitude - 1);
                    if (diff > 0.99)
                        forward = gObject.transform.right;

                    gObject.transform.position = hitResult.point;
                    gObject.transform.rotation = Quaternion.LookRotation(forward, normal);
                }
            }
        }

        private static void SetIndicesDFS(Transform gObject, int layer)
        {
            gObject.gameObject.layer = layer;

            foreach (Transform child in gObject.transform)
            {
                child.gameObject.layer = layer;
                if (child.childCount > 0) SetIndicesDFS(child, layer);
            }
        }
    }
}