using System.Diagnostics;
using Godot;
using Utilities;

namespace UI;
public abstract partial class VideoDropDownSetting : Node
{
	[Export]
	public OptionButton dropDown;

	protected string property;

	public override void _Ready()
	{
		Debug.Assert(property is not null, "VideoDropDownSettings::_Ready() - property must not be null. Override _Ready() accordingly");

		Callable valueSelected = new(this, MethodName._on_value_selected);
		if (!dropDown.IsConnected(OptionButton.SignalName.ItemSelected, valueSelected))
			dropDown.Connect(OptionButton.SignalName.ItemSelected, valueSelected);

		string res = PropertyToString(VideoManager.Instance.GetIndexed(property));
		for (int i = 0; i < dropDown.ItemCount; i++)
		{
			if (res.Equals(dropDown.GetItemText(i)))
			{
				dropDown.Selected = i;
				break;
			}
		}
	}

	public void _on_value_selected(long idx)
	{
		Logger.WriteInfo($"{GetClass()}::_on_value_selected({idx}) - User selected resolution {idx}");
		VideoManager.Instance.SetDeferred(property, StringToProperty(dropDown.GetItemText((int)idx)));
	}

	protected abstract string PropertyToString(Variant property);
	protected abstract Variant StringToProperty(string s);
}
