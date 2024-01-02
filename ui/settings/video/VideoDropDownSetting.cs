using System.Diagnostics;
using Godot;
using Utilities;

namespace UI;

// General class to connect drop down to a video setting
public abstract partial class VideoDropDownSetting : Node
{
    [Export(PropertyHint.NodePathValidTypes, "OptionButton")]
    public NodePath DropDown;

    protected OptionButton DropDownSetting;

    protected string Property;

    public override void _Ready()
    {
        Debug.Assert(Property is not null,
                     "VideoDropDownSettings::_Ready() - property must not be null. Override _Ready() accordingly");

        DropDownSetting = GetNode<OptionButton>(DropDown);
        LoadProperty();
        DropDownSetting.CheckedConnect(OptionButton.SignalName.ItemSelected, Callable.From<long>(_on_value_selected));
    }

    protected void LoadProperty()
    {
        string res = PropertyToString(VideoManager.Instance.GetIndexed(Property));
        for (int i = 0; i < DropDownSetting.ItemCount; i++)
        {
            if (res.Equals(DropDownSetting.GetItemText(i)))
            {
                DropDownSetting.Selected = i;
                break;
            }
        }
    }

    public void _on_value_selected(long idx)
    {
        Logger.WriteInfo($"{GetClass()}::_on_value_selected({idx}) - User selected resolution {idx}");
        VideoManager.Instance.SetDeferred(Property, StringToProperty(DropDownSetting.GetItemText((int)idx)));
    }

    protected abstract string PropertyToString(Variant property);
    protected abstract Variant StringToProperty(string s);
}
