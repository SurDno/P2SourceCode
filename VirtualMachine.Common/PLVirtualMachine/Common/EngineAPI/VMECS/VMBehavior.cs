// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMBehavior
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using System;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("BehaviorComponent", typeof (IBehaviorComponent))]
  public class VMBehavior : VMEngineComponent<IBehaviorComponent>
  {
    public const string ComponentName = "BehaviorComponent";

    [Event("Behavior Success", "")]
    public event Action Success;

    [Event("Behavior Fail", "")]
    public event Action Fail;

    [Event("Behavior Custom", "value")]
    public event Action<string> Custom;

    [Method("SetValue", "name,object", "")]
    public void SetValue(string name, IEntity value) => this.Component.SetValue(name, value);

    [Method("SetBoolValue", "name,value", "")]
    public void SetBoolValue(string name, bool value) => this.Component.SetBoolValue(name, value);

    [Method("SetIntValue", "name,value", "")]
    public void SetIntValue(string name, int value) => this.Component.SetIntValue(name, value);

    [Method("SetFloatValue", "name,value", "")]
    public void SetFloatValue(string name, float value)
    {
      this.Component.SetFloatValue(name, value);
    }

    [Method("SetBehaviorForced", "behavior", "")]
    public void SetBehaviorForced(IBehaviorObject behavior)
    {
      this.Component.SetBehaviorForced(behavior);
    }

    [Property("BehaviorObject", "", false)]
    public IBehaviorObject Behavior
    {
      get
      {
        if (this.Component != null)
          return this.Component.BehaviorObject;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return (IBehaviorObject) null;
      }
      set
      {
        if (this.Component == null)
        {
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        }
        else
        {
          try
          {
            this.Component.BehaviorObject = value;
          }
          catch (Exception ex)
          {
            Logger.AddError(string.Format("Behavior object set error: {0} at {1}", (object) ex.ToString(), (object) this.Parent.Name));
          }
        }
      }
    }

    [Property("BehaviorObjectForced", "", false)]
    public IBehaviorObject BehaviorForced
    {
      get
      {
        if (this.Component != null)
          return this.Component.BehaviorObjectForced;
        Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        return (IBehaviorObject) null;
      }
      set
      {
        if (this.Component == null)
        {
          Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", (object) this.Name, (object) this.Parent.Name));
        }
        else
        {
          try
          {
            this.Component.BehaviorObjectForced = value;
          }
          catch (Exception ex)
          {
            Logger.AddError(string.Format("Behavior object set error: {0} at {1}", (object) ex.ToString(), (object) this.Parent.Name));
          }
        }
      }
    }

    public override void Clear()
    {
      if (!this.InstanceValid)
        return;
      this.Component.SuccessEvent -= new Action<IBehaviorComponent>(this.SuccessEvent);
      this.Component.FailEvent -= new Action<IBehaviorComponent>(this.FailEvent);
      this.Component.CustomEvent -= new Action<IBehaviorComponent, string>(this.CustomEvent);
      base.Clear();
    }

    protected override void Init()
    {
      if (this.IsTemplate)
        return;
      this.Component.SuccessEvent += new Action<IBehaviorComponent>(this.SuccessEvent);
      this.Component.FailEvent += new Action<IBehaviorComponent>(this.FailEvent);
      this.Component.CustomEvent += new Action<IBehaviorComponent, string>(this.CustomEvent);
    }

    private void SuccessEvent(IBehaviorComponent target)
    {
      Action success = this.Success;
      if (success == null)
        return;
      success();
    }

    private void FailEvent(IBehaviorComponent target)
    {
      Action fail = this.Fail;
      if (fail == null)
        return;
      fail();
    }

    private void CustomEvent(IBehaviorComponent target, string message)
    {
      Action<string> custom = this.Custom;
      if (custom == null)
        return;
      custom(message);
    }
  }
}
