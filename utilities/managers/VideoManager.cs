using System.Diagnostics;
using Godot;

namespace Utilities;

[DebuggerDisplay("(Resolution={Resolution}, WindowMode={WindowMode}, RefreshRate={RefreshRate}, VSyncMode={VSyncMode})")]
public partial class VideoManager : Node
{
	public static VideoManager Instance { get; private set; }

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

	public enum WinMode : long
	{
		ExclusiveFullscreen = Window.ModeEnum.ExclusiveFullscreen,
		Fullscreen = Window.ModeEnum.Fullscreen,
		Windowed = Window.ModeEnum.Windowed,
		Maximized = Window.ModeEnum.Maximized,
		Minimized = Window.ModeEnum.Minimized,
		BorderlessWindowed = -1
	};

	private const string VIDEO_SECTION = "Video";
	public Vector2I Resolution
	{
		get => GetWindow().Size;
		set
		{
			// TODO: Changing resolutions and window mode gets a bit buggy
			Debug.Assert(value.Sign().Equals(Vector2I.One), $"VideoManager::Resolution = {value} must be positive in both axes");
			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.Resolution, value);
			GetWindow().Size = value;
		}
	}

	public int RefreshRate
	{
		get => Engine.MaxFps;
		set
		{
			Debug.Assert(value >= 0, $"VideoManager::SetRefreshRate({value}) - {value} must be greater than or equal to zero");
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
			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.WindowMode, (int)value);
		}
	}

	public DisplayServer.VSyncMode VSyncMode
	{
		get => DisplayServer.WindowGetVsyncMode();
		set
		{
			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.VSyncMode, (int)value);
			DisplayServer.WindowSetVsyncMode(value);
		}
	}

	public override void _Ready()
	{
#if DEBUG
		ulong ticks = Time.GetTicksMsec();
#endif

		DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, true);
		GetWindow().Position = Vector2I.Zero;
		GetWindow().MinSize = new Vector2I(320, 180);

		if (Configuration.Instance.HasSection(VIDEO_SECTION))
		{
			Logger.WriteInfo("VideoManager::_Ready() - Initializing Settings from Configuration");
			WindowMode = (WinMode)Configuration.Instance.GetSetting<int>(VIDEO_SECTION, PropertyName.WindowMode);
			RefreshRate = Configuration.Instance.GetSetting<int>(VIDEO_SECTION, PropertyName.RefreshRate);
			VSyncMode = (DisplayServer.VSyncMode)Configuration.Instance.GetSetting<int>(VIDEO_SECTION, PropertyName.VSyncMode);
			Resolution = Configuration.Instance.GetSetting<Vector2I>(VIDEO_SECTION, PropertyName.Resolution); // Must be after changing window mode, otherwise might be overwritten
			Configuration.Instance.Flush();
		}
		else
		{
			Logger.WriteInfo("VideoManager::_Ready() - Default Initialization");

			// Initialize without calling video configuration methods
			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.WindowMode, (int)WindowMode);
			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.RefreshRate, RefreshRate);
			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.VSyncMode, (int)VSyncMode);
			Resolution = DisplayServer.ScreenGetSize();
		}

#if DEBUG
		Logger.WriteDebug($"VideoManager::_Ready() - Time to Initialize {Time.GetTicksMsec() - ticks} ms");
#endif
	}
}
