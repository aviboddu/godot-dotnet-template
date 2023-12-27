using System.Linq;
using Godot;
using Utilities;

namespace UI;
public partial class InputSetting : Container
{
	private const string INPUT_ACTION_SETTING_PATH = "res://ui/settings/InputActionSetting.tscn";
	public override void _Ready()
	{
		ResourceLoader.LoadThreadedRequest(INPUT_ACTION_SETTING_PATH);
		this.CheckedConnect(SignalName.VisibilityChanged, Callable.From(LoadActions));
	}

	private void LoadActions()
	{
		foreach (StringName action in InputManager.GetCustomActions())
		{
			if (InputMap.ActionGetEvents(action).Any((e) => e is InputEventMouseMotion || e is InputEventJoypadMotion))
				continue;
			InputActionSetting actionSetting = ((PackedScene)ResourceLoader.LoadThreadedGet(INPUT_ACTION_SETTING_PATH)).Instantiate<InputActionSetting>();
			AddChild(actionSetting);
			actionSetting.Action = action;
		}
		this.Disconnect(SignalName.VisibilityChanged, Callable.From(LoadActions));
	}
}
