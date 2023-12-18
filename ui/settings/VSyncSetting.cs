using Godot;
using Utilities;

namespace UI;
public partial class VSyncSetting : HBoxContainer
{
	[Export(PropertyHint.NodePathValidTypes, "CheckButton")]
	NodePath VSyncButton;

	private CheckButton VSyncCheckButton;

	public override void _Ready()
	{
		VSyncCheckButton = GetNode<CheckButton>(VSyncButton);
		Callable buttonToggled = new(this, MethodName._on_button_toggled);
		if (!VSyncCheckButton.IsConnected(CheckBox.SignalName.Toggled, buttonToggled))
			VSyncCheckButton.Connect(CheckBox.SignalName.Toggled, buttonToggled);

		bool isVsync = VideoManager.Instance.VSyncMode == DisplayServer.VSyncMode.Enabled;
		VSyncCheckButton.SetPressedNoSignal(isVsync);
	}

	public void _on_button_toggled(bool toggled)
	{
		Logger.WriteInfo($"VSyncSetting::_on_button_toggled({toggled}) - User toggled VSync");
		if (toggled)
			VideoManager.Instance.SetDeferred(VideoManager.PropertyName.VSyncMode, Variant.From(DisplayServer.VSyncMode.Enabled));
		else
			VideoManager.Instance.SetDeferred(VideoManager.PropertyName.VSyncMode, Variant.From(DisplayServer.VSyncMode.Disabled));
	}
}
