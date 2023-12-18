using Godot;

namespace Utilities;
public abstract partial class Singleton<T> : Node where T : Singleton<T>
{
	public static T Instance { get; protected set; }

	public override void _EnterTree()
	{
		if (Instance.IsValid())
			QueueFree(); // The singleton is already loaded, kill this instance
		Instance = (T)this;
	}
}
