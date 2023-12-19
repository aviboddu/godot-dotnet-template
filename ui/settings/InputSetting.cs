using Godot;
using Utilities;

namespace UI;
public partial class InputSetting : Node
{
	[Export]
	Button inputKey;

	[Export]
	StringName action;

	public override void _Ready()
	{
		inputKey.Text = InputMap.ActionGetEvents(action)[0].AsText();
	}

	public void _on_button_pressed()
	{
		// Spawn Popup Listener
		// Connect listener
	}

	public void _update_input(InputEvent inputEvent)
	{
		InputManager.Instance.SwapEvent(action, InputMap.ActionGetEvents(action)[0], inputEvent);
		inputKey.Text = InputMap.ActionGetEvents(action)[0].AsText();
	}
}
