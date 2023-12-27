// ReSharper disable InconsistentNaming
namespace Utilities;

// Standard Exit Codes to use when crashing. Add relevant codes as needed.
public enum ExitCodes: int
{
    EXIT_OK = 0, // successful termination
    EXIT_NOFILE = 1, // missing file
    EXIT_LOAD = 2, // failed to load file
    EXIT_SOFTWARE = 3, // general software error
}