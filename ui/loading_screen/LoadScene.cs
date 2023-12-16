using Godot;
using Utilities;

namespace UI;
public partial class LoadScene : Button
{
	const string LOADING_SCREEN_PATH = "res://ui/loading_screen/LoadingScreen.tscn";

	[Export(PropertyHint.File, "*.tscn")]
	public string sceneToLoad;

	[Export]
	public Node sceneToUnload;

	public override void _Ready()
	{
		base._Ready();

		Callable onPressed = new(this, MethodName._on_pressed);
		if (!IsConnected(SignalName.Pressed, onPressed))
			Connect(SignalName.Pressed, onPressed);
	}

	public void _on_pressed()
	{
		if (sceneToLoad is not null)
		{
			sceneToUnload.QueueFree();
			Logger.WriteDebug($"LoadScene::_on_pressed() - Loading {sceneToLoad}");
			LoadingScreen loadingScreen = GD.Load<PackedScene>(LOADING_SCREEN_PATH).Instantiate<LoadingScreen>();
			loadingScreen.ScenePath = sceneToLoad;
			GetTree().Root.AddChild(loadingScreen);
		}
	}
}
