namespace Utilities;

// Standard Exit Codes to use when crashing. Add relevant codes as needed.
public static class ExitCodes
{
    public const int EXIT_OK = 0; // successful termination
    public const int EXIT_NOFILE = 1; // missing file
    public const int EXIT_LOAD = 2; // failed to load file
    public const int EXIT_SOFTWARE = 3; // general software error
}