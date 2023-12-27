using Godot;
using Utilities;

namespace UI;
public partial class VolumeSetting : HBoxContainer
{
	[Export(PropertyHint.Enum, "MasterVolume,MusicVolume,SfxVolume")]
	string volumeName; // Allows the same class to modify different properties

	private Slider volumeSlider;
	private Label volumeValue;


	public override void _Ready()
	{
		GetNode<Label>("%Volume Label").Text = VolumeToLabel();
		volumeSlider = GetNode<Slider>("%Volume Slider");
		volumeValue = GetNode<Label>("%Volume Value");

		volumeSlider.CheckedConnect(Slider.SignalName.DragEnded, Callable.From<bool>(_on_drag_ended));
		volumeSlider.CheckedConnect(Slider.SignalName.ValueChanged, Callable.From<double>(_on_value_changed));

		float volume = (float)AudioManager.Instance.GetIndexed(volumeName) * 100f;
		volumeSlider.SetValueNoSignal(volume);
		volumeValue.Text = volume.ToString();
	}

	public void _on_drag_ended(bool changed)
	{
		if (!changed)
			return;

		Logger.WriteInfo($"MasterVolumeSetting::_on_drag_ended({changed}) - User changed volume to {volumeSlider.Value}");
		AudioManager.Instance.SetDeferred(volumeName, volumeSlider.Value * 0.01f);
	}

	public void _on_value_changed(double value)
	{
		volumeValue.Text = value.ToString();
	}

	private string VolumeToLabel()
	{
		switch (volumeName)
		{
			case "MasterVolume": return "Master Volume";
			case "MusicVolume": return "Music Volume";
			case "SfxVolume": return "SFX Volume";
			default:
				Logger.WriteError($"VolumeSetting::volumeToLabel - {volumeName} is not one of the possible values");
				return null;
		}
	}
}
