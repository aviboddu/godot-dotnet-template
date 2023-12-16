using Godot;
using Utilities;

namespace UI;
public partial class Play : Button
{
	const string LOADING_SCREEN_PATH = "res://ui/loading_screen/LoadingScreen.tscn";

	[Export]
	public PackedScene gameScene;

	[Export]
	public Node mainMenu;

	public void _on_pressed()
	{
		if (gameScene is not null)
		{
			Logger.Instance.WriteDebug($"Play::_on_pressed() - Loading {gameScene.ResourceName}");
			LoadingScreen loadingScreen = GD.Load<PackedScene>(LOADING_SCREEN_PATH).Instantiate<LoadingScreen>();
			loadingScreen.ScenePath = gameScene.ResourcePath;
			GetTree().Root.AddChild(loadingScreen);
			mainMenu.QueueFree();
		}
	}
}
