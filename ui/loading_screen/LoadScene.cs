using System.Diagnostics;
using Godot;
using Utilities;

namespace UI;
public partial class LoadScene : Button
{
	[Export(PropertyHint.File, "*.tscn")]
	public string SceneToLoad;

	public override void _Ready()
	{
		base._Ready();

		Debug.Assert(SceneToLoad is not null, $"LoadScene::_Ready() - {PropertyName.SceneToLoad} is null");
		this.CheckedConnect(BaseButton.SignalName.Pressed, Callable.From(_on_pressed));
	}

	public void _on_pressed() => SceneManager.ChangeScene(SceneToLoad);
}
