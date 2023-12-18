namespace Utilities;
public partial class SceneManager : Singleton<SceneManager>
{
	const string LOADING_SCREEN_PATH = "res://ui/loading_screen/LoadingScreen.tscn";

	public static string DesiredScene { get; private set; }

	public static void ChangeScene(string sceneToLoad) => Instance._changeScene(sceneToLoad);

	private void _changeScene(string sceneToLoad)
	{
		Logger.WriteDebug($"LoadScene::_on_pressed() - Loading {sceneToLoad}");
		DesiredScene = sceneToLoad;
		GetTree().ChangeSceneToFile(LOADING_SCREEN_PATH);
	}
}
