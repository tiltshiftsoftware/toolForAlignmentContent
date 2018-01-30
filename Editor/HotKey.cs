using UnityEngine;
using System;

namespace TiltShift.EditorUtility
{
    [Serializable]
    public class Hotkey
    {
        public static Hotkey LeftMouse = new Hotkey(0);
        public static Hotkey RightMouse = new Hotkey(1);
        public static Hotkey MiddleMouse = new Hotkey(2);

        EventType EventType;
        public string Title;
        public KeyCode KeyCode;
        public int MouseButton;
        public bool Shift, Control, Alt;


        public Hotkey() : this(string.Empty, KeyCode.None, -1, 0, false, false, false) { }
        public Hotkey(string title) : this(title, KeyCode.None, -1, EventType.KeyUp, false, false, false) { }
        public Hotkey(int mouseButton) : this(string.Empty, KeyCode.None, mouseButton, EventType.MouseDown, false, false, false) { }
        public Hotkey(KeyCode keyCode) : this(string.Empty, keyCode, -1, EventType.KeyUp, false, false, false) { }
        public Hotkey(string title, KeyCode keycode) : this(title, keycode, -1, EventType.KeyUp, false, false, false) { }
        public Hotkey(string title, int mouseButton) : this(title, KeyCode.None, mouseButton, EventType.MouseDown, false, false, false) { }
        public Hotkey(string title, KeyCode keyCode, int mouseButton, EventType eventType, bool shift, bool control, bool alt)
        {
            Title = title;
            KeyCode = keyCode;
            MouseButton = mouseButton;
            EventType = eventType;
            Shift = shift;
            Control = control;
            Alt = alt;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            else if (obj is Hotkey)
            {
                var rhs = obj as Hotkey;
                return KeyCode == rhs.KeyCode && Shift == rhs.Shift && Control == rhs.Control && Alt == rhs.Alt;
            }
            else if (obj is Event)
            {
                var rhs = obj as Event;

                switch (rhs.type)
                {
                    case EventType.KeyUp:
                        return KeyCode != KeyCode.None && KeyCode == rhs.keyCode && Shift == rhs.shift &&
                               Control == rhs.control && Alt == rhs.alt;

                    case EventType.MouseDown:
                        return MouseButton == rhs.button;

                    default:
                        return false;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode() + (int) KeyCode + Convert.ToInt32(Shift) + Convert.ToInt32(Control) +
                   Convert.ToInt32(Alt);
        }

        public override string ToString()
        {
            return string.Format("{0} Keycode: {1}, Shift: {2}, Ctrl: {3}, Alt: {4}, Type: {5}", Title, KeyCode, Shift,
                Control, Alt, EventType);
        }
    }
}