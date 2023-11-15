using Godot;
using System;

public partial class MainMenu : Control
{
	public void _on_quit_pressed() {
		GetTree().Quit();
	}
}
