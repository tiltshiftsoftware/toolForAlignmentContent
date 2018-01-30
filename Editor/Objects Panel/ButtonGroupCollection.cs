using System.Collections.Generic;
using UnityEngine;

namespace TiltShift.EditorUtility
{
    public enum Layout
    {
        Vertical,
        Horizontal,
        Grid
    }

    public enum ButtonContentType
    {
        String,
        Icon,
        Both
    }

    [System.Serializable]
    public class ButtonGroupCollection : ScriptableObject
    {
        public Texture2D SettingsTexture;
        public int Width, Height;
        public List<ButtonGroup> Groups = new List<ButtonGroup>();
    }

    [System.Serializable]
    public struct ButtonGroup
    {
        public string Title;
        public Layout Layout;
        public List<TiltShiftAssetButton> Buttons;
    }

    [System.Serializable]
    public class TiltShiftAssetButton
    {
        public string Title;
        public Texture2D Icon;
        public ButtonContentType ContentType;
        public GameObject Prefab;

        public TiltShiftAssetButton(string title, Texture2D icon, ButtonContentType contentType, GameObject prefab)
        {
            Title = title;
            Icon = icon;
            ContentType = contentType;
            Prefab = prefab;
        }
    }
}