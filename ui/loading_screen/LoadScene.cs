using System.Diagnostics;
using Godot;
using Utilities;

namespace UI;
public partial class LoadScene : Button
{
	const string LOADING_SCREEN_PATH = "res://ui/loading_screen/LoadingScreen.tscn";

	[Export(PropertyHint.File, "*.tscn")]
	public string sceneToLoad;

	public override void _Ready()
	{
		Debug.Assert(sceneToLoad is not null, $"LoadScene::_Ready() - {PropertyName.sceneToLoad} is null");

		base._Ready();

		Callable onPressed = new(this, MethodName._on_pressed);
		if (!IsConnected(SignalName.Pressed, onPressed))
			Connect(SignalName.Pressed, onPressed);
	}

	public void _on_pressed()
	{
		Logger.WriteDebug($"LoadScene::_on_pressed() - Loading {sceneToLoad}");
		LoadingScreen loadingScreen = GD.Load<PackedScene>(LOADING_SCREEN_PATH).Instantiate<LoadingScreen>(PackedScene.GenEditState.Main);
		loadingScreen.ScenePath = sceneToLoad;

		Node CurrentScene = GetTree().Root.GetChild(GetTree().Root.GetChildCount() - 1);
		GetTree().Root.AddChild(loadingScreen);
		GetTree().CurrentScene = loadingScreen;
		CurrentScene.QueueFree();
	}
}
