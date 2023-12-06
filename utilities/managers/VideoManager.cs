using System;
using Godot;

namespace Utilities;
public partial class VideoManager : Node
{
	public enum ScreenMode
	{
		ExclusiveFullscreen,
		Fullscreen,
		BorderlessWindowed,
		Windowed
	};

	private const string VIDEO_SECTION = "Video";

	private Vector2I _resolution;
	public Vector2I Resolution
	{
		get => _resolution;
		set
		{
			_resolution = value;
			GetWindow().Size = value;
			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.Resolution, value);
		}
	}

	private int _refreshRate;
	public int RefreshRate
	{
		get => _refreshRate;
		set
		{
			_refreshRate = value;
			Engine.MaxFps = value;
			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.RefreshRate, value);
		}
	}

	// TODO: More sophisticated window mode using more normal settings (Exclusive fullscreen, Fullscreen, Borderless Windowed, Windowed)
	private ScreenMode _windowMode;
	public ScreenMode WindowMode
	{
		get => _windowMode;
		set
		{
			_windowMode = value;
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
			}
			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.WindowMode, (int)value);
		}
	}

	private DisplayServer.VSyncMode _vsyncMode;
	public DisplayServer.VSyncMode VSyncMode
	{
		get => _vsyncMode;
		set
		{
			_vsyncMode = value;
			DisplayServer.WindowSetVsyncMode(value);
			Configuration.Instance.ChangeSetting(VIDEO_SECTION, PropertyName.VSyncMode, Enum.GetName(value));
		}
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
		ulong ticks = Time.GetTicksMsec();
		DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, true);
		if (Configuration.Instance.HasSection(VIDEO_SECTION))
		{
			Logger.Instance.WriteInfo("VideoManager::_Ready() - Initializing Settings from Configuration");
			Resolution = Configuration.Instance.GetSetting<Vector2I>(VIDEO_SECTION, PropertyName.Resolution);
			RefreshRate = Configuration.Instance.GetSetting<int>(VIDEO_SECTION, PropertyName.RefreshRate);
			WindowMode = IntToScreenMode(Configuration.Instance.GetSetting<int>(VIDEO_SECTION, PropertyName.WindowMode));
			VSyncMode = Enum.Parse<DisplayServer.VSyncMode>(Configuration.Instance.GetSetting<string>(VIDEO_SECTION, PropertyName.VSyncMode));
		}
		else
		{
			Logger.Instance.WriteInfo("VideoManager::_Ready() - Default Initialization");
			Resolution = DisplayServer.ScreenGetSize();
			RefreshRate = 0;
			WindowMode = ScreenMode.Windowed;
			VSyncMode = DisplayServer.VSyncMode.Disabled;
		}
		Logger.Instance.WriteInfo($"VideoManager::_Ready() - Time to Initialize {Time.GetTicksMsec() - ticks}");
	}

	private ScreenMode IntToScreenMode(int i)
	{
		foreach (ScreenMode s in Enum.GetValuesAsUnderlyingType<ScreenMode>())
		{
			if ((int)s == i)
				return s;
		}
		throw new ArgumentException($"VideoManager::IntToScreenMode({i}) - {i} is not an Screen Mode");
	}
}
