using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

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
        if (this.Component != null)
          return this.Component.IsHibernation;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
    }

    [Property("Is indoor", "", false)]
    public bool IsIndoor
    {
      get
      {
        if (this.Component != null)
          return this.Component.IsIndoor;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
    }

    [Property("Location", "", false)]
    public IEntity Location
    {
      get
      {
        if (this.Component == null)
        {
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
          return (IEntity) null;
        }
        return this.Component.LogicLocation == null ? (IEntity) null : this.Component.LogicLocation.Owner;
      }
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.OnHibernationChanged -= new Action<ILocationItemComponent>(this.FireHibernationChanged);
      this.Component.OnChangeLocation -= new Action<ILocationItemComponent, ILocationComponent>(this.FireChangeLocation);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.OnHibernationChanged += new Action<ILocationItemComponent>(this.FireHibernationChanged);
      this.Component.OnChangeLocation += new Action<ILocationItemComponent, ILocationComponent>(this.FireChangeLocation);
    }

    private void FireHibernationChanged(ILocationItemComponent sender)
    {
      Action changeHibernation = this.OnChangeHibernation;
      if (changeHibernation == null)
        return;
      changeHibernation();
    }

    private void FireChangeLocation(ILocationItemComponent sender, ILocationComponent location)
    {
      Action<IEntity> onChangeLocation = this.OnChangeLocation;
      if (onChangeLocation == null)
        return;
      onChangeLocation(location?.Owner);
    }
  }
}
