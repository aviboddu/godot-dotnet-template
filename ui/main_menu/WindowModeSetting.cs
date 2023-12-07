using System;
using Godot;
using Utilities;

namespace UI;
public partial class WindowModeSetting : HBoxContainer
{
	private const string FULLSCREEN = "Fullscreen";
	private const string EXCLUSIVE_FULLSCREEN = "Exclusive Fullscreen";
	private const string WINDOWED = "Windowed";
	private const string BORDERLESS_WINDOWED = "Borderless Windowed";

	[Export]
	OptionButton windowModeDropDown;

	public override void _Ready()
	{
		windowModeDropDown.ItemSelected += _on_value_selected;
		string mode = ScreenModeToString(VideoManager.Instance.WindowMode);
		for (int i = 0; i < windowModeDropDown.ItemCount; i++)
		{
			if (mode.Equals(windowModeDropDown.GetItemText(i)))
			{
				windowModeDropDown.Selected = i;
				break;
			}
		}
	}

	public void _on_value_selected(long idx)
	{
		Logger.Instance.WriteInfo($"WindowModeSetting::_on_value_selected({idx}) - User selected window mode {idx}");
		VideoManager.Instance.WindowMode = StringToScreenMode(windowModeDropDown.GetItemText((int)idx));
	}

	private static VideoManager.ScreenMode StringToScreenMode(string s)
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

	private static string ScreenModeToString(VideoManager.ScreenMode mode)
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
}
