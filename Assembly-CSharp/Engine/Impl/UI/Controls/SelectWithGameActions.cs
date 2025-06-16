// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.SelectWithGameActions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class SelectWithGameActions : MonoBehaviour
  {
    [SerializeField]
    private GameObject[] selectables;
    [SerializeField]
    private GameActionType[] decreaseActions;
    [SerializeField]
    private GameActionType[] increaseActions;
    [SerializeField]
    private int defaultSelected = -1;
    [SerializeField]
    private bool wrap;
    private GameActionHandle onDecreaseAction;
    private GameActionHandle onIncreaseAction;

    private int Current()
    {
      GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
      if ((Object) selectedGameObject == (Object) null)
        return -1;
      for (int index = 0; index < this.selectables.Length; ++index)
      {
        if ((Object) selectedGameObject == (Object) this.selectables[index])
          return index;
      }
      return -1;
    }

    private bool OnDecrease(GameActionType type, bool down)
    {
      if (!down)
        return false;
      int index = this.Current();
      switch (index)
      {
        case -1:
          index = 0;
          break;
        case 0:
          if (this.wrap)
          {
            index = this.selectables.Length - 1;
            break;
          }
          break;
        default:
          --index;
          break;
      }
      this.Select(index);
      return true;
    }

    private bool OnIncrease(GameActionType type, bool down)
    {
      if (!down)
        return false;
      int index = this.Current();
      int num = this.selectables.Length - 1;
      if (index == -1)
        index = num;
      else if (index == num)
      {
        if (this.wrap)
          index = 0;
      }
      else
        ++index;
      this.Select(index);
      return true;
    }

    private void OnDisable()
    {
      if (this.Current() != -1)
        EventSystem.current.SetSelectedGameObject((GameObject) null);
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      for (int index = 0; index < this.decreaseActions.Length; ++index)
        service.RemoveListener(this.decreaseActions[index], this.onDecreaseAction);
      for (int index = 0; index < this.increaseActions.Length; ++index)
        service.RemoveListener(this.increaseActions[index], this.onIncreaseAction);
    }

    private void OnEnable()
    {
      if (this.onDecreaseAction == null)
        this.onDecreaseAction = new GameActionHandle(this.OnDecrease);
      if (this.onIncreaseAction == null)
        this.onIncreaseAction = new GameActionHandle(this.OnIncrease);
      GameActionService service = ServiceLocator.GetService<GameActionService>();
      for (int index = 0; index < this.decreaseActions.Length; ++index)
        service.AddListener(this.decreaseActions[index], this.onDecreaseAction);
      for (int index = 0; index < this.increaseActions.Length; ++index)
        service.AddListener(this.increaseActions[index], this.onIncreaseAction);
      if (this.defaultSelected < 0 || this.defaultSelected >= this.selectables.Length)
        return;
      this.Select(this.defaultSelected);
    }

    private void Select(int index)
    {
      EventSystem.current.SetSelectedGameObject(this.selectables[index]);
    }
  }
}
