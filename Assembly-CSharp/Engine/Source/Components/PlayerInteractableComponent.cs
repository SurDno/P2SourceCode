// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.PlayerInteractableComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Components.Interactable;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings.External;
using Engine.Source.UI;
using Engine.Source.Utility;
using InputServices;
using Inspectors;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Engine.Source.Components
{
  [Factory]
  [Required(typeof (LocationItemComponent))]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PlayerInteractableComponent : EngineComponent, IUpdatable, IPlayerActivated
  {
    [Inspected]
    private IEntity currentTarget;
    [Inspected]
    private InteractableComponent currentInteractable;
    [Inspected]
    private List<InteractItemInfo> validateItems = new List<InteractItemInfo>();
    [Inspected]
    private HashSet<GameActionType> pressed = new HashSet<GameActionType>();
    [FromLocator]
    private GameActionService gameActionService;
    [FromLocator]
    private UIService uiService;
    [FromLocator]
    private PickingService pickingService;
    private List<InteractItemInfo> tmp = new List<InteractItemInfo>();
    private bool isJoystick;
    private IOrderedEnumerable<InteractItemInfo> validateItemsOrdered;
    private List<GameActionType> cachedLastInteractionTypes = new List<GameActionType>();
    private List<GameActionType> comparationList = new List<GameActionType>();

    public InteractableComponent Interactable => this.currentInteractable;

    public IEnumerable<InteractItemInfo> ValidateItems
    {
      get => (IEnumerable<InteractItemInfo>) this.validateItems;
    }

    public override void OnChangeEnabled()
    {
      base.OnChangeEnabled();
      this.currentTarget = (IEntity) null;
      this.currentInteractable = (InteractableComponent) null;
      this.validateItems.Clear();
    }

    public void ComputeUpdate()
    {
      if (!PlayerUtility.IsPlayerCanControlling)
      {
        this.ClearActions();
      }
      else
      {
        if (this.isJoystick != InputService.Instance.JoystickUsed)
        {
          this.isJoystick = InputService.Instance.JoystickUsed;
          this.cachedLastInteractionTypes = (List<GameActionType>) null;
        }
        IEntity entity = this.pickingService.TargetEntity;
        if (entity != null && (double) this.pickingService.TargetEntityDistance > (double) ExternalSettingsInstance<ExternalCommonSettings>.Instance.InteractionDistance)
          entity = (IEntity) null;
        if (entity != this.currentTarget)
        {
          this.currentTarget = entity;
          this.currentInteractable = (InteractableComponent) null;
          this.cachedLastInteractionTypes = (List<GameActionType>) null;
          if (this.currentTarget != null)
            this.currentInteractable = this.currentTarget.GetComponent<InteractableComponent>();
        }
        this.UpdateActions();
      }
    }

    private void ClearActions()
    {
      this.validateItems.Clear();
      this.cachedLastInteractionTypes = (List<GameActionType>) null;
    }

    private void UpdateActions()
    {
      bool isSame = false;
      if (this.currentInteractable != null && !this.currentInteractable.IsDisposed && this.currentInteractable.Owner.IsEnabledInHierarchy && this.currentInteractable.IsEnabled)
      {
        this.comparationList.Clear();
        this.currentInteractable.Items.ForEach((Action<InteractItem>) (i =>
        {
          if (!InteractValidationService.Validate((IInteractableComponent) this.currentInteractable, i).Result)
            return;
          this.comparationList.Add(i.Action);
        }));
        if (this.cachedLastInteractionTypes == null || !this.cachedLastInteractionTypes.SequenceEqual<GameActionType>((IEnumerable<GameActionType>) this.comparationList))
        {
          this.ClearActions();
          this.validateItems.AddRange(this.currentInteractable.GetValidateItems(this.Owner));
          this.cachedLastInteractionTypes = new List<GameActionType>((IEnumerable<GameActionType>) this.comparationList);
        }
        else
          isSame = true;
      }
      else
        this.ClearActions();
      if (!isSame)
      {
        if (this.validateItemsOrdered == null)
          this.validateItemsOrdered = this.validateItems.OrderBy<InteractItemInfo, bool>((Func<InteractItemInfo, bool>) (o => o.Invalid));
        this.tmp.Clear();
        this.tmp.AddRange((IEnumerable<InteractItemInfo>) this.validateItemsOrdered);
        for (int index = 1; index < this.tmp.Count && !this.tmp[index].Invalid; ++index)
        {
          if (this.tmp[index].Item.Action == this.tmp[index - 1].Item.Action)
          {
            this.tmp[index].Invalid = true;
            this.tmp[index].Dublicate = true;
          }
        }
        int index1 = 0;
        for (int index2 = 0; index2 < this.validateItems.Count; ++index2)
        {
          InteractItemInfo validateItem = this.validateItems[index2];
          if (validateItem.Invalid)
          {
            validateItem.OverrideAction = index1 < InteractUtility.DebugActions.Length ? InteractUtility.DebugActions[index1].Action : InteractUtility.DebugActions[InteractUtility.DebugActions.Length - 1].Action;
            ++index1;
          }
        }
        CoroutineService.Instance.WaitFrame(1, (Action) (() => this.UpdateIcons(false)));
      }
      this.UpdateIcons(isSame);
    }

    private void UpdateIcons(bool isSame)
    {
      IHudWindow hudWindow = this.uiService.Get<IHudWindow>();
      if (hudWindow == null)
        return;
      InteractableWindow interactableInterface = hudWindow.InteractableInterface;
      if ((UnityEngine.Object) interactableInterface == (UnityEngine.Object) null)
        return;
      if (!isSame)
      {
        InteractableWindow.IconType info = InteractableWindow.IconType.None;
        if (!InputService.Instance.JoystickUsed)
        {
          string text = "";
          if (this.currentInteractable != null && !this.currentInteractable.IsDisposed)
          {
            info = DefaultInteractableMapping.GetIconType((IInteractableComponent) this.currentInteractable, this.validateItems);
            text = DefaultInteractableMapping.GetText(this.validateItems);
          }
          interactableInterface.SetInfo(info, text);
        }
        else
        {
          List<KeyValuePair<Sprite, bool>> iconSprites = (List<KeyValuePair<Sprite, bool>>) null;
          if (this.currentInteractable != null && !this.currentInteractable.IsDisposed)
          {
            InteractableWindow.IconType iconType = DefaultInteractableMapping.GetIconType((IInteractableComponent) this.currentInteractable, this.validateItems);
            List<KeyValuePair<GameActionType, bool>> actions;
            string[] text = DefaultInteractableMapping.GetText(this.validateItems, out iconSprites, out actions);
            interactableInterface.SetInfo(iconType, text, iconSprites, actions);
          }
          else
            interactableInterface.DeactivateAllTitles();
        }
      }
      else
        interactableInterface.UpdateProgress();
    }

    private bool Listener(GameActionType type, bool down)
    {
      if (!PlayerUtility.IsPlayerCanControlling || !down || this.currentInteractable == null || this.currentInteractable.IsDisposed)
        return false;
      InteractItemInfo interactItemInfo = (InteractItemInfo) null;
      foreach (InteractItemInfo validateItem in this.validateItems)
      {
        if (validateItem.Invalid)
        {
          if (InstanceByRequest<EngineApplication>.Instance.IsDebug && validateItem.OverrideAction == type)
          {
            interactItemInfo = validateItem;
            break;
          }
        }
        else if (validateItem.Item.Action == type)
        {
          interactItemInfo = validateItem;
          break;
        }
      }
      if (interactItemInfo == null)
        return false;
      this.currentInteractable.BeginInteract(this.Owner, interactItemInfo.Item.Type);
      this.currentInteractable = (InteractableComponent) null;
      this.currentTarget = (IEntity) null;
      this.validateItems.Clear();
      this.UpdateIcons(false);
      return true;
    }

    public void PlayerActivated()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
      foreach (GameActionType interactAction in InteractUtility.InteractActions)
        this.gameActionService.AddListener(interactAction, new GameActionHandle(this.Listener));
      JoystickLayoutSwitcher.Instance.OnLayoutChanged += new Action<JoystickLayoutSwitcher.KeyLayouts>(this.OnLayoutChanged);
    }

    public void PlayerDeactivated()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
      foreach (GameActionType interactAction in InteractUtility.InteractActions)
        this.gameActionService.RemoveListener(interactAction, new GameActionHandle(this.Listener));
      JoystickLayoutSwitcher.Instance.OnLayoutChanged -= new Action<JoystickLayoutSwitcher.KeyLayouts>(this.OnLayoutChanged);
    }

    private void OnLayoutChanged(JoystickLayoutSwitcher.KeyLayouts newLayout)
    {
      this.cachedLastInteractionTypes = (List<GameActionType>) null;
      this.UpdateActions();
    }
  }
}
