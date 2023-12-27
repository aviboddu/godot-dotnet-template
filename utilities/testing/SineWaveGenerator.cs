using Godot;

namespace Utilities;
public partial class SineWaveGenerator : AudioStreamPlayer
{
	private const float PULSE_HZ = 440f; // The frequency of the sound wave.

	// Saved buffer to push to generator repeatedly.
	private Vector2[] frames;
	private AudioStreamGeneratorPlayback Playback => (AudioStreamGeneratorPlayback)GetStreamPlayback();

	public override void _Ready()
	{
		if (Stream is AudioStreamGenerator generator)
		{
			Play();
			CreateFullBuffer();
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

	private void FillBuffer() => Playback.PushBuffer(frames);

	private void CreateFullBuffer()
	{
		float increment = Mathf.Tau * PULSE_HZ / ((AudioStreamGenerator)Stream).MixRate;
		float phase = 0;
		int framesAvailable = Playback.GetFramesAvailable();
		frames = new Vector2[framesAvailable];
		for (int i = 0; i < framesAvailable; i++)
		{
			frames[i] = Vector2.One * Mathf.Sin(phase);
			phase += increment;
		}
	}
}
