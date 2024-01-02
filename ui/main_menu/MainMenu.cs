using Godot;
using Utilities;

namespace UI;

public partial class MainMenu : Control
{
    private Control _settings;
    private Control _startMenu;

    public override void _Ready()
    {
        base._Ready();
        _settings = GetNode<Control>("%Settings");
        _startMenu = GetNode<Control>("%StartMenu");

        _settings.CheckedConnect(Settings.SignalName.BackPressed, Callable.From(_on_back_pressed));
    }

    public void _on_settings_pressed()
    {
        Logger.WriteInfo("User Opened Settings");
        _settings.Visible = true;
        _startMenu.Visible = false;
    }

    public void _on_back_pressed()
    {
        Logger.WriteInfo("User Exited Settings");
        _settings.Visible = false;
        _startMenu.Visible = true;
    }

    public void _on_quit_pressed()
    {
        Logger.WriteInfo("User Quit");
        GetTree().Exit();
    }
}
