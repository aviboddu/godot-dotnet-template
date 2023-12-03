using Godot;
using System.IO;

namespace Utilities;

public abstract partial class Configuration : Node
{
	private readonly string configFilePath = "./config.ini";
	private ConfigFile configFile;

	private Logger logger;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		logger = Logger.Instance;
		configFile = new();
		if (File.Exists(configFilePath))
		{
			if (configFile.Load(configFilePath) != Error.Ok)
			{
				logger.WriteWarning("Configuration::_Ready() - Failed to load config file");
				LoadDefaultConfig();
				configFile.Save(configFilePath);
			}
		}
		else
		{
			LoadDefaultConfig();
			configFile.Save(configFilePath);
		}
	}

	protected abstract void LoadDefaultConfig();
}
