using Godot;
using System.IO;

namespace Utilities;

public abstract partial class Configuration : Node
{
	private const string CONFIG_FILE_PATH = "./config.ini";
	private readonly ConfigFile configFile = new();

	public override void _Ready()
	{
		if (File.Exists(CONFIG_FILE_PATH))
		{
			if (configFile.Load(CONFIG_FILE_PATH) != Error.Ok)
			{
				Logger.Instance.WriteWarning("Configuration::_Ready() - Failed to load config file");
				LoadDefaultConfig();
				Save();
			}
			else
			{
				Logger.Instance.WriteInfo("Configuration::_Ready() - Loaded config file");
			}
		}
		else
		{
			Logger.Instance.WriteInfo("Configuration::_Ready() - No config file found");
			LoadDefaultConfig();
			Save();
		}
	}

	protected abstract void LoadDefaultConfig();

	protected void ChangeSetting(string section, string key, Variant value)
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
