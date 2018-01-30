using System.Collections.Generic;

namespace TiltShift.EditorUtility
{
    [System.Serializable]
    public class TiltShiftTool
    {
        public string Title;
        public bool ToolEnable;
        public bool FlexibleHotkeys;
        public List<Hotkey> Hotkeys;

        public TiltShiftTool()
        {
            Title = string.Empty;
            Hotkeys = new List<Hotkey>();
        }
    }
}