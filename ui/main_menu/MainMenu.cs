using Godot;
using Utilities;

namespace UI;
public partial class MainMenu : Control
{
	[Export]
	Control Settings;

	[Export]
	Control StartMenu;

	public void _on_settings_pressed()
	{
		Logger.WriteInfo("User Opened Settings");
		Settings.Visible = true;
		StartMenu.Visible = false;
	}

	public void _on_back_pressed()
	{
		Logger.WriteInfo("User Exited Settings");
		Settings.Visible = false;
		StartMenu.Visible = true;
	}

	public void _on_quit_pressed()
	{
		Logger.WriteInfo("User Quit");
		GetTree().Quit();
	}
}
