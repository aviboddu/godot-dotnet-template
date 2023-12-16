using Godot;

namespace Utilities;
public partial class Test2DScene : Node2D
{
	[Export]
	public Control pauseMenu;

	public override void _Process(double delta)
	{
		if (!pauseMenu.Visible && Input.IsActionJustPressed("Pause"))
		{
			pauseMenu.Visible = true;
			Engine.TimeScale = 0;
		}
	}
}
