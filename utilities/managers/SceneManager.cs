using System;
using Godot;

namespace Utilities;
public partial class SceneManager : Node
{
	public static SceneManager Instance { get; private set; }
	private readonly Lazy<PackedScene> _loadingScreen = new(() => (PackedScene)GD.Load(LOADING_SCREEN_PATH), false);

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

	const string LOADING_SCREEN_PATH = "res://ui/loading_screen/LoadingScreen.tscn";

	// This should always be the scene we want to load.
	public static string DesiredScene { get; private set; }

	public static void ChangeScene(in string sceneToLoad) => Instance._changeScene(sceneToLoad);

	private void _changeScene(in string sceneToLoad)
	{
		Logger.WriteDebug($"SceneManager::_changeScene({sceneToLoad}) - Loading {sceneToLoad}");
		DesiredScene = sceneToLoad;
		GetTree().ChangeSceneToPacked(_loadingScreen.Value);
	}
}
