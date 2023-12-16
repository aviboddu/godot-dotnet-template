using Godot;

namespace UI;
public partial class ResolutionSetting : VideoDropDownSetting<Vector2I>
{
	public override void _Ready()
	{
		property = "Resolution";
		base._Ready();
	}

	protected override string PropertyToString(Vector2I resolution) => $"{resolution.X}x{resolution.Y}";

	protected override Vector2I StringToProperty(string s)
	{
		string[] split = s.Split('x', 2);
		return new Vector2I(int.Parse(split[0]), int.Parse(split[1]));
	}
}
