using Godot;
using Godot.Collections;

namespace Utilities;

public partial class InputManager : Singleton<InputManager>
{
	private const string INPUT_SECTION = "Input";

	public override void _Ready()
	{
#if DEBUG
		ulong ticks = Time.GetTicksMsec();
#endif

		if (Configuration.Instance.HasSection(INPUT_SECTION))
		{
			Logger.WriteInfo("InputManager::_Ready() - Initializing Bindings from Configuration");
			foreach (StringName action in InputMap.GetActions())
			{
				InputMap.ActionEraseEvents(action);
				foreach (InputEvent inputEvent in Configuration.Instance.GetSetting<Array<InputEvent>>(INPUT_SECTION, action))
					InputMap.ActionAddEvent(action, inputEvent);
			}
		}
		else
		{
			Logger.WriteInfo("InputManager::_Ready() - Default Initialization");
			foreach (StringName action in InputMap.GetActions())
				Configuration.Instance.ChangeSetting(INPUT_SECTION, action, InputMap.ActionGetEvents(action));
			Configuration.Instance.Flush();
		}
#if DEBUG
		Logger.WriteDebug($"InputManager::_Ready() - Time to Initialize {Time.GetTicksMsec() - ticks} ms");
#endif
	}

	public void SwapEvent(StringName action, InputEvent remove, InputEvent add)
	{
		if (remove != null) InputMap.ActionEraseEvent(action, remove);
		if (add != null) InputMap.ActionAddEvent(action, add);
		Configuration.Instance.ChangeSetting(INPUT_SECTION, action, InputMap.ActionGetEvents(action));
	}
}
