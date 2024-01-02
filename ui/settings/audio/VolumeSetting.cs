using System.Globalization;
using Godot;
using Utilities;

namespace UI;

public partial class VolumeSetting : HBoxContainer
{
    private Slider _volumeSlider;
    private Label _volumeValue;

    [Export(PropertyHint.Enum, "MasterVolume,MusicVolume,SfxVolume")]
    public string VolumeName; // Allows the same class to modify different properties


    public override void _Ready()
    {
        GetNode<Label>("%Volume Label").Text = VolumeToLabel();
        _volumeSlider = GetNode<Slider>("%Volume Slider");
        _volumeValue = GetNode<Label>("%Volume Value");

        _volumeSlider.CheckedConnect(Slider.SignalName.DragEnded, Callable.From<bool>(_on_drag_ended));
        _volumeSlider.CheckedConnect(Range.SignalName.ValueChanged, Callable.From<double>(_on_value_changed));

        int volume = (int)((float)AudioManager.Instance.GetIndexed(VolumeName) * 100f);
        _volumeSlider.SetValueNoSignal(volume);
        _volumeValue.Text = volume.ToString();
    }

    public void _on_drag_ended(bool changed)
    {
        if (!changed)
            return;

        Logger.WriteInfo($"MasterVolumeSetting::_on_drag_ended(true) - User changed volume to {_volumeSlider.Value}");
        AudioManager.Instance.SetDeferred(VolumeName, _volumeSlider.Value * 0.01f);
    }

    public void _on_value_changed(double value)
    {
        _volumeValue.Text = value.ToString(CultureInfo.CurrentCulture);
    }

    private string VolumeToLabel()
    {
        switch (VolumeName)
        {
            case "MasterVolume": return "Master Volume";
            case "MusicVolume": return "Music Volume";
            case "SfxVolume": return "SFX Volume";
            default:
                Logger.WriteError($"VolumeSetting::volumeToLabel - {VolumeName} is not one of the possible values");
                return null;
        }
    }
}
