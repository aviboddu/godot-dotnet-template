using System.Diagnostics;
using Godot;

namespace Utilities;

[DebuggerDisplay("(MasterVolume={MasterVolume}, MusicVolume={MusicVolume}, SFXVolume={SfxVolume})")]
public partial class AudioManager : Node
{
	public static AudioManager Instance { get; private set; }

	public override void _EnterTree()
	{
		if (Instance is not null)
		{
			QueueFree();
			return;
		}
		Instance = this;
		base._EnterTree();
	}

	private const string AUDIO_SECTION = "Audio";
	private const string MASTER_BUS_NAME = "Master";
	private const string MUSIC_BUS_NAME = "Music";
	private const string SFX_BUS_NAME = "SFX";

	private readonly int _masterBusIndex = AudioServer.GetBusIndex(MASTER_BUS_NAME);
	private readonly int _musicBusIndex = AudioServer.GetBusIndex(MUSIC_BUS_NAME);
	private readonly int _sfxBusIndex = AudioServer.GetBusIndex(SFX_BUS_NAME);

	public float MasterVolume
	{
		get => GetBusVolume(_masterBusIndex);
		set
		{
			if (MasterVolume == value) return;
			Debug.Assert(0 <= value && value <= 1, $"MasterVolume = {value} must be between 0 and 1");
			Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.MasterVolume, value);
			SetBusVolume(_masterBusIndex, value);
		}
	}

	public float MusicVolume
	{
		get => GetBusVolume(_musicBusIndex);
		set
		{
			if (MusicVolume == value) return;
			Debug.Assert(0 <= value && value <= 1, $"MusicVolume = {value} must be between 0 and 1");
			Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.MusicVolume, value);
			SetBusVolume(_musicBusIndex, value);
		}
	}

	public float SfxVolume
	{
		get => GetBusVolume(_sfxBusIndex);
		set
		{
			if (SfxVolume == value) return;
			Debug.Assert(0 <= value && value <= 1, $"SfxVolume = {value} must be between 0 and 1");
			Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.SfxVolume, value);
			SetBusVolume(_sfxBusIndex, value);
		}
	}

	public override void _Ready()
	{
#if DEBUG
		ulong ticks = Time.GetTicksMsec();
#endif

		if (Configuration.Instance.HasSection(AUDIO_SECTION))
		{
			Logger.WriteInfo("AudioManager::_Ready() - Initializing Volumes from Configuration");
			SetBusVolume(_masterBusIndex, Configuration.Instance.GetSetting<float>(AUDIO_SECTION, PropertyName.MasterVolume));
			SetBusVolume(_musicBusIndex, Configuration.Instance.GetSetting<float>(AUDIO_SECTION, PropertyName.MusicVolume));
			SetBusVolume(_sfxBusIndex, Configuration.Instance.GetSetting<float>(AUDIO_SECTION, PropertyName.SfxVolume));
		}
		else
		{
			Logger.WriteInfo("AudioManager::_Ready() - Default Initialization");
			Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.MasterVolume, MasterVolume);
			Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.MusicVolume, MusicVolume);
			Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.SfxVolume, SfxVolume);
			Configuration.Instance.Flush();
		}
#if DEBUG
		Logger.WriteDebug($"AudioManager::_Ready() - Time to Initialize {Time.GetTicksMsec() - ticks} ms");
#endif
	}

	private static float ConvertToDecibels(float x)
	{
		Debug.Assert(x > 0, $"AudioManager::ConvertToDecibels({x}) - {x} must be greater than 0");
		return 20 * (float)System.Math.Log10(x);
	}

	private static float ConvertFromDecibels(float db) => Mathf.Pow(10, db / 20);

	private static void SetBusVolume(int busIndex, float volume)
	{
		bool isZero = volume == 0;
		AudioServer.SetBusMute(busIndex, isZero); // Set Mute Correctly
		if (!isZero) AudioServer.SetBusVolumeDb(busIndex, ConvertToDecibels(volume)); // Adjust Decibels accordingly
	}

	private static float GetBusVolume(int busIndex)
	{
		if (AudioServer.IsBusMute(busIndex))
			return 0;
		return ConvertFromDecibels(AudioServer.GetBusVolumeDb(busIndex));
	}
}