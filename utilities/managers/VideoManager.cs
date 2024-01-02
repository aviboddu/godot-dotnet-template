using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

namespace Utilities;

[DebuggerDisplay(
    "(Resolution={Resolution}, WindowMode={WindowMode}, RefreshRate={RefreshRate}, VSyncMode={VSyncMode})")]
public partial class VideoManager : Node
{
    [Signal]
    public delegate void WindowModeChangedEventHandler();

    public enum WinMode : long
    {
        ExclusiveFullscreen = Window.ModeEnum.ExclusiveFullscreen,
        Fullscreen = Window.ModeEnum.Fullscreen,
        Windowed = Window.ModeEnum.Windowed,
        Maximized = Window.ModeEnum.Maximized,
        Minimized = Window.ModeEnum.Minimized,
        BorderlessWindowed = -1
    }

    private const string VIDEO_SECTION = "Video";
    public static VideoManager Instance { get; private set; }

    public Vector2I Resolution
    {
        get => GetWindow().GetSizeWithDecorations();
        set
        {
            if (value == Resolution) return;
            Debug.Assert(value.Sign().Equals(Vector2I.One),
                         $"VideoManager::Resolution = {value} must be positive in both axes");
            Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.Resolution, value);
            GetWindow().Size = value;
        }
    }

    public int RefreshRate
    {
        get => Engine.MaxFps;
        set
        {
            if (value == RefreshRate) return;
            Debug.Assert(
                value >= 0, $"VideoManager::SetRefreshRate({value}) - {value} must be greater than or equal to zero");
            Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.RefreshRate, value);
            Engine.MaxFps = value;
        }
    }

    public WinMode WindowMode
    {
        get
        {
            Window.ModeEnum modeEnum = GetWindow().Mode;
            switch (modeEnum)
            {
                case Window.ModeEnum.Windowed:
                    if (GetWindow().Borderless) return WinMode.BorderlessWindowed;
                    return WinMode.Windowed;
                default:
                    return (WinMode)modeEnum;
            }
        }
        set
        {
            if (value == WindowMode) return;
            switch (value)
            {
                case WinMode.BorderlessWindowed:
                    GetWindow().Mode = Window.ModeEnum.Windowed;
                    GetWindow().Borderless = true;
                    break;
                case WinMode.Windowed:
                    GetWindow().Mode = Window.ModeEnum.Windowed;
                    GetWindow().Borderless = false;
                    break;
                default:
                    GetWindow().Mode = (Window.ModeEnum)value;
                    break;
            }

            EmitSignal(SignalName.WindowModeChanged);
            Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.WindowMode, (int)value);
        }
    }

    public DisplayServer.VSyncMode VSyncMode
    {
        get => DisplayServer.WindowGetVsyncMode();
        set
        {
            if (value == VSyncMode) return;
            Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.VSyncMode, (int)value);
            DisplayServer.WindowSetVsyncMode(value);
        }
    }

    public override void _EnterTree()
    {
        if (Instance is not null)
        {
            QueueFree();
            return;
        }

        Instance = this;
        base._EnterTree();
    }

    public override void _Ready()
    {
        ulong ticks = Time.GetTicksMsec();

        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, true);
        GetWindow().Position = Vector2I.Zero;
        GetWindow().MinSize = new Vector2I(320, 180);

        if (Configuration.Instance.HasSection(VIDEO_SECTION))
        {
            Logger.WriteInfo("VideoManager::_Ready() - Initializing Settings from Configuration");

            HashSet<string> cmdArgs = ParseCmdKeys();
            if (!cmdArgs.Contains("resolution"))
                Resolution = Configuration.Instance.GetSetting<Vector2I>(VIDEO_SECTION, PropertyName.Resolution);

            string[] windowModeRelatedArgs = ["f", "w", "m", "fullscreen", "windowed", "maximized"];
            if (!windowModeRelatedArgs.Any(cmdArgs.Contains))
                WindowMode = (WinMode)Configuration.Instance.GetSetting<int>(VIDEO_SECTION, PropertyName.WindowMode);

            if (!cmdArgs.Contains("fixed-fps"))
                RefreshRate = Configuration.Instance.GetSetting<int>(VIDEO_SECTION, PropertyName.RefreshRate);

            if (!cmdArgs.Contains("disable-vsync"))
                VSyncMode =
                    (DisplayServer.VSyncMode)Configuration.Instance.GetSetting<int>(
                        VIDEO_SECTION, PropertyName.VSyncMode);
        }
        else
        {
            Logger.WriteInfo("VideoManager::_Ready() - Default Initialization");

            // Initialize without calling video configuration methods
            Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.WindowMode, (int)WindowMode);
            Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.RefreshRate, RefreshRate);
            Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.VSyncMode, (int)VSyncMode);
            Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.Resolution, Resolution);
            Configuration.Instance.Flush();
        }

        Logger.WriteDebug($"VideoManager::_Ready() - Time to Initialize {Time.GetTicksMsec() - ticks} ms");
    }

    private static HashSet<string> ParseCmdKeys()
    {
        HashSet<string> cmd = new(OS.GetCmdlineArgs().Length + OS.GetCmdlineUserArgs().Length);
        foreach (string s in OS.GetCmdlineArgs())
            cmd.Add(ParseArgKey(s));
        foreach (string s in OS.GetCmdlineUserArgs())
            cmd.Add(ParseArgKey(s));
        return cmd;
    }

    private static string ParseArgKey(string s)
    {
        int equalsIndex = s.IndexOf('=');

        s = s.TrimStart('-');
        if (equalsIndex != -1)
            return s[..(equalsIndex - 1)];
        return s;
    }
}
