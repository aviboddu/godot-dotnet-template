using System;
using System.Linq;
using Godot;
using Utilities;

namespace UI;
public partial class InputSetting : Container
{
	private const string INPUT_ACTION_SETTING_PATH = "res://ui/settings/input/InputActionSetting.tscn";

	private readonly Lazy<PackedScene> inputActionPackedScene = new(() => (PackedScene)ResourceLoader.LoadThreadedGet(INPUT_ACTION_SETTING_PATH));

	public override void _Ready()
	{
		Error err = ResourceLoader.LoadThreadedRequest(INPUT_ACTION_SETTING_PATH);
		if (err == Error.Failed)
			Logger.WriteError($"InputSettings::_Ready() - ResourceLoader returned Error.Failed");
		this.CheckedConnect(CanvasItem.SignalName.VisibilityChanged, Callable.From(LoadActions)); // Create nodes only if user opens input menu
	}

	private void LoadActions()
	{
		foreach (StringName action in InputManager.GetCustomActions())
		{
			// Can only change events with buttons (for now), not analog movements
			if (InputMap.ActionGetEvents(action).Any((e) => e is InputEventMouseMotion || e is InputEventJoypadMotion))
				continue;
			InputActionSetting actionSetting = inputActionPackedScene.Value.Instantiate<InputActionSetting>();
			AddChild(actionSetting);
			actionSetting.Action = action;
			actionSetting.CheckedConnect(InputActionSetting.SignalName.ChangedEvent, Callable.From<InputEvent, InputEvent>(PreventConflicts));
		}
		Disconnect(CanvasItem.SignalName.VisibilityChanged, Callable.From(LoadActions));
	}

	// Checks that no other actions have events which will conflict with the newEvent, and replaces them with oldEvent accordingly.
	public void PreventConflicts(InputEvent oldEvent, InputEvent newEvent)
	{
		foreach (InputActionSetting actionSetting in GetChildren().Cast<InputActionSetting>())
		{
			if (actionSetting.GetInputEvents().Any(e => InputActionSetting.EventsConflict(newEvent, e)))
			{
				actionSetting.ForceChangeEvent(newEvent, oldEvent);
				return;
			}
		}
	}
}
