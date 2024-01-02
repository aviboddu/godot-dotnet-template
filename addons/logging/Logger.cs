using System;
using System.Diagnostics;
using Godot;

namespace Utilities;

public static class Logger
{
    public enum LogLevel : int
    {
        Error = 3,
        Warning = 2,
        Info = 1,
        Debug = 0
    }

    public const string LOG_LEVEL_SETTING = "plugins/logger/level";

    // Prevents reallocation of the same string.
    private static readonly string[] LogLevelToString = ["DEBUG", "INFO", "WARN", "ERROR"];

    // The minimum log level to write
    private static readonly LogLevel MinLogLevel =
        (LogLevel)(int)ProjectSettings.GetSetting(LOG_LEVEL_SETTING, (int)LogLevel.Info);

    [Conditional("DEBUG")]
    private static void Write(in object message, LogLevel logLevel)
    {
        if (MinLogLevel > logLevel)
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
    }

    [Conditional("DEBUG")]
    public static void WriteError(in object message)
    {
        Write(message, LogLevel.Error);
    }

    [Conditional("DEBUG")]
    public static void WriteWarning(in object message)
    {
        Write(message, LogLevel.Warning);
    }

    [Conditional("DEBUG")]
    public static void WriteInfo(in object message)
    {
        Write(message, LogLevel.Info);
    }

    [Conditional("DEBUG")]
    public static void WriteDebug(in object message)
    {
        Write(message, LogLevel.Debug);
    }

    private static string FormatMessage(in object message, LogLevel level)
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("[{0}][{1}] {2}", FormatDateTime(Miscellaneous.FastNow()), LogLevelToString[(int)level],
                             message);
    }

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

    private static char FromDigit(int digit)
    {
        return (char)('0' + digit);
    }
}
