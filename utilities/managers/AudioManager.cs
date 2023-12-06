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
		set
		{
			if (_masterVolume == value) return;
			_masterVolume = value;
			SetBusVolume(_masterBusIndex, value);
			Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.MasterVolume, value);
		}
	}

	private float _musicVolume = float.NaN;
	public float MusicVolume
	{
		get => _musicVolume;
		set
		{
			if (_musicVolume == value) return;
			_musicVolume = value;
			SetBusVolume(_musicBusIndex, value);
			Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.MusicVolume, value);
		}
	}

	private float _sfxVolume = float.NaN;
	public float SfxVolume
	{
		get => _sfxVolume;
		set
		{
			if (_sfxVolume == value) return;
			_sfxVolume = value;
			SetBusVolume(_sfxBusIndex, value);
			Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.SfxVolume, value);
		}
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
			MasterVolume = GetBusVolume(_masterBusIndex);
			MusicVolume = GetBusVolume(_musicBusIndex);
			SfxVolume = GetBusVolume(_sfxBusIndex);
		}
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

	private static float ConvertToDecibels(float x)
	{
		Debug.Assert(x > 0, $"AudioManager::ConvertToDecibels({x}) - {x} must be greater than 0");
		return 20 * (float)System.Math.Log10(x);
	}

	private static float ConvertFromDecibels(float db)
	{
		return (float)System.Math.Pow(10, db / 20);
	}
}