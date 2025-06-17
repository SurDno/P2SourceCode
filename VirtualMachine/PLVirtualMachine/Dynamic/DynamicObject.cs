using System;
using Cofe.Loggers;
using PLVirtualMachine.Common;

namespace PLVirtualMachine.Dynamic
{
  public abstract class DynamicObject(VMEntity entity, bool bActive = true) : IRealTimeModifiable 
  {
    protected VMEntity entity = entity;
    private IObject staticObject;
    protected bool active = bActive;
    protected bool modified;

    public abstract void Think();

    public void InitStatic(IObject staticObject)
    {
      if (staticObject == null)
        Logger.AddError("Invalid dvirtual machine object creation: null static object received");
      this.staticObject = staticObject;
    }

    public virtual bool Active
    {
      get => active;
      set => active = value;
    }

    public VMEntity Entity => entity;

    public IObject StaticObject => staticObject;

    public Guid DynamicGuid => Entity == null ? Guid.Empty : entity.EngineGuid;

    public ulong StaticGuid => staticObject == null ? 0UL : staticObject.BaseGuid;

    public virtual void OnModify()
    {
      modified = true;
      if (ModifiableParent == null)
        return;
      ModifiableParent.OnModify();
    }

    public bool Modified => modified;

    public IRealTimeModifiable ModifiableParent => entity;
  }
}
