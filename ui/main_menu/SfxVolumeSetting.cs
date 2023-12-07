using Godot;
using Utilities;

namespace UI;
public partial class SfxVolumeSetting : HBoxContainer
{
	[Export]
	Slider volumeSlider;

	[Export]
	Label volumeLabel;

	private int volume;


	public override void _Ready()
	{
		volumeSlider.DragEnded += _on_drag_ended;
		volumeSlider.ValueChanged += _on_value_changed;

		volume = Mathf.RoundToInt(AudioManager.Instance.SfxVolume * 100);
		volumeSlider.SetValueNoSignal(volume);
		volumeLabel.Text = volume.ToString();
	}

	public void _on_drag_ended(bool changed)
	{
		if (!changed)
			return;

		Logger.Instance.WriteInfo($"SfxVolumeSetting::_on_drag_ended({changed}) - User changed volume to {volumeSlider.Value}");
		volume = Mathf.RoundToInt(volumeSlider.Value);
		AudioManager.Instance.SfxVolume = volume / 100f;
	}

	public void _on_value_changed(double value)
	{
		volumeLabel.Text = Mathf.RoundToInt(value).ToString();
	}
}
