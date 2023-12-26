using Godot;
using System.IO;
using System.Threading.Tasks;

namespace Utilities;

public partial class Configuration : Node
{
	public static Configuration Instance { get; private set; }

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

	private const string CONFIG_FILE_PATH = "./config.ini";
	private const string TEMP_FILE_PATH = "./config_temp.ini"; // Must be in the save volume as CONFIG_FILE_PATH
	private const float TIME_TO_FLUSH_IN_SECONDS = 1f;

	private readonly ConfigFile configFile = new();
	private readonly Timer saveDelay = new()
	{
		OneShot = true,
		WaitTime = TIME_TO_FLUSH_IN_SECONDS
	};

	private bool savePending = false;

	public override void _Ready()
	{
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

	public T GetSetting<[MustBeVariant] T>(in string section, in string key) => configFile.GetValue(section, key).As<T>();

	public bool HasSection(in string section) => configFile.HasSection(section);

	public bool HasSetting(in string section, in string key) => configFile.HasSectionKey(section, key);

	public void ChangeSetting(in string section, in string key, in Variant value)
	{
		if (HasSetting(section, key) && value.Equals(GetSetting<Variant>(section, key)))
			return;
		lock (configFile)
			configFile.SetValue(section, key, value);

		Logger.WriteDebug($"Configuration::ChangeSetting() - Changed {section}:{key} to {value}");

		if (saveDelay.IsStopped())
			saveDelay.Start();
	}

	// Writes any pending changes to file at the end of the frame
	public void Flush()
	{
		if (saveDelay.IsStopped() || savePending)
			return;
		Logger.WriteDebug($"Configuration::Flush() - Queued Save");
		savePending = true;
		saveDelay.Stop();
	}

	private void SaveInNewThread() => Task.Run(Save);

	private void Save()
	{
		lock (configFile)
		{
			Error err = configFile.Save(TEMP_FILE_PATH);
			if (err != Error.Ok)
			{
				Logger.WriteError($"Configuration::Save() - Failed to save config. Error {err}");
				if (File.Exists(TEMP_FILE_PATH)) File.Delete(TEMP_FILE_PATH);
			}
			else
			{
				System.IO.File.Move(TEMP_FILE_PATH, CONFIG_FILE_PATH, true);
				File.Delete(TEMP_FILE_PATH);
				Logger.WriteDebug("Configuration::Save() - Saved config");
			}
			savePending = false;
		}
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest && !saveDelay.IsStopped())
		{
			Logger.WriteInfo($"Configuration::_Notification({what}) - User Quit While Save Pending");
			Save(); // Force a save if user tries to quit while save is pending.
		}
		base._Notification(what);
	}
}
