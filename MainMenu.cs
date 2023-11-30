using Godot;
using Utilities;

public partial class MainMenu : Control
{
	public void _on_quit_pressed()
	{
		Logger.Instance.WriteInfo("User Quit");
		GetTree().Quit();
	}
}
