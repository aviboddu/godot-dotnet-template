using Godot;

namespace Utilities;

// TODO: Add helper functions to play audio files.
public partial class AudioManager : Node
{

	private const string AUDIO_SECTION = "Audio";

	private float _masterVolume;
	public float MasterVolume
	{
		get => _masterVolume;
		set
		{
			_masterVolume = value;
			Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.MasterVolume, value);
		}
	}

	private float _musicVolume;
	public float MusicVolume
	{
		get => _musicVolume;
		set
		{
			_musicVolume = value;
			Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.MusicVolume, value);
		}
	}

	private float _sfxVolume;
	public float SfxVolume
	{
		get => _sfxVolume;
		set
		{
			_sfxVolume = value;
			Configuration.Instance.ChangeSetting(AUDIO_SECTION, PropertyName.SfxVolume, value);
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Configuration.Instance.HasSetting(AUDIO_SECTION, PropertyName.MasterVolume))
		{
			Logger.Instance.WriteInfo("AudioManager::_Ready() - Initializing Volumes from Configuration");
			_masterVolume = Configuration.Instance.GetSetting<float>(AUDIO_SECTION, PropertyName.MasterVolume);
			_musicVolume = Configuration.Instance.GetSetting<float>(AUDIO_SECTION, PropertyName.MusicVolume);
			_sfxVolume = Configuration.Instance.GetSetting<float>(AUDIO_SECTION, PropertyName.SfxVolume);
		}
		else
		{
			Logger.Instance.WriteInfo("AudioManager::_Ready() - Default Initialization from Configuration");
			MasterVolume = 1F;
			MusicVolume = 1F;
			SfxVolume = 1F;
		}
	}
}
