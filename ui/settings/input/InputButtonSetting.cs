using Godot;
using Utilities;

namespace UI;

public partial class InputButtonSetting : Button
{
	private const string NOT_SET = "Not Set";

	[Export]
	public InputEvent Event
	{
		get => _event;
		set
		{
			EmitSignal(SignalName.EventChanged, Event, value);
			SetEventNoSignal(value);
		}
	}
	// ReSharper disable once InconsistentNaming
	private InputEvent _event;

	[Signal]
	public delegate void EventChangedEventHandler(InputEvent oldEvent, InputEvent newEvent);

	private InputButtonPopupListener popupListener;

	public override void _Ready()
	{
		popupListener = GetNode<InputButtonPopupListener>("%PopupListener");
		popupListener.CheckedConnect(InputButtonPopupListener.SignalName.HeardEvent, Callable.From<InputEvent>(SetEvent));
	}

	public void SetEventNoSignal(InputEvent value)
	{
		_event = value;
		if (Event is InputEventKey keyEvent)
			Text = DisplayServer.KeyboardGetLabelFromPhysical(keyEvent.PhysicalKeycode).ToString();
		else
			Text = Event == default ? NOT_SET : Event.AsText();
	}

	public void SetEvent(InputEvent value) => Event = value;

	public override void _GuiInput(InputEvent @event)
	{
		base._GuiInput(@event);
		if (@event is InputEventMouseButton mouseButtonEvent && @event.IsPressed())
		{
			switch (mouseButtonEvent.ButtonIndex)
			{
				case MouseButton.Left:
					popupListener.Visible = true;
					GetViewport().SetInputAsHandled();
					break;
				case MouseButton.Middle:
					Event = default;
					break;
			}
		}
	}
}
