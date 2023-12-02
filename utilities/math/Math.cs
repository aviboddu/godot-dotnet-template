using Godot;

namespace Utilities;
public static partial class Math
{
	 public static float DeltaLerp(float from, float to, float weight, float delta) => 
        Mathf.Lerp(from, to, 1f - Mathf.Exp(-delta * weight));
}
