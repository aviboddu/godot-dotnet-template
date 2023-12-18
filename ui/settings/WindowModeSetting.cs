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
		VideoManager.ScreenMode mode = prop.As<VideoManager.ScreenMode>();
		return mode switch
		{
			VideoManager.ScreenMode.Fullscreen => FULLSCREEN,
			VideoManager.ScreenMode.ExclusiveFullscreen => EXCLUSIVE_FULLSCREEN,
			VideoManager.ScreenMode.Windowed => WINDOWED,
			VideoManager.ScreenMode.BorderlessWindowed => BORDERLESS_WINDOWED,
			_ => throw new ArgumentException($"WindowModeSetting::ScreenModeToString({mode}) - {mode} was not valid"),
		};
	}

	protected override Variant StringToProperty(string s)
	{
		return s switch
		{
			FULLSCREEN => Variant.From(VideoManager.ScreenMode.Fullscreen),
			EXCLUSIVE_FULLSCREEN => Variant.From(VideoManager.ScreenMode.ExclusiveFullscreen),
			WINDOWED => Variant.From(VideoManager.ScreenMode.Windowed),
			BORDERLESS_WINDOWED => Variant.From(VideoManager.ScreenMode.BorderlessWindowed),
			_ => throw new ArgumentException($"WindowModeSetting::StringToScreenMode({s}) - {s} was not a valid string"),
		};
	}

}
