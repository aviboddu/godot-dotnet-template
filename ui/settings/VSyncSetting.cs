using Godot;
using Utilities;

namespace UI;
public partial class VSyncSetting : HBoxContainer
{
	[Export]
	CheckButton VSyncButton;

	public override void _Ready()
	{
		Callable buttonToggled = new(this, MethodName._on_button_toggled);
		if (!VSyncButton.IsConnected(CheckBox.SignalName.Toggled, buttonToggled))
			VSyncButton.Connect(CheckBox.SignalName.Toggled, buttonToggled);

		bool isVsync = VideoManager.Instance.VSyncMode == DisplayServer.VSyncMode.Enabled;
		VSyncButton.SetPressedNoSignal(isVsync);
	}

	public void _on_button_toggled(bool toggled)
	{
		Logger.Instance.WriteInfo($"VSyncSetting::_on_button_toggled({toggled}) - User toggled VSync");
		if (toggled)
			VideoManager.Instance.SetDeferred(VideoManager.PropertyName.VSyncMode, Variant.From(DisplayServer.VSyncMode.Enabled));
		else
			VideoManager.Instance.SetDeferred(VideoManager.PropertyName.VSyncMode, Variant.From(DisplayServer.VSyncMode.Disabled));
	}
}
