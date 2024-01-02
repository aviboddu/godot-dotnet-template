using Godot;
using Utilities;

namespace UI;

public partial class VSyncSetting : HBoxContainer
{
    private CheckButton _vSyncCheckButton;

    [Export(PropertyHint.NodePathValidTypes, "CheckButton")]
    public NodePath VSyncButton;

    public override void _Ready()
    {
        _vSyncCheckButton = GetNode<CheckButton>(VSyncButton);
        _vSyncCheckButton.CheckedConnect(BaseButton.SignalName.Toggled, Callable.From<bool>(_on_button_toggled));

        bool isVsync = VideoManager.Instance.VSyncMode == DisplayServer.VSyncMode.Enabled;
        _vSyncCheckButton.SetPressedNoSignal(isVsync);
    }

    public void _on_button_toggled(bool toggled)
    {
        Logger.WriteInfo($"VSyncSetting::_on_button_toggled({toggled}) - User toggled VSync");
        VideoManager.Instance.SetDeferred(VideoManager.PropertyName.VSyncMode,
                                          Variant.From(toggled
                                                           ? DisplayServer.VSyncMode.Enabled
                                                           : DisplayServer.VSyncMode.Disabled));
    }
}
