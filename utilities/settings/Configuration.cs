using Godot;
using System.IO;

namespace Utilities;

public abstract partial class Configuration : Node
{
	private readonly string configFilePath = "./config.ini";
	private ConfigFile configFile;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		configFile = new();
		if (File.Exists(configFilePath))
		{
			if (configFile.Load(configFilePath) != Error.Ok)
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
			LoadDefaultConfig();
			Save();
		}
	}

	protected abstract void LoadDefaultConfig();

	protected void ChangeSetting(string section, string key, string value)
	{
		configFile.SetValue(section, key, value);
		Logger.Instance.WriteInfo($"Configuration::ChangeSetting() - Changed {section}:{key} to {value}");
		CallDeferred(MethodName.Save);
	}

	private void Save()
	{
		if (configFile.Save(configFilePath) != Error.Ok)
			Logger.Instance.WriteWarning("Configuration::Save() - Failed to save config");
		else
			Logger.Instance.WriteDebug("Configuration::Save() - Saved config");
	}
}
