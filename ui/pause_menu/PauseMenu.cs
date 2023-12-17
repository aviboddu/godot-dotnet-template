using Godot;

namespace UI;
public partial class PauseMenu : Control
{
	[Export]
	public Node ContainingScene;

	[ExportGroup("Internal")]
	[Export]
	public LoadScene QuitToMainMenuButton;

	[Export]
	public Settings settings;

	[Export]
	public Control MainPauseMenu;

	public float TimeScaleToReturn { get; set; } = 1f;

	public override void _Ready()
	{
		QuitToMainMenuButton.sceneToUnload = ContainingScene;

		Callable mainMenuUnpause = new(this, MethodName._main_menu_unpause);
		if (!QuitToMainMenuButton.IsConnected(Button.SignalName.Pressed, mainMenuUnpause))
			QuitToMainMenuButton.Connect(Button.SignalName.Pressed, mainMenuUnpause);
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

	private void _main_menu_unpause() => Engine.TimeScale = 1;

	public void Pause(float timeScaleToReturn = 1f)
	{
		Visible = true;
		TimeScaleToReturn = timeScaleToReturn;
		Engine.TimeScale = 0;
	}

	public void UnPause()
	{
		// Ensures next pause will be from normal state
		settings.Visible = false;
		MainPauseMenu.Visible = true;

		Visible = false;
		Engine.TimeScale = TimeScaleToReturn;
	}
}
