using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Location", typeof (ILocationComponent))]
  public class VMLocation : VMEngineComponent<ILocationComponent>
  {
    public const string ComponentName = "Location";

    [Event("Hibernation changed", "")]
    public event Action OnHibernationChange;

    [Event("Player inside changed", "is inside")]
    public event Action<bool> OnPlayerInside;

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

    [Property("Player in Location", "", false)]
    public IEntity Player
    {
      get
      {
        if (this.Component != null)
          return this.Component.Player;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return (IEntity) null;
      }
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.OnHibernationChanged -= new Action<ILocationComponent>(this.HibernationChanged);
      this.Component.OnPlayerChanged -= new Action(this.PlayerChanged);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.OnHibernationChanged += new Action<ILocationComponent>(this.HibernationChanged);
      this.Component.OnPlayerChanged += new Action(this.PlayerChanged);
    }

    private void HibernationChanged(ILocationComponent sender)
    {
      Action hibernationChange = this.OnHibernationChange;
      if (hibernationChange == null)
        return;
      hibernationChange();
    }

    private void PlayerChanged()
    {
      if (this.OnPlayerInside == null)
        return;
      if (this.Component.IsDisposed)
      {
        Logger.AddError("Location is disposed");
      }
      else
      {
        string hierarchyPath = this.Component.Owner.HierarchyPath;
        ILocationComponent logicLocation = this.Component.LogicLocation;
        if (logicLocation == null)
        {
          if (this.Component.Owner.Parent == ServiceCache.Simulation.Hierarchy)
            return;
          Logger.AddError("Logic location not found in location : " + hierarchyPath);
        }
        else
        {
          string str = hierarchyPath + (logicLocation.IsIndoor ? " (indoor)" : " (outdoor)");
          IEntity player = this.Component.Player;
          if (player == null)
          {
            Logger.AddInfo("Player exit from location : " + str);
            Action<bool> onPlayerInside = this.OnPlayerInside;
            if (onPlayerInside == null)
              return;
            onPlayerInside(false);
          }
          else
          {
            INavigationComponent component = player.GetComponent<INavigationComponent>();
            if (component == null)
              Logger.AddError("Navigation not found in player : " + player.HierarchyPath + " , in location : " + str);
            else if (!component.HasPrevTeleport)
            {
              Logger.AddInfo("Prev teleport not found, player : " + player.HierarchyPath + " , in location : " + str);
            }
            else
            {
              Logger.AddInfo("Player entry to location : " + str);
              Action<bool> onPlayerInside = this.OnPlayerInside;
              if (onPlayerInside == null)
                return;
              onPlayerInside(true);
            }
          }
        }
      }
    }
  }
}
