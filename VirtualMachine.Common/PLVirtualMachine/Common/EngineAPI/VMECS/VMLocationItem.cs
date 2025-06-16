using System;
using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("LocationItem", typeof (ILocationItemComponent))]
  public class VMLocationItem : VMEngineComponent<ILocationItemComponent>
  {
    public const string ComponentName = "LocationItem";

    [Event("Hibernation changed", "")]
    public event Action OnChangeHibernation;

    [Event("Location changed", "Location")]
    public event Action<IEntity> OnChangeLocation;

    [Property("Is hibernation", "", false)]
    public bool IsHibernation
    {
      get
      {
        if (Component != null)
          return Component.IsHibernation;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
    }

    [Property("Is indoor", "", false)]
    public bool IsIndoor
    {
      get
      {
        if (Component != null)
          return Component.IsIndoor;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
    }

    [Property("Location", "", false)]
    public IEntity Location
    {
      get
      {
        if (Component == null)
        {
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
          return null;
        }
        return Component.LogicLocation == null ? null : Component.LogicLocation.Owner;
      }
    }

    public override void Clear()
    {
      if (!InstanceValid)
        return;
      Component.OnHibernationChanged -= FireHibernationChanged;
      Component.OnChangeLocation -= FireChangeLocation;
      base.Clear();
    }

    protected override void Init()
    {
      if (IsTemplate)
        return;
      Component.OnHibernationChanged += FireHibernationChanged;
      Component.OnChangeLocation += FireChangeLocation;
    }

    private void FireHibernationChanged(ILocationItemComponent sender)
    {
      Action changeHibernation = OnChangeHibernation;
      if (changeHibernation == null)
        return;
      changeHibernation();
    }

    private void FireChangeLocation(ILocationItemComponent sender, ILocationComponent location)
    {
      Action<IEntity> onChangeLocation = OnChangeLocation;
      if (onChangeLocation == null)
        return;
      onChangeLocation(location?.Owner);
    }
  }
}
