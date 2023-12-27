using Godot;

namespace UI;
public partial class ResolutionSetting : VideoDropDownSetting
{
	public override void _Ready()
	{
		property = "Resolution";
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
