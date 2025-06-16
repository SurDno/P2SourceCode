using Cofe.Loggers;
using PLVirtualMachine.Common;
using System;

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
