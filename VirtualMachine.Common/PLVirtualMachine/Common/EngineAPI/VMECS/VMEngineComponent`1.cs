using Cofe.Loggers;
using Engine.Common;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  public class VMEngineComponent<T> : VMComponent where T : class, IComponent
  {
    private T component;

    public override void Initialize(VMBaseEntity parent) => base.Initialize(parent);

    public override void Initialize(VMBaseEntity parent, IComponent component)
    {
      base.Initialize(parent, component);
      this.component = (T) component;
      if (component == null)
        Logger.AddError(string.Format("Engine component not defined, perhaps need special vm component initialization, type : {0}", GetType()));
      Init();
    }

    public T TemplateComponent
    {
      get
      {
        if (component == null)
        {
          Logger.AddError(string.Format("Entity {0} component {1} instance is null at {2}", OwnerTemplateName, GetType(), EngineAPIManager.Instance.CurrentFSMStateInfo));
          return default (T);
        }
        if (component.Owner == null)
        {
          Logger.AddError(string.Format("Entity {0} component {1} owner instance is null at {2}", OwnerTemplateName, GetType(), EngineAPIManager.Instance.CurrentFSMStateInfo));
          return component;
        }
        if (!component.Owner.IsDisposed)
          return component;
        Logger.AddError("Entity is disposed , type : " + GetType() + " , id : " + component.Owner.Id + " , template id : " + component.Owner.TemplateId);
        return component;
      }
    }

    public T Component
    {
      get
      {
        if (component == null)
        {
          Logger.AddError(string.Format("Entity {0} component {1} instance is null at {2}", OwnerTemplateName, GetType(), EngineAPIManager.Instance.CurrentFSMStateInfo));
          return default (T);
        }
        if (component.Owner == null)
        {
          Logger.AddError(string.Format("Entity {0} component {1} owner instance is null at {2}", OwnerTemplateName, GetType(), EngineAPIManager.Instance.CurrentFSMStateInfo));
          return component;
        }
        if (component.Owner.IsDisposed)
        {
          Logger.AddError("Entity is disposed , type : " + GetType() + " , id : " + component.Owner.Id + " , template id : " + component.Owner.TemplateId);
          return component;
        }
        if (!component.Owner.IsTemplate)
          return component;
        Logger.AddError("Entity is template , type : " + GetType() + " , id : " + component.Owner.Id + " , template id : " + component.Owner.TemplateId);
        return component;
      }
    }

    public override void Clear() => component = default (T);

    public bool IsTemplate => TemplateComponent.Owner.IsTemplate;

    protected override bool InstanceValid => component != null && component.Owner != null;

    protected virtual void Init()
    {
    }

    protected string OwnerTemplateName
    {
      get
      {
        string ownerTemplateName = "none";
        if (Parent != null)
          ownerTemplateName = Parent.Name;
        return ownerTemplateName;
      }
    }
  }
}
