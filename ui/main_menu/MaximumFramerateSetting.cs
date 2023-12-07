using Godot;
using Utilities;

namespace UI;
public partial class MaximumFramerateSetting : HBoxContainer
{
	private const string UNLIMITED = "Unlimited";

	[Export]
	OptionButton maxFPSDropDown;

	public override void _Ready()
	{
		maxFPSDropDown.ItemSelected += _on_value_selected;

		string MaxFPS = RefreshRateToString(VideoManager.Instance.RefreshRate);
		for (int i = 0; i < maxFPSDropDown.ItemCount; i++)
		{
			if (MaxFPS.Equals(maxFPSDropDown.GetItemText(i)))
			{
				maxFPSDropDown.Selected = i;
				break;
			}
		}
	}

	public void _on_value_selected(long idx)
	{
		Logger.Instance.WriteInfo($"MaximumFramerateSetting::_on_value_selected({idx}) - User selected maximum framerate {idx}");
		VideoManager.Instance.RefreshRate = StringToRefreshRate(maxFPSDropDown.GetItemText((int)idx));
	}

	public static string RefreshRateToString(int refreshRate)
	{
		if (refreshRate == 0)
			return UNLIMITED;
		return refreshRate.ToString();
	}

	public static int StringToRefreshRate(string s)
	{
		if (UNLIMITED.Equals(s))
			return 0;
		return int.Parse(s);
	}
}
