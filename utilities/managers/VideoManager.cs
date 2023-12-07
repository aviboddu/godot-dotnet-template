using System;
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

	private Vector2I _resolution = -Vector2I.One; // Deliberately set to bad values
	public Vector2I Resolution
	{
		get => _resolution;
		set => SetResolution(value, true);
	}

	private int _refreshRate = -1;
	public int RefreshRate
	{
		get => _refreshRate;
		set => SetRefreshRate(value, true);
	}

	private ScreenMode _windowMode;
	public ScreenMode WindowMode
	{
		get => _windowMode;
		set => SetWindowMode(value, true);
	}

	private DisplayServer.VSyncMode _vsyncMode;
	public DisplayServer.VSyncMode VSyncMode
	{
		get => _vsyncMode;
		set => SetVsyncMode(value, true);
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
			Logger.Instance.WriteInfo("VideoManager::_Ready() - Initializing Settings from Configuration");
			Resolution = Configuration.Instance.GetSetting<Vector2I>(VIDEO_SECTION, PropertyName.Resolution);
			RefreshRate = Configuration.Instance.GetSetting<int>(VIDEO_SECTION, PropertyName.RefreshRate);
			WindowMode = (ScreenMode)Configuration.Instance.GetSetting<int>(VIDEO_SECTION, PropertyName.WindowMode);
			VSyncMode = (DisplayServer.VSyncMode)Configuration.Instance.GetSetting<int>(VIDEO_SECTION, PropertyName.VSyncMode);
		}
		else
		{
			Logger.Instance.WriteInfo("VideoManager::_Ready() - Default Initialization");
			SetResolution(DisplayServer.ScreenGetSize(), false);
			SetRefreshRate(Engine.MaxFps, false);
			SetWindowMode(GetScreenMode(), false);
			SetVsyncMode(DisplayServer.WindowGetVsyncMode(), false);
			Configuration.Instance.Save(); // We know we're going to change a bunch of settings, so we batch the write.
		}

#if DEBUG
		Logger.Instance.WriteDebug($"VideoManager::_Ready() - Time to Initialize {Time.GetTicksMsec() - ticks} ms");
#endif
	}

	private void SetResolution(Vector2I value, bool saveNow)
	{
		Debug.Assert(value.Sign().Equals(Vector2I.One), $"VideoManager::SetResolution({value}, {saveNow}) - {value} must be positive in both axes");
		Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.Resolution, value, saveNow);
		_resolution = value;
		GetWindow().Size = value;
	}

	private void SetRefreshRate(int value, bool saveNow)
	{
		Debug.Assert(value >= 0, $"VideoManager::SetRefreshRate({value}, {saveNow}) - {value} must be greater than or equal to zero");
		Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.RefreshRate, value, saveNow);
		_refreshRate = value;
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
		_windowMode = value;
	}

	private void SetVsyncMode(DisplayServer.VSyncMode value, bool saveNow)
	{
		Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.VSyncMode, (int)value, saveNow);
		_vsyncMode = value;
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
