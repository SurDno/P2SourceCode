using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Components;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;
using Inspectors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public class CraftBreweryWindow : SelectableInventoryWindow, ICraftBreweryWindow, IWindow
  {
    [SerializeField]
    private CraftBrewingSlotAnchor[] brewingSlotAnchors;
    [SerializeField]
    private CraftBrewingSlot brewingSlotPrefab;
    [SerializeField]
    private ItemView resultPreview;
    [SerializeField]
    private AudioClip craftAudio;
    [SerializeField]
    private GameObject slotNavigation;
    private bool isCraftAvailable = false;
    private List<CraftBrewingSlotAnchor> activeBrewingSlots = new List<CraftBrewingSlotAnchor>();
    private int currentBrewingSlotIndex = -1;
    private int CurrentActiveCrafts;
    private CraftBrewingSlotAnchor lastBrewingSlot = (CraftBrewingSlotAnchor) null;
    private bool isSlotNavigationEnabled = false;

    [Inspected]
    public IStorageComponent Target { get; set; }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.slotNavigation.SetActive(false);
      this.actors.Clear();
      this.actors.Add(this.Actor);
      this.Build2();
      this.CreateContainers();
      if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
      {
        this.HideInfoWindow();
        this.selectedStorable.SetSelected(false);
        this.selectedStorable = (StorableUI) null;
      }
      foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
      {
        StorableUI storableByComponent = this.GetStorableByComponent(ingredientSelector.Item);
        if ((UnityEngine.Object) storableByComponent != (UnityEngine.Object) null)
        {
          this.selectedStorable = storableByComponent;
          this.OnSelectObject(this.selectedStorable.gameObject);
          break;
        }
      }
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in this.brewingSlotAnchors)
      {
        brewingSlotAnchor.Slot.OnCraftPerformed += new CraftBrewingSlot.OnCraft(this.SlotCraftStarted);
        brewingSlotAnchor.Slot.OnItemTaken += new CraftBrewingSlot.OnCraft(this.SlotItemTaken);
      }
      this.CurrentActiveCrafts = ((IEnumerable<CraftBrewingSlotAnchor>) this.brewingSlotAnchors).Count<CraftBrewingSlotAnchor>((Func<CraftBrewingSlotAnchor, bool>) (anchor => anchor.Slot.IsSlotAvailable));
      if (this.CurrentActiveCrafts == 0)
        return;
      if (this.activeBrewingSlots.Count == 0)
        this.activeBrewingSlots = ((IEnumerable<CraftBrewingSlotAnchor>) this.brewingSlotAnchors).Where<CraftBrewingSlotAnchor>((Func<CraftBrewingSlotAnchor, bool>) (anchor => anchor.Slot.IsSlotAvailable)).ToList<CraftBrewingSlotAnchor>();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickUp, new GameActionHandle(this.NavigateOnSlots));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickDown, new GameActionHandle(this.NavigateOnSlots));
      this.isSlotNavigationEnabled = true;
      if ((UnityEngine.Object) this.lastBrewingSlot == (UnityEngine.Object) null)
        this.lastBrewingSlot = ((IEnumerable<CraftBrewingSlotAnchor>) this.brewingSlotAnchors).Where<CraftBrewingSlotAnchor>((Func<CraftBrewingSlotAnchor, bool>) (anchor => anchor.Slot.IsSlotAvailable)).Last<CraftBrewingSlotAnchor>();
      if ((UnityEngine.Object) this.lastBrewingSlot != (UnityEngine.Object) null)
      {
        this.lastBrewingSlot.Slot.SetEnabled(true);
        this.lastBrewingSlot.Slot.SetSelected(true);
        this.currentBrewingSlotIndex = this.activeBrewingSlots.IndexOf(this.lastBrewingSlot);
      }
    }

    protected override void OnDisable()
    {
      this.DisableCraftButtons();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.CraftBrewery, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
      this.currentBrewingSlotIndex = -1;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, new GameActionHandle(this.NavigateOnSlots));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, new GameActionHandle(this.NavigateOnSlots));
      this.lastBrewingSlot = (CraftBrewingSlotAnchor) null;
      this.activeBrewingSlots.Clear();
      this.isCraftAvailable = false;
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in this.brewingSlotAnchors)
      {
        brewingSlotAnchor.Slot.OnCraftPerformed -= new CraftBrewingSlot.OnCraft(this.SlotCraftStarted);
        brewingSlotAnchor.Slot.OnItemTaken -= new CraftBrewingSlot.OnCraft(this.SlotItemTaken);
      }
      base.OnDisable();
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      this.slotNavigation.SetActive(joystick);
      if (!joystick)
      {
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.CraftBrewery, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
          ingredientSelector.SetSelection(false);
        this.PreviousMode = this.CurrentMode;
        this.CurrentMode = SelectableInventoryWindow.Modes.None;
        foreach (CraftBrewingSlotAnchor brewingSlotAnchor in this.brewingSlotAnchors)
          brewingSlotAnchor.Slot.SetSelected(false);
      }
      else
      {
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.CraftBrewery, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        if (this.PreviousMode != 0)
          this.CurrentMode = this.PreviousMode;
        else
          this.CurrentMode = SelectableInventoryWindow.Modes.Craft;
        if (this.currentBrewingSlotIndex == -1)
          this.currentBrewingSlotIndex = this.activeBrewingSlots.Count - 1;
        if ((UnityEngine.Object) this.lastBrewingSlot != (UnityEngine.Object) null)
        {
          if (!this.lastBrewingSlot.Slot.IsEnabled)
            this.lastBrewingSlot.Slot.SetEnabled(true);
          if (!this.lastBrewingSlot.Slot.IsSelected)
            this.lastBrewingSlot.Slot.SetSelected(true);
          this.currentBrewingSlotIndex = this.activeBrewingSlots.IndexOf(this.lastBrewingSlot);
        }
      }
    }

    private bool NavigateOnSlots(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || !down)
        return false;
      if (this.currentBrewingSlotIndex != -1 && this.currentBrewingSlotIndex < this.activeBrewingSlots.Count && (UnityEngine.Object) this.activeBrewingSlots[this.currentBrewingSlotIndex] != (UnityEngine.Object) null)
        this.activeBrewingSlots[this.currentBrewingSlotIndex]?.Slot.SetSelected(false);
      this.currentBrewingSlotIndex += type == GameActionType.RStickUp ? 1 : -1;
      if (this.currentBrewingSlotIndex < 0)
        this.currentBrewingSlotIndex = this.currentBrewingSlotIndex = this.activeBrewingSlots.Count - 1;
      if (this.currentBrewingSlotIndex > this.activeBrewingSlots.Count - 1)
        this.currentBrewingSlotIndex = 0;
      if ((UnityEngine.Object) this.activeBrewingSlots[this.currentBrewingSlotIndex] != (UnityEngine.Object) null)
      {
        this.activeBrewingSlots[this.currentBrewingSlotIndex].Slot.SetSelected(true);
        if (this.activeBrewingSlots[this.currentBrewingSlotIndex].Slot.IsSlotAvailable)
          this.lastBrewingSlot = this.activeBrewingSlots[this.currentBrewingSlotIndex];
      }
      return true;
    }

    protected override SelectableInventoryWindow.Modes CurrentMode
    {
      get => base.CurrentMode;
      set
      {
        if (this.CurrentMode == value || !InputService.Instance.JoystickUsed)
          return;
        switch (value)
        {
          case SelectableInventoryWindow.Modes.Inventory:
            return;
          case SelectableInventoryWindow.Modes.Craft:
            if (InputService.Instance.JoystickUsed && this.isCraftAvailable)
            {
              ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickUp, new GameActionHandle(this.NavigateOnSlots));
              ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickDown, new GameActionHandle(this.NavigateOnSlots));
              if (this.currentBrewingSlotIndex == -1)
                this.currentBrewingSlotIndex = this.activeBrewingSlots.Count - 1;
              this.activeBrewingSlots[this.currentBrewingSlotIndex].Slot.SetSelected(true);
              break;
            }
            break;
        }
        base.CurrentMode = value;
      }
    }

    private void SlotCraftStarted(CraftBrewingSlot slot)
    {
      foreach (CraftBrewingSlotAnchor activeBrewingSlot in this.activeBrewingSlots)
      {
        if ((UnityEngine.Object) activeBrewingSlot.Slot == (UnityEngine.Object) slot)
        {
          this.lastBrewingSlot = activeBrewingSlot;
          break;
        }
      }
      this.OnInvalidate();
    }

    private void SlotItemTaken(CraftBrewingSlot slot)
    {
      if ((UnityEngine.Object) this.lastBrewingSlot == (UnityEngine.Object) slot)
        this.lastBrewingSlot = (CraftBrewingSlotAnchor) null;
      if (this.activeBrewingSlots.Count == 0)
        this.activeBrewingSlots = ((IEnumerable<CraftBrewingSlotAnchor>) this.brewingSlotAnchors).Where<CraftBrewingSlotAnchor>((Func<CraftBrewingSlotAnchor, bool>) (anchor => anchor.Slot.IsSlotAvailable)).ToList<CraftBrewingSlotAnchor>();
      this.currentBrewingSlotIndex = this.activeBrewingSlots.IndexOf(slot.GetComponentInParent<CraftBrewingSlotAnchor>());
      if (!this.isCraftAvailable)
        this.activeBrewingSlots.Remove(this.activeBrewingSlots[this.currentBrewingSlotIndex]);
      else
        this.activeBrewingSlots[this.currentBrewingSlotIndex].Slot.SetSelected(false);
      if (this.activeBrewingSlots.Count == 0)
      {
        this.activeBrewingSlots.Clear();
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, new GameActionHandle(this.NavigateOnSlots));
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, new GameActionHandle(this.NavigateOnSlots));
        this.isSlotNavigationEnabled = false;
      }
      else if (!this.isCraftAvailable)
        this.NavigateOnSlots(GameActionType.RStickDown, true);
      else
        CoroutineService.Instance.WaitFrame(1, (Action) (() =>
        {
          if (this.currentBrewingSlotIndex >= this.activeBrewingSlots.Count || this.activeBrewingSlots[this.currentBrewingSlotIndex].Slot.IsSelected)
            return;
          this.activeBrewingSlots[this.currentBrewingSlotIndex].Slot.SetSelected(true);
        }));
      this.OnInvalidate();
    }

    protected override void OnItemClick(IStorableComponent storable)
    {
      if (storable.Storage == this.Actor)
      {
        foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
        {
          if (storable == ingredientSelector.Item)
          {
            ingredientSelector.Item = (IStorableComponent) null;
            return;
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
        if (craftTimeParameter != null && craftTimeParameter.Value > TimeSpan.Zero)
          return;
        this.MoveItem(storable, this.Actor);
        StorableComponentUtility.PlayTakeSound(storable);
        this.OnInvalidate();
      }
    }

    protected override void Clear()
    {
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in this.brewingSlotAnchors)
        brewingSlotAnchor.SetTarget((IEntity) null);
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

    private void CreateContainers()
    {
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in this.brewingSlotAnchors)
        brewingSlotAnchor.SetTarget(this.Target.Owner);
      foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
        ingredientSelector.Storage = this.Actor;
    }

    private void Craft(IInventoryComponent targetContainer)
    {
      this.DisableCraftButtons();
      List<IStorableComponent> ingredients = new List<IStorableComponent>();
      for (int index = 0; index < this.ingredientSelectors.Length; ++index)
      {
        IStorableComponent storableComponent = this.ingredientSelectors[index].Item;
        if (storableComponent != null)
          ingredients.Add(storableComponent);
      }
      CraftRecipe recipe;
      IStorableComponent craftResult = CraftHelper.GetCraftResult(ingredients, out recipe);
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
        this.SetItemTimer(component, recipe.Time);
        this.Target.AddItem(component, targetContainer);
        if ((UnityEngine.Object) this.craftAudio != (UnityEngine.Object) null)
          this.PlayAudio(this.craftAudio);
      }
      this.OnInvalidate();
    }

    private void PredictCraftRecipe()
    {
      this.DisableCraftButtons();
      if (this.currentBrewingSlotIndex != -1 && this.currentBrewingSlotIndex < this.activeBrewingSlots.Count)
        this.activeBrewingSlots[this.currentBrewingSlotIndex].Slot.SetSelected(true);
      List<IStorableComponent> ingredients = new List<IStorableComponent>();
      for (int index = 0; index < this.ingredientSelectors.Length; ++index)
      {
        IStorableComponent storableComponent = this.ingredientSelectors[index].Item;
        if (storableComponent != null)
          ingredients.Add(storableComponent);
      }
      CraftRecipe recipe;
      IStorableComponent craftResult = CraftHelper.GetCraftResult(ingredients, out recipe);
      if (craftResult != null)
      {
        this.resultPreview.Storable = (StorableComponent) craftResult;
        this.EnableCraftButtons(recipe.Time);
        this.isCraftAvailable = true;
        if (!this.isSlotNavigationEnabled)
        {
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickUp, new GameActionHandle(this.NavigateOnSlots));
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickDown, new GameActionHandle(this.NavigateOnSlots));
          this.isSlotNavigationEnabled = true;
        }
        if (this.currentBrewingSlotIndex == -1 || this.currentBrewingSlotIndex > this.activeBrewingSlots.Count - 1)
          this.currentBrewingSlotIndex = this.activeBrewingSlots.Count - 1;
        if (!InputService.Instance.JoystickUsed)
          return;
        this.activeBrewingSlots[this.currentBrewingSlotIndex].Slot.SetSelected(true);
        this.slotNavigation.SetActive(true);
      }
      else
      {
        this.isCraftAvailable = false;
        for (int index = 0; index < this.activeBrewingSlots.Count; ++index)
        {
          if (!this.activeBrewingSlots[index].Slot.IsItemCrafted)
          {
            this.activeBrewingSlots[index].Slot.SetSelected(false);
            this.activeBrewingSlots.Remove(this.activeBrewingSlots[index]);
          }
        }
        if (this.currentBrewingSlotIndex < 0)
          this.currentBrewingSlotIndex = 0;
        if (this.currentBrewingSlotIndex > this.activeBrewingSlots.Count - 1)
          this.currentBrewingSlotIndex = this.activeBrewingSlots.Count - 1;
        this.CurrentActiveCrafts = ((IEnumerable<CraftBrewingSlotAnchor>) this.brewingSlotAnchors).Count<CraftBrewingSlotAnchor>((Func<CraftBrewingSlotAnchor, bool>) (anchor => anchor.Slot.IsSlotAvailable));
        if (this.CurrentActiveCrafts == 0)
        {
          this.currentBrewingSlotIndex = -1;
          ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, new GameActionHandle(this.NavigateOnSlots));
          ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, new GameActionHandle(this.NavigateOnSlots));
          this.slotNavigation.SetActive(false);
          if (this.isSlotNavigationEnabled)
            this.isSlotNavigationEnabled = false;
        }
      }
    }

    private void EnableCraftButtons(float time)
    {
      string text1 = ServiceLocator.GetService<LocalizationService>().GetText("{UI.Craft.PrepareTime}");
      TimeSpan timeSpan = TimeSpan.FromMinutes((double) time);
      string text2 = text1.Replace("<hours>", timeSpan.Hours.ToString("D2")).Replace("<minutes>", timeSpan.Minutes.ToString("D2"));
      this.activeBrewingSlots.Clear();
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in this.brewingSlotAnchors)
      {
        brewingSlotAnchor.Slot.SetCraftTime(text2);
        brewingSlotAnchor.Slot.SetEnabled(true);
        if (brewingSlotAnchor.Slot.IsVisible)
          this.activeBrewingSlots.Add(brewingSlotAnchor);
      }
    }

    private void DisableCraftButtons()
    {
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in this.brewingSlotAnchors)
        brewingSlotAnchor.Slot.SetEnabled(false);
    }

    private void SetItemTimer(IStorableComponent item, float timeInMinutes)
    {
      TimeSpan timeSpan = TimeSpan.FromMinutes((double) timeInMinutes);
      IParameter<TimeSpan> craftTimeParameter = CraftHelper.GetCraftTimeParameter(item);
      if (craftTimeParameter == null)
        return;
      TimeSpan gameTime = ServiceLocator.GetService<TimeService>().GameTime;
      craftTimeParameter.MinValue = gameTime;
      craftTimeParameter.MaxValue = gameTime + timeSpan;
    }

    public override void Initialize()
    {
      this.RegisterLayer<ICraftBreweryWindow>((ICraftBreweryWindow) this);
      foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
        ingredientSelector.ChangeItemEvent += new Action<ItemSelector, IStorableComponent, IStorableComponent>(this.OnIngredientChange);
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in this.brewingSlotAnchors)
      {
        brewingSlotAnchor.Initialize(this.brewingSlotPrefab);
        brewingSlotAnchor.CraftEvent += new Action<IInventoryComponent>(this.Craft);
        brewingSlotAnchor.TakeEvent += new Action<IStorableComponent>(((SelectableInventoryWindow) this).OnItemClick);
      }
      base.Initialize();
    }

    public override System.Type GetWindowType() => typeof (ICraftBreweryWindow);

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
      foreach (ItemSelector ingredientSelector in this.ingredientSelectors)
      {
        if (item == ingredientSelector.Item)
          return true;
        foreach (StorableGroup group in ingredientSelector.Groups)
        {
          if (item.Groups.Contains<StorableGroup>(group))
            return true;
        }
      }
      return false;
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
