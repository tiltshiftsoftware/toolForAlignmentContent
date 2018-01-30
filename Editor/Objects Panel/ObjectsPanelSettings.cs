using UnityEngine;
using UnityEditor;
using System.Linq;

namespace TiltShift.EditorUtility
{
    public class ObjectsPanelSettings : EditorWindow
    {
        static bool[] markupFoldout;

        public static void ShowWindow()
        {
            GetWindow<ObjectsPanelSettings>();
            markupFoldout = Enumerable.Repeat(true, TiltShiftExtentionManager.Tools.Count).ToArray();
        }


        private void OnGUI()
        {
            var tools = TiltShiftExtentionManager.Tools;

            if (tools == null)
            {
                EditorGUILayout.HelpBox("Loading params...", MessageType.Info);
                return;
            }
            for (int i = 0; i < tools.Count; i++)
            {
                EditorGUI.indentLevel = 0;
                if (markupFoldout[i] = EditorGUILayout.Foldout(markupFoldout[i], tools[i].Title))
                {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.indentLevel = 1;
                    tools[i].ToolEnable = EditorGUILayout.Toggle("Tool enable", tools[i].ToolEnable);

                    if (tools[i].FlexibleHotkeys)
                    {
                        int count = EditorGUILayout.IntField("Hotkeys count: ", tools[i].Hotkeys.Count);

                        if (count != tools[i].Hotkeys.Count)
                        {
                            if (count > tools[i].Hotkeys.Count)
                                tools[i]
                                    .Hotkeys.AddRange(Enumerable.Repeat(new Hotkey(KeyCode.None),
                                        count - tools[i].Hotkeys.Count));
                            else
                                tools[i].Hotkeys.RemoveRange(count, tools[i].Hotkeys.Count - count);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    for (int j = 0; j < tools[i].Hotkeys.Count; j++)
                    {
                        var hotkey = tools[i].Hotkeys[j];

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(20);

                        GUILayout.Label("Title: ", GUILayout.MaxWidth(60));
                        GUILayout.FlexibleSpace();
                        hotkey.Title = EditorGUILayout.TextField(hotkey.Title);

                        GUILayout.Label("Keycode: ", GUILayout.MaxWidth(60));
                        GUILayout.FlexibleSpace();
                        hotkey.KeyCode = (KeyCode) EditorGUILayout.EnumPopup(hotkey.KeyCode);

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(EditorGUIUtility.currentViewWidth / 2);

                        GUILayout.FlexibleSpace();

                        EditorGUILayout.LabelField("Shift", GUILayout.Width(50));
                        hotkey.Shift = EditorGUILayout.Toggle(hotkey.Shift);

                        EditorGUILayout.LabelField("Ctrl", GUILayout.Width(40));
                        hotkey.Control = EditorGUILayout.Toggle(hotkey.Control);

                        EditorGUILayout.LabelField("Alt", GUILayout.Width(40));
                        hotkey.Alt = EditorGUILayout.Toggle(hotkey.Alt);

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Space();
                    }
                }
            }



            float buttonWidth = (position.width - 20) / 3;
            float buttonHeight = 20;

            float buttomSpace = position.height - buttonHeight - 10;

            Rect bl = new Rect(10, buttomSpace, buttonWidth, buttonHeight);
            Rect bc = new Rect(10 + buttonWidth, buttomSpace, buttonWidth, buttonHeight);
            Rect br = new Rect(10 + buttonWidth * 2, buttomSpace, buttonWidth, buttonHeight);

            if (GUI.Button(bl, "Ok"))
            {
                TiltShiftExtentionManager.Save();
                Close();
            }
            if (GUI.Button(bc, "Cancel"))
            {
                Close();
            }
            if (GUI.Button(br, "Reset"))
            {
                TiltShiftExtentionManager.Reset();
            }

        }
    }
}