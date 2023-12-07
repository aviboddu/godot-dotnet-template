using System;
using Utilities;

namespace UI;
public partial class WindowModeSetting : VideoDropDownSetting<VideoManager.ScreenMode>
{
	private const string FULLSCREEN = "Fullscreen";
	private const string EXCLUSIVE_FULLSCREEN = "Exclusive Fullscreen";
	private const string WINDOWED = "Windowed";
	private const string BORDERLESS_WINDOWED = "Borderless Windowed";

	public override void _Ready()
	{
		property = "WindowMode";
		base._Ready();
	}

	protected override string PropertyToString(VideoManager.ScreenMode mode)
	{
		return mode switch
		{
			VideoManager.ScreenMode.Fullscreen => FULLSCREEN,
			VideoManager.ScreenMode.ExclusiveFullscreen => EXCLUSIVE_FULLSCREEN,
			VideoManager.ScreenMode.Windowed => WINDOWED,
			VideoManager.ScreenMode.BorderlessWindowed => BORDERLESS_WINDOWED,
			_ => throw new ArgumentException($"WindowModeSetting::ScreenModeToString({mode}) - {mode} was not valid"),
		};
	}

	protected override VideoManager.ScreenMode StringToProperty(string s)
	{
		return s switch
		{
			FULLSCREEN => VideoManager.ScreenMode.Fullscreen,
			EXCLUSIVE_FULLSCREEN => VideoManager.ScreenMode.ExclusiveFullscreen,
			WINDOWED => VideoManager.ScreenMode.Windowed,
			BORDERLESS_WINDOWED => VideoManager.ScreenMode.BorderlessWindowed,
			_ => throw new ArgumentException($"WindowModeSetting::StringToScreenMode({s}) - {s} was not a valid string"),
		};
	}

}
