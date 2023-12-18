using Godot;

namespace Utilities;
public static partial class MathUtils
{
        public static float DeltaLerp(float from, float to, float weight, float delta)
        {
                return Mathf.Lerp(from, to, 1f - Mathf.Exp(-delta * weight));
        }
}
