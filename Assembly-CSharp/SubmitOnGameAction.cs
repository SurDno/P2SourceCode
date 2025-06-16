// Decompiled with JetBrains decompiler
// Type: SubmitOnGameAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
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
    for (int index = 0; index < this.actions.Length; ++index)
      service.RemoveListener(this.actions[index], this.onAction);
  }

  private void OnEnable()
  {
    if (this.onAction == null)
      this.onAction = new GameActionHandle(this.OnAction);
    GameActionService service = ServiceLocator.GetService<GameActionService>();
    for (int index = 0; index < this.actions.Length; ++index)
      service.AddListener(this.actions[index], this.onAction);
  }
}
