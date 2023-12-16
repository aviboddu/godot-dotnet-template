using Godot;

namespace Utilities;
public partial class Test3DScene : Node3D
{
	[Export]
	public Control pauseMenu;

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventKey key)
		{
			if (key.IsActionPressed("Pause"))
				Pause();
		}
	}

	private void Pause()
	{
		if (!pauseMenu.Visible && Input.IsActionJustPressed("Pause"))
		{
			pauseMenu.Visible = true;
			Engine.TimeScale = 0;
		}
	}
}
