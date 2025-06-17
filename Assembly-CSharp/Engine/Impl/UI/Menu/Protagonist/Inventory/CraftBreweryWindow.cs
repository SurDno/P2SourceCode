using System;
using System.Collections.Generic;
using System.Linq;
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
    private bool isCraftAvailable;
    private List<CraftBrewingSlotAnchor> activeBrewingSlots = [];
    private int currentBrewingSlotIndex = -1;
    private int CurrentActiveCrafts;
    private CraftBrewingSlotAnchor lastBrewingSlot;
    private bool isSlotNavigationEnabled;

    [Inspected]
    public IStorageComponent Target { get; set; }

    protected override void OnEnable()
    {
      base.OnEnable();
      slotNavigation.SetActive(false);
      actors.Clear();
      actors.Add(Actor);
      Build2();
      CreateContainers();
      if (selectedStorable != null)
      {
        HideInfoWindow();
        selectedStorable.SetSelected(false);
        selectedStorable = null;
      }
      foreach (ItemSelector ingredientSelector in ingredientSelectors)
      {
        StorableUI storableByComponent = GetStorableByComponent(ingredientSelector.Item);
        if (storableByComponent != null)
        {
          selectedStorable = storableByComponent;
          OnSelectObject(selectedStorable.gameObject);
          break;
        }
      }
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in brewingSlotAnchors)
      {
        brewingSlotAnchor.Slot.OnCraftPerformed += SlotCraftStarted;
        brewingSlotAnchor.Slot.OnItemTaken += SlotItemTaken;
      }
      CurrentActiveCrafts = brewingSlotAnchors.Count(anchor => anchor.Slot.IsSlotAvailable);
      if (CurrentActiveCrafts == 0)
        return;
      if (activeBrewingSlots.Count == 0)
        activeBrewingSlots = brewingSlotAnchors.Where(anchor => anchor.Slot.IsSlotAvailable).ToList();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickUp, NavigateOnSlots);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickDown, NavigateOnSlots);
      isSlotNavigationEnabled = true;
      if (lastBrewingSlot == null)
        lastBrewingSlot = brewingSlotAnchors.Where(anchor => anchor.Slot.IsSlotAvailable).Last();
      if (lastBrewingSlot != null)
      {
        lastBrewingSlot.Slot.SetEnabled(true);
        lastBrewingSlot.Slot.SetSelected(true);
        currentBrewingSlotIndex = activeBrewingSlots.IndexOf(lastBrewingSlot);
      }
    }

    protected override void OnDisable()
    {
      DisableCraftButtons();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.CraftBrewery, WithoutJoystickCancelListener);
      currentBrewingSlotIndex = -1;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, NavigateOnSlots);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, NavigateOnSlots);
      lastBrewingSlot = null;
      activeBrewingSlots.Clear();
      isCraftAvailable = false;
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in brewingSlotAnchors)
      {
        brewingSlotAnchor.Slot.OnCraftPerformed -= SlotCraftStarted;
        brewingSlotAnchor.Slot.OnItemTaken -= SlotItemTaken;
      }
      base.OnDisable();
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      slotNavigation.SetActive(joystick);
      if (!joystick)
      {
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.CraftBrewery, WithoutJoystickCancelListener);
        foreach (ItemSelector ingredientSelector in ingredientSelectors)
          ingredientSelector.SetSelection(false);
        PreviousMode = CurrentMode;
        CurrentMode = Modes.None;
        foreach (CraftBrewingSlotAnchor brewingSlotAnchor in brewingSlotAnchors)
          brewingSlotAnchor.Slot.SetSelected(false);
      }
      else
      {
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.CraftBrewery, WithoutJoystickCancelListener);
        if (PreviousMode != 0)
          CurrentMode = PreviousMode;
        else
          CurrentMode = Modes.Craft;
        if (currentBrewingSlotIndex == -1)
          currentBrewingSlotIndex = activeBrewingSlots.Count - 1;
        if (lastBrewingSlot != null)
        {
          if (!lastBrewingSlot.Slot.IsEnabled)
            lastBrewingSlot.Slot.SetEnabled(true);
          if (!lastBrewingSlot.Slot.IsSelected)
            lastBrewingSlot.Slot.SetSelected(true);
          currentBrewingSlotIndex = activeBrewingSlots.IndexOf(lastBrewingSlot);
        }
      }
    }

    private bool NavigateOnSlots(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || !down)
        return false;
      if (currentBrewingSlotIndex != -1 && currentBrewingSlotIndex < activeBrewingSlots.Count && activeBrewingSlots[currentBrewingSlotIndex] != null)
        activeBrewingSlots[currentBrewingSlotIndex]?.Slot.SetSelected(false);
      currentBrewingSlotIndex += type == GameActionType.RStickUp ? 1 : -1;
      if (currentBrewingSlotIndex < 0)
        currentBrewingSlotIndex = currentBrewingSlotIndex = activeBrewingSlots.Count - 1;
      if (currentBrewingSlotIndex > activeBrewingSlots.Count - 1)
        currentBrewingSlotIndex = 0;
      if (activeBrewingSlots[currentBrewingSlotIndex] != null)
      {
        activeBrewingSlots[currentBrewingSlotIndex].Slot.SetSelected(true);
        if (activeBrewingSlots[currentBrewingSlotIndex].Slot.IsSlotAvailable)
          lastBrewingSlot = activeBrewingSlots[currentBrewingSlotIndex];
      }
      return true;
    }

    protected override Modes CurrentMode
    {
      get => base.CurrentMode;
      set
      {
        if (CurrentMode == value || !InputService.Instance.JoystickUsed)
          return;
        switch (value)
        {
          case Modes.Inventory:
            return;
          case Modes.Craft:
            if (InputService.Instance.JoystickUsed && isCraftAvailable)
            {
              ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickUp, NavigateOnSlots);
              ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickDown, NavigateOnSlots);
              if (currentBrewingSlotIndex == -1)
                currentBrewingSlotIndex = activeBrewingSlots.Count - 1;
              activeBrewingSlots[currentBrewingSlotIndex].Slot.SetSelected(true);
            }
            break;
        }
        base.CurrentMode = value;
      }
    }

    private void SlotCraftStarted(CraftBrewingSlot slot)
    {
      foreach (CraftBrewingSlotAnchor activeBrewingSlot in activeBrewingSlots)
      {
        if (activeBrewingSlot.Slot == slot)
        {
          lastBrewingSlot = activeBrewingSlot;
          break;
        }
      }
      OnInvalidate();
    }

    private void SlotItemTaken(CraftBrewingSlot slot)
    {
      if (lastBrewingSlot == slot)
        lastBrewingSlot = null;
      if (activeBrewingSlots.Count == 0)
        activeBrewingSlots = brewingSlotAnchors.Where(anchor => anchor.Slot.IsSlotAvailable).ToList();
      currentBrewingSlotIndex = activeBrewingSlots.IndexOf(slot.GetComponentInParent<CraftBrewingSlotAnchor>());
      if (!isCraftAvailable)
        activeBrewingSlots.Remove(activeBrewingSlots[currentBrewingSlotIndex]);
      else
        activeBrewingSlots[currentBrewingSlotIndex].Slot.SetSelected(false);
      if (activeBrewingSlots.Count == 0)
      {
        activeBrewingSlots.Clear();
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, NavigateOnSlots);
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, NavigateOnSlots);
        isSlotNavigationEnabled = false;
      }
      else if (!isCraftAvailable)
        NavigateOnSlots(GameActionType.RStickDown, true);
      else
        CoroutineService.Instance.WaitFrame(1, (Action) (() =>
        {
          if (currentBrewingSlotIndex >= activeBrewingSlots.Count || activeBrewingSlots[currentBrewingSlotIndex].Slot.IsSelected)
            return;
          activeBrewingSlots[currentBrewingSlotIndex].Slot.SetSelected(true);
        }));
      OnInvalidate();
    }

    protected override void OnItemClick(IStorableComponent storable)
    {
      if (storable.Storage == Actor)
      {
        foreach (ItemSelector ingredientSelector in ingredientSelectors)
        {
          if (storable == ingredientSelector.Item)
          {
            ingredientSelector.Item = null;
            return;
          }
        }
        foreach (ItemSelector ingredientSelector in ingredientSelectors)
        {
          foreach (StorableGroup group in ingredientSelector.Groups)
          {
            if (storable.Groups.Contains(group))
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
        MoveItem(storable, Actor);
        StorableComponentUtility.PlayTakeSound(storable);
        OnInvalidate();
      }
    }

    protected override void Clear()
    {
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in brewingSlotAnchors)
        brewingSlotAnchor.SetTarget(null);
      foreach (ItemSelector ingredientSelector in ingredientSelectors)
        ingredientSelector.Storage = null;
      base.Clear();
    }

    protected override void OnInvalidate()
    {
      base.OnInvalidate();
      PredictCraftRecipe();
      if (!InputService.Instance.JoystickUsed)
        return;
      HideInfoWindow();
    }

    private void CreateContainers()
    {
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in brewingSlotAnchors)
        brewingSlotAnchor.SetTarget(Target.Owner);
      foreach (ItemSelector ingredientSelector in ingredientSelectors)
        ingredientSelector.Storage = Actor;
    }

    private void Craft(IInventoryComponent targetContainer)
    {
      DisableCraftButtons();
      List<IStorableComponent> ingredients = [];
      for (int index = 0; index < ingredientSelectors.Length; ++index)
      {
        IStorableComponent storableComponent = ingredientSelectors[index].Item;
        if (storableComponent != null)
          ingredients.Add(storableComponent);
      }

      IStorableComponent craftResult = CraftHelper.GetCraftResult(ingredients, out CraftRecipe recipe);
      if (craftResult != null)
      {
        foreach (IStorableComponent storableComponent in ingredients)
        {
          if (storableComponent.Count > 1)
            --storableComponent.Count;
          else
            storableComponent.Owner.Dispose();
        }
        IEntity entity = ServiceLocator.GetService<IFactory>().Instantiate(craftResult.Owner);
        ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
        IStorableComponent component = entity.GetComponent<IStorableComponent>();
        SetItemTimer(component, recipe.Time);
        Target.AddItem(component, targetContainer);
        if (craftAudio != null)
          PlayAudio(craftAudio);
      }
      OnInvalidate();
    }

    private void PredictCraftRecipe()
    {
      DisableCraftButtons();
      if (currentBrewingSlotIndex != -1 && currentBrewingSlotIndex < activeBrewingSlots.Count)
        activeBrewingSlots[currentBrewingSlotIndex].Slot.SetSelected(true);
      List<IStorableComponent> ingredients = [];
      for (int index = 0; index < ingredientSelectors.Length; ++index)
      {
        IStorableComponent storableComponent = ingredientSelectors[index].Item;
        if (storableComponent != null)
          ingredients.Add(storableComponent);
      }

      IStorableComponent craftResult = CraftHelper.GetCraftResult(ingredients, out CraftRecipe recipe);
      if (craftResult != null)
      {
        resultPreview.Storable = (StorableComponent) craftResult;
        EnableCraftButtons(recipe.Time);
        isCraftAvailable = true;
        if (!isSlotNavigationEnabled)
        {
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickUp, NavigateOnSlots);
          ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.RStickDown, NavigateOnSlots);
          isSlotNavigationEnabled = true;
        }
        if (currentBrewingSlotIndex == -1 || currentBrewingSlotIndex > activeBrewingSlots.Count - 1)
          currentBrewingSlotIndex = activeBrewingSlots.Count - 1;
        if (!InputService.Instance.JoystickUsed)
          return;
        activeBrewingSlots[currentBrewingSlotIndex].Slot.SetSelected(true);
        slotNavigation.SetActive(true);
      }
      else
      {
        isCraftAvailable = false;
        for (int index = 0; index < activeBrewingSlots.Count; ++index)
        {
          if (!activeBrewingSlots[index].Slot.IsItemCrafted)
          {
            activeBrewingSlots[index].Slot.SetSelected(false);
            activeBrewingSlots.Remove(activeBrewingSlots[index]);
          }
        }
        if (currentBrewingSlotIndex < 0)
          currentBrewingSlotIndex = 0;
        if (currentBrewingSlotIndex > activeBrewingSlots.Count - 1)
          currentBrewingSlotIndex = activeBrewingSlots.Count - 1;
        CurrentActiveCrafts = brewingSlotAnchors.Count(anchor => anchor.Slot.IsSlotAvailable);
        if (CurrentActiveCrafts == 0)
        {
          currentBrewingSlotIndex = -1;
          ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickUp, NavigateOnSlots);
          ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.RStickDown, NavigateOnSlots);
          slotNavigation.SetActive(false);
          if (isSlotNavigationEnabled)
            isSlotNavigationEnabled = false;
        }
      }
    }

    private void EnableCraftButtons(float time)
    {
      string text1 = ServiceLocator.GetService<LocalizationService>().GetText("{UI.Craft.PrepareTime}");
      TimeSpan timeSpan = TimeSpan.FromMinutes(time);
      string text2 = text1.Replace("<hours>", timeSpan.Hours.ToString("D2")).Replace("<minutes>", timeSpan.Minutes.ToString("D2"));
      activeBrewingSlots.Clear();
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in brewingSlotAnchors)
      {
        brewingSlotAnchor.Slot.SetCraftTime(text2);
        brewingSlotAnchor.Slot.SetEnabled(true);
        if (brewingSlotAnchor.Slot.IsVisible)
          activeBrewingSlots.Add(brewingSlotAnchor);
      }
    }

    private void DisableCraftButtons()
    {
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in brewingSlotAnchors)
        brewingSlotAnchor.Slot.SetEnabled(false);
    }

    private void SetItemTimer(IStorableComponent item, float timeInMinutes)
    {
      TimeSpan timeSpan = TimeSpan.FromMinutes(timeInMinutes);
      IParameter<TimeSpan> craftTimeParameter = CraftHelper.GetCraftTimeParameter(item);
      if (craftTimeParameter == null)
        return;
      TimeSpan gameTime = ServiceLocator.GetService<TimeService>().GameTime;
      craftTimeParameter.MinValue = gameTime;
      craftTimeParameter.MaxValue = gameTime + timeSpan;
    }

    public override void Initialize()
    {
      RegisterLayer<ICraftBreweryWindow>(this);
      foreach (ItemSelector ingredientSelector in ingredientSelectors)
        ingredientSelector.ChangeItemEvent += OnIngredientChange;
      foreach (CraftBrewingSlotAnchor brewingSlotAnchor in brewingSlotAnchors)
      {
        brewingSlotAnchor.Initialize(brewingSlotPrefab);
        brewingSlotAnchor.CraftEvent += Craft;
        brewingSlotAnchor.TakeEvent += OnItemClick;
      }
      base.Initialize();
    }

    public override Type GetWindowType() => typeof (ICraftBreweryWindow);

    private void OnIngredientChange(
      ItemSelector itemSelector,
      IStorableComponent prevIngredient,
      IStorableComponent newIngredient)
    {
      if (prevIngredient != null)
        StorableComponentUtility.PlayPutSound(prevIngredient);
      if (newIngredient != null)
        StorableComponentUtility.PlayTakeSound(newIngredient);
      OnInvalidate();
    }

    protected override bool ItemIsInteresting(IStorableComponent item)
    {
      foreach (ItemSelector ingredientSelector in ingredientSelectors)
      {
        if (item == ingredientSelector.Item)
          return true;
        foreach (StorableGroup group in ingredientSelector.Groups)
        {
          if (item.Groups.Contains(group))
            return true;
        }
      }
      return false;
    }

    protected override bool ItemIsSelected(IStorableComponent storable)
    {
      foreach (ItemSelector ingredientSelector in ingredientSelectors)
      {
        if (ingredientSelector.Item == storable)
          return true;
      }
      return false;
    }
  }
}
