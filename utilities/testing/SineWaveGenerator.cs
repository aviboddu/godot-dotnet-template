using Godot;

namespace Utilities;
public partial class SineWaveGenerator : AudioStreamPlayer
{
	private AudioStreamGeneratorPlayback _playback { get => (AudioStreamGeneratorPlayback)GetStreamPlayback(); }
	private float _increment;
	private const float PULSE_HZ = 440f; // The frequency of the sound wave.

	private Vector2[] _frames;

	public override void _Ready()
	{
		if (Stream is AudioStreamGenerator generator)
		{
			_increment = Mathf.Tau * PULSE_HZ / generator.MixRate;
			Play();
			_frames = CreateFullBuffer();
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

	private void FillBuffer() => _playback.PushBuffer(_frames);

	private Vector2[] CreateFullBuffer()
	{
		float phase = 0;
		int framesAvailable = _playback.GetFramesAvailable();
		Vector2[] frames = new Vector2[framesAvailable];
		for (int i = 0; i < framesAvailable; i++)
		{
			frames[i] = Vector2.One * Mathf.Sin(phase);
			phase += _increment;
		}

		return frames;
	}
}
