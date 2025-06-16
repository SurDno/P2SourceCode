using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Base
{
  public abstract class VMBaseObject : 
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable
  {
    private ulong guid;
    [FieldData("Name", DataFieldType.None)]
    protected string name = "";
    [FieldData("Parent", DataFieldType.Reference)]
    protected IContainer parent;

    public VMBaseObject(ulong guid) => this.guid = guid;

    public abstract EObjectCategory GetCategory();

    public bool IsVirtual => this.guid == 0UL;

    public string Name => this.name;

    public ulong BaseGuid => this.guid;

    public virtual string GuidStr => this.BaseGuid.ToString();

    public IContainer Parent => this.parent;

    public virtual IContainer Owner => this.Parent;

    public virtual void Update() => this.IsUpdated = true;

    public bool IsUpdated { get; private set; }

    public virtual bool IsEqual(IObject other)
    {
      return other != null && (long) this.BaseGuid == (long) other.BaseGuid;
    }

    public virtual void OnPostLoad()
    {
    }

    public virtual void Clear() => this.parent = (IContainer) null;
  }
}
