using Godot;
using Utilities;

namespace UI;
public partial class MainMenu : Control
{
	private Control Settings;
	private Control StartMenu;

	public override void _Ready()
	{
		base._Ready();
		Settings = GetNode<Control>("%Settings");
		StartMenu = GetNode<Control>("%StartMenu");

		Callable backPressed = new Callable(this, MethodName._on_back_pressed);
		if (!Settings.IsConnected(UI.Settings.SignalName.BackPressed, backPressed))
			Settings.Connect(UI.Settings.SignalName.BackPressed, backPressed);
	}

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
