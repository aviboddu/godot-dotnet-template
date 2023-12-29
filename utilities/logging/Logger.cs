using Godot;
using System;

namespace Utilities;

[System.Diagnostics.DebuggerDisplay("(minLogLevel: {MIN_LOG_LEVEL})")]
public static class Logger
{

	public enum LogLevel : byte
	{
		Error = 3,
		Warning = 2,
		Info = 1,
		Debug = 0
	}
	// Prevents reallocation of the same string.
	private static readonly string[] LogLevelToString = ["DEBUG", "INFO", "WARN", "ERROR"];

	// The minimum log level to write
	[Export]
	private const LogLevel MIN_LOG_LEVEL = LogLevel.Debug;

	private static void Write(in object message, LogLevel logLevel)
	{
#if DEBUG
		if (MIN_LOG_LEVEL > logLevel)
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

	public static void WriteError(in object message) => Write(message, LogLevel.Error);
	public static void WriteWarning(in object message) => Write(message, LogLevel.Warning);
	public static void WriteInfo(in object message) => Write(message, LogLevel.Info);
	public static void WriteDebug(in object message) => Write(message, LogLevel.Debug);

	private static string FormatMessage(in object message, LogLevel level) => $"[{FormatDateTime(Miscellaneous.FastNow())}][{LevelToString(level)}] {message}";

	// Custom formatter is faster and means logging is less of a performance hit.
	private static string FormatDateTime(DateTime dateTime)
	{
		return string.Create(23, dateTime, (chars, dt) =>
		{
			WriteNChars(chars, 0, dt.Year, 4);
			chars[4] = '/';
			WriteNChars(chars, 5, dt.Month, 2);
			chars[7] = '/';
			WriteNChars(chars, 8, dt.Day, 2);
			chars[10] = ' ';
			WriteNChars(chars, 11, dt.Hour, 2);
			chars[13] = ':';
			WriteNChars(chars, 14, dt.Minute, 2);
			chars[16] = ':';
			WriteNChars(chars, 17, dt.Second, 2);
			chars[19] = ':';
			WriteNChars(chars, 20, dt.Millisecond, 3);
		});
	}

	private static void WriteNChars(in Span<char> chars, int offset, int value, int n)
	{
		for (int i = offset + n - 1; i >= offset; i--)
		{
			chars[i] = FromDigit(value % 10);
			value /= 10;
		}
	}

	private static char FromDigit(int digit) => (char)('0' + digit);

	// Having a direct method here improves performance and removes Enum.GetName calls.
	// Also allows custom text for different enums.
	private static string LevelToString(LogLevel level) => LogLevelToString[(byte)level];
}