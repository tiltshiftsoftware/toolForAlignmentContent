using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace TiltShift.EditorUtility
{
    public class ObjectsPanel : EditorWindow
    {
        [MenuItem("Window/TiltShift objects panel")]
        public static void ShowWindow()
        {
            GetWindow<ObjectsPanel>("Objects panel");
        }

        ButtonGroupCollection deviceButtons;
        bool[] devicesFoldout;

        Texture2D settingsIcon;

        GameObject CurrentAsset;
        int selectedCount;

        private void OnEnable()
        {
            var tempButtonsCollection = FindAssetsByType<ButtonGroupCollection>();
            if (tempButtonsCollection.Count > 0)
                deviceButtons = tempButtonsCollection[0];
            else
                deviceButtons = CreateInstance<ButtonGroupCollection>();
            
            settingsIcon = deviceButtons.SettingsTexture;
            devicesFoldout = Enumerable.Repeat(true, deviceButtons.Groups.Count).ToArray();
        }

        private void OnGUI()
        {
            float windowCenter = position.width / 2;

            for (int i = 0; i < deviceButtons.Groups.Count; i++)
            {
                EditorGUI.indentLevel = 0;
                if (devicesFoldout[i] = EditorGUILayout.Foldout(devicesFoldout[i], deviceButtons.Groups[i].Title))
                {
                    EditorGUI.indentLevel = 1;
                    if (i == deviceButtons.Groups.Count - 1)
                        deviceButtons.Groups[i].Buttons[0].Prefab =
                            EditorGUILayout.ObjectField(deviceButtons.Groups[i].Buttons[0].Prefab,
                                typeof(GameObject)) as GameObject;

                    // Draw device buttons
                    BeginLayout(deviceButtons.Groups[i].Layout);
                    for (int j = 0; j < deviceButtons.Groups[i].Buttons.Count; j++)
                    {
                        DrawButton(deviceButtons.Groups[i].Buttons[j]);
                    }
                    EndLayout(deviceButtons.Groups[i].Layout);
                }
            }

            // Bottom tools
            int butSize = 35;
            if (GUI.Button(new Rect(windowCenter - butSize / 2, position.height - butSize - 30, butSize, butSize),
                settingsIcon)) //( 0, position.height - 45, 45, 45), settingsIcon))
                ObjectsPanelSettings.ShowWindow();


            int labelWidth = 85;
            Rect selectedRect = new Rect(windowCenter - labelWidth / 2, position.height - 20, labelWidth, 20);
            GUI.Label(selectedRect, "Selected: " + selectedCount);
        }


        void BeginLayout(Layout layout)
        {
            switch (layout)
            {
                case Layout.Vertical:
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(position.width / 2 - deviceButtons.Width / 2);
                    EditorGUILayout.BeginVertical();
                    break;
                case Layout.Horizontal:
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(position.width / 2 - deviceButtons.Width / 2);
                    break;
                case Layout.Grid:
                    break;
                default:
                    EditorGUILayout.BeginVertical();
                    break;
            }
        }
        void EndLayout(Layout layout)
        {
            switch (layout)
            {
                case Layout.Vertical:
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    break;
                case Layout.Horizontal:
                    EditorGUILayout.EndHorizontal();
                    break;
                case Layout.Grid:
                    break;
                default:
                    EditorGUILayout.EndVertical();
                    break;
            }
        }

        GUIContent CreateContent(TiltShiftAssetButton button)
        {
            GUIContent content;

            switch (button.ContentType)
            {
                case ButtonContentType.String:
                    content = new GUIContent(button.Title);
                    break;

                case ButtonContentType.Icon:
                    content = new GUIContent(button.Icon);
                    break;

                case ButtonContentType.Both:
                    content = new GUIContent(button.Title, button.Icon);
                    break;

                default:
                    content = new GUIContent(button.Title);
                    break;
            }

            return content;
        }

        void DrawButton(TiltShiftAssetButton button)
        {
            if (button.Prefab == CurrentAsset && button.Prefab != null)
                GUI.backgroundColor = Color.green;
            else
                GUI.backgroundColor = Color.gray;


            if (GUILayout.Button(CreateContent(button), GUILayout.Width(deviceButtons.Width),
                GUILayout.Height(deviceButtons.Height)))
            {
                if (CurrentAsset == button.Prefab)
                {
                    CurrentAsset = null;
                }
                else
                {
                    CurrentAsset = button.Prefab;
                }
            }
            GUI.backgroundColor = Color.gray;
        }



        private void OnSceneGUI(SceneView scene)
        {
            if (selectedCount != Selection.gameObjects.Length)
            {
                selectedCount = Selection.gameObjects.Length;
                Repaint();
            }


            if (CurrentAsset == null) return;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (Hotkey.LeftMouse.Equals(Event.current))
            {
                Transform parrent = GetParrentObject();

                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                Vector3 spawnPosition;
                Quaternion spawnRotation;

                RaycastHit hitResult;
                if (Physics.Raycast(ray, out hitResult, 1000))
                {
                    spawnPosition = hitResult.point;

                    var normal = hitResult.normal;
                    var forward = CurrentAsset.transform.forward;


                    float diff = Mathf.Abs((normal - forward).magnitude - 1);
                    if (diff > 0.99)
                        forward = CurrentAsset.transform.right;

                    spawnRotation = Quaternion.LookRotation(forward, normal);
                }
                else
                {
                    spawnPosition = scene.pivot;
                    spawnRotation = Quaternion.identity;
                }

                Instantiate(CurrentAsset, spawnPosition, spawnRotation, parrent);
            }
            else if (Hotkey.RightMouse.Equals(Event.current))
            {
                CurrentAsset = null;
                Repaint();
            }
        }

        private Transform GetParrentObject()
        {
            Transform entities;
            var entitiesObj = GameObject.Find("Entities");
            if (entitiesObj != null)
                entities = entitiesObj.transform;
            else
                entities = new GameObject("Entities").transform;


            var parrent = entities.Find(CurrentAsset.name);
            if (parrent == null)
            {
                parrent = new GameObject(CurrentAsset.name).transform;
                parrent.parent = entities;
            }

            return parrent;
        }

        private void OnFocus()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        private void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        private List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }
    }
}