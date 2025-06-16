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
    private Item[] items = new Item[0];

    private void Start()
    {
      UpdateVisibility();
      foreach (Item obj in items)
      {
        HideableView hideableView = obj.GetHideableView();
        if (hideableView != null)
        {
          hideableView.OnChangeEvent += UpdateVisibility;
          hideableView.OnSkipAnimationEvent += SkipAnimation;
        }
      }
    }

    private void OnDestroy()
    {
      foreach (Item obj in items)
      {
        HideableView hideableView = obj.GetHideableView();
        if (hideableView != null)
        {
          hideableView.OnChangeEvent -= UpdateVisibility;
          hideableView.OnSkipAnimationEvent -= SkipAnimation;
        }
      }
    }

    private void SkipAnimation()
    {
      if (!(hideableView != null))
        return;
      hideableView.SkipAnimation();
    }

    private void UpdateVisibility()
    {
      bool defaultVisibility = this.defaultVisibility;
      for (int index = 0; index < items.Length; ++index)
      {
        HideableView hideableView = items[index].GetHideableView();
        if (!(hideableView == null))
        {
          bool flag = hideableView.Visible;
          if (items[index].Negated)
            flag = !flag;
          switch (items[index].Operation)
          {
            case Item.OperationEnum.Or:
              defaultVisibility |= flag;
              break;
            case Item.OperationEnum.And:
              defaultVisibility &= flag;
              break;
            case Item.OperationEnum.ExclusiveOr:
              defaultVisibility ^= flag;
              break;
          }
        }
      }
      if (!(this.hideableView != null))
        return;
      this.hideableView.Visible = defaultVisibility;
    }

    [Serializable]
    public struct Item
    {
      public GameObject Reference;
      public OperationEnum Operation;
      public bool Negated;

      public HideableView GetHideableView() => Reference?.GetComponent<HideableView>();

      public enum OperationEnum
      {
        Or,
        And,
        ExclusiveOr,
      }
    }
  }
}
