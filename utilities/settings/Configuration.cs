using System.IO;
using System.Threading.Tasks;
using Godot;

namespace Utilities;

public partial class Configuration : Node
{
    private const string CONFIG_FILE_PATH = "./config.ini";
    private const string TEMP_FILE_PATH = "./config_temp.ini"; // Must be in the save volume as CONFIG_FILE_PATH
    private const float TIME_TO_FLUSH_IN_SECONDS = 0.5f;

    private readonly ConfigFile _configFile = new();

    private readonly Timer _saveDelay = new()
    {
        OneShot = true,
        WaitTime = TIME_TO_FLUSH_IN_SECONDS,
        ProcessMode = ProcessModeEnum.Always
    };

    private bool _saveQueued;
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

    public override void _Ready()
    {
        _saveDelay.CheckedConnect(Timer.SignalName.Timeout, Callable.From(SaveInNewThread));
        AddChild(_saveDelay);

        if (File.Exists(CONFIG_FILE_PATH))
        {
            Error err = _configFile.Load(CONFIG_FILE_PATH);
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

    public T GetSetting<[MustBeVariant] T>(in string section, in string key)
    {
        return _configFile.GetValue(section, key).As<T>();
    }

    public bool HasSection(in string section)
    {
        return _configFile.HasSection(section);
    }

    public bool HasSetting(in string section, in string key)
    {
        return _configFile.HasSectionKey(section, key);
    }

    public void ChangeSetting(in string section, in string key, in Variant value)
    {
        if (HasSetting(section, key) && value.Equals(GetSetting<Variant>(section, key)))
            return;
        lock (_configFile)
        {
            _configFile.SetValue(section, key, value);
        }

        Logger.WriteDebug($"Configuration::ChangeSetting() - Changed {section}:{key} to {value}");

        if (_saveDelay.IsStopped())
            _saveDelay.Start();
    }

    // Writes any pending changes to file at the end of the frame
    public void Flush()
    {
        if (_saveDelay.IsStopped() || _saveQueued) // Either there is no delta, or a save is queued anyway.
            return;
        Logger.WriteDebug("Configuration::Flush() - Queued Save");
        _saveQueued = true;
        _saveDelay.Stop();
    }

    private void SaveInNewThread()
    {
        Task.Run(Save);
    }

    private void Save()
    {
        lock (_configFile)
        {
            Error
                err = _configFile.Save(
                    TEMP_FILE_PATH); // Uses a temporary file path to ensure atomic file writes. Old config will be used if write fails.
            if (err != Error.Ok)
            {
                Logger.WriteError($"Configuration::Save() - Failed to save config. Error {err}");
                if (File.Exists(TEMP_FILE_PATH)) File.Delete(TEMP_FILE_PATH);
            }
            else
            {
                File.Move(TEMP_FILE_PATH, CONFIG_FILE_PATH, true);
                File.Delete(TEMP_FILE_PATH);
                Logger.WriteDebug("Configuration::Save() - Saved config");
            }

            _saveQueued = false;
        }
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest && !_saveDelay.IsStopped())
        {
            Logger.WriteInfo($"Configuration::_Notification({what}) - User Quit While Save Pending");
            Save(); // Force a save if user tries to quit while save is pending.
        }

        base._Notification(what);
    }
}
