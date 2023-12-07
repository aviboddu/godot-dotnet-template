using System.Diagnostics;
using Godot;

namespace Utilities;

public partial class AudioManager : Node
{

	private const string AUDIO_SECTION = "Audio";
	private const string MASTER_BUS_NAME = "Master";
	private const string MUSIC_BUS_NAME = "Music";
	private const string SFX_BUS_NAME = "SFX";

	private readonly int _masterBusIndex = AudioServer.GetBusIndex(MASTER_BUS_NAME);
	private readonly int _musicBusIndex = AudioServer.GetBusIndex(MUSIC_BUS_NAME);
	private readonly int _sfxBusIndex = AudioServer.GetBusIndex(SFX_BUS_NAME);

	private float _masterVolume = float.NaN;
	public float MasterVolume
	{
		get => _masterVolume;
		set => SetMasterVolume(value, true);
	}

	private float _musicVolume = float.NaN;
	public float MusicVolume
	{
		get => _musicVolume;
		set => SetMusicVolume(value, true);
	}

	private float _sfxVolume = float.NaN;
	public float SfxVolume
	{
		get => _sfxVolume;
		set => SetSfxVolume(value, true);
	}

	public static AudioManager Instance { get; private set; }

	public override void _EnterTree()
	{
		if (Instance != null)
			QueueFree(); // The singleton is already loaded, kill this instance
		Instance = this;
	}

	public override void _Ready()
	{
#if DEBUG
		ulong ticks = Time.GetTicksMsec();
#endif

		if (Configuration.Instance.HasSection(AUDIO_SECTION))
		{
			Logger.Instance.WriteInfo("AudioManager::_Ready() - Initializing Volumes from Configuration");
			MasterVolume = Configuration.Instance.GetSetting<float>(AUDIO_SECTION, PropertyName.MasterVolume);
			MusicVolume = Configuration.Instance.GetSetting<float>(AUDIO_SECTION, PropertyName.MusicVolume);
			SfxVolume = Configuration.Instance.GetSetting<float>(AUDIO_SECTION, PropertyName.SfxVolume);
		}
		else
		{
			Logger.Instance.WriteInfo("AudioManager::_Ready() - Default Initialization");
			SetMasterVolume(GetBusVolume(_masterBusIndex), false);
			SetMusicVolume(GetBusVolume(_musicBusIndex), false);
			SetSfxVolume(GetBusVolume(_sfxBusIndex), false);
			Configuration.Instance.Save(); // We know we're going to change a bunch of settings, so we batch the write.
		}
#if DEBUG
		Logger.Instance.WriteDebug($"AudioManager::_Ready() - Time to Initialize {Time.GetTicksMsec() - ticks} ms");
#endif
	}

	private void SetMasterVolume(float value, bool saveNow)
	{
		if (_masterVolume == value) return;
		Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.MasterVolume, value, saveNow);
		_masterVolume = value;
		SetBusVolume(_masterBusIndex, value);
	}

	private void SetMusicVolume(float value, bool saveNow)
	{
		if (_musicVolume == value) return;
		Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.MusicVolume, value, saveNow);
		_musicVolume = value;
		SetBusVolume(_musicBusIndex, value);
	}

	private void SetSfxVolume(float value, bool saveNow)
	{
		if (_sfxVolume == value) return;
		Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.SfxVolume, value, saveNow);
		_sfxVolume = value;
		SetBusVolume(_sfxBusIndex, value);
	}
	private static float ConvertToDecibels(float x)
	{
		Debug.Assert(x > 0, $"AudioManager::ConvertToDecibels({x}) - {x} must be greater than 0");
		return 20 * (float)System.Math.Log10(x);
	}

	private static float ConvertFromDecibels(float db)
	{
		return (float)System.Math.Pow(10, db / 20);
	}

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