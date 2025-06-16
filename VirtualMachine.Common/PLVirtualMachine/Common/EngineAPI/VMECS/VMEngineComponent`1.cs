// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMEngineComponent`1
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Engine.Common;

#nullable disable
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
        Logger.AddError(string.Format("Engine component not defined, perhaps need special vm component initialization, type : {0}", (object) this.GetType()));
      this.Init();
    }

    public T TemplateComponent
    {
      get
      {
        if ((object) this.component == null)
        {
          Logger.AddError(string.Format("Entity {0} component {1} instance is null at {2}", (object) this.OwnerTemplateName, (object) this.GetType(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          return default (T);
        }
        if (this.component.Owner == null)
        {
          Logger.AddError(string.Format("Entity {0} component {1} owner instance is null at {2}", (object) this.OwnerTemplateName, (object) this.GetType(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          return this.component;
        }
        if (!this.component.Owner.IsDisposed)
          return this.component;
        Logger.AddError("Entity is disposed , type : " + (object) this.GetType() + " , id : " + (object) this.component.Owner.Id + " , template id : " + (object) this.component.Owner.TemplateId);
        return this.component;
      }
    }

    public T Component
    {
      get
      {
        if ((object) this.component == null)
        {
          Logger.AddError(string.Format("Entity {0} component {1} instance is null at {2}", (object) this.OwnerTemplateName, (object) this.GetType(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          return default (T);
        }
        if (this.component.Owner == null)
        {
          Logger.AddError(string.Format("Entity {0} component {1} owner instance is null at {2}", (object) this.OwnerTemplateName, (object) this.GetType(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          return this.component;
        }
        if (this.component.Owner.IsDisposed)
        {
          Logger.AddError("Entity is disposed , type : " + (object) this.GetType() + " , id : " + (object) this.component.Owner.Id + " , template id : " + (object) this.component.Owner.TemplateId);
          return this.component;
        }
        if (!this.component.Owner.IsTemplate)
          return this.component;
        Logger.AddError("Entity is template , type : " + (object) this.GetType() + " , id : " + (object) this.component.Owner.Id + " , template id : " + (object) this.component.Owner.TemplateId);
        return this.component;
      }
    }

    public override void Clear() => this.component = default (T);

    public bool IsTemplate => this.TemplateComponent.Owner.IsTemplate;

    protected override bool InstanceValid
    {
      get => (object) this.component != null && this.component.Owner != null;
    }

    protected virtual void Init()
    {
    }

    protected string OwnerTemplateName
    {
      get
      {
        string ownerTemplateName = "none";
        if (this.Parent != null)
          ownerTemplateName = this.Parent.Name;
        return ownerTemplateName;
      }
    }
  }
}
