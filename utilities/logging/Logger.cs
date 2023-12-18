using Godot;
using System;

namespace Utilities;

[System.Diagnostics.DebuggerDisplay("(minLogLevel: {MinLogLevel})")]
public partial class Logger : Singleton<Logger>
{
	public enum LogLevel
	{
		Error = 4,
		Warning = 3,
		Info = 2,
		Debug = 1
	}

	// The minimum log level to write
	[Export]
	public LogLevel minLogLevel = LogLevel.Debug;

	public void Write(in object message, LogLevel logLevel)
	{
#if DEBUG
		if (minLogLevel > logLevel)
			return;

		string formattedMessage = FormatMessage(message, logLevel);
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
	}

	public static void WriteError(in object message) => Instance.Write(message, LogLevel.Error);
	public static void WriteWarning(in object message) => Instance.Write(message, LogLevel.Warning);
	public static void WriteInfo(in object message) => Instance.Write(message, LogLevel.Info);
	public static void WriteDebug(in object message) => Instance.Write(message, LogLevel.Debug);

	private static string FormatMessage(in object message, LogLevel level) => $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}][{level}] {message}";
}