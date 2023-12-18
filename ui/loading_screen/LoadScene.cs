using System.Diagnostics;
using Godot;
using Utilities;

namespace UI;
public partial class LoadScene : Button
{
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

	public void _on_pressed() => SceneManager.ChangeScene(sceneToLoad);
}
