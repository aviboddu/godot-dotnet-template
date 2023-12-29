using Godot;

namespace UI;
public partial class Settings : Control
{
	[Signal]
	public delegate void BackPressedEventHandler();
	public void _on_back_pressed() => EmitSignal(SignalName.BackPressed);
}
