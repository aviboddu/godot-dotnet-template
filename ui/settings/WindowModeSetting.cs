using System;
using Godot;
using Utilities;

namespace UI;
public partial class WindowModeSetting : VideoDropDownSetting
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

	protected override string PropertyToString(Variant prop)
	{
		VideoManager.WinMode mode = prop.As<VideoManager.WinMode>();
		switch (mode)
		{
			case VideoManager.WinMode.Fullscreen: return FULLSCREEN;
			case VideoManager.WinMode.ExclusiveFullscreen: return EXCLUSIVE_FULLSCREEN;
			case VideoManager.WinMode.Windowed: return WINDOWED;
			case VideoManager.WinMode.BorderlessWindowed: return BORDERLESS_WINDOWED;
			default:
				Logger.WriteError($"WindowModeSetting::ScreenModeToString({mode}) - {mode} was not valid");
				return null;
		}
	}

	protected override Variant StringToProperty(string s)
	{
		switch (s)
		{
			case FULLSCREEN: return Variant.From(VideoManager.WinMode.Fullscreen);
			case EXCLUSIVE_FULLSCREEN: return Variant.From(VideoManager.WinMode.ExclusiveFullscreen);
			case WINDOWED: return Variant.From(VideoManager.WinMode.Windowed);
			case BORDERLESS_WINDOWED: return Variant.From(VideoManager.WinMode.BorderlessWindowed);
			default:
				Logger.WriteError($"WindowModeSetting::StringToScreenMode({s}) - {s} was not a valid string");
				return default;
		};
	}

}
