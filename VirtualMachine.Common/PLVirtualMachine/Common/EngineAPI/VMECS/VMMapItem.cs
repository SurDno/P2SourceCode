// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMMapItem
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Maps;
using Engine.Common.MindMap;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

#nullable disable
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
      if (!(this.GetType() == typeof (VMMapItem)))
        return;
      Logger.AddError("!!!");
    }

    [Property("Title", "", true)]
    public ITextRef Title
    {
      get => this.title;
      set
      {
        if (this.Component == null)
        {
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        }
        else
        {
          this.title = value;
          this.Component.Title = EngineAPIManager.CreateEngineTextInstance(this.title);
        }
      }
    }

    [Property("Text", "", true)]
    public ITextRef Text
    {
      get => this.text;
      set
      {
        if (this.Component == null)
        {
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        }
        else
        {
          this.text = value;
          this.Component.Text = EngineAPIManager.CreateEngineTextInstance(this.text);
        }
      }
    }

    [Property("TooltipText", "", true)]
    public ITextRef TooltipText
    {
      get => this.tooltipText;
      set
      {
        if (this.Component == null)
        {
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        }
        else
        {
          this.tooltipText = value;
          this.Component.TooltipText = EngineAPIManager.CreateEngineTextInstance(this.tooltipText);
        }
      }
    }

    [Property("Placeholder", "", false)]
    public IMapPlaceholder Placeholder
    {
      get => this.Component == null ? (IMapPlaceholder) null : this.Component.Resource;
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Resource = value;
      }
    }

    [Property("TooltipResource", "", false)]
    public IMapTooltipResource TooltipResource
    {
      get => this.Component == null ? (IMapTooltipResource) null : this.Component.TooltipResource;
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.TooltipResource = value;
      }
    }

    [Property("Enabled", "", false, true)]
    public bool EnableMapItem
    {
      get
      {
        if (this.Component != null)
          return this.Component.IsEnabled;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.IsEnabled = value;
      }
    }

    [Property("Bound health state", "", false, BoundHealthStateEnum.None, false)]
    public BoundHealthStateEnum BoundHealthState
    {
      get
      {
        if (this.Component != null)
          return this.Component.BoundHealthState.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return BoundHealthStateEnum.None;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.BoundHealthState.Value = value;
      }
    }

    [Property("Discovered", "", false, false)]
    public bool Discovered
    {
      get => this.Component.Discovered;
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.Discovered = value;
      }
    }

    [Property("SavePointIcon", "", false, false)]
    public bool SavePointIcon
    {
      get
      {
        if (this.Component != null)
          return this.Component.SavePointIcon.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.SavePointIcon.Value = value;
      }
    }

    [Property("SleepIcon", "", false, false)]
    public bool SleepIcon
    {
      get
      {
        if (this.Component != null)
          return this.Component.SleepIcon.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.SleepIcon.Value = value;
      }
    }

    [Property("CraftIcon", "", false, false)]
    public bool CraftIcon
    {
      get
      {
        if (this.Component != null)
          return this.Component.CraftIcon.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.CraftIcon.Value = value;
      }
    }

    [Property("StorageIcon", "", false, false)]
    public bool StorageIcon
    {
      get
      {
        if (this.Component != null)
          return this.Component.StorageIcon.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.StorageIcon.Value = value;
      }
    }

    [Property("MerchantIcon", "", false, false)]
    public bool MerchantIcon
    {
      get
      {
        if (this.Component != null)
          return this.Component.MerchantIcon.Value;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return false;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.MerchantIcon.Value = value;
      }
    }

    [Property("BoundCharacter", "", false)]
    public IEntity BoundCharacter
    {
      get
      {
        if (this.Component != null)
          return this.Component.BoundCharacter;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return (IEntity) null;
      }
      set
      {
        if (this.Component == null)
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        else
          this.Component.BoundCharacter = value;
      }
    }

    [Method("Add MMNode", "MMNode", "")]
    public void AddNode(IMMNode node)
    {
      if (this.Component == null)
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
      else
        this.Component.AddNode(node);
    }

    [Method("Remove MMNode", "MMNode", "")]
    public void RemoveNode(IMMNode node)
    {
      if (this.Component == null)
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
      else
        this.Component.RemoveNode(node);
    }

    [Method("Clear nodes", "", "")]
    public void ClearNodes()
    {
      if (this.Component == null)
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
      else
        this.Component.ClearNodes();
    }
  }
}
