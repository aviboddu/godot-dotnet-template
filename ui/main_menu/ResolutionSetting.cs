using Godot;
using Utilities;

namespace UI;
public partial class ResolutionSetting : HBoxContainer
{
	[Export]
	OptionButton resolutionDropDown;


	public override void _Ready()
	{
		resolutionDropDown.ItemSelected += _on_value_selected;
		string res = ResolutionToString(VideoManager.Instance.Resolution);
		for (int i = 0; i < resolutionDropDown.ItemCount; i++)
		{
			if (res.Equals(resolutionDropDown.GetItemText(i)))
			{
				resolutionDropDown.Selected = i;
				break;
			}
		}
	}

	public void _on_value_selected(long idx)
	{
		Logger.Instance.WriteInfo($"ResolutionSetting::_on_value_selected({idx}) - User selected resolution {idx}");
		VideoManager.Instance.Resolution = StringToResolution(resolutionDropDown.GetItemText((int)idx));
	}

	private static string ResolutionToString(Vector2I resolution) => $"{resolution.X}x{resolution.Y}";

	private static Vector2I StringToResolution(string s)
	{
		string[] split = s.Split('x', 2);
		return new Vector2I(int.Parse(split[0]), int.Parse(split[1]));
	}
}
