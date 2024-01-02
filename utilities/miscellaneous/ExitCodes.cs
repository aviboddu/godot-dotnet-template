using System.Diagnostics.CodeAnalysis;

namespace Utilities;

// Standard Exit Codes to use when crashing. Add relevant codes as needed.
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum ExitCodes : int
{
    OK = 0, // successful termination
    NO_FILE = 1, // missing file
    LOAD = 2, // failed to load file
    SOFTWARE = 3 // general software error
}
