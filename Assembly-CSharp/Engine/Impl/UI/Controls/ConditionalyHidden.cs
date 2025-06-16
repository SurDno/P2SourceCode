using System;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ConditionalyHidden : MonoBehaviour
  {
    [SerializeField]
    private HideableView hideableView;
    [SerializeField]
    private bool defaultVisibility;
    [SerializeField]
    private ConditionalyHidden.Item[] items = new ConditionalyHidden.Item[0];

    private void Start()
    {
      this.UpdateVisibility();
      foreach (ConditionalyHidden.Item obj in this.items)
      {
        HideableView hideableView = obj.GetHideableView();
        if ((UnityEngine.Object) hideableView != (UnityEngine.Object) null)
        {
          hideableView.OnChangeEvent += new Action(this.UpdateVisibility);
          hideableView.OnSkipAnimationEvent += new Action(this.SkipAnimation);
        }
      }
    }

    private void OnDestroy()
    {
      foreach (ConditionalyHidden.Item obj in this.items)
      {
        HideableView hideableView = obj.GetHideableView();
        if ((UnityEngine.Object) hideableView != (UnityEngine.Object) null)
        {
          hideableView.OnChangeEvent -= new Action(this.UpdateVisibility);
          hideableView.OnSkipAnimationEvent -= new Action(this.SkipAnimation);
        }
      }
    }

    private void SkipAnimation()
    {
      if (!((UnityEngine.Object) this.hideableView != (UnityEngine.Object) null))
        return;
      this.hideableView.SkipAnimation();
    }

    private void UpdateVisibility()
    {
      bool defaultVisibility = this.defaultVisibility;
      for (int index = 0; index < this.items.Length; ++index)
      {
        HideableView hideableView = this.items[index].GetHideableView();
        if (!((UnityEngine.Object) hideableView == (UnityEngine.Object) null))
        {
          bool flag = hideableView.Visible;
          if (this.items[index].Negated)
            flag = !flag;
          switch (this.items[index].Operation)
          {
            case ConditionalyHidden.Item.OperationEnum.Or:
              defaultVisibility |= flag;
              break;
            case ConditionalyHidden.Item.OperationEnum.And:
              defaultVisibility &= flag;
              break;
            case ConditionalyHidden.Item.OperationEnum.ExclusiveOr:
              defaultVisibility ^= flag;
              break;
          }
        }
      }
      if (!((UnityEngine.Object) this.hideableView != (UnityEngine.Object) null))
        return;
      this.hideableView.Visible = defaultVisibility;
    }

    [Serializable]
    public struct Item
    {
      public GameObject Reference;
      public ConditionalyHidden.Item.OperationEnum Operation;
      public bool Negated;

      public HideableView GetHideableView() => this.Reference?.GetComponent<HideableView>();

      public enum OperationEnum
      {
        Or,
        And,
        ExclusiveOr,
      }
    }
  }
}
