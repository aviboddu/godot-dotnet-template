using System.Linq;
using Godot;
using Godot.Collections;

namespace Utilities;

public partial class InputManager : Node
{
    [Signal]
    public delegate void IsControllerChangedEventHandler(bool isController);

    private const string INPUT_SECTION = "Input";
    private bool _isController;
    public static InputManager Instance { get; private set; }

    public bool IsController
    {
        get => _isController;
        private set
        {
            if (IsController == value)
                return;
            _isController = value;
            EmitSignal(SignalName.IsControllerChanged, value);
        }
    }

    public override void _EnterTree()
    {
        if (Instance is not null)
        {
            QueueFree();
            return;
        }

        Instance = this;
        base._EnterTree();
    }

    public override void _Ready()
    {
        ulong ticks = Time.GetTicksMsec();

        if (Configuration.Instance.HasSection(INPUT_SECTION))
        {
            Logger.WriteInfo("InputManager::_Ready() - Initializing Bindings from Configuration");
            foreach (StringName action in GetCustomActions())
            {
                InputMap.ActionEraseEvents(action);
                foreach (InputEvent inputEvent in Configuration.Instance.GetSetting<Array<InputEvent>>(
                             INPUT_SECTION, action))
                    InputMap.ActionAddEvent(action, inputEvent);
            }
        }
        else
        {
            Logger.WriteInfo("InputManager::_Ready() - Default Initialization");
            foreach (StringName action in GetCustomActions())
                Configuration.Instance.ChangeSetting(INPUT_SECTION, action, InputMap.ActionGetEvents(action));
            Configuration.Instance.Flush();
        }

        Logger.WriteDebug($"InputManager::_Ready() - Time to Initialize {Time.GetTicksMsec() - ticks} ms");
    }

    public static void SwapEvent(StringName action, InputEvent remove, InputEvent add)
    {
        if (remove != default) InputMap.ActionEraseEvent(action, remove);
        if (add != default) InputMap.ActionAddEvent(action, add);
        Configuration.Instance.ChangeSetting(INPUT_SECTION, action, InputMap.ActionGetEvents(action));
    }

    // Gets all relevant events. If the user is using a controller, then we return all controller events,
    // 		if the user is using KB+M, we return all KB+M events
    public static Array<InputEvent> GetInputEvents(StringName action)
    {
        Array<InputEvent> events = InputMap.ActionGetEvents(action);
        return new Array<InputEvent>(events.Where(e => Instance.IsController == IsControllerEvent(e)));
    }

    // Gets custom (not built-in) events
    public static Array<StringName> GetCustomActions()
    {
        Array<StringName> actions = InputMap.GetActions();
        return new Array<StringName>(actions.Where(s => !IsBuiltIn(s)));
    }

    private static bool IsControllerEvent(InputEvent inputEvent)
    {
        return inputEvent is InputEventJoypadButton || inputEvent is InputEventJoypadMotion;
    }

    private static bool IsBuiltIn(StringName action)
    {
        return action.ToString().StartsWith("ui") && InputMap.HasAction(action);
    }

    // Keeps track of whether user is using a controller or KB+M
    public override void _Input(InputEvent @event)
    {
        IsController = IsControllerEvent(@event);
        base._Input(@event);
    }
}
