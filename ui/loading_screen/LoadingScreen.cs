using Godot;
using Godot.Collections;
using Utilities;

namespace UI;
public partial class LoadingScreen : Control
{
	private ProgressBar progressBar;
	private Array progressPercentage = [];
	private string sceneToLoad;
	private float startTime;

	public override void _Ready()
	{
		base._Ready();
		startTime = Time.GetTicksMsec();
		sceneToLoad = SceneManager.DesiredScene;
		progressBar = GetNode<ProgressBar>("%ProgressBar");
		progressPercentage.Resize(1); // Only need capacity of 1
		Error err = ResourceLoader.LoadThreadedRequest(sceneToLoad, useSubThreads: true);
		if (err != Error.Ok)
		{
			Logger.WriteError($"LoadingScreen::_Ready() - ResourceLoader returned {err}");
			GetTree().Crash(ExitCodes.EXIT_LOAD);
		}
	}

	public override void _Process(double delta)
	{
		ResourceLoader.ThreadLoadStatus loadStatus = ResourceLoader.LoadThreadedGetStatus(sceneToLoad, progressPercentage);
		switch (loadStatus)
		{
			case ResourceLoader.ThreadLoadStatus.Loaded:
				Error err = GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.LoadThreadedGet(sceneToLoad));
				switch (err)
				{
					case Error.Ok:
						Logger.WriteInfo($"LoadingScreen::_Process - Successfully loaded {sceneToLoad}");
						Logger.WriteDebug($"LoadingScreen::_Process - Time to load {sceneToLoad} = {Time.GetTicksMsec() - startTime} ms");
						break;
					default:
						Logger.WriteError($"LoadingScreen::_Process - Failed to load {sceneToLoad} - Error {err}");
						GetTree().Crash(ExitCodes.EXIT_LOAD);
						break;
				}
				break;
			case ResourceLoader.ThreadLoadStatus.InProgress:
				progressBar.Value = (double)progressPercentage[0] * 100;
				break;
			case ResourceLoader.ThreadLoadStatus.InvalidResource:
				Logger.WriteError($"LoadingScreen::_Process - {sceneToLoad} is invalid");
				GetTree().Crash(ExitCodes.EXIT_NOFILE);
				break;
			case ResourceLoader.ThreadLoadStatus.Failed:
				Logger.WriteError($"LoadingScreen::_Process - {sceneToLoad} failed to load");
				GetTree().Crash(ExitCodes.EXIT_LOAD);
				break;
		}
	}
}
