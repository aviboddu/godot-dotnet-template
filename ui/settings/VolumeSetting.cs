using System;
using Godot;
using Utilities;

namespace UI;
public partial class VolumeSetting : HBoxContainer
{

	public enum VolumeSettingEnum
	{
		MasterVolume,
		MusicVolume,
		SfxVolume
	};

	[Export]
	Slider volumeSlider;

	[Export]
	Label volumeLabel;

	[Export]
	VolumeSettingEnum volumeName;

	private int volume;


	public override void _Ready()
	{
		Callable dragEnded = new(this, MethodName._on_drag_ended);
		Callable valueChanged = new(this, MethodName._on_value_changed);
		Logger.Instance.WriteInfo(Slider.SignalName.DragEnded);
		Logger.Instance.WriteInfo(dragEnded);
		Logger.Instance.WriteInfo(volumeSlider);
		if (volumeSlider.IsConnected(Slider.SignalName.DragEnded, dragEnded))
			volumeSlider.Connect(Slider.SignalName.DragEnded, dragEnded);
		if (volumeSlider.IsConnected(Slider.SignalName.ValueChanged, valueChanged))
			volumeSlider.Connect(Slider.SignalName.ValueChanged, valueChanged);

		volume = Mathf.RoundToInt((float)AudioManager.Instance.GetIndexed(Enum.GetName(volumeName)) * 100);
		volumeSlider.SetValueNoSignal(volume);
		volumeLabel.Text = volume.ToString();
	}

	public void _on_drag_ended(bool changed)
	{
		if (!changed)
			return;

		Logger.Instance.WriteInfo($"MasterVolumeSetting::_on_drag_ended({changed}) - User changed volume to {volumeSlider.Value}");
		volume = Mathf.RoundToInt(volumeSlider.Value);
		AudioManager.Instance.SetDeferred(Enum.GetName(volumeName), volume / 100f);
	}

	public void _on_value_changed(double value)
	{
		volumeLabel.Text = Mathf.RoundToInt(value).ToString();
	}
}
