using Godot;
using UI;

namespace Utilities;
public partial class Test3DScene : Node3D
{
	[Export]
	public PauseMenu pauseMenu;

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventKey key)
		{
			if (key.IsActionPressed("Pause"))
				if (pauseMenu.Visible)
					pauseMenu.UnPause();
				else
					pauseMenu.Pause();
		}
	}
}
