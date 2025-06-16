// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.DynamicObject
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using PLVirtualMachine.Common;
using System;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public abstract class DynamicObject : IRealTimeModifiable
  {
    protected VMEntity entity;
    private IObject staticObject;
    protected bool active;
    protected bool modified;

    public DynamicObject(VMEntity entity, bool bActive = true)
    {
      this.entity = entity;
      this.active = bActive;
    }

    public abstract void Think();

    public void InitStatic(IObject staticObject)
    {
      if (staticObject == null)
        Logger.AddError("Invalid dvirtual machine object creation: null static object received");
      this.staticObject = staticObject;
    }

    public virtual bool Active
    {
      get => this.active;
      set => this.active = value;
    }

    public VMEntity Entity => this.entity;

    public IObject StaticObject => this.staticObject;

    public Guid DynamicGuid => this.Entity == null ? Guid.Empty : this.entity.EngineGuid;

    public ulong StaticGuid => this.staticObject == null ? 0UL : this.staticObject.BaseGuid;

    public virtual void OnModify()
    {
      this.modified = true;
      if (this.ModifiableParent == null)
        return;
      this.ModifiableParent.OnModify();
    }

    public bool Modified => this.modified;

    public IRealTimeModifiable ModifiableParent => (IRealTimeModifiable) this.entity;
  }
}
