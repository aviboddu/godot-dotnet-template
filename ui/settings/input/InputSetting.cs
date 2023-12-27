using System.Linq;
using Godot;
using Utilities;

namespace UI;
public partial class InputSetting : Container
{
	private const string INPUT_ACTION_SETTING_PATH = "res://ui/settings/input/InputActionSetting.tscn";
	public override void _Ready()
	{
		Error err = ResourceLoader.LoadThreadedRequest(INPUT_ACTION_SETTING_PATH);
		if (err == Error.Failed)
			Logger.WriteError($"InputSettings::_Ready() - ResourceLoader returned Error.Failed");
		this.CheckedConnect(SignalName.VisibilityChanged, Callable.From(LoadActions));
	}

	private void LoadActions()
	{
		ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(INPUT_ACTION_SETTING_PATH);
		switch (status)
		{
			case ResourceLoader.ThreadLoadStatus.InvalidResource:
				Logger.WriteError($"InputSettings::LoadActions() - ResourceLoader returned InvalidResource");
				return;
			case ResourceLoader.ThreadLoadStatus.Failed:
				Logger.WriteError($"InputSettings::LoadActions() - ResourceLoader returns Status.Failed");
				return;
		}
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
