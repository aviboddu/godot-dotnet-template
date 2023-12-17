using System.Diagnostics;
using Godot;
using Godot.Collections;
using Utilities;

namespace UI;
public partial class LoadingScreen : Control
{
	[Export(PropertyHint.File, "*.tscn")]
	public string ScenePath { get; set; }

	private ProgressBar progressBar;

	private Array progressPercentage;

#if DEBUG
	private float startTime;
#endif

	public override void _Ready()
	{
		base._Ready();
		Debug.Assert(ScenePath is not null, $"LoadingScreen::_Ready() - {PropertyName.ScenePath} is null");
#if DEBUG
		startTime = Time.GetTicksMsec();
#endif
		progressBar = GetNode<ProgressBar>("%ProgressBar");
		progressPercentage = [];
		progressPercentage.Resize(1);
		progressBar.Value = 0;
		ResourceLoader.LoadThreadedRequest(ScenePath);
	}

	public override void _Process(double delta)
	{
		ResourceLoader.ThreadLoadStatus loadStatus = ResourceLoader.LoadThreadedGetStatus(ScenePath, progressPercentage);
		switch (loadStatus)
		{
			case ResourceLoader.ThreadLoadStatus.Loaded:
				Error err = GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.LoadThreadedGet(ScenePath));
				switch (err)
				{
					case Error.Ok:
						Logger.WriteInfo($"LoadingScreen::_Process - Successfully loaded {ScenePath}");
						break;
					case Error.CantCreate:
						Logger.WriteError($"LoadingScreen::_Process - Failed to load {ScenePath}");
						break;
					case Error.InvalidParameter:
						Logger.WriteError($"LoadingScreen::_Process - {ScenePath} is invalid");
						break;
				}
#if DEBUG
				Logger.WriteDebug($"LoadingScreen::_Process - Time to load {ScenePath} = {Time.GetTicksMsec() - startTime} ms");
#endif
				break;
			case ResourceLoader.ThreadLoadStatus.InProgress:
				progressBar.Value = (double)progressPercentage[0] * 100;
				break;
			case ResourceLoader.ThreadLoadStatus.InvalidResource:
				Logger.WriteError($"LoadingScreen::_Process - {ScenePath} is invalid");
				GetTree().Quit(-1);
				break;
			case ResourceLoader.ThreadLoadStatus.Failed:
				Logger.WriteError($"LoadingScreen::_Process - {ScenePath} failed to load");
				GetTree().Quit(-1);
				break;
		}
	}
}
