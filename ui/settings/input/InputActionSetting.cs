using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;
using Utilities;

namespace UI;

public partial class InputActionSetting : Node
{
    [Signal]
    public delegate void ChangedEventEventHandler(InputEvent oldEvent, InputEvent newEvent);

    private StringName _action;
    private Label _actionLabel;
    private Array<InputButtonSetting> _inputKeys;

    [Export(PropertyHint.NodePathValidTypes, "InputButtonSetting")]
    public Array<NodePath> InputKeyPaths;

    [Export]
    public StringName Action
    {
        get => _action;
        set
        {
            if (Action == value) return;
            _action = value;
            _actionLabel.Text = value.ToString();
            LoadEvents();
        }
    }

    public override void _Ready()
    {
        _actionLabel = GetNode<Label>("%Action");
        _inputKeys = new Array<InputButtonSetting>(InputKeyPaths.Select(GetNode<InputButtonSetting>));
        InputManager.Instance.CheckedConnect(InputManager.SignalName.IsControllerChanged, Callable.From(LoadEvents));
        foreach (InputButtonSetting inputKey in _inputKeys)
            inputKey.CheckedConnect(InputButtonSetting.SignalName.EventChanged,
                                    Callable.From<InputEvent, InputEvent>(SwapEvents));
    }

    // This is called by the parent when this action's event conflicts with another action's desired event.
    public void ForceChangeEvent(InputEvent oldEvent, InputEvent newEvent)
    {
        foreach (InputButtonSetting inputKey in _inputKeys)
        {
            if (EventsConflict(inputKey.Event, oldEvent))
            {
                InputManager.SwapEvent(_action, inputKey.Event, newEvent);
                inputKey.SetEventNoSignal(newEvent);
                return;
            }
        }
    }

    public Array<InputEvent> GetInputEvents()
    {
        return new Array<InputEvent>(_inputKeys.Select(button => button.Event));
    }

    private void LoadEvents()
    {
        Array<InputEvent> inputEvents = InputManager.GetInputEvents(Action);
        for (int i = 0; i < _inputKeys.Count; i++)
            _inputKeys[i].SetEventNoSignal(i >= inputEvents.Count ? default : inputEvents[i]);
    }

    private void SwapEvents(InputEvent oldEvent, InputEvent newEvent)
    {
        foreach (InputButtonSetting inputKey in _inputKeys)
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
                Debug.Assert(eventTwoKey != null, nameof(eventTwoKey) + " != null");
                return eventOneKey.PhysicalKeycode == eventTwoKey.PhysicalKeycode;
            case InputEventJoypadButton eventOneJoypad:
                InputEventJoypadButton eventTwoJoypad = eventTwo as InputEventJoypadButton;
                Debug.Assert(eventTwoJoypad != null, nameof(eventTwoJoypad) + " != null");
                return eventOneJoypad.ButtonIndex == eventTwoJoypad.ButtonIndex;
            case InputEventMouseButton eventOneMouseButton:
                InputEventMouseButton eventTwoMouseButton = eventTwo as InputEventMouseButton;
                Debug.Assert(eventTwoMouseButton != null, nameof(eventTwoMouseButton) + " != null");
                return eventOneMouseButton.ButtonIndex == eventTwoMouseButton.ButtonIndex;
            default:
                return false;
        }
    }
}
