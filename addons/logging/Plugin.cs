#if TOOLS
using Godot;
using Godot.Collections;

namespace Utilities;

[Tool]
public partial class Plugin : EditorPlugin
{
    public override void _EnterTree()
    {
        if (!ProjectSettings.HasSetting(Logger.LOG_LEVEL_SETTING))
            ProjectSettings.SetSetting(Logger.LOG_LEVEL_SETTING, (int)Logger.LogLevel.Debug);
        Dictionary propertyInfo = new()
        {
            { "name", Logger.LOG_LEVEL_SETTING },
            { "type", (int)Variant.Type.Int },
            { "hint", (int)PropertyHint.Enum },
            { "hint_string", "Debug:0,Info:1,Warning:2,Error:3" }
        };
        ProjectSettings.AddPropertyInfo(propertyInfo);
        ProjectSettings.SetInitialValue(Logger.LOG_LEVEL_SETTING, (int)Logger.LogLevel.Info);
        ProjectSettings.SetAsBasic(Logger.LOG_LEVEL_SETTING, true);
    }
}
#endif
