using Godot;
using System;

namespace UI;
public partial class Settings : VBoxContainer
{
	[Signal]
	public delegate void BackPressedEventHandler();

	public void _on_back_pressed()
	{
		EmitSignal(SignalName.BackPressed);
	}
}
