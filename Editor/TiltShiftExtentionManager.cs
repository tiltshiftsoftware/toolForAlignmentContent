using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using TiltShift.EditorUtility.Tools;

namespace TiltShift.EditorUtility
{
    [InitializeOnLoad]
    public class TiltShiftExtentionManager
    {
        static List<Type> toolsTypes;
        public static List<TiltShiftTool> Tools;
        static string filePath;

        public delegate void EditorExtentionsEventHandler(object sender);

        public static event EditorExtentionsEventHandler InformationUpdate;

        static void OnInformationUpdate()
        {
            if (InformationUpdate != null)
                InformationUpdate.Invoke(null);
        }


        static TiltShiftExtentionManager()
        {
            toolsTypes = new List<Type>
            {
                typeof(GroupSelected),
                typeof(RAMeshGlued),
                typeof(RAMeshTransformator),
                typeof(RASnapToFloor),
                typeof(ReplaceSelected)
            };

            filePath = Directory.GetCurrentDirectory() + "\\RAToolsCache\\RAToolsParams.xml";
            Load();
        }

        // Setup all tool after loading params from disk
        static void InitializeComponents()
        {
            foreach (var tool in toolsTypes)
            {
                var toolEnable = tool.GetField("ToolEnable", BindingFlags.NonPublic | BindingFlags.Static);
                var hotkeys = tool.GetField("Hotkeys", BindingFlags.NonPublic | BindingFlags.Static);
                var initialize = tool.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static);

                var settings = Find(tool);
                if (settings == null)
                {
                    var newtool = new TiltShiftTool {Title = tool.Name, Hotkeys = new List<Hotkey>()};
                    newtool.Hotkeys.Add(new Hotkey(KeyCode.None));

                    toolEnable.SetValue(null, false);
                    hotkeys.SetValue(null, newtool.Hotkeys);
                    initialize.Invoke(null, null);

                    Tools.Add(newtool);
                }
                else
                {
                    toolEnable.SetValue(null, settings.ToolEnable);
                    hotkeys.SetValue(null, settings.Hotkeys);
                    initialize.Invoke(null, null);
                }
            }
        }


        static List<TiltShiftTool> MakeToolTree()
        {
            List<TiltShiftTool> toolsTree = new List<TiltShiftTool>();

            toolsTree.Add(new TiltShiftTool
            {
                Title = typeof(RAMeshGlued).Name,
                Hotkeys = new List<Hotkey>(),
                FlexibleHotkeys = false
            });
            toolsTree[0].Hotkeys.Add(new Hotkey("Select"));

            toolsTree.Add(new TiltShiftTool
            {
                Title = typeof(RASnapToFloor).Name,
                Hotkeys = new List<Hotkey>(),
                FlexibleHotkeys = false
            });
            toolsTree[1].Hotkeys.Add(new Hotkey("Snap"));

            toolsTree.Add(new TiltShiftTool
            {
                Title = typeof(ReplaceSelected).Name,
                Hotkeys = new List<Hotkey>(),
                FlexibleHotkeys = false
            });
            toolsTree[2].Hotkeys.Add(new Hotkey("Open window"));

            toolsTree.Add(new TiltShiftTool
            {
                Title = typeof(GroupSelected).Name,
                Hotkeys = new List<Hotkey>(),
                FlexibleHotkeys = false
            });
            toolsTree[3].Hotkeys.Add(new Hotkey("Group"));

            toolsTree.Add(new TiltShiftTool
            {
                Title = typeof(RAMeshTransformator).Name,
                Hotkeys = new List<Hotkey>(),
                FlexibleHotkeys = true
            });
            toolsTree[4].Hotkeys.Add(new Hotkey("Move_X 1"));
            toolsTree[4].Hotkeys.Add(new Hotkey("Move_Y 1"));
            toolsTree[4].Hotkeys.Add(new Hotkey("Move_Z 1"));
            toolsTree[4].Hotkeys.Add(new Hotkey("Transform_reset"));
            toolsTree[4].Hotkeys.Add(new Hotkey("Rotate_X 90"));
            toolsTree[4].Hotkeys.Add(new Hotkey("Rotate_Y 90"));
            toolsTree[4].Hotkeys.Add(new Hotkey("Rotate_Z 90"));
            toolsTree[4].Hotkeys.Add(new Hotkey("Rotation_reset"));


            return toolsTree;
        }

        public static void Load()
        {
            // directory check
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\RAToolsCache"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\RAToolsCache");

            StreamReader stream = null;

            try
            {
                stream = new StreamReader(filePath);
                XmlSerializer serializer = new XmlSerializer(typeof(List<TiltShiftTool>));

                Tools = serializer.Deserialize(stream) as List<TiltShiftTool>;
            }
            catch (InvalidOperationException ex)
            {
                Tools = MakeToolTree();
                Debug.LogError(ex.Message);
            }
            catch (FileNotFoundException)
            {
                Tools = MakeToolTree();
                Save();
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            InitializeComponents();
        }

        public static void Save()
        {
            StreamWriter stream = null;

            try
            {
                stream = new StreamWriter(filePath);
                XmlSerializer serializer = new XmlSerializer(typeof(List<TiltShiftTool>));

                serializer.Serialize(stream, Tools);
                OnInformationUpdate();
            }
            catch (InvalidOperationException ex)
            {
                Debug.LogError(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            InitializeComponents();
        }

        public static void Reset()
        {
            Tools = MakeToolTree();
            Save();
        }

        public static TiltShiftTool Find(Type tool)
        {
            var toolName = tool.Name;
            foreach (var item in Tools)
            {
                if (item.Title == toolName)
                    return item;
            }

            return null;
        }
    }
}