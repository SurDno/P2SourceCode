// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMBoundCharacterComponent
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common;
using Engine.Common.BoundCharacters;
using Engine.Common.Commons;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("BoundCharacterComponent", typeof (IBoundCharacterComponent))]
  public class VMBoundCharacterComponent : VMEngineComponent<IBoundCharacterComponent>
  {
    public const string ComponentName = "BoundCharacterComponent";
    private ITextRef boundCharacterName;

    [Property("Bound health state ", "")]
    public BoundHealthStateEnum BoundHealthState
    {
      get => this.Component.BoundHealthState.Value;
      set
      {
        try
        {
          this.Component.BoundHealthState.Value = value;
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("BoundHealthState set error: {0} at {1}", (object) ex.ToString(), (object) this.Parent.Name));
        }
      }
    }

    [Property("Group", "")]
    public BoundCharacterGroup Group
    {
      get => this.Component.Group;
      set
      {
        try
        {
          this.Component.Group = value;
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Group set error: {0} at {1}", (object) ex.ToString(), (object) this.Parent.Name));
        }
      }
    }

    [Property("Is Discovered", "", false, false)]
    public bool Discovered
    {
      get => this.Component.Discovered;
      set
      {
        try
        {
          this.Component.Discovered = value;
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Discovered set error: {0} at {1}", (object) ex.ToString(), (object) this.Parent.Name));
        }
      }
    }

    [Property("Is enabled", "", false, false)]
    public bool IsEnabled
    {
      get => this.Component.IsEnabled;
      set
      {
        try
        {
          this.Component.IsEnabled = value;
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("IsEnabled set error: {0} at {1}", (object) ex.ToString(), (object) this.Parent.Name));
        }
      }
    }

    [Property("Name", "", false)]
    public ITextRef NameText
    {
      get => this.boundCharacterName;
      set
      {
        this.boundCharacterName = value;
        this.Component.Name = EngineAPIManager.CreateEngineTextInstance(this.boundCharacterName);
      }
    }

    [Property("Random Roll", "")]
    public float RandomRoll
    {
      get => this.Component.RandomRoll.Value;
      set
      {
        try
        {
          this.Component.RandomRoll.Value = value;
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Random Roll set error: {0} at {1}", (object) ex.ToString(), (object) this.Parent.Name));
        }
      }
    }

    [Property("Sort order", "", false)]
    public int SortOrder
    {
      get => this.Component.SortOrder;
      set
      {
        try
        {
          this.Component.SortOrder = value;
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("SortOrder set error: {0} at {1}", (object) ex.ToString(), (object) this.Parent.Name));
        }
      }
    }

    [Property("Resource", "", false)]
    public IBoundCharacterPlaceholder Object
    {
      get => this.Component.Resource;
      set
      {
        try
        {
          this.Component.Resource = value;
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Resource set error: {0} at {1}", (object) ex.ToString(), (object) this.Parent.Name));
        }
      }
    }

    [Property("Home Region", "", false)]
    public IEntity HomeRegion
    {
      get => this.Component.HomeRegion;
      set
      {
        try
        {
          this.Component.HomeRegion = value;
        }
        catch (Exception ex)
        {
          Logger.AddError(string.Format("Home Region set error: {0} at {1}", (object) ex.ToString(), (object) this.Parent.Name));
        }
      }
    }

    [Method("Store Pre Roll State", "", "")]
    public void StorePreRollState()
    {
      if (this.Component == null)
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
      else
        this.Component.StorePreRollState();
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.BoundHealthState.ChangeValueEvent -= new Action<BoundHealthStateEnum>(this.ChangeBoundHealthStateEvent);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.BoundHealthState.ChangeValueEvent += new Action<BoundHealthStateEnum>(this.ChangeBoundHealthStateEvent);
    }

    private void ChangeBoundHealthStateEvent(BoundHealthStateEnum value)
    {
      Action<BoundHealthStateEnum> boundHealthState = this.OnChangeBoundHealthState;
      if (boundHealthState == null)
        return;
      boundHealthState(value);
    }

    [Event("Change bound health state", "Value")]
    public event Action<BoundHealthStateEnum> OnChangeBoundHealthState;
  }
}
