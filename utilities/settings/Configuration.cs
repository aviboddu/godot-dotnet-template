using Godot;
using System.IO;

namespace Utilities;

public partial class Configuration : Node
{
	private const string CONFIG_FILE_PATH = "./config.ini";
	private readonly ConfigFile configFile = new();

	public static Configuration Instance { get; private set; }
	public override void _EnterTree()
	{
		if (Instance != null)
			QueueFree(); // The singleton is already loaded, kill this instance
		Instance = this;
	}

	public override void _Ready()
	{
		if (File.Exists(CONFIG_FILE_PATH))
		{
			if (configFile.Load(CONFIG_FILE_PATH) != Error.Ok)
				Logger.Instance.WriteWarning("Configuration::_Ready() - Failed to load config file");
			else
				Logger.Instance.WriteInfo("Configuration::_Ready() - Loaded config file");
		}
		else
		{
			Logger.Instance.WriteInfo("Configuration::_Ready() - No config file found");
		}
	}

	public T GetSetting<[MustBeVariant] T>(string section, string key)
	{
		return configFile.GetValue(section, key).As<T>();
	}


	public bool HasSetting(string section, string key)
	{
		return configFile.HasSectionKey(section, key);
	}

	public void ChangeSetting(string section, string key, Variant value)
	{
		configFile.SetValue(section, key, value);
		Logger.Instance.WriteInfo($"Configuration::ChangeSetting() - Changed {section}:{key} to {value}");
		CallDeferred(MethodName.Save);
	}

	private void Save()
	{
		if (configFile.Save(CONFIG_FILE_PATH) != Error.Ok)
			Logger.Instance.WriteWarning("Configuration::Save() - Failed to save config");
		else
			Logger.Instance.WriteDebug("Configuration::Save() - Saved config");
	}
}
