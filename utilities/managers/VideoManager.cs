using System.Diagnostics;
using Godot;

namespace Utilities;
public partial class VideoManager : Node
{
	public enum ScreenMode
	{
		ExclusiveFullscreen,
		Fullscreen,
		BorderlessWindowed,
		Windowed,
		Maximized,
		Minimized
	};

	private const string VIDEO_SECTION = "Video";
	public Vector2I Resolution
	{
		get => GetWindow().Size;
		set => SetResolution(value, false);
	}

	public int RefreshRate
	{
		get => Engine.MaxFps;
		set => SetRefreshRate(value, false);
	}

	public ScreenMode WindowMode
	{
		get => GetScreenMode();
		set => SetWindowMode(value, false);
	}

	public DisplayServer.VSyncMode VSyncMode
	{
		get => DisplayServer.WindowGetVsyncMode();
		set => SetVsyncMode(value, false);
	}

	public static VideoManager Instance { get; private set; }

	public override void _EnterTree()
	{
		if (Instance != null)
			QueueFree(); // The singleton is already loaded, kill this instance
		Instance = this;
	}

	public override void _Ready()
	{
#if DEBUG
		ulong ticks = Time.GetTicksMsec();
#endif

		DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, true);
		GetWindow().Position = Vector2I.Zero;

		if (Configuration.Instance.HasSection(VIDEO_SECTION))
		{
			Logger.WriteInfo("VideoManager::_Ready() - Initializing Settings from Configuration");
			WindowMode = (ScreenMode)Configuration.Instance.GetSetting<int>(VIDEO_SECTION, PropertyName.WindowMode);
			RefreshRate = Configuration.Instance.GetSetting<int>(VIDEO_SECTION, PropertyName.RefreshRate);
			VSyncMode = (DisplayServer.VSyncMode)Configuration.Instance.GetSetting<int>(VIDEO_SECTION, PropertyName.VSyncMode);
			Resolution = Configuration.Instance.GetSetting<Vector2I>(VIDEO_SECTION, PropertyName.Resolution); // Must be after changing window mode, otherwise might be overwritten
		}
		else
		{
			Logger.WriteInfo("VideoManager::_Ready() - Default Initialization");

			// Initialize without calling video configuration methods
			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.WindowMode, (int)GetScreenMode(), false);

			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.RefreshRate, RefreshRate, false);

			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.VSyncMode, (int)VSyncMode, false);

			;
			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.Resolution, GetWindow().Size, false);
		}

#if DEBUG
		Logger.WriteDebug($"VideoManager::_Ready() - Time to Initialize {Time.GetTicksMsec() - ticks} ms");
#endif
	}

	// TODO: Changing resolutions and window mode gets a bit buggy
	private void SetResolution(Vector2I value, bool saveNow)
	{
		Debug.Assert(value.Sign().Equals(Vector2I.One), $"VideoManager::SetResolution({value}, {saveNow}) - {value} must be positive in both axes");
		Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.Resolution, value, saveNow);
		GetWindow().Size = value;
	}

	private void SetRefreshRate(int value, bool saveNow)
	{
		Debug.Assert(value >= 0, $"VideoManager::SetRefreshRate({value}, {saveNow}) - {value} must be greater than or equal to zero");
		Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.RefreshRate, value, saveNow);
		Engine.MaxFps = value;
	}

	private void SetWindowMode(ScreenMode value, bool saveNow)
	{
		switch (value)
		{
			case ScreenMode.ExclusiveFullscreen:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
				break;
			case ScreenMode.Fullscreen:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				break;
			case ScreenMode.BorderlessWindowed:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
				break;
			case ScreenMode.Windowed:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
				break;
			case ScreenMode.Maximized:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
				break;
			default: // Don't want to save any error states or minimized state
				return;
		}
		Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.WindowMode, (int)value, saveNow);
	}

	private void SetVsyncMode(DisplayServer.VSyncMode value, bool saveNow)
	{
		Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.VSyncMode, (int)value, saveNow);
		DisplayServer.WindowSetVsyncMode(value);
	}

	private ScreenMode GetScreenMode()
	{
		Window.ModeEnum modeEnum = GetWindow().Mode;
		switch (modeEnum)
		{
			case Window.ModeEnum.ExclusiveFullscreen:
				return ScreenMode.ExclusiveFullscreen;
			case Window.ModeEnum.Fullscreen:
				return ScreenMode.Fullscreen;
			case Window.ModeEnum.Minimized:
				return ScreenMode.Minimized;
			case Window.ModeEnum.Maximized:
				return ScreenMode.Maximized;
			default:
				if (GetWindow().Borderless) return ScreenMode.BorderlessWindowed;
				return ScreenMode.Windowed;
		}
	}
}
