using Engine.Impl.UI.Menu;

public interface IBaseConsoleController {
	void SetViewToOperate(UIWindow window);

	void ComputeJoystick();

	void ProcessMouseEvent();

	void SetFocusToFirstElement();
}