using Godot;

namespace UI;
public partial class MaximumFramerateSetting : VideoDropDownSetting
{
	private const string UNLIMITED = "Unlimited";

	public override void _Ready()
	{
		property = "RefreshRate";
		base._Ready();
	}

	protected override string PropertyToString(Variant refreshRate)
	{
		if (refreshRate.AsInt32() == 0)
			return UNLIMITED;
		return refreshRate.ToString();
	}

	protected override Variant StringToProperty(string s)
	{
		if (UNLIMITED.Equals(s))
			return 0;
		return int.Parse(s);
	}

}
