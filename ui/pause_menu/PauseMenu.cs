using Godot;
using Utilities;

namespace UI;

public partial class PauseMenu : Control
{
    private const string PAUSE = "Pause";
    private Control _mainPauseMenu;
    private Settings _settings;

    public override void _Ready()
    {
        base._Ready();
        _settings = GetNode<Settings>("%Settings");
        _mainPauseMenu = GetNode<Control>("%MainPauseMenu");

        _settings.CheckedConnect(Settings.SignalName.BackPressed, Callable.From(_on_back_pressed));
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed(PAUSE))
            if (GetTree().Paused)
                UnPause();
            else
                Pause();
    }

    public void _on_back_pressed()
    {
        _settings.Visible = false;
        _mainPauseMenu.Visible = true;
    }

    public void _on_settings_pressed()
    {
        _settings.Visible = true;
        _mainPauseMenu.Visible = false;
    }

    public void _on_quit_to_os_pressed()
    {
        Logger.WriteInfo("User Quit");
        GetTree().Exit();
    }

    public void Pause()
    {
        Logger.WriteInfo("PauseMenu::Pause() - Game paused");
        Visible = true;
        GetTree().Paused = true;
    }

    public void UnPause()
    {
        Logger.WriteInfo("PauseMenu::UnPause() - Game unpaused");

        // Ensures next pause will be from normal state
        _settings.Visible = false;
        _mainPauseMenu.Visible = true;

        // Flush any pending settings
        Configuration.Instance.Flush();

        Visible = false;
        GetTree().Paused = false;
    }
}
