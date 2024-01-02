using Godot;
using Godot.Collections;
using Utilities;

namespace UI;

public partial class LoadingScreen : Control
{
    private readonly float _startTime = Time.GetTicksMsec();
    private ProgressBar _progressBar;
    private Array _progressPercentage = [];
    private string _sceneToLoad;

    public override void _Ready()
    {
        base._Ready();
        _sceneToLoad = SceneManager.DesiredScene;
        _progressBar = GetNode<ProgressBar>("%ProgressBar");
        _progressPercentage.Resize(1); // Only need capacity of 1
        Error err = ResourceLoader.LoadThreadedRequest(_sceneToLoad, useSubThreads: true);
        if (err != Error.Ok)
        {
            Logger.WriteError($"LoadingScreen::_Ready() - ResourceLoader returned {err}");
            GetTree().Crash(ExitCodes.LOAD);
        }
    }

    public override void _Process(double delta)
    {
        ResourceLoader.ThreadLoadStatus loadStatus =
            ResourceLoader.LoadThreadedGetStatus(_sceneToLoad, _progressPercentage);
        switch (loadStatus)
        {
            case ResourceLoader.ThreadLoadStatus.Loaded:
                Error err = GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.LoadThreadedGet(_sceneToLoad));
                switch (err)
                {
                    case Error.Ok:
                        Logger.WriteInfo($"LoadingScreen::_Process - Successfully loaded {_sceneToLoad}");
                        Logger.WriteDebug(
                            $"LoadingScreen::_Process - Time to load {_sceneToLoad} = {Time.GetTicksMsec() - _startTime} ms");
                        break;
                    default:
                        Logger.WriteError($"LoadingScreen::_Process - Failed to load {_sceneToLoad} - Error {err}");
                        GetTree().Crash(ExitCodes.LOAD);
                        break;
                }

                break;
            case ResourceLoader.ThreadLoadStatus.InProgress:
                _progressBar.Value = (double)_progressPercentage[0] * 100;
                break;
            case ResourceLoader.ThreadLoadStatus.InvalidResource:
                Logger.WriteError($"LoadingScreen::_Process - {_sceneToLoad} is invalid");
                GetTree().Crash(ExitCodes.NO_FILE);
                break;
            case ResourceLoader.ThreadLoadStatus.Failed:
                Logger.WriteError($"LoadingScreen::_Process - {_sceneToLoad} failed to load");
                GetTree().Crash(ExitCodes.LOAD);
                break;
        }
    }
}
