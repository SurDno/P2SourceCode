using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Impl.UI.Menu.Protagonist.Inventory.Windows;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using Engine.Source.UI.Controls;
using InputServices;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public class HealingWindow : 
    BaseInventoryWindow<HealingWindow>,
    IHealingWindow,
    IWindow,
    IChangeParameterListener
  {
    [SerializeField]
    private ItemsSlidingContainer slidingContainer;
    [SerializeField]
    private HideableView hasValidItems;
    [SerializeField]
    private EntityView[] parameterViews;
    [SerializeField]
    private UIControl item;
    [SerializeField]
    private HoldableButton2 buttonUse;
    [SerializeField]
    private GameObject[] symptomsImages;
    [SerializeField]
    private IEntitySerializable symptom1;
    [SerializeField]
    private IEntitySerializable symptom2;
    [SerializeField]
    private IEntitySerializable symptom3;
    [SerializeField]
    private IEntitySerializable symptom4;
    [SerializeField]
    private IEntitySerializable symptom5;
    [SerializeField]
    private IEntitySerializable symptom6;
    [SerializeField]
    private IEntitySerializable symptom7;
    [SerializeField]
    private IEntitySerializable shmowder;
    [SerializeField]
    private IEntitySerializable backerShmowder;
    [SerializeField]
    private IEntitySerializable panacea;
    [SerializeField]
    private Sprite symptomHiddenSprite;
    [SerializeField]
    private HideableView definitiveDiagnosisCheck;
    [SerializeField]
    private ItemView selectedItemUI;
    [SerializeField]
    private EventView usedImmuneBoosterMessage;
    [SerializeField]
    private EventView usedPainkillerMessage;
    [SerializeField]
    private EventView usedCorrectAntibioticMessage;
    [SerializeField]
    private EventView usedWrongAntibioticMessage;
    [SerializeField]
    private EventView usedMaracleMessage;
    [SerializeField]
    private EventView clearMessages;
    private IEntitySerializable[] symptomTemplates;
    private List<IEntity> targetSymptoms = new List<IEntity>();
    private float oldPain;
    private float oldInfection;
    private StorableComponent selectedItem;
    private int currentSliderIndex = -1;
    [SerializeField]
    private List<GameObject> buttonHealingHints;
    [SerializeField]
    private GameObject _administerHealingHint;
    [SerializeField]
    private List<GameObject> consoleStrainsTextInfo;
    [SerializeField]
    private List<HideableView> strainsEntities;
    private Coroutine scrollCoroutine = (Coroutine) null;
    private bool IsHealingEnded;
    private bool IsHealingPerformed;
    private StorableUI ItemToUse;

    public IStorageComponent Target { get; set; }

    private void SelectItem(StorableComponent storable)
    {
      if (selectedItem != null)
      {
        StorableUI storable1 = storables[selectedItem];
        if ((UnityEngine.Object) storable1 != (UnityEngine.Object) null)
          storable1.SetSelected(false);
      }
      selectedItem = storable;
      StorableUI storable2 = storables[storable];
      if ((UnityEngine.Object) storable2 != (UnityEngine.Object) null)
        storable2.SetSelected(true);
      StorableComponentUtility.PlayTakeSound(storable);
      SetSelectedItemInfo();
      HandleHealingHintsVisibility(InputService.Instance.JoystickUsed);
    }

    private void SetSelectedItemInfo()
    {
      selectedItemUI.Storable = selectedItem;
      if (selectedItem == null)
      {
        buttonUse.interactable = false;
        if (!InputService.Instance.JoystickUsed)
          return;
        _administerHealingHint.SetActive(false);
      }
      else
      {
        buttonUse.interactable = true;
        if (InputService.Instance.JoystickUsed)
          _administerHealingHint.SetActive(true);
      }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
      if ((UnityEngine.Object) windowContextMenu != (UnityEngine.Object) null)
      {
        HideContextMenu();
      }
      else
      {
        if (!intersect.IsIntersected)
          return;
        StorableComponent storableComponent = intersect.Storables.FirstOrDefault();
        if (storableComponent == null || eventData.button != PointerEventData.InputButton.Left)
          return;
        SelectItem(storableComponent);
        ItemToUse = GetStorableByComponent(storableComponent);
        if (!((UnityEngine.Object) ItemToUse != (UnityEngine.Object) null))
          return;
        currentSliderIndex = SliderItems.IndexOf(ItemToUse);
      }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
    }

    private void SubscribeToParameters(bool subscribe)
    {
      IEntity owner = Target?.Owner;
      if (owner == null)
        return;
      ParametersComponent component = owner.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<float> byName1 = component.GetByName<float>(ParameterNameEnum.Pain);
        if (byName1 != null)
        {
          byName1.RemoveListener(this);
          if (subscribe)
          {
            byName1.AddListener(this);
            oldPain = byName1.Value;
          }
        }
        IParameter<float> byName2 = component.GetByName<float>(ParameterNameEnum.Infection);
        byName2.RemoveListener(this);
        if (subscribe)
        {
          byName2.AddListener(this);
          oldInfection = byName2.Value;
        }
      }
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      Unsubscribe();
      UnsubscribeNavigation();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, MedicineNavigation);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, MedicineNavigation);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, AdministerHealing);
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      buttonUse.OpenEndEvent += OnUseButtonEnd;
      if (symptomTemplates == null)
        symptomTemplates = new IEntitySerializable[7]
        {
          symptom1,
          symptom2,
          symptom3,
          symptom4,
          symptom5,
          symptom6,
          symptom7
        };
      selectedItem = null;
      CountSymptoms();
      actors.Clear();
      Build2();
      IEntity owner = Target?.Owner;
      foreach (EntityView parameterView in parameterViews)
      {
        parameterView.Value = owner;
        parameterView.SkipAnimation();
      }
      ShowSymptoms();
      CreateSlideContainers();
      SetSelectedItemInfo();
      selectedItemUI.SkipAnimation();
      SubscribeToParameters(true);
      foreach (HideableView strainsEntity in strainsEntities)
        strainsEntity.OnVisibilityChanged += OnHideableVisibleChanged;
      currentSliderIndex = 0;
      SetInfoWindowShowMode(true, false);
    }

    private void OnUseButtonEnd(bool success)
    {
      if (!success)
        return;
      UseItemOnTarget();
    }

    protected override void OnDisable()
    {
      SubscribeToParameters(false);
      buttonUse.OpenEndEvent -= OnUseButtonEnd;
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Heal, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, MedicineNavigation);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, MedicineNavigation);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, AdministerHealing);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      if ((UnityEngine.Object) selectedStorable != (UnityEngine.Object) null)
      {
        selectedStorable.HoldSelected(false);
        selectedStorable.SetSelected(false);
        selectedStorable = null;
      }
      ItemToUse = null;
      IsHealingEnded = false;
      IsHealingPerformed = false;
      foreach (HideableView strainsEntity in strainsEntities)
        strainsEntity.OnVisibilityChanged -= OnHideableVisibleChanged;
      foreach (GameObject gameObject in consoleStrainsTextInfo)
        gameObject.SetActive(false);
      foreach (EntityView parameterView in parameterViews)
      {
        parameterView.Value = null;
        parameterView.SkipAnimation();
      }
      foreach (IEntity targetSymptom in targetSymptoms)
        targetSymptom?.GetComponent<ParametersComponent>()?.GetByName<bool>(ParameterNameEnum.IsOpen)?.RemoveListener(this);
      clearMessages?.Invoke();
      ClearSlideContainers();
      base.OnDisable();
    }

    private void OnHideableVisibleChanged(bool value, HideableView view)
    {
      int index = strainsEntities.IndexOf(view);
      if (index == -1)
        return;
      consoleStrainsTextInfo[index].SetActive(value);
    }

    private List<StorableUI> SliderItems { get; set; }

    private void HandleHealingHintsVisibility(bool visible)
    {
      Text componentInChildren = buttonHealingHints.Where((Func<GameObject, bool>) (hint => hint.GetComponent<HideableView>().Visible)).First()?.GetComponentInChildren<Text>();
      if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null && buttonUse.interactable)
      {
        _administerHealingHint.transform.parent = componentInChildren.transform;
        _administerHealingHint.transform.localPosition = Vector3.zero;
        _administerHealingHint.transform.localPosition = new Vector3((float) ((double) componentInChildren.transform.localPosition.x - (double) componentInChildren.preferredWidth / 2.0 - 15.0), _administerHealingHint.transform.localPosition.y);
        _administerHealingHint.SetActive(visible);
      }
      else
        _administerHealingHint.SetActive(false);
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      controlPanel.SetActive(joystick && !IsHealingEnded && SliderItems.Count > 0);
      HandleHealingHintsVisibility(joystick);
      if (joystick)
      {
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Heal, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        if (IsHealingEnded)
          return;
        if (SliderItems.Count == 0)
          SliderItems = null;
        else if ((UnityEngine.Object) ItemToUse == (UnityEngine.Object) null && SliderItems.Count > 0)
          ItemToUse = SliderItems.First();
        if (currentSliderIndex < 0)
          currentSliderIndex = 0;
        if ((UnityEngine.Object) ItemToUse != (UnityEngine.Object) null)
          ItemToUse.HoldSelected(true);
        if ((UnityEngine.Object) selectedStorable != (UnityEngine.Object) ItemToUse && (UnityEngine.Object) ItemToUse != (UnityEngine.Object) null)
        {
          selectedStorable = ItemToUse;
          currentSliderIndex = SliderItems.IndexOf(selectedStorable);
        }
        if ((UnityEngine.Object) selectedStorable == (UnityEngine.Object) null && SliderItems.Count > 0)
          selectedStorable = SliderItems.First();
        slidingContainer.ScrollTo(currentSliderIndex, SliderItems.Count);
        if ((UnityEngine.Object) selectedStorable != (UnityEngine.Object) null)
        {
          selectedStorable.SetSelected(true);
          SelectItemFromSlider();
        }
        for (int index = 0; index < strainsEntities.Count; ++index)
        {
          if (strainsEntities[index].Visible)
            consoleStrainsTextInfo[index].SetActive(joystick);
        }
      }
      else
      {
        buttonUse.GamepadEndHold();
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Heal, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        if ((UnityEngine.Object) selectedStorable != (UnityEngine.Object) null)
        {
          selectedStorable?.SetSelected(false);
          selectedStorable?.HoldSelected(false);
        }
        if ((UnityEngine.Object) ItemToUse != (UnityEngine.Object) null)
          ItemToUse.HoldSelected(false);
        foreach (GameObject gameObject in consoleStrainsTextInfo)
          gameObject.SetActive(false);
      }
    }

    private IEnumerator ScrollCoroutine(
      ScrollHandle handle,
      GameActionType type,
      bool down)
    {
      while (true)
      {
        if (handle != null)
        {
          int num = handle(type, down) ? 1 : 0;
        }
        yield return (object) new WaitForSeconds(0.5f);
      }
    }

    private bool MedicineNavigation(GameActionType type, bool down)
    {
      if (down)
      {
        if (scrollCoroutine != null)
          this.StopCoroutine(scrollCoroutine);
        scrollCoroutine = this.StartCoroutine(ScrollCoroutine(OnMedicineNavigation, type, down));
        return true;
      }
      if (scrollCoroutine != null)
      {
        this.StopCoroutine(scrollCoroutine);
        scrollCoroutine = (Coroutine) null;
      }
      return false;
    }

    private bool OnMedicineNavigation(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || ((type == GameActionType.LStickUp ? 1 : (type == GameActionType.LStickDown ? 1 : 0)) & (down ? 1 : 0)) == 0 || IsHealingEnded)
        return false;
      if (SliderItems.Count == 0)
        SliderItems = null;
      if (SliderItems.Count < 1)
        return false;
      currentSliderIndex += type == GameActionType.LStickUp ? -1 : 1;
      if (currentSliderIndex < 0)
        currentSliderIndex = SliderItems.Count - 1;
      if (currentSliderIndex >= SliderItems.Count)
        currentSliderIndex = 0;
      if ((UnityEngine.Object) selectedStorable != (UnityEngine.Object) null && (UnityEngine.Object) selectedStorable != (UnityEngine.Object) ItemToUse)
        selectedStorable?.SetSelected(false);
      selectedStorable = SliderItems[currentSliderIndex];
      if ((UnityEngine.Object) selectedStorable != (UnityEngine.Object) null)
        selectedStorable?.SetSelected(true);
      slidingContainer.ScrollTo(currentSliderIndex, SliderItems.Count);
      return SelectItemFromSlider();
    }

    private bool SelectItemFromSlider()
    {
      if ((UnityEngine.Object) selectedStorable != (UnityEngine.Object) null)
      {
        HideInfoWindow();
        SelectItem((StorableComponent) selectedStorable.Internal);
        buttonUse.GamepadEndHold();
        if ((UnityEngine.Object) ItemToUse != (UnityEngine.Object) null && (UnityEngine.Object) ItemToUse != (UnityEngine.Object) selectedStorable)
          ItemToUse.HoldSelected(false);
        ItemToUse = selectedStorable;
        ItemToUse.HoldSelected(true);
        StorableUI cachedItemToUse = ItemToUse;
        CoroutineService.Instance.WaitFrame(1, (Action) (() => ShowInfoWindow(cachedItemToUse.Internal)));
      }
      return true;
    }

    protected override void PositionWindow(UIControl window, IStorableComponent storable)
    {
      base.PositionWindow(window, storable);
      if (!InputService.Instance.JoystickUsed)
        return;
      StorableUI itemToUse = ItemToUse;
      if ((UnityEngine.Object) itemToUse == (UnityEngine.Object) null)
        return;
      RectTransform component1 = window.GetComponent<RectTransform>();
      RectTransform component2 = slidingContainer.GetComponent<RectTransform>();
      float num1 = 20f;
      float hintsBottomBorder = HintsBottomBorder;
      float num2 = itemToUse.Image.rectTransform.rect.height / 2f * itemToUse.Image.rectTransform.lossyScale.y;
      double num3 = (double) itemToUse.Image.transform.position.y + num2;
      Rect rect = component1.rect;
      double num4 = (double) rect.height * (double) component1.lossyScale.y;
      if (num3 - num4 < hintsBottomBorder)
      {
        rect = component1.rect;
        double height = (double) rect.height;
        rect = itemToUse.Image.rectTransform.rect;
        double num5 = (double) rect.height / 2.0;
        num2 = (float) (height - num5) * itemToUse.Image.rectTransform.lossyScale.y;
      }
      rect = component1.rect;
      double num6 = -(double) rect.width * (double) component1.lossyScale.x;
      rect = component2.rect;
      double num7 = (double) rect.width * (double) component2.lossyScale.x / 2.0;
      float num8 = (float) (num6 - num7 - num1 * (double) component2.lossyScale.x / 2.0);
      window.Transform.position = new Vector3(itemToUse.Image.transform.position.x + num8, itemToUse.Image.transform.position.y + num2);
    }

    private bool AdministerHealing(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (type == GameActionType.Submit & down)
      {
        buttonUse.GamepadStartHold();
        return true;
      }
      if (type != GameActionType.Submit || down)
        return false;
      buttonUse.GamepadEndHold();
      return true;
    }

    private void CountSymptoms()
    {
      targetSymptoms.Clear();
      foreach (IStorableComponent storableComponent in Target.Items)
      {
        if (storableComponent.Groups.Contains(StorableGroup.Symptom))
        {
          targetSymptoms.Add(storableComponent.Owner);
          IParameter<bool> byName = storableComponent.Owner?.GetComponent<ParametersComponent>()?.GetByName<bool>(ParameterNameEnum.IsOpen);
          if (byName != null)
          {
            byName.RemoveListener(this);
            byName.AddListener(this);
          }
        }
      }
    }

    private void ShowSymptoms()
    {
      for (int i = 0; i < symptomTemplates.Length; i++)
      {
        bool flag = false;
        IEntity entity = targetSymptoms.Find(x => StorageUtility.GetItemId(x) == StorageUtility.GetItemId(symptomTemplates[i].Value));
        if (entity != null)
        {
          IParameter<bool> byName = entity?.GetComponent<ParametersComponent>()?.GetByName<bool>(ParameterNameEnum.IsOpen);
          if (byName != null)
            flag = byName.Value;
        }
        HideableView component = symptomsImages[i]?.GetComponent<HideableView>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.Visible = flag;
      }
    }

    public override void Initialize()
    {
      RegisterLayer((IHealingWindow) this);
      base.Initialize();
    }

    public override Type GetWindowType() => typeof (IHealingWindow);

    private void UseItemOnTarget()
    {
      if (selectedItem == null)
        return;
      IParameter<BoundHealthStateEnum> byName1 = Target.Owner.GetComponent<ParametersComponent>()?.GetByName<BoundHealthStateEnum>(ParameterNameEnum.BoundHealthState);
      if (byName1 == null || byName1.Value != BoundHealthStateEnum.Diseased && byName1.Value != BoundHealthStateEnum.Danger && byName1.Value != BoundHealthStateEnum.TutorialPain && byName1.Value != BoundHealthStateEnum.TutorialDiagnostics)
        return;
      StorableGroup storableGroup = StorableGroup.None;
      foreach (StorableGroup group in selectedItem.Groups)
      {
        if ((byName1.Value == BoundHealthStateEnum.Diseased || byName1.Value == BoundHealthStateEnum.TutorialPain || byName1.Value == BoundHealthStateEnum.TutorialDiagnostics) && (group == StorableGroup.Diagnostic || group == StorableGroup.Antibiotic || group == StorableGroup.Painkiller || group == StorableGroup.Miracle))
        {
          storableGroup = group;
          break;
        }
        if (byName1.Value == BoundHealthStateEnum.Danger && group == StorableGroup.ImmuneBooster)
        {
          storableGroup = group;
          break;
        }
      }
      if (storableGroup == StorableGroup.None)
        return;
      LogicEventService service = ServiceLocator.GetService<LogicEventService>();
      IEntity template = (IEntity) selectedItem.Owner.Template;
      service.FireEntityEvent(storableGroup == StorableGroup.Diagnostic ? "WillSpendDiagnosticOn" : "WillSpendMedicineOn", Target.Owner);
      StorableComponentUtility.Use(selectedItem);
      service.FireEntityEvent(storableGroup == StorableGroup.Diagnostic ? "SpendDiagnostic" : "SpendMedicine", template);
      switch (storableGroup)
      {
        case StorableGroup.Antibiotic:
          IParameter<StammKind> byName2 = Target.Owner.GetComponent<ParametersComponent>()?.GetByName<StammKind>(ParameterNameEnum.StammKind);
          IParameter<StammKind> byName3 = template.GetComponent<ParametersComponent>()?.GetByName<StammKind>(ParameterNameEnum.StammKind);
          if (byName2 != null && byName3 != null)
          {
            if (byName3.Value == byName2.Value)
            {
              usedCorrectAntibioticMessage?.Invoke();
              service.FireEntityEvent("HealingAntibioticCorrect", Target.Owner);
            }
            else
            {
              usedWrongAntibioticMessage?.Invoke();
              service.FireEntityEvent("HealingAntibioticWrong", Target.Owner);
            }
            IsHealingPerformed = true;
          }
          break;
        case StorableGroup.Painkiller:
          usedPainkillerMessage?.Invoke();
          service.FireEntityEvent("UsedPainkiller", Target.Owner);
          break;
        case StorableGroup.ImmuneBooster:
          usedImmuneBoosterMessage?.Invoke();
          service.FireEntityEvent("UsedImmuneBooster", Target.Owner);
          break;
        case StorableGroup.Miracle:
          usedMaracleMessage?.Invoke();
          if (StorageUtility.GetItemId(template) == StorageUtility.GetItemId(shmowder.Value))
            service.FireEntityEvent("HealingPowder", Target.Owner);
          else if (StorageUtility.GetItemId(template) == StorageUtility.GetItemId(backerShmowder.Value))
            service.FireEntityEvent("HealingPowder", Target.Owner);
          else if (StorageUtility.GetItemId(template) == StorageUtility.GetItemId(panacea.Value))
            service.FireEntityEvent("HealingPanacea", Target.Owner);
          IsHealingEnded = true;
          break;
      }
      IParameter<float> byName4 = Target.Owner.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Pain);
      if (byName4 != null && byName4.Value >= (double) byName4.MaxValue)
        service.FireEntityEvent("HealingPainMax", Target.Owner);
      ItemToUse = null;
      selectedItem = null;
      SetSelectedItemInfo();
      CreateSlideContainers();
      if (!IsHealingEnded)
      {
        currentSliderIndex = 0;
        if (SliderItems.Count != 0)
        {
          selectedStorable = SliderItems[currentSliderIndex];
          slidingContainer.ScrollTo(currentSliderIndex, SliderItems.Count);
          SelectItemFromSlider();
        }
      }
      else
      {
        foreach (StorableUI sliderItem in SliderItems)
          sliderItem.SetSelected(false);
      }
      HandleHealingHintsVisibility(InputService.Instance.JoystickUsed);
    }

    public override IEntity GetUseTarget() => Target.Owner;

    private void ClearSlideContainers()
    {
      slidingContainer.Clear(containers, storables);
    }

    private void CreateSlideContainers()
    {
      ParametersComponent component = Target.Owner.GetComponent<ParametersComponent>();
      IParameter<BoundHealthStateEnum> byName1 = component?.GetByName<BoundHealthStateEnum>(ParameterNameEnum.BoundHealthState);
      ClearSlideContainers();
      if (byName1 == null || byName1.Value == BoundHealthStateEnum.None || byName1.Value == BoundHealthStateEnum.Normal || byName1.Value == BoundHealthStateEnum.Dead)
      {
        slidingContainer.gameObject.SetActive(false);
      }
      else
      {
        List<StorableComponent> list = Actor.Items.Cast<StorableComponent>().ToList();
        List<List<StorableComponent>> itemsList = new List<List<StorableComponent>>();
        List<string> groupSignatures = new List<string>();
        switch (byName1.Value)
        {
          case BoundHealthStateEnum.Danger:
            IParameter<bool> byName2 = component?.GetByName<bool>(ParameterNameEnum.ImmuneBoostAttempted);
            if (byName2 == null || !byName2.Value)
            {
              List<StorableComponent> all = list.FindAll(x => x.Groups.Contains(StorableGroup.ImmuneBooster));
              if (all != null && all.Count > 0)
              {
                itemsList.Add(all);
                groupSignatures.Add("{UI.Menu.Protagonist.Healing.ItemGroup.ImmuneBoosters}");
              }
            }
            break;
          case BoundHealthStateEnum.Diseased:
            List<StorableComponent> miracles = list.FindAll(x => x.Groups.Contains(StorableGroup.Miracle));
            IParameter<bool> byName3 = component?.GetByName<bool>(ParameterNameEnum.HealingAttempted);
            bool flag = byName3 != null && byName3.Value;
            if (flag && !IsHealingPerformed)
              IsHealingPerformed = true;
            if (!flag)
            {
              List<StorableComponent> diagnostics = list.FindAll(x => x.Groups.Contains(StorableGroup.Diagnostic));
              List<StorableComponent> antibiotics = list.FindAll(x => x.Groups.Contains(StorableGroup.Antibiotic));
              List<StorableComponent> all = list.FindAll(x => x.Groups.Contains(StorableGroup.Painkiller));
              diagnostics.RemoveAll(x => miracles.Contains(x));
              antibiotics.RemoveAll(x => miracles.Contains(x));
              antibiotics.RemoveAll(x => diagnostics.Contains(x));
              all.RemoveAll(x => miracles.Contains(x));
              all.RemoveAll(x => diagnostics.Contains(x));
              all.RemoveAll(x => antibiotics.Contains(x));
              if (all != null && all.Count > 0)
              {
                itemsList.Add(all);
                groupSignatures.Add("{UI.Menu.Protagonist.Healing.ItemGroup.Painkillers}");
              }
              if (diagnostics != null && diagnostics.Count > 0)
              {
                itemsList.Add(diagnostics);
                groupSignatures.Add("{UI.Menu.Protagonist.Healing.ItemGroup.Diagnostics}");
              }
              if (antibiotics != null && antibiotics.Count > 0)
              {
                itemsList.Add(antibiotics);
                groupSignatures.Add("{UI.Menu.Protagonist.Healing.ItemGroup.Antibiotics}");
              }
            }
            if (miracles != null && miracles.Count > 0)
            {
              itemsList.Add(miracles);
              groupSignatures.Add("{UI.Menu.Protagonist.Healing.ItemGroup.Miracles}");
              break;
            }
            if (flag)
            {
              IsHealingEnded = true;
            }
            break;
          case BoundHealthStateEnum.TutorialPain:
            List<StorableComponent> all1 = list.FindAll(x => x.Groups.Contains(StorableGroup.Painkiller));
            if (all1 != null && all1.Count > 0)
            {
              itemsList.Add(all1);
              groupSignatures.Add("{UI.Menu.Protagonist.Healing.ItemGroup.Painkillers}");
            }
            break;
          case BoundHealthStateEnum.TutorialDiagnostics:
            List<StorableComponent> diagnostics1 = list.FindAll(x => x.Groups.Contains(StorableGroup.Diagnostic));
            List<StorableComponent> all2 = list.FindAll(x => x.Groups.Contains(StorableGroup.Painkiller));
            all2.RemoveAll(x => diagnostics1.Contains(x));
            if (all2 != null && all2.Count > 0)
            {
              itemsList.Add(all2);
              groupSignatures.Add("{UI.Menu.Protagonist.Healing.ItemGroup.Painkillers}");
            }
            if (diagnostics1 != null && diagnostics1.Count > 0)
            {
              itemsList.Add(diagnostics1);
              groupSignatures.Add("{UI.Menu.Protagonist.Healing.ItemGroup.Diagnostics}");
            }
            break;
        }
        hasValidItems.Visible = itemsList.Count > 0;
        slidingContainer.CreateSlots(itemsList, groupSignatures, (StorageComponent) Actor, containers, storables);
        slidingContainer.gameObject.SetActive(true);
        controlPanel.SetActive(!IsHealingEnded && InputService.Instance.JoystickUsed);
        HideInfoWindow();
        SliderItems = slidingContainer.ItemsUI;
      }
    }

    protected override void OnInvalidate()
    {
      base.OnInvalidate();
      CreateSlideContainers();
    }

    protected override void AddActionsToInfoWindow(
      InfoWindowNew window,
      IStorableComponent storable)
    {
      if (!ItemIsInteresting(storable))
        return;
      window.AddActionTooltip(GameActionType.Submit, "{StorableTooltip.Select}");
    }

    public void OnParameterChanged(IParameter parameter)
    {
      if (parameter.Name == ParameterNameEnum.Pain)
      {
        if (((IParameter<float>) parameter).Value < (double) oldPain)
          Actor.GetComponent<PlayerControllerComponent>().ComputeHealPain(Target?.Owner, oldPain - ((IParameter<float>) parameter).Value);
        oldPain = ((IParameter<float>) parameter).Value;
      }
      else if (parameter.Name == ParameterNameEnum.Infection)
      {
        if (((IParameter<float>) parameter).Value < (double) oldInfection)
        {
          PlayerControllerComponent component = Actor.GetComponent<PlayerControllerComponent>();
          component.ComputeHealInfection(Target?.Owner, oldInfection - ((IParameter<float>) parameter).Value);
          if (((IParameter<float>) parameter).Value == 0.0)
            component.ComputeCureInfection(Target?.Owner);
        }
        oldInfection = ((IParameter<float>) parameter).Value;
      }
      else
      {
        if (parameter.Name != ParameterNameEnum.IsOpen)
          return;
        ShowSymptoms();
        if ((UnityEngine.Object) definitiveDiagnosisCheck != (UnityEngine.Object) null && definitiveDiagnosisCheck.Visible)
          ServiceLocator.GetService<LogicEventService>().FireEntityEvent("DefinitiveDiagnosis", Target.Owner);
      }
    }

    private delegate bool ScrollHandle(GameActionType type, bool down);
  }
}
