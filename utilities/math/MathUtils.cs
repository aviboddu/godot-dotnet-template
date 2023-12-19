using Godot;

namespace Utilities;
public static partial class MathUtils
{
        /*
        * Linearly interpolates between two values by a normalized value, correctly adjusted by delta
        */
        public static float DeltaLerp(float from, float to, float weight, float delta)
        {
                return Mathf.Lerp(from, to, 1f - Mathf.Exp(-delta * weight));
        }
}
