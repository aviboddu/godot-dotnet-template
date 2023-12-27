using System.Linq;
using Godot;
using Godot.Collections;
using Utilities;

namespace UI;
public partial class InputActionSetting : Node
{
	[Signal]
	public delegate void ChangedEventEventHandler(InputEvent oldEvent, InputEvent newEvent);

	[Export]
	public Array<InputButtonSetting> InputKeys;

	[Export]
	public StringName Action
	{
		get => action;
		set
		{
			if (Action == value) return;
			action = value;
			actionLabel.Text = value.ToString();
			LoadEvents();
		}
	}
	private StringName action;
	private Label actionLabel;

	public override void _Ready()
	{
		actionLabel = GetNode<Label>("%Action");
		InputManager.Instance.CheckedConnect(InputManager.SignalName.IsControllerChanged, Callable.From(LoadEvents));
		foreach (InputButtonSetting inputKey in InputKeys)
			inputKey.CheckedConnect(InputButtonSetting.SignalName.EventChanged, Callable.From<InputEvent, InputEvent>(SwapEvents));
	}

	// This is called by the parent when this action's event conflicts with another action's desired event.
	public void ForceChangeEvent(InputEvent oldEvent, InputEvent newEvent)
	{
		foreach (InputButtonSetting inputKey in InputKeys)
		{
			if (EventsConflict(inputKey.Event, oldEvent))
			{
				InputManager.SwapEvent(action, inputKey.Event, newEvent);
				inputKey.SetEventNoSignal(newEvent);
				return;
			}
		}
	}

	public Array<InputEvent> GetInputEvents() => new(InputKeys.Select(button => button.Event));

	private void LoadEvents()
	{
		Array<InputEvent> inputEvents = InputManager.GetInputEvents(Action);
		for (int i = 0; i < InputKeys.Count; i++)
		{
			InputKeys[i].SetEventNoSignal(i >= inputEvents.Count ? default : inputEvents[i]);
		}
	}

	private void SwapEvents(InputEvent oldEvent, InputEvent newEvent)
	{
		foreach (InputButtonSetting inputKey in InputKeys)
		{
			if (inputKey.Event != default && EventsConflict(inputKey.Event, newEvent))
			{
				// The new value set conflicts with one of the other values, swap accordingly
				inputKey.SetEventNoSignal(oldEvent);
				return;
			}
		}
		EmitSignal(SignalName.ChangedEvent, oldEvent, newEvent);
		InputManager.SwapEvent(Action, oldEvent, newEvent);
	}

	public static bool EventsConflict(InputEvent eventOne, InputEvent eventTwo)
	{
		if (eventOne == default || eventTwo == default)
			return false;
		if (eventOne.GetType() != eventTwo.GetType())
			return false;

		switch (eventOne)
		{
			case InputEventKey eventOneKey:
				InputEventKey eventTwoKey = eventTwo as InputEventKey;
				return eventOneKey.PhysicalKeycode == eventTwoKey!.PhysicalKeycode;
			case InputEventJoypadButton eventOneJoypad:
				InputEventJoypadButton eventTwoJoypad = eventTwo as InputEventJoypadButton;
				return eventOneJoypad.ButtonIndex == eventTwoJoypad!.ButtonIndex;
			case InputEventMouseButton eventOneMouseButton:
				InputEventMouseButton eventTwoMouseButton = eventTwo as InputEventMouseButton;
				return eventOneMouseButton.ButtonIndex == eventTwoMouseButton!.ButtonIndex;
			default:
				return false;
		}
	}
}
