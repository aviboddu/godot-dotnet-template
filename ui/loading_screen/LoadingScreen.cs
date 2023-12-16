using Godot;
using Godot.Collections;
using Utilities;

namespace UI;
public partial class LoadingScreen : Control
{
	[Export]
	public NodePath ScenePath { get; set; }

	[Export]
	public ProgressBar progressBar;

	private Array progressPercentage;

#if DEBUG
	private float startTime;
#endif

	public override void _Ready()
	{
#if DEBUG
		startTime = Time.GetTicksMsec();
#endif
		progressPercentage = [];
		progressPercentage.Resize(1);
		progressBar.Value = 0;
		ResourceLoader.LoadThreadedRequest(ScenePath);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ResourceLoader.ThreadLoadStatus loadStatus = ResourceLoader.LoadThreadedGetStatus(ScenePath, progressPercentage);
		switch (loadStatus)
		{
			case ResourceLoader.ThreadLoadStatus.Loaded:
				Logger.Instance.WriteInfo($"LoadingScreen::_Process - Successfully loaded {ScenePath}");
				GetTree().Root.AddChild(((PackedScene)ResourceLoader.LoadThreadedGet(ScenePath)).Instantiate());
				QueueFree();
#if DEBUG
				Logger.Instance.WriteDebug($"LoadingScreen::_Process - Time to load {ScenePath} = {Time.GetTicksMsec() - startTime} ms");
#endif
				break;
			case ResourceLoader.ThreadLoadStatus.InProgress:
				progressBar.Value = (double)progressPercentage[0];
				break;
			case ResourceLoader.ThreadLoadStatus.InvalidResource:
				Logger.Instance.WriteError($"LoadingScreen::_Process - {ScenePath} is invalid");
				GetTree().Quit(-1);
				break;
			case ResourceLoader.ThreadLoadStatus.Failed:
				Logger.Instance.WriteError($"LoadingScreen::_Process - {ScenePath} failed to load");
				GetTree().Quit(-1);
				break;
		}
	}
}
