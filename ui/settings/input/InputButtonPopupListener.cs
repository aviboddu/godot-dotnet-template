using Godot;

namespace UI;
public partial class InputButtonPopupListener : PopupPanel
{
	[Signal]
	public delegate void HeardEventEventHandler(InputEvent heardEvent);

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (!Visible || !@event.IsPressed() || @event.IsEcho()) return;
		switch (@event)
		{
			case InputEventKey keyEvent:
				if (keyEvent.Keycode != Key.Backspace)
				{
					InputEventKey newKeyEvent = new()
					{
						PhysicalKeycode = keyEvent.PhysicalKeycode,
						Echo = false,
						Pressed = true,
					};
					EmitSignal(SignalName.HeardEvent, newKeyEvent);
				}
				break;
			case InputEventJoypadButton joypadButton:
				InputEventJoypadButton newJoypadEvent = new()
				{
					ButtonIndex = joypadButton.ButtonIndex,
					Pressed = true,
				};
				EmitSignal(SignalName.HeardEvent, newJoypadEvent);
				break;
			case InputEventMouseButton mouseButton:
				InputEventMouseButton newMouseEvent = new()
				{
					ButtonIndex = mouseButton.ButtonIndex,
					ButtonMask = mouseButton.ButtonMask,
					Pressed = true
				};
				EmitSignal(SignalName.HeardEvent, newMouseEvent);
				break;
		}
		Visible = false;
	}
}
