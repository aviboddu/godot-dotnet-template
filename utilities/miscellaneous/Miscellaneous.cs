using Godot;

namespace Utilities;
public static class Miscellaneous
{
	public static bool IsValid<T>(this T node) where T : GodotObject
	{
		return node is not null
				&& GodotObject.IsInstanceValid(node)
				&& !node.IsQueuedForDeletion();
	}

	public static Error CheckedConnect<T>(this T node, in StringName signal, in Callable callable, uint flags = 0) where T : Node
	{
		if (!node.IsConnected(signal, callable)
			|| ((GodotObject.ConnectFlags)flags).HasFlag(GodotObject.ConnectFlags.ReferenceCounted))
			return node.Connect(signal, callable, flags);
		return Error.InvalidParameter;
	}

	public static void Crash<T>(this T tree, int exitCode) where T : SceneTree
	{
		tree.Root.PropagateNotification((int)Node.NotificationCrash);
		tree.Quit(exitCode);
	}

	public static void Exit<T>(this T tree) where T : SceneTree
	{
		tree.Root.PropagateNotification((int)Node.NotificationWMCloseRequest);
		tree.Quit();
	}
}
