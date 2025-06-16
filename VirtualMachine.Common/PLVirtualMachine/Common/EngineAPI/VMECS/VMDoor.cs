// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMDoor
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Components.Parameters;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Gate", typeof (IDoorComponent))]
  [Depended("Interactive")]
  public class VMDoor : VMEngineComponent<IDoorComponent>
  {
    public const string ComponentName = "Gate";
    private PriorityParameterEnum defaultPriority = PriorityParameterEnum.Default;

    [Property("IsFree", "")]
    public bool IsFree
    {
      get => this.Component.IsFree.Value;
      set => this.Component.IsFree.SetValue(this.defaultPriority, value);
    }

    [Method("Set IsFree value", "Priority,Value", "")]
    public void SetIsFreeValue(PriorityParameterEnum priority, bool value)
    {
      this.Component.IsFree.SetValue(priority, value);
    }

    [Method("Reset IsFree value", "Priority", "")]
    public void ResetIsFreeValue(PriorityParameterEnum priority)
    {
      this.Component.IsFree.ResetValue(priority);
    }

    [Property("Bolted", "")]
    public bool Bolted
    {
      get => this.Component.Bolted.Value;
      set => this.Component.Bolted.SetValue(this.defaultPriority, value);
    }

    [Method("Set Bolted value", "Priority,Value", "")]
    public void SetBoltedValue(PriorityParameterEnum priority, bool value)
    {
      this.Component.Bolted.SetValue(priority, value);
    }

    [Method("Reset Bolted value", "Priority", "")]
    public void ResetBoltedValue(PriorityParameterEnum priority)
    {
      this.Component.Bolted.ResetValue(priority);
    }

    [Property("Marked", "")]
    public bool Marked
    {
      get => this.Component.Marked.Value;
      set => this.Component.Marked.SetValue(this.defaultPriority, value);
    }

    [Property("CanBeMarked", "")]
    public bool CanBeMarked
    {
      get => this.Component.CanBeMarked.Value;
      set => this.Component.CanBeMarked.Value = value;
    }

    [Property("Knockable", "")]
    public bool Knockable
    {
      get => this.Component.Knockable.Value;
      set => this.Component.Knockable.Value = value;
    }

    [Property("Pickable", "")]
    public bool Pickable
    {
      get => this.Component.Pickable.Value;
      set => this.Component.Pickable.Value = value;
    }

    [Property("Difficulty", "")]
    public int Difficulty
    {
      get => this.Component.Difficulty.Value;
      set => this.Component.Difficulty.Value = value;
    }

    [Method("Set Marked value", "Priority,Value", "")]
    public void SetMarkedValue(PriorityParameterEnum priority, bool value)
    {
      this.Component.Marked.SetValue(priority, value);
    }

    [Method("Reset Marked value", "Priority", "")]
    public void ResetMarkedValue(PriorityParameterEnum priority)
    {
      this.Component.Marked.ResetValue(priority);
    }

    [Property("Opened", "")]
    public bool Opened
    {
      get => this.Component.Opened.Value;
      set => this.Component.Opened.SetValue(this.defaultPriority, value);
    }

    [Method("Set Opened value", "Priority,Value", "")]
    public void SetOpenedValue(PriorityParameterEnum priority, bool value)
    {
      this.Component.Opened.SetValue(priority, value);
    }

    [Method("Set Opened value by type", "Priority,Value,is outdoor", "")]
    public void SetOpenedValueByType(PriorityParameterEnum priority, bool value, bool isOutdoor)
    {
      if (this.Component.IsOutdoor != isOutdoor)
        return;
      this.Component.Opened.SetValue(priority, value);
    }

    [Method("Reset Opened value", "Priority", "")]
    public void ResetOpenedValue(PriorityParameterEnum priority)
    {
      this.Component.Opened.ResetValue(priority);
    }

    [Property("Lock State", "")]
    public LockState LockState
    {
      get => this.Component.LockState.Value;
      set => this.Component.LockState.Value = value;
    }

    [Method("Set LockState value", "Priority,Value", "")]
    public void SetLockStateValue(PriorityParameterEnum priority, LockState value)
    {
      this.Component.LockState.SetValue(priority, value);
    }

    [Method("Set LockState value by type", "Priority,Value,is outdoor", "")]
    public void SetLockStateValueByType(
      PriorityParameterEnum priority,
      LockState value,
      bool isOutdoor)
    {
      if (this.Component.IsOutdoor != isOutdoor)
        return;
      this.Component.LockState.SetValue(priority, value);
    }

    [Method("Reset LockState value", "Priority", "")]
    public void ResetLockStateValue(PriorityParameterEnum priority)
    {
      this.Component.LockState.ResetValue(priority);
    }

    [Property("Min Reputation", "")]
    public float MinReputation
    {
      get => this.Component.MinReputation;
      set => this.Component.MinReputation = value;
    }

    [Property("Max Reputation", "")]
    public float MaxReputation
    {
      get => this.Component.MaxReputation;
      set => this.Component.MaxReputation = value;
    }

    [Property("Is Outdoor", "")]
    public bool IsOutdoor => this.Component.IsOutdoor;

    [Method("Add lock pick", "Storable", "")]
    public void AddPicklock([Template] IEntity storable)
    {
      if (storable == null)
        Logger.AddError(string.Format("Storable template for Picklock adding not defined at {0} !", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      else
        this.Component.AddPicklock(storable);
    }

    [Method("Remove lock pick", "Storable", "")]
    public void RemoveLockPick([Template] IEntity storable)
    {
      if (storable == null)
        Logger.AddError(string.Format("Storable template for Picklock removing not defined at {0} !", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      else
        this.Component.RemovePicklock(storable);
    }

    [Method("Add picklock", "Priority,Storable", "")]
    public void AddPicklock_v1(PriorityParameterEnum priority, [Template] IEntity storable)
    {
      if (storable == null)
        Logger.AddError(string.Format("Storable template for Picklock adding not defined at {0} !", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      else
        this.Component.AddPicklock(priority, storable);
    }

    [Method("Remove picklock", "Priority,Storable", "")]
    public void RemovePicklock(PriorityParameterEnum priority, [Template] IEntity storable)
    {
      if (storable == null)
        Logger.AddError(string.Format("Storable template for Picklock removing not defined at {0} !", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      else
        this.Component.RemovePicklock(priority, storable);
    }

    [Method("Reset picklocks", "Priority", "")]
    public void ResetPicklocks(PriorityParameterEnum priority)
    {
      this.Component.ResetPicklocks(priority);
    }

    [Method("Add key", "Storable", "")]
    public void AddKey([Template] IEntity storable) => this.Component.AddKey(storable);

    [Method("Remove key", "Storable", "")]
    public void RemoveKey([Template] IEntity storable) => this.Component.RemoveKey(storable);

    [Method("Add key", "Priority,Storable", "")]
    public void AddKey_v1(PriorityParameterEnum priority, [Template] IEntity storable)
    {
      this.Component.AddKey(priority, storable);
    }

    [Method("Remove key", "Priority,Storable", "")]
    public void RemoveKey_v1(PriorityParameterEnum priority, [Template] IEntity storable)
    {
      this.Component.RemoveKey(priority, storable);
    }

    [Method("Reset keys", "Priority", "")]
    public void ResetKeys(PriorityParameterEnum priority) => this.Component.ResetKeys(priority);

    public void SetDefaultPriority(PriorityParameterEnum defPriority)
    {
      this.defaultPriority = defPriority;
    }
  }
}
