using System;
using System.Collections;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using Engine.Source.Inventory;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  public class ProtagonistWindow : 
    BaseInventoryWindow<ProtagonistWindow>,
    IInventoryWindow,
    IWindow,
    IPauseMenu
  {
    [SerializeField]
    private IEntitySerializable investigationTemplate;
    [SerializeField]
    private IEntitySerializable dropTemplate;
    [SerializeField]
    private GameObject characterPicture;
    private IStorageComponent investigationTableStorage;
    private IStorageComponent dropTableStorage;

    protected override bool ValidateContainer(
      IInventoryComponent container,
      IStorageComponent storage)
    {
      return base.ValidateContainer(container, storage);
    }

    protected override void DragEnd(Intersect intersect)
    {
      if (!drag.IsEnabled || !intersect.IsIntersected)
        return;
      base.DragEnd(intersect);
    }

    protected override void Subscribe()
    {
      base.Subscribe();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.LTrigger, OnLeftTrigger);
    }

    protected override void Unsubscribe()
    {
      base.Unsubscribe();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.LTrigger, OnLeftTrigger);
    }

    private bool OnLeftTrigger(GameActionType type, bool down)
    {
      if (!down)
        return false;
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player != null)
        InventorySorter.Sort(player.GetComponent<StorageComponent>());
      actors.Clear();
      actors.Add(Actor);
      CreateContainers();
      actors.Add(investigationTableStorage);
      actors.Add(dropTableStorage);
      Build2();
      GetFirstStorable();
      return true;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.GenericPlayerMenu, new GameActionHandle(((UIWindow) this).CancelListener), true);
      InstanceByRequest<EngineApplication>.Instance.IsPaused = true;
      actors.Clear();
      actors.Add(Actor);
      CreateContainers();
      actors.Add(investigationTableStorage);
      actors.Add(dropTableStorage);
      Build2();
    }

    protected override void OnDisable()
    {
      DestroyContainers();
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Inventory, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.GenericPlayerMenu, new GameActionHandle(((UIWindow) this).CancelListener));
      InstanceByRequest<EngineApplication>.Instance.IsPaused = false;
      base.OnDisable();
    }

    protected override void OnJoystick(bool joystick)
    {
      base.OnJoystick(joystick);
      if (joystick)
        ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.Inventory, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
      else
        ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.Inventory, new GameActionHandle(((UIWindow) this).WithoutJoystickCancelListener));
    }

    private void CreateContainers()
    {
      IEntity template1 = investigationTemplate.Value;
      IEntity entity1 = ServiceLocator.GetService<IFactory>().Instantiate(template1);
      entity1.Name = "InvestigationTable";
      investigationTableStorage = entity1.GetComponent<IStorageComponent>();
      ServiceLocator.GetService<ISimulation>().Add(entity1, ServiceLocator.GetService<ISimulation>().Others);
      IEntity template2 = dropTemplate.Value;
      IEntity entity2 = ServiceLocator.GetService<IFactory>().Instantiate(template2);
      entity2.Name = "DropTable";
      dropTableStorage = entity2.GetComponent<IStorageComponent>();
      ServiceLocator.GetService<ISimulation>().Add(entity2, ServiceLocator.GetService<ISimulation>().Others);
    }

    private void DestroyContainers()
    {
      investigationTableStorage.Owner.Dispose();
      investigationTableStorage = null;
      dropTableStorage.Owner.Dispose();
      dropTableStorage = null;
    }

    protected override bool ValidateComputeActor(IStorageComponent actor)
    {
      return base.ValidateComputeActor(actor) || actor == investigationTableStorage || actor == dropTableStorage;
    }

    public override void Initialize()
    {
      RegisterLayer<IInventoryWindow>(this);
      base.Initialize();
    }

    public override Type GetWindowType() => typeof (IInventoryWindow);

    protected override void HideInfoWindow() => base.HideInfoWindow();

    public override IEnumerator OnOpened()
    {
      SimplePlayerWindowSwapper.SetLastOpenedPlayerWindow<IInventoryWindow>(this);
      return base.OnOpened();
    }

    public override bool IsWindowAvailable
    {
      get => !ServiceLocator.GetService<InterfaceBlockingService>().BlockInventoryInterface;
    }
  }
}
