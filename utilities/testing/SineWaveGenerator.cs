using Godot;

namespace Utilities;
public partial class SineWaveGenerator : AudioStreamPlayer
{
	private AudioStreamGeneratorPlayback _playback; // Will hold the AudioStreamGeneratorPlayback.
	private float _increment;
	private float _pulseHz = 440.0f; // The frequency of the sound wave.

	public override void _Ready()
	{
		if (Stream is AudioStreamGenerator generator)
		{
			_increment = _pulseHz / generator.MixRate;
			Play();
			_playback = (AudioStreamGeneratorPlayback)GetStreamPlayback();
			FillBuffer();

			Timer timer = new()
			{
				WaitTime = generator.BufferLength * 3,
				Autostart = true
			};
			timer.CheckedConnect(Timer.SignalName.Timeout, Callable.From(FillBuffer));
			AddChild(timer);
		}
	}

	private void FillBuffer()
	{
		float phase = 0;
		int framesAvailable = _playback.GetFramesAvailable();
		Vector2[] frames = new Vector2[framesAvailable];

		for (int i = 0; i < framesAvailable; i++)
		{
			frames[i] = Vector2.One * Mathf.Sin(phase * Mathf.Tau);
			phase = Mathf.PosMod(phase + _increment, 1f);
		}
		_playback.PushBuffer(frames);
	}
}
