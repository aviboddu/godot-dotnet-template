using Godot;
using System;
using System.IO;

namespace Utilities;

[System.Diagnostics.DebuggerDisplay("(minLogLevel: {MinLogLevel}, logFile: {logFilePath})")]
public partial class Logger : Node
{
	public enum LogLevel
	{
		Error = 4,
		Warning = 3,
		Info = 2,
		Debug = 1
	}

	// The minimum log level to write to the console and file
	public LogLevel minLogLevel = LogLevel.Info;
	private readonly string logFilePath = $"./Logs/{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.log";
	private StreamWriter logFile;

	public static Logger Instance { get; private set; }
	public override void _EnterTree()
	{
		if (Instance != null)
			QueueFree(); // The Singleton is already loaded, kill this instance
		Instance = this;
	}

	public override void _Ready()
	{
		bool fileOverwritten = File.Exists(logFilePath);
		Directory.CreateDirectory(logFilePath.GetBaseDir());
		logFile = File.CreateText(logFilePath);
		logFile.AutoFlush = true; // Make sure to flush quickly, so logs are available in the event of a crash.

		WriteDebug("Log File Created");
		if (fileOverwritten)
			WriteWarning("Log File Overwritten");
	}

	public void WriteError(in object message)
	{
		if (minLogLevel > LogLevel.Error)
			return;

		string formattedMessage = FormatMessage(message, LogLevel.Error);
		GD.PushError(formattedMessage);
		logFile.WriteLine(formattedMessage);
	}

	public void WriteWarning(in object message)
	{
		if (minLogLevel > LogLevel.Warning)
			return;

		string formattedMessage = FormatMessage(message, LogLevel.Warning);
		GD.PushWarning(formattedMessage); // TODO: Change to print warning if/when possible. (Could write an extension for that if it's important.)
		logFile.WriteLine(formattedMessage);
	}

	public void WriteInfo(in object message)
	{
		if (minLogLevel > LogLevel.Info)
			return;

		string formattedMessage = FormatMessage(message, LogLevel.Info);
		GD.Print(formattedMessage);
		logFile.WriteLine(formattedMessage);
	}

	public void WriteDebug(in object message)
	{
		if (minLogLevel > LogLevel.Debug)
			return;

		string formattedMessage = FormatMessage(message, LogLevel.Debug);
		GD.Print(formattedMessage);
		logFile.WriteLine(formattedMessage);
	}

	private static string FormatMessage(in object message, LogLevel level) => $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}][{level}] {message}";

	public override string ToString() => $"Logger(minLogLevel={minLogLevel}, logFilePath={logFilePath})";
}