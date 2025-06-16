// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.Inventory.HealingWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable disable
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
    private bool IsHealingEnded = false;
    private bool IsHealingPerformed = false;
    private StorableUI ItemToUse = (StorableUI) null;

    public IStorageComponent Target { get; set; }

    private void SelectItem(StorableComponent storable)
    {
      if (this.selectedItem != null)
      {
        StorableUI storable1 = this.storables[(IStorableComponent) this.selectedItem];
        if ((UnityEngine.Object) storable1 != (UnityEngine.Object) null)
          storable1.SetSelected(false);
      }
      this.selectedItem = storable;
      StorableUI storable2 = this.storables[(IStorableComponent) storable];
      if ((UnityEngine.Object) storable2 != (UnityEngine.Object) null)
        storable2.SetSelected(true);
      StorableComponentUtility.PlayTakeSound((IStorableComponent) storable);
      this.SetSelectedItemInfo();
      this.HandleHealingHintsVisibility(InputService.Instance.JoystickUsed);
    }

    private void SetSelectedItemInfo()
    {
      this.selectedItemUI.Storable = this.selectedItem;
      if (this.selectedItem == null)
      {
        this.buttonUse.interactable = false;
        if (!InputService.Instance.JoystickUsed)
          return;
        this._administerHealingHint.SetActive(false);
      }
      else
      {
        this.buttonUse.interactable = true;
        if (InputService.Instance.JoystickUsed)
          this._administerHealingHint.SetActive(true);
      }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
      if ((UnityEngine.Object) this.windowContextMenu != (UnityEngine.Object) null)
      {
        this.HideContextMenu();
      }
      else
      {
        if (!this.intersect.IsIntersected)
          return;
        StorableComponent storableComponent = this.intersect.Storables.FirstOrDefault<StorableComponent>();
        if (storableComponent == null || eventData.button != PointerEventData.InputButton.Left)
          return;
        this.SelectItem(storableComponent);
        this.ItemToUse = this.GetStorableByComponent((IStorableComponent) storableComponent);
        if (!((UnityEngine.Object) this.ItemToUse != (UnityEngine.Object) null))
          return;
        this.currentSliderIndex = this.SliderItems.IndexOf(this.ItemToUse);
      }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
    }

    private void SubscribeToParameters(bool subscribe)
    {
      IEntity owner = this.Target?.Owner;
      if (owner == null)
        return;
      ParametersComponent component = owner.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<float> byName1 = component.GetByName<float>(ParameterNameEnum.Pain);
        if (byName1 != null)
        {
          byName1.RemoveListener((IChangeParameterListener) this);
          if (subscribe)
          {
            byName1.AddListener((IChangeParameterListener) this);
            this.oldPain = byName1.Value;
          }
        }
        IParameter<float> byName2 = component.GetByName<float>(ParameterNameEnum.Infection);
        byName2.RemoveListener((IChangeParameterListener) this);
        if (subscribe)
        {
          byName2.AddListener((IChangeParameterListener) this);
          this.oldInfection = byName2.Value;
        }
      }
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      this.Unsubscribe();
      this.UnsubscribeNavigation();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickUp, new GameActionHandle(this.MedicineNavigation));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LStickDown, new GameActionHandle(this.MedicineNavigation));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Submit, new GameActionHandle(this.AdministerHealing));
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      this.buttonUse.OpenEndEvent += new Action<bool>(this.OnUseButtonEnd);
      if (this.symptomTemplates == null)
        this.symptomTemplates = new IEntitySerializable[7]
        {
          this.symptom1,
          this.symptom2,
          this.symptom3,
          this.symptom4,
          this.symptom5,
          this.symptom6,
          this.symptom7
        };
      this.selectedItem = (StorableComponent) null;
      this.CountSymptoms();
      this.actors.Clear();
      this.Build2();
      IEntity owner = this.Target?.Owner;
      foreach (EntityView parameterView in this.parameterViews)
      {
        parameterView.Value = owner;
        parameterView.SkipAnimation();
      }
      this.ShowSymptoms();
      this.CreateSlideContainers();
      this.SetSelectedItemInfo();
      this.selectedItemUI.SkipAnimation();
      this.SubscribeToParameters(true);
      foreach (HideableView strainsEntity in this.strainsEntities)
        strainsEntity.OnVisibilityChanged += new HideableView.VisibilityChanged(this.OnHideableVisibleChanged);
      this.currentSliderIndex = 0;
      this.SetInfoWindowShowMode(true, false);
    }

    private void OnUseButtonEnd(bool success)
    {
      if (!success)
        return;
      this.UseItemOnTarget();
    }

    protected override void OnDisable()
    {
      this.SubscribeToParameters(false);
      this.buttonUse.OpenEndEvent -= new Action<bool>(this.OnUseButtonEnd);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Heal, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickUp, new GameActionHandle(this.MedicineNavigation));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LStickDown, new GameActionHandle(this.MedicineNavigation));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Submit, new GameActionHandle(this.AdministerHealing));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Cancel, new GameActionHandle(((UIWindow) this).CancelListener));
      if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
      {
        this.selectedStorable.HoldSelected(false);
        this.selectedStorable.SetSelected(false);
        this.selectedStorable = (StorableUI) null;
      }
      this.ItemToUse = (StorableUI) null;
      this.IsHealingEnded = false;
      this.IsHealingPerformed = false;
      foreach (HideableView strainsEntity in this.strainsEntities)
        strainsEntity.OnVisibilityChanged -= new HideableView.VisibilityChanged(this.OnHideableVisibleChanged);
      foreach (GameObject gameObject in this.consoleStrainsTextInfo)
        gameObject.SetActive(false);
      foreach (EntityView parameterView in this.parameterViews)
      {
        parameterView.Value = (IEntity) null;
        parameterView.SkipAnimation();
      }
      foreach (IEntity targetSymptom in this.targetSymptoms)
        targetSymptom?.GetComponent<ParametersComponent>()?.GetByName<bool>(ParameterNameEnum.IsOpen)?.RemoveListener((IChangeParameterListener) this);
      this.clearMessages?.Invoke();
      this.ClearSlideContainers();
      base.OnDisable();
    }

    private void OnHideableVisibleChanged(bool value, HideableView view)
    {
      int index = this.strainsEntities.IndexOf(view);
      if (index == -1)
        return;
      this.consoleStrainsTextInfo[index].SetActive(value);
    }

    private List<StorableUI> SliderItems { get; set; }

    private void HandleHealingHintsVisibility(bool visible)
    {
      Text componentInChildren = this.buttonHealingHints.Where<GameObject>((Func<GameObject, bool>) (hint => hint.GetComponent<HideableView>().Visible)).First<GameObject>()?.GetComponentInChildren<Text>();
      if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null && this.buttonUse.interactable)
      {
        this._administerHealingHint.transform.parent = componentInChildren.transform;
        this._administerHealingHint.transform.localPosition = Vector3.zero;
        this._administerHealingHint.transform.localPosition = new Vector3((float) ((double) componentInChildren.transform.localPosition.x - (double) componentInChildren.preferredWidth / 2.0 - 15.0), this._administerHealingHint.transform.localPosition.y);
        this._administerHealingHint.SetActive(visible);
      }
      else
        this._administerHealingHint.SetActive(false);
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      this.controlPanel.SetActive(joystick && !this.IsHealingEnded && this.SliderItems.Count > 0);
      this.HandleHealingHintsVisibility(joystick);
      if (joystick)
      {
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Heal, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        if (this.IsHealingEnded)
          return;
        if (this.SliderItems.Count == 0)
          this.SliderItems = (List<StorableUI>) null;
        else if ((UnityEngine.Object) this.ItemToUse == (UnityEngine.Object) null && this.SliderItems.Count > 0)
          this.ItemToUse = this.SliderItems.First<StorableUI>();
        if (this.currentSliderIndex < 0)
          this.currentSliderIndex = 0;
        if ((UnityEngine.Object) this.ItemToUse != (UnityEngine.Object) null)
          this.ItemToUse.HoldSelected(true);
        if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) this.ItemToUse && (UnityEngine.Object) this.ItemToUse != (UnityEngine.Object) null)
        {
          this.selectedStorable = this.ItemToUse;
          this.currentSliderIndex = this.SliderItems.IndexOf(this.selectedStorable);
        }
        if ((UnityEngine.Object) this.selectedStorable == (UnityEngine.Object) null && this.SliderItems.Count > 0)
          this.selectedStorable = this.SliderItems.First<StorableUI>();
        this.slidingContainer.ScrollTo(this.currentSliderIndex, this.SliderItems.Count);
        if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
        {
          this.selectedStorable.SetSelected(true);
          this.SelectItemFromSlider();
        }
        for (int index = 0; index < this.strainsEntities.Count; ++index)
        {
          if (this.strainsEntities[index].Visible)
            this.consoleStrainsTextInfo[index].SetActive(joystick);
        }
      }
      else
      {
        this.buttonUse.GamepadEndHold();
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Heal, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
        if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
        {
          this.selectedStorable?.SetSelected(false);
          this.selectedStorable?.HoldSelected(false);
        }
        if ((UnityEngine.Object) this.ItemToUse != (UnityEngine.Object) null)
          this.ItemToUse.HoldSelected(false);
        foreach (GameObject gameObject in this.consoleStrainsTextInfo)
          gameObject.SetActive(false);
      }
    }

    private IEnumerator ScrollCoroutine(
      HealingWindow.ScrollHandle handle,
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
        if (this.scrollCoroutine != null)
          this.StopCoroutine(this.scrollCoroutine);
        this.scrollCoroutine = this.StartCoroutine(this.ScrollCoroutine(new HealingWindow.ScrollHandle(this.OnMedicineNavigation), type, down));
        return true;
      }
      if (this.scrollCoroutine != null)
      {
        this.StopCoroutine(this.scrollCoroutine);
        this.scrollCoroutine = (Coroutine) null;
      }
      return false;
    }

    private bool OnMedicineNavigation(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed || ((type == GameActionType.LStickUp ? 1 : (type == GameActionType.LStickDown ? 1 : 0)) & (down ? 1 : 0)) == 0 || this.IsHealingEnded)
        return false;
      if (this.SliderItems.Count == 0)
        this.SliderItems = (List<StorableUI>) null;
      if (this.SliderItems.Count < 1)
        return false;
      this.currentSliderIndex += type == GameActionType.LStickUp ? -1 : 1;
      if (this.currentSliderIndex < 0)
        this.currentSliderIndex = this.SliderItems.Count - 1;
      if (this.currentSliderIndex >= this.SliderItems.Count)
        this.currentSliderIndex = 0;
      if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null && (UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) this.ItemToUse)
        this.selectedStorable?.SetSelected(false);
      this.selectedStorable = this.SliderItems[this.currentSliderIndex];
      if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
        this.selectedStorable?.SetSelected(true);
      this.slidingContainer.ScrollTo(this.currentSliderIndex, this.SliderItems.Count);
      return this.SelectItemFromSlider();
    }

    private bool SelectItemFromSlider()
    {
      if ((UnityEngine.Object) this.selectedStorable != (UnityEngine.Object) null)
      {
        this.HideInfoWindow();
        this.SelectItem((StorableComponent) this.selectedStorable.Internal);
        this.buttonUse.GamepadEndHold();
        if ((UnityEngine.Object) this.ItemToUse != (UnityEngine.Object) null && (UnityEngine.Object) this.ItemToUse != (UnityEngine.Object) this.selectedStorable)
          this.ItemToUse.HoldSelected(false);
        this.ItemToUse = this.selectedStorable;
        this.ItemToUse.HoldSelected(true);
        StorableUI cachedItemToUse = this.ItemToUse;
        CoroutineService.Instance.WaitFrame(1, (Action) (() => this.ShowInfoWindow(cachedItemToUse.Internal)));
      }
      return true;
    }

    protected override void PositionWindow(UIControl window, IStorableComponent storable)
    {
      base.PositionWindow(window, storable);
      if (!InputService.Instance.JoystickUsed)
        return;
      StorableUI itemToUse = this.ItemToUse;
      if ((UnityEngine.Object) itemToUse == (UnityEngine.Object) null)
        return;
      RectTransform component1 = window.GetComponent<RectTransform>();
      RectTransform component2 = this.slidingContainer.GetComponent<RectTransform>();
      float num1 = 20f;
      float hintsBottomBorder = this.HintsBottomBorder;
      float num2 = itemToUse.Image.rectTransform.rect.height / 2f * itemToUse.Image.rectTransform.lossyScale.y;
      double num3 = (double) itemToUse.Image.transform.position.y + (double) num2;
      Rect rect = component1.rect;
      double num4 = (double) rect.height * (double) component1.lossyScale.y;
      if (num3 - num4 < (double) hintsBottomBorder)
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
      float num8 = (float) (num6 - num7 - (double) num1 * (double) component2.lossyScale.x / 2.0);
      window.Transform.position = new Vector3(itemToUse.Image.transform.position.x + num8, itemToUse.Image.transform.position.y + num2);
    }

    private bool AdministerHealing(GameActionType type, bool down)
    {
      if (!InputService.Instance.JoystickUsed)
        return false;
      if (type == GameActionType.Submit & down)
      {
        this.buttonUse.GamepadStartHold();
        return true;
      }
      if (type != GameActionType.Submit || down)
        return false;
      this.buttonUse.GamepadEndHold();
      return true;
    }

    private void CountSymptoms()
    {
      this.targetSymptoms.Clear();
      foreach (IStorableComponent storableComponent in this.Target.Items)
      {
        if (storableComponent.Groups.Contains<StorableGroup>(StorableGroup.Symptom))
        {
          this.targetSymptoms.Add(storableComponent.Owner);
          IParameter<bool> byName = storableComponent.Owner?.GetComponent<ParametersComponent>()?.GetByName<bool>(ParameterNameEnum.IsOpen);
          if (byName != null)
          {
            byName.RemoveListener((IChangeParameterListener) this);
            byName.AddListener((IChangeParameterListener) this);
          }
        }
      }
    }

    private void ShowSymptoms()
    {
      for (int i = 0; i < this.symptomTemplates.Length; i++)
      {
        bool flag = false;
        IEntity entity = this.targetSymptoms.Find((Predicate<IEntity>) (x => StorageUtility.GetItemId(x) == StorageUtility.GetItemId(this.symptomTemplates[i].Value)));
        if (entity != null)
        {
          IParameter<bool> byName = entity?.GetComponent<ParametersComponent>()?.GetByName<bool>(ParameterNameEnum.IsOpen);
          if (byName != null)
            flag = byName.Value;
        }
        HideableView component = this.symptomsImages[i]?.GetComponent<HideableView>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.Visible = flag;
      }
    }

    public override void Initialize()
    {
      this.RegisterLayer<IHealingWindow>((IHealingWindow) this);
      base.Initialize();
    }

    public override System.Type GetWindowType() => typeof (IHealingWindow);

    private void UseItemOnTarget()
    {
      if (this.selectedItem == null)
        return;
      IParameter<BoundHealthStateEnum> byName1 = this.Target.Owner.GetComponent<ParametersComponent>()?.GetByName<BoundHealthStateEnum>(ParameterNameEnum.BoundHealthState);
      if (byName1 == null || byName1.Value != BoundHealthStateEnum.Diseased && byName1.Value != BoundHealthStateEnum.Danger && byName1.Value != BoundHealthStateEnum.TutorialPain && byName1.Value != BoundHealthStateEnum.TutorialDiagnostics)
        return;
      StorableGroup storableGroup = StorableGroup.None;
      foreach (StorableGroup group in this.selectedItem.Groups)
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
      IEntity template = (IEntity) this.selectedItem.Owner.Template;
      service.FireEntityEvent(storableGroup == StorableGroup.Diagnostic ? "WillSpendDiagnosticOn" : "WillSpendMedicineOn", this.Target.Owner);
      StorableComponentUtility.Use((IStorableComponent) this.selectedItem);
      service.FireEntityEvent(storableGroup == StorableGroup.Diagnostic ? "SpendDiagnostic" : "SpendMedicine", template);
      switch (storableGroup)
      {
        case StorableGroup.Antibiotic:
          IParameter<StammKind> byName2 = this.Target.Owner.GetComponent<ParametersComponent>()?.GetByName<StammKind>(ParameterNameEnum.StammKind);
          IParameter<StammKind> byName3 = template.GetComponent<ParametersComponent>()?.GetByName<StammKind>(ParameterNameEnum.StammKind);
          if (byName2 != null && byName3 != null)
          {
            if (byName3.Value == byName2.Value)
            {
              this.usedCorrectAntibioticMessage?.Invoke();
              service.FireEntityEvent("HealingAntibioticCorrect", this.Target.Owner);
            }
            else
            {
              this.usedWrongAntibioticMessage?.Invoke();
              service.FireEntityEvent("HealingAntibioticWrong", this.Target.Owner);
            }
            this.IsHealingPerformed = true;
            break;
          }
          break;
        case StorableGroup.Painkiller:
          this.usedPainkillerMessage?.Invoke();
          service.FireEntityEvent("UsedPainkiller", this.Target.Owner);
          break;
        case StorableGroup.ImmuneBooster:
          this.usedImmuneBoosterMessage?.Invoke();
          service.FireEntityEvent("UsedImmuneBooster", this.Target.Owner);
          break;
        case StorableGroup.Miracle:
          this.usedMaracleMessage?.Invoke();
          if (StorageUtility.GetItemId(template) == StorageUtility.GetItemId(this.shmowder.Value))
            service.FireEntityEvent("HealingPowder", this.Target.Owner);
          else if (StorageUtility.GetItemId(template) == StorageUtility.GetItemId(this.backerShmowder.Value))
            service.FireEntityEvent("HealingPowder", this.Target.Owner);
          else if (StorageUtility.GetItemId(template) == StorageUtility.GetItemId(this.panacea.Value))
            service.FireEntityEvent("HealingPanacea", this.Target.Owner);
          this.IsHealingEnded = true;
          break;
      }
      IParameter<float> byName4 = this.Target.Owner.GetComponent<ParametersComponent>()?.GetByName<float>(ParameterNameEnum.Pain);
      if (byName4 != null && (double) byName4.Value >= (double) byName4.MaxValue)
        service.FireEntityEvent("HealingPainMax", this.Target.Owner);
      this.ItemToUse = (StorableUI) null;
      this.selectedItem = (StorableComponent) null;
      this.SetSelectedItemInfo();
      this.CreateSlideContainers();
      if (!this.IsHealingEnded)
      {
        this.currentSliderIndex = 0;
        if (this.SliderItems.Count != 0)
        {
          this.selectedStorable = this.SliderItems[this.currentSliderIndex];
          this.slidingContainer.ScrollTo(this.currentSliderIndex, this.SliderItems.Count);
          this.SelectItemFromSlider();
        }
      }
      else
      {
        foreach (StorableUI sliderItem in this.SliderItems)
          sliderItem.SetSelected(false);
      }
      this.HandleHealingHintsVisibility(InputService.Instance.JoystickUsed);
    }

    public override IEntity GetUseTarget() => this.Target.Owner;

    private void ClearSlideContainers()
    {
      this.slidingContainer.Clear(this.containers, this.storables);
    }

    private void CreateSlideContainers()
    {
      ParametersComponent component = this.Target.Owner.GetComponent<ParametersComponent>();
      IParameter<BoundHealthStateEnum> byName1 = component?.GetByName<BoundHealthStateEnum>(ParameterNameEnum.BoundHealthState);
      this.ClearSlideContainers();
      if (byName1 == null || byName1.Value == BoundHealthStateEnum.None || byName1.Value == BoundHealthStateEnum.Normal || byName1.Value == BoundHealthStateEnum.Dead)
      {
        this.slidingContainer.gameObject.SetActive(false);
      }
      else
      {
        List<StorableComponent> list = this.Actor.Items.Cast<StorableComponent>().ToList<StorableComponent>();
        List<List<StorableComponent>> itemsList = new List<List<StorableComponent>>();
        List<string> groupSignatures = new List<string>();
        switch (byName1.Value)
        {
          case BoundHealthStateEnum.Danger:
            IParameter<bool> byName2 = component?.GetByName<bool>(ParameterNameEnum.ImmuneBoostAttempted);
            if (byName2 == null || !byName2.Value)
            {
              List<StorableComponent> all = list.FindAll((Predicate<StorableComponent>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.ImmuneBooster)));
              if (all != null && all.Count > 0)
              {
                itemsList.Add(all);
                groupSignatures.Add("{UI.Menu.Protagonist.Healing.ItemGroup.ImmuneBoosters}");
                break;
              }
              break;
            }
            break;
          case BoundHealthStateEnum.Diseased:
            List<StorableComponent> miracles = list.FindAll((Predicate<StorableComponent>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.Miracle)));
            IParameter<bool> byName3 = component?.GetByName<bool>(ParameterNameEnum.HealingAttempted);
            bool flag = byName3 != null && byName3.Value;
            if (flag && !this.IsHealingPerformed)
              this.IsHealingPerformed = true;
            if (!flag)
            {
              List<StorableComponent> diagnostics = list.FindAll((Predicate<StorableComponent>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.Diagnostic)));
              List<StorableComponent> antibiotics = list.FindAll((Predicate<StorableComponent>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.Antibiotic)));
              List<StorableComponent> all = list.FindAll((Predicate<StorableComponent>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.Painkiller)));
              diagnostics.RemoveAll((Predicate<StorableComponent>) (x => miracles.Contains(x)));
              antibiotics.RemoveAll((Predicate<StorableComponent>) (x => miracles.Contains(x)));
              antibiotics.RemoveAll((Predicate<StorableComponent>) (x => diagnostics.Contains(x)));
              all.RemoveAll((Predicate<StorableComponent>) (x => miracles.Contains(x)));
              all.RemoveAll((Predicate<StorableComponent>) (x => diagnostics.Contains(x)));
              all.RemoveAll((Predicate<StorableComponent>) (x => antibiotics.Contains(x)));
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
              this.IsHealingEnded = true;
              break;
            }
            break;
          case BoundHealthStateEnum.TutorialPain:
            List<StorableComponent> all1 = list.FindAll((Predicate<StorableComponent>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.Painkiller)));
            if (all1 != null && all1.Count > 0)
            {
              itemsList.Add(all1);
              groupSignatures.Add("{UI.Menu.Protagonist.Healing.ItemGroup.Painkillers}");
              break;
            }
            break;
          case BoundHealthStateEnum.TutorialDiagnostics:
            List<StorableComponent> diagnostics1 = list.FindAll((Predicate<StorableComponent>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.Diagnostic)));
            List<StorableComponent> all2 = list.FindAll((Predicate<StorableComponent>) (x => x.Groups.Contains<StorableGroup>(StorableGroup.Painkiller)));
            all2.RemoveAll((Predicate<StorableComponent>) (x => diagnostics1.Contains(x)));
            if (all2 != null && all2.Count > 0)
            {
              itemsList.Add(all2);
              groupSignatures.Add("{UI.Menu.Protagonist.Healing.ItemGroup.Painkillers}");
            }
            if (diagnostics1 != null && diagnostics1.Count > 0)
            {
              itemsList.Add(diagnostics1);
              groupSignatures.Add("{UI.Menu.Protagonist.Healing.ItemGroup.Diagnostics}");
              break;
            }
            break;
        }
        this.hasValidItems.Visible = itemsList.Count > 0;
        this.slidingContainer.CreateSlots(itemsList, groupSignatures, (StorageComponent) this.Actor, this.containers, this.storables);
        this.slidingContainer.gameObject.SetActive(true);
        this.controlPanel.SetActive(!this.IsHealingEnded && InputService.Instance.JoystickUsed);
        this.HideInfoWindow();
        this.SliderItems = this.slidingContainer.ItemsUI;
      }
    }

    protected override void OnInvalidate()
    {
      base.OnInvalidate();
      this.CreateSlideContainers();
    }

    protected override void AddActionsToInfoWindow(
      InfoWindowNew window,
      IStorableComponent storable)
    {
      if (!this.ItemIsInteresting(storable))
        return;
      window.AddActionTooltip(GameActionType.Submit, "{StorableTooltip.Select}");
    }

    public void OnParameterChanged(IParameter parameter)
    {
      if (parameter.Name == ParameterNameEnum.Pain)
      {
        if ((double) ((IParameter<float>) parameter).Value < (double) this.oldPain)
          this.Actor.GetComponent<PlayerControllerComponent>().ComputeHealPain(this.Target?.Owner, this.oldPain - ((IParameter<float>) parameter).Value);
        this.oldPain = ((IParameter<float>) parameter).Value;
      }
      else if (parameter.Name == ParameterNameEnum.Infection)
      {
        if ((double) ((IParameter<float>) parameter).Value < (double) this.oldInfection)
        {
          PlayerControllerComponent component = this.Actor.GetComponent<PlayerControllerComponent>();
          component.ComputeHealInfection(this.Target?.Owner, this.oldInfection - ((IParameter<float>) parameter).Value);
          if ((double) ((IParameter<float>) parameter).Value == 0.0)
            component.ComputeCureInfection(this.Target?.Owner);
        }
        this.oldInfection = ((IParameter<float>) parameter).Value;
      }
      else
      {
        if (parameter.Name != ParameterNameEnum.IsOpen)
          return;
        this.ShowSymptoms();
        if ((UnityEngine.Object) this.definitiveDiagnosisCheck != (UnityEngine.Object) null && this.definitiveDiagnosisCheck.Visible)
          ServiceLocator.GetService<LogicEventService>().FireEntityEvent("DefinitiveDiagnosis", this.Target.Owner);
      }
    }

    private delegate bool ScrollHandle(GameActionType type, bool down);
  }
}
