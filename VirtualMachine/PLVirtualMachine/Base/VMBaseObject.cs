using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;

namespace PLVirtualMachine.Base
{
  public abstract class VMBaseObject(ulong guid) :
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable {
    [FieldData("Name")]
    protected string name = "";
    [FieldData("Parent", DataFieldType.Reference)]
    protected IContainer parent;

    public abstract EObjectCategory GetCategory();

    public bool IsVirtual => guid == 0UL;

    public string Name => name;

    public ulong BaseGuid => guid;

    public virtual string GuidStr => BaseGuid.ToString();

    public IContainer Parent => parent;

    public virtual IContainer Owner => Parent;

    public virtual void Update() => IsUpdated = true;

    public bool IsUpdated { get; private set; }

    public virtual bool IsEqual(IObject other)
    {
      return other != null && (long) BaseGuid == (long) other.BaseGuid;
    }

    public virtual void OnPostLoad()
    {
    }

    public virtual void Clear() => parent = null;
  }
}
