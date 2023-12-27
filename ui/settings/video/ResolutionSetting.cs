using Godot;
using Utilities;

namespace UI;
public partial class ResolutionSetting : VideoDropDownSetting
{
	public override void _Ready()
	{
		Property = "Resolution";

		// Changing window mode can change resolution, so we connect this to reload properties just in case
		VideoManager.Instance.CheckedConnect(VideoManager.SignalName.WindowModeChanged, Callable.From(LoadProperty));
		base._Ready();
	}

	protected override string PropertyToString(Variant prop)
	{
		Vector2I resolution = prop.As<Vector2I>();
		return $"{resolution.X}x{resolution.Y}";
	}

	protected override Variant StringToProperty(string s)
	{
		string[] split = s.Split('x', 2);
		return new Vector2I(int.Parse(split[0]), int.Parse(split[1]));
	}
}
