// Remove these if you don't want to relevant behavior
#define WRITE_TO_FILE
#define WRITE_TO_CONSOLE

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
	[Export]
	public LogLevel minLogLevel = LogLevel.Info;
	private readonly string logFilePath = $"./Logs/{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.log";
	private StreamWriter logFile;

	public static Logger Instance { get; private set; }
	public override void _EnterTree()
	{
		if (Instance != null)
			QueueFree(); // The singleton is already loaded, kill this instance
		Instance = this;
	}

	public override void _Ready()
	{
#if DEBUG && WRITE_TO_FILE
		bool fileOverwritten = File.Exists(logFilePath);
		Directory.CreateDirectory(logFilePath.GetBaseDir());
		logFile = File.CreateText(logFilePath);
		logFile.AutoFlush = true; // Make sure to flush quickly, so logs are available in the event of a crash

		WriteDebug("Logger::_Ready() - Log File Created");
		if (fileOverwritten)
			WriteWarning("Logger::_Ready() - Log File Overwritten");
#endif
	}

	public void Write(in object message, LogLevel logLevel)
	{
#if DEBUG && (WRITE_TO_CONSOLE || WRITE_TO_FILE)
		if (minLogLevel > logLevel)
			return;

		string formattedMessage = FormatMessage(message, logLevel);

#if WRITE_TO_CONSOLE
		switch (logLevel)
		{
			case LogLevel.Error:
				GD.PushError(formattedMessage);
				break;
			case LogLevel.Warning:
				GD.PushWarning(formattedMessage);
				break;
			default:
				GD.Print(formattedMessage);
				break;
		}
#endif
#if WRITE_TO_FILE
		logFile.WriteLine(formattedMessage);
#endif
#endif
	}

	public static void WriteError(in object message)
	{
		Instance.Write(message, LogLevel.Error);
	}

	public static void WriteWarning(in object message)
	{
		Instance.Write(message, LogLevel.Warning);
	}

	public static void WriteInfo(in object message)
	{
		Instance.Write(message, LogLevel.Info);
	}

	public static void WriteDebug(in object message)
	{
		Instance.Write(message, LogLevel.Debug);
	}

	private static string FormatMessage(in object message, LogLevel level) => $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}][{level}] {message}";

	public override string ToString() => $"Logger(minLogLevel={minLogLevel}, logFilePath={logFilePath})";
}