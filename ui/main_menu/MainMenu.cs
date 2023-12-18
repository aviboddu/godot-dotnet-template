using Godot;
using Utilities;

namespace UI;
public partial class MainMenu : Control
{
	private Control settings;
	private Control StartMenu;

	public override void _Ready()
	{
		base._Ready();
		settings = GetNode<Control>("%Settings");
		StartMenu = GetNode<Control>("%StartMenu");

		settings.CheckedConnect(Settings.SignalName.BackPressed, Callable.From(_on_back_pressed));
	}

	public void _on_settings_pressed()
	{
		Logger.WriteInfo("User Opened Settings");
		settings.Visible = true;
		StartMenu.Visible = false;
	}

	public void _on_back_pressed()
	{
		Logger.WriteInfo("User Exited Settings");
		settings.Visible = false;
		StartMenu.Visible = true;
	}

	public void _on_quit_pressed()
	{
		Logger.WriteInfo("User Quit");
		GetTree().Quit();
	}
}
