namespace UI;
public partial class MaximumFramerateSetting : VideoDropDownSetting<int>
{
	private const string UNLIMITED = "Unlimited";

	public override void _Ready()
	{
		property = "RefreshRate";
		base._Ready();
	}

	protected override string PropertyToString(int refreshRate)
	{
		if (refreshRate == 0)
			return UNLIMITED;
		return refreshRate.ToString();
	}

	protected override int StringToProperty(string s)
	{
		if (UNLIMITED.Equals(s))
			return 0;
		return int.Parse(s);
	}

}
