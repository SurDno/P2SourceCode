using Engine.Common.Components;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory.Windows
{
  [DisallowMultipleComponent]
  public class ContextMenuWindow : UIControl
  {
    [SerializeField]
    private Button buttonInvestigate;
    [SerializeField]
    private Button buttonDrop;
    [SerializeField]
    private Button buttonWear;
    [SerializeField]
    private Button buttonUse;
    private IStorableComponent target;

    public event Action<IStorableComponent> OnButtonInvestigate;

    public event Action<IStorableComponent> OnButtonDrop;

    public event Action<IStorableComponent> OnButtonWear;

    public event Action<IStorableComponent> OnButtonUse;

    public IStorableComponent Target
    {
      get => this.target;
      set
      {
        if (this.target == value)
          return;
        this.target = value;
        this.buttonUse.gameObject.SetActive(StorableComponentUtility.IsUsable(this.target));
        this.buttonWear.gameObject.SetActive(StorableComponentUtility.IsWearable(this.target));
      }
    }

    public static ContextMenuWindow Instantiate(GameObject prefab)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
      gameObject.name = prefab.name;
      ContextMenuWindow component = gameObject.GetComponent<ContextMenuWindow>();
      component.buttonInvestigate.onClick.AddListener(new UnityAction(component.Investigate));
      component.buttonDrop.onClick.AddListener(new UnityAction(component.Drop));
      component.buttonUse.onClick.AddListener(new UnityAction(component.Use));
      component.buttonWear.onClick.AddListener(new UnityAction(component.Wear));
      return component;
    }

    public void Investigate()
    {
      if (this.target == null || this.OnButtonInvestigate == null)
        return;
      this.OnButtonInvestigate(this.target);
    }

    public void Drop()
    {
      if (this.target == null || this.OnButtonDrop == null)
        return;
      this.OnButtonDrop(this.target);
    }

    public void Wear()
    {
      if (this.target == null || this.OnButtonWear == null || !StorableComponentUtility.IsWearable(this.target))
        return;
      this.OnButtonWear(this.target);
    }

    public void Use()
    {
      if (this.target == null || this.OnButtonUse == null || !StorableComponentUtility.IsUsable(this.target))
        return;
      this.OnButtonUse(this.target);
    }
  }
}
