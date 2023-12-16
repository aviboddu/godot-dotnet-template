using Godot;

namespace UI;
public partial class PauseMenu : Control
{
	[Export]
	public Node ContainingScene;

	[Export]
	public LoadScene QuitToMainMenuButton;

	[Export]
	public Settings settings;

	[Export]
	public Control MainPauseMenu;

	public float TimeScaleToReturn { get; set; } = 1f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		QuitToMainMenuButton.sceneToUnload = ContainingScene;
	}

	public void _on_resume_pressed()
	{
		Engine.TimeScale = TimeScaleToReturn;
		Visible = false;
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
		GetTree().Quit();
	}
}
