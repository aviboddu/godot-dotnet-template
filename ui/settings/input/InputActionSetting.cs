using Godot;
using Godot.Collections;
using Utilities;

namespace UI;
public partial class InputActionSetting : Node
{
	[Export]
	Array<InputButtonSetting> InputKeys;

	[Export]
	public StringName Action
	{
		get => _action;
		set
		{
			if (Action == value) return;
			_action = value;
			actionLabel.Text = value.ToString();
			LoadEvents();
		}
	}
	private StringName _action;
	private Label actionLabel;

	public override void _Ready()
	{
		actionLabel = GetNode<Label>("%Action");
		InputManager.Instance.CheckedConnect(InputManager.SignalName.IsControllerChanged, Callable.From(LoadEvents));
		foreach (InputButtonSetting inputKey in InputKeys)
			inputKey.CheckedConnect(InputButtonSetting.SignalName.EventChanged, Callable.From<InputEvent, InputEvent>(SwapEvents));
	}

	private void LoadEvents()
	{
		Array<InputEvent> inputEvents = InputManager.GetInputEvents(Action);
		for (int i = 0; i < InputKeys.Count; i++)
		{
			if (i >= inputEvents.Count)
				InputKeys[i].SetEventNoSignal(default);
			else
				InputKeys[i].SetEventNoSignal(inputEvents[i]);
		}
	}

	private void SwapEvents(InputEvent oldEvent, InputEvent newEvent)
	{
		foreach (InputButtonSetting InputKey in InputKeys)
		{
			if (InputKey.Event != default && EventsConflict(InputKey.Event, newEvent))
			{
				// The new value set conflicts with one of the other values, swap accordingly
				InputKey.SetEventNoSignal(oldEvent);
				return;
			}
		}
		InputManager.SwapEvent(Action, oldEvent, newEvent);
	}

	private static bool EventsConflict(InputEvent eventOne, InputEvent eventTwo)
	{
		if (eventOne.GetType() != eventTwo.GetType())
			return false;

		switch (eventOne)
		{
			case InputEventKey eventOneKey:
				InputEventKey eventTwoKey = (InputEventKey)eventTwo;
				return eventOneKey.PhysicalKeycode == eventTwoKey.PhysicalKeycode;
			case InputEventJoypadButton eventOneJoypad:
				InputEventJoypadButton eventTwoJoypad = (InputEventJoypadButton)eventTwo;
				return eventOneJoypad.ButtonIndex == eventTwoJoypad.ButtonIndex;
			case InputEventMouseButton eventOneMouseButton:
				InputEventMouseButton eventTwoMouseButton = (InputEventMouseButton)eventTwo;
				return eventOneMouseButton.ButtonIndex == eventTwoMouseButton.ButtonIndex;
		}
		return false;
	}
}
