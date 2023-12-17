using Godot;
using Utilities;

namespace UI;
public partial class PauseMenu : Control
{
	[ExportGroup("Internal")]
	[Export]
	public Settings settings;

	[Export]
	public Control MainPauseMenu;

	public float TimeScaleToReturn { get; set; } = 1f;

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventKey key)
		{
			if (key.IsActionPressed("Pause"))
				if (GetTree().Paused)
					UnPause();
				else
					Pause();
		}
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

	public void _on_quit_to_os_pressed() => GetTree().Quit();

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
