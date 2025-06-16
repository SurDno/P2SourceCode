using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Maps;
using Engine.Common.MindMap;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("MapItemComponent", typeof (IMapItemComponent))]
  public class VMMapItem : VMEngineComponent<IMapItemComponent>
  {
    public const string ComponentName = "MapItemComponent";
    private ITextRef title;
    private ITextRef text;
    private ITextRef tooltipText;

    public VMMapItem()
    {
      if (!(GetType() == typeof (VMMapItem)))
        return;
      Logger.AddError("!!!");
    }

    [Property("Title", "", true)]
    public ITextRef Title
    {
      get => title;
      set
      {
        if (Component == null)
        {
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        }
        else
        {
          title = value;
          Component.Title = EngineAPIManager.CreateEngineTextInstance(title);
        }
      }
    }

    [Property("Text", "", true)]
    public ITextRef Text
    {
      get => text;
      set
      {
        if (Component == null)
        {
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        }
        else
        {
          text = value;
          Component.Text = EngineAPIManager.CreateEngineTextInstance(text);
        }
      }
    }

    [Property("TooltipText", "", true)]
    public ITextRef TooltipText
    {
      get => tooltipText;
      set
      {
        if (Component == null)
        {
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        }
        else
        {
          tooltipText = value;
          Component.TooltipText = EngineAPIManager.CreateEngineTextInstance(tooltipText);
        }
      }
    }

    [Property("Placeholder", "", false)]
    public IMapPlaceholder Placeholder
    {
      get => Component == null ? null : Component.Resource;
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Resource = value;
      }
    }

    [Property("TooltipResource", "", false)]
    public IMapTooltipResource TooltipResource
    {
      get => Component == null ? null : Component.TooltipResource;
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.TooltipResource = value;
      }
    }

    [Property("Enabled", "", false, true)]
    public bool EnableMapItem
    {
      get
      {
        if (Component != null)
          return Component.IsEnabled;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.IsEnabled = value;
      }
    }

    [Property("Bound health state", "", false, BoundHealthStateEnum.None, false)]
    public BoundHealthStateEnum BoundHealthState
    {
      get
      {
        if (Component != null)
          return Component.BoundHealthState.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return BoundHealthStateEnum.None;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.BoundHealthState.Value = value;
      }
    }

    [Property("Discovered", "", false, false)]
    public bool Discovered
    {
      get => Component.Discovered;
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.Discovered = value;
      }
    }

    [Property("SavePointIcon", "", false, false)]
    public bool SavePointIcon
    {
      get
      {
        if (Component != null)
          return Component.SavePointIcon.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.SavePointIcon.Value = value;
      }
    }

    [Property("SleepIcon", "", false, false)]
    public bool SleepIcon
    {
      get
      {
        if (Component != null)
          return Component.SleepIcon.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.SleepIcon.Value = value;
      }
    }

    [Property("CraftIcon", "", false, false)]
    public bool CraftIcon
    {
      get
      {
        if (Component != null)
          return Component.CraftIcon.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.CraftIcon.Value = value;
      }
    }

    [Property("StorageIcon", "", false, false)]
    public bool StorageIcon
    {
      get
      {
        if (Component != null)
          return Component.StorageIcon.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.StorageIcon.Value = value;
      }
    }

    [Property("MerchantIcon", "", false, false)]
    public bool MerchantIcon
    {
      get
      {
        if (Component != null)
          return Component.MerchantIcon.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return false;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.MerchantIcon.Value = value;
      }
    }

    [Property("BoundCharacter", "", false)]
    public IEntity BoundCharacter
    {
      get
      {
        if (Component != null)
          return Component.BoundCharacter;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        return null;
      }
      set
      {
        if (Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
        else
          Component.BoundCharacter = value;
      }
    }

    [Method("Add MMNode", "MMNode", "")]
    public void AddNode(IMMNode node)
    {
      if (Component == null)
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
      else
        Component.AddNode(node);
    }

    [Method("Remove MMNode", "MMNode", "")]
    public void RemoveNode(IMMNode node)
    {
      if (Component == null)
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
      else
        Component.RemoveNode(node);
    }

    [Method("Clear nodes", "", "")]
    public void ClearNodes()
    {
      if (Component == null)
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
      else
        Component.ClearNodes();
    }
  }
}
