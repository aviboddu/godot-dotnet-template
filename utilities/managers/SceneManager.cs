using System.Diagnostics;
using Godot;
using UI;

namespace Utilities;
public partial class SceneManager : Node
{
	const string LOADING_SCREEN_PATH = "res://ui/loading_screen/LoadingScreen.tscn";

	public static SceneManager Instance { get; private set; }

	public static string DesiredScene { get; private set; }

	public override void _EnterTree()
	{
		if (Instance != null)
			QueueFree(); // The singleton is already loaded, kill this instance
		Instance = this;
	}

	public static void ChangeScene(string sceneToLoad) => Instance._changeScene(sceneToLoad);

	private void _changeScene(string sceneToLoad)
	{
		Logger.WriteDebug($"LoadScene::_on_pressed() - Loading {sceneToLoad}");
		DesiredScene = sceneToLoad;
		GetTree().ChangeSceneToFile(LOADING_SCREEN_PATH);
	}
}
