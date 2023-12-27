using Godot;
using Utilities;

namespace UI;
public partial class PauseMenu : Control
{
	private const string PAUSE = "Pause";
	private Settings settings;
	private Control MainPauseMenu;

	public override void _Ready()
	{
		base._Ready();
		settings = GetNode<Settings>("%Settings");
		MainPauseMenu = GetNode<Control>("%MainPauseMenu");

		settings.CheckedConnect(Settings.SignalName.BackPressed, Callable.From(_on_back_pressed));
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event.IsActionPressed(PAUSE))
			if (GetTree().Paused)
				UnPause();
			else
				Pause();
	}

	public void _on_back_pressed()
	{
		settings.Visible = false;
		MainPauseMenu.Visible = true;
	}

	public void _on_settings_pressed()
	{
		settings.Visible = true;
		MainPauseMenu.Visible = false;
	}

	public void _on_quit_to_os_pressed()
	{
		Logger.WriteInfo($"User Quit");
		GetTree().Exit();
	}

	public void Pause()
	{
		Logger.WriteInfo($"PauseMenu::Pause() - Game paused");
		Visible = true;
		GetTree().Paused = true;
	}

	public void UnPause()
	{
		Logger.WriteInfo($"PauseMenu::UnPause() - Game unpaused");

		// Ensures next pause will be from normal state
		settings.Visible = false;
		MainPauseMenu.Visible = true;

		Visible = false;
		GetTree().Paused = false;
	}
}
