using Engine.Common.Services;
using Engine.Source.Services.Inputs;

public class SubmitOnGameAction : MonoBehaviour
{
  [SerializeField]
  private GameActionType[] actions;
  private GameActionHandle onAction;

  private bool OnAction(GameActionType type, bool down)
  {
    if (!down)
      return false;
    EventSystem current = EventSystem.current;
    GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
    if ((Object) selectedGameObject == (Object) null)
      return false;
    PointerEventData eventData = new PointerEventData(EventSystem.current);
    ExecuteEvents.Execute<ISubmitHandler>(selectedGameObject, (BaseEventData) eventData, ExecuteEvents.submitHandler);
    return true;
  }

  private void OnDisable()
  {
    GameActionService service = ServiceLocator.GetService<GameActionService>();
    for (int index = 0; index < actions.Length; ++index)
      service.RemoveListener(actions[index], onAction);
  }

  private void OnEnable()
  {
    if (onAction == null)
      onAction = OnAction;
    GameActionService service = ServiceLocator.GetService<GameActionService>();
    for (int index = 0; index < actions.Length; ++index)
      service.AddListener(actions[index], onAction);
  }
}
