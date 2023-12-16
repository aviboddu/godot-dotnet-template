using System.Diagnostics;
using Godot;
using Utilities;

namespace UI;
public abstract partial class VideoDropDownSetting<[MustBeVariant] T> : Node
{
	[Export]
	public OptionButton dropDown;

	protected string property;

	public override void _Ready()
	{
		Debug.Assert(property is not null, "VideoDropDownSettings::_Ready() - property must not be null. Override _Ready() accordingly");
		dropDown.ItemSelected += _on_value_selected;
		string res = PropertyToString(VideoManager.Instance.GetIndexed(property).As<T>());
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
		Logger.Instance.WriteInfo($"{GetClass()}::_on_value_selected({idx}) - User selected resolution {idx}");
		VideoManager.Instance.SetDeferred(property.ToString(), Variant.From(StringToProperty(dropDown.GetItemText((int)idx))));
	}

	protected abstract string PropertyToString(T property);
	protected abstract T StringToProperty(string s);
}
