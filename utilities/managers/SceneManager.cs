using System;
using Godot;

namespace Utilities;
public partial class SceneManager : Node
{
	const string LOADING_SCREEN_PATH = "res://ui/loading_screen/LoadingScreen.tscn";
	private static readonly PackedScene _loadingScreen = GD.Load<PackedScene>(LOADING_SCREEN_PATH);

	private static SceneManager Instance;
	public override void _EnterTree()
	{
		if (Instance is not null)
		{
			QueueFree();
			return;
		}
		Instance = this;
		base._EnterTree();
	}

	// This should always be the scene we want to load.
	public static string DesiredScene { get; private set; }

	public static void ChangeScene(in string sceneToLoad) => Instance._changeScene(sceneToLoad);

	private void _changeScene(in string sceneToLoad)
	{
		Logger.WriteDebug($"SceneManager::_changeScene({sceneToLoad}) - Loading {sceneToLoad}");
		DesiredScene = sceneToLoad;
		GetTree().ChangeSceneToPacked(_loadingScreen);
	}
}
