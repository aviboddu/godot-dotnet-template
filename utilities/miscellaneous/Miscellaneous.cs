using System;
using Godot;

namespace Utilities;
public static class Miscellaneous
{
	public static bool IsValid<T>(this T node) where T : GodotObject
	{
		return node != null
				&& GodotObject.IsInstanceValid(node)
				&& !node.IsQueuedForDeletion();
	}

	public static Error CheckedConnect<T>(this T node, StringName signal, Callable callable, uint flags = 0) where T : Node
	{
		if (!node.IsConnected(signal, callable)
			|| ((GodotObject.ConnectFlags)flags).HasFlag(GodotObject.ConnectFlags.ReferenceCounted))
			return node.Connect(signal, callable, flags);
		return Error.InvalidParameter;
	}
}
