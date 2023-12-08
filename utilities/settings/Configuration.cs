using Godot;
using System.IO;
using System.Threading.Tasks;

namespace Utilities;

public partial class Configuration : Node
{
	private const string CONFIG_FILE_PATH = "./config.ini";
	private readonly ConfigFile configFile = new();

	public static Configuration Instance { get; private set; }

	private Timer saveDelay = new();

	private bool goingToSave = false; // Makes sure that we don't try to save again if we have a save queued
	public override void _EnterTree()
	{
		if (Instance != null)
			QueueFree(); // The singleton is already loaded, kill this instance
		Instance = this;
	}

	public override void _Ready()
	{
		AddChild(saveDelay);
		saveDelay.WaitTime = 1;
		saveDelay.Timeout += () => Task.Run(Save);
		if (File.Exists(CONFIG_FILE_PATH))
		{
			if (configFile.Load(CONFIG_FILE_PATH) != Error.Ok)
				Logger.Instance.WriteError("Configuration::_Ready() - Failed to load config file");
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

	public bool HasSection(string section)
	{
		return configFile.HasSection(section);
	}

	public bool HasSetting(string section, string key)
	{
		return configFile.HasSectionKey(section, key);
	}

	public void ChangeSetting(string section, string key, Variant value, bool saveNow = true)
	{
		if (HasSetting(section, key) && value.Equals(GetSetting<Variant>(section, key)))
			return;
		lock (configFile)
			configFile.SetValue(section, key, value);
		Logger.Instance.WriteDebug($"Configuration::ChangeSetting() - Changed {section}:{key} to {value}");

		if (saveNow)
		{
			Task.Run(Save);
			if (saveDelay.TimeLeft != 0)
				saveDelay.Stop();
		}
		else if (saveDelay.TimeLeft == 0)
			saveDelay.Start();
	}

	public void Save()
	{
		goingToSave = true;
		lock (configFile)
		{
			if (configFile.Save(CONFIG_FILE_PATH) != Error.Ok)
				Logger.Instance.WriteError("Configuration::Save() - Failed to save config");
			else
				Logger.Instance.WriteDebug("Configuration::Save() - Saved config");
		}
		goingToSave = false;
	}
}
