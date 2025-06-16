// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.Inventory.CraftMixtureWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;
using Inspectors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public class CraftMixtureWindow : SelectableInventoryWindow, ICraftMixtureWindow, IWindow
  {
    [SerializeField]
    private HideableView craftButtonInteractable;
    [SerializeField]
    private Button craftButton;
    [SerializeField]
    private ItemView resultView;
    [SerializeField]
    private float resultShowTime = 1f;
    [SerializeField]
    private AudioClip craftAudio;
    [SerializeField]
    private GameObject craftButtonConsole;
    private Text craftBrewText;
    private bool isCraftAvailable = false;

    [Inspected]
    public IStorageComponent Target { get; set; }

    protected override void CraftWindowSubscribe()
    {
      base.CraftWindowSubscribe();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(((SelectableInventoryWindow) this).ConsoleController));
    }

    protected override void CraftWindowUnsubscribe()
    {
      base.CraftWindowUnsubscribe();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(((SelectableInventoryWindow) this).ConsoleController));
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.craftBrewText = this.craftButton.GetComponentInChildren<Text>();
      this.actors.Clear();
      this.actors.Add(this.Actor);
      this.Build2();
      this.CreateContainers();
    }

    protected override void OnDisable()
    {
      this.DisableCraftButtons();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.CraftMixture, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
      this.CraftWindowUnsubscribe();
      base.OnDisable();
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      if (!joystick)
      {
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.CraftMixture, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
          ingredientSelector.SetSelection(false);
        this.PredictCraftRecipe();
        Text componentInChildren1 = this.ingredientSelectors[0].GetComponentInChildren<Text>();
        componentInChildren1.transform.localPosition = new Vector3(componentInChildren1.transform.localPosition.x, 190f);
        Text componentInChildren2 = this.ingredientSelectors[1].GetComponentInChildren<Text>();
        componentInChildren2.transform.localPosition = new Vector3(componentInChildren2.transform.localPosition.x, -190f);
      }
      else
      {
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.CraftMixture, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        this.PredictCraftRecipe();
        Text componentInChildren3 = this.ingredientSelectors[0].GetComponentInChildren<Text>();
        componentInChildren3.transform.localPosition = new Vector3(componentInChildren3.transform.localPosition.x, 0.0f);
        Text componentInChildren4 = this.ingredientSelectors[1].GetComponentInChildren<Text>();
        componentInChildren4.transform.localPosition = new Vector3(componentInChildren4.transform.localPosition.x, 0.0f);
      }
      if (this.isCraftAvailable)
        this.craftButtonConsole.SetActive(joystick);
      else
        this.craftButtonConsole.SetActive(false);
    }

    protected override bool ConsoleController(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (type == GameActionType.Submit & down && this.craftButton.gameObject.activeInHierarchy)
      {
        this.craftButton.onClick.Invoke();
        this.HideInfoWindow();
        return true;
      }
      base.ConsoleController(type, down);
      return false;
    }

    protected override void OnItemClick(IStorableComponent storable)
    {
      if (storable.Storage == this.Actor)
      {
        foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
        {
          if (ingredientSelector.Item == storable)
          {
            if (ingredientSelector.AvoidNull)
              return;
            ingredientSelector.Item = (IStorableComponent) null;
            return;
          }
        }
        foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
        {
          if (ingredientSelector.Item == null)
          {
            foreach (StorableGroup group in ingredientSelector.Groups)
            {
              if (storable.Groups.Contains<StorableGroup>(group))
              {
                ingredientSelector.Item = storable;
                return;
              }
            }
          }
        }
        foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
        {
          foreach (StorableGroup group in ingredientSelector.Groups)
          {
            if (storable.Groups.Contains<StorableGroup>(group))
            {
              ingredientSelector.Item = storable;
              return;
            }
          }
        }
      }
      else
      {
        IParameter<TimeSpan> craftTimeParameter = CraftHelper.GetCraftTimeParameter(storable);
        if (craftTimeParameter != null && craftTimeParameter.Value >= TimeSpan.Zero)
          return;
        this.MoveItem(storable, this.Actor);
        StorableComponentUtility.PlayTakeSound(storable);
        this.OnInvalidate();
      }
    }

    protected override void Clear()
    {
      foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
        ingredientSelector.Storage = (IStorageComponent) null;
      base.Clear();
    }

    protected override void OnInvalidate()
    {
      base.OnInvalidate();
      this.PredictCraftRecipe();
      if (!InputService.Instance.JoystickUsed)
        return;
      this.HideInfoWindow();
    }

    private void ClearResultView() => this.resultView.Storable = (StorableComponent) null;

    private void CreateContainers()
    {
      foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
        ingredientSelector.Storage = this.Actor;
    }

    private void Craft()
    {
      this.DisableCraftButtons();
      List<IStorableComponent> ingredients = new List<IStorableComponent>();
      for (int index = 0; index < this.ingredientSelectors.Length; ++index)
      {
        IStorableComponent storableComponent = this.ingredientSelectors[index].Item;
        if (storableComponent != null)
          ingredients.Add(storableComponent);
      }
      IStorableComponent craftResult = CraftHelper.GetCraftResult(ingredients, out CraftRecipe _);
      if (craftResult != null)
      {
        foreach (IStorableComponent storableComponent in ingredients)
        {
          if (storableComponent.Count > 1)
            --storableComponent.Count;
          else
            storableComponent.Owner.Dispose();
        }
        IEntity entity = ServiceLocator.GetService<IFactory>().Instantiate<IEntity>(craftResult.Owner);
        ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
        IStorableComponent component = entity.GetComponent<IStorableComponent>();
        this.Actor.AddItemOrDrop(component, (IInventoryComponent) null);
        this.CancelInvoke("ClearResultView");
        this.resultView.Storable = (StorableComponent) component;
        this.Invoke("ClearResultView", this.resultShowTime);
        if ((UnityEngine.Object) this.craftAudio != (UnityEngine.Object) null)
          this.PlayAudio(this.craftAudio);
      }
      this.OnInvalidate();
    }

    private void PredictCraftRecipe()
    {
      this.DisableCraftButtons();
      this.craftButtonConsole.SetActive(false);
      List<IStorableComponent> ingredients = new List<IStorableComponent>();
      for (int index = 0; index < this.ingredientSelectors.Length; ++index)
      {
        IStorableComponent storableComponent = this.ingredientSelectors[index].Item;
        if (storableComponent != null)
          ingredients.Add(storableComponent);
      }
      if (CraftHelper.GetCraftResult(ingredients, out CraftRecipe _) != null)
      {
        this.isCraftAvailable = true;
        if (InputService.Instance.JoystickUsed)
          this.craftButtonConsole.SetActive(true);
        this.EnableCraftButtons();
      }
      else
        this.isCraftAvailable = false;
    }

    private void EnableCraftButtons() => this.craftButtonInteractable.Visible = true;

    private void DisableCraftButtons() => this.craftButtonInteractable.Visible = false;

    public override void Initialize()
    {
      this.RegisterLayer<ICraftMixtureWindow>((ICraftMixtureWindow) this);
      foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
      {
        ingredientSelector.ChangeItemEvent += new Action<ItemSelector, IStorableComponent, IStorableComponent>(this.OnIngredientChange);
        ingredientSelector.ValidateItemEvent += new Func<ItemSelector, IStorableComponent, bool>(this.OnValidateIngredient);
      }
      this.craftButton.onClick.AddListener(new UnityAction(this.Craft));
      base.Initialize();
    }

    public override System.Type GetWindowType() => typeof (ICraftMixtureWindow);

    private bool OnValidateIngredient(ItemSelector itemSelector, IStorableComponent ingredient)
    {
      for (int index = 0; index < this.ingredientSelectors.Length; ++index)
      {
        ItemSelector ingredientSelector = this.ingredientSelectors[index];
        IStorableComponent storableComponent = ingredientSelector.Item;
        if ((UnityEngine.Object) itemSelector == (UnityEngine.Object) ingredientSelector && ingredient == storableComponent)
          return true;
        if (storableComponent != null && StorageUtility.GetItemId(ingredient.Owner) == StorageUtility.GetItemId(storableComponent.Owner))
          return false;
      }
      return true;
    }

    private void OnIngredientChange(
      ItemSelector itemSelector,
      IStorableComponent prevIngredient,
      IStorableComponent newIngredient)
    {
      if (prevIngredient != null)
        StorableComponentUtility.PlayPutSound(prevIngredient);
      if (newIngredient != null)
        StorableComponentUtility.PlayTakeSound(newIngredient);
      this.OnInvalidate();
    }

    protected override bool ItemIsInteresting(IStorableComponent item)
    {
      bool flag1 = false;
      bool flag2 = false;
      foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
      {
        if (item == ingredientSelector.Item)
          return !ingredientSelector.AvoidNull;
        foreach (StorableGroup group in ingredientSelector.Groups)
        {
          if (item.Groups.Contains<StorableGroup>(group))
            flag1 = true;
        }
        if (ingredientSelector.Item != null && StorageUtility.GetItemId(item.Owner) == StorageUtility.GetItemId(ingredientSelector.Item.Owner))
          flag2 = true;
      }
      return flag1 && !flag2;
    }

    protected override bool ItemIsSelected(IStorableComponent storable)
    {
      foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
      {
        if (ingredientSelector.Item == storable)
          return true;
      }
      return false;
    }
  }
}
