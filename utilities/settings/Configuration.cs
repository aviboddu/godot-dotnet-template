using Godot;
using System.IO;
using System.Threading.Tasks;

namespace Utilities;

public partial class Configuration : Singleton<Configuration>
{
	private const string CONFIG_FILE_PATH = "./config.ini";
	private const float TIME_TO_FLUSH_IN_SECONDS = 1f;

	private readonly ConfigFile configFile = new();

	private Timer saveDelay = new();

	public override void _Ready()
	{
		saveDelay.OneShot = true;
		saveDelay.WaitTime = TIME_TO_FLUSH_IN_SECONDS;
		saveDelay.CheckedConnect(Timer.SignalName.Timeout, Callable.From(SaveInNewThread));
		AddChild(saveDelay);

		if (File.Exists(CONFIG_FILE_PATH))
		{
			Error err = configFile.Load(CONFIG_FILE_PATH);
			if (err != Error.Ok)
				Logger.WriteError($"Configuration::_Ready() - Failed to load config file. Error {err}");
			else
				Logger.WriteInfo("Configuration::_Ready() - Loaded config file");
		}
		else
		{
			Logger.WriteInfo("Configuration::_Ready() - No config file found");
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

	public void ChangeSetting(string section, string key, Variant value)
	{
		if (HasSetting(section, key) && value.Equals(GetSetting<Variant>(section, key)))
			return;
		lock (configFile)
			configFile.SetValue(section, key, value);

		Logger.WriteDebug($"Configuration::ChangeSetting() - Changed {section}:{key} to {value}");

		if (saveDelay.IsStopped())
			saveDelay.Start();
	}

	public void Flush()
	{
		if (saveDelay.IsStopped())
			return;
		CallDeferred(MethodName.SaveInNewThread);
		saveDelay.Stop();
	}

	private void SaveInNewThread() => Task.Run(Save);

	private void Save()
	{
		lock (configFile)
		{
			Error err = configFile.Save(CONFIG_FILE_PATH);
			if (err != Error.Ok)
				Logger.WriteError($"Configuration::Save() - Failed to save config. Error {err}");
			else
				Logger.WriteDebug("Configuration::Save() - Saved config");
		}
	}
}
