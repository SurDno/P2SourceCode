using Cofe.Loggers;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.Serialization;
using System.Xml;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  public class VMComponent : 
    ISerializeStateSave,
    IDynamicLoadSerializable,
    IRealTimeModifiable,
    INeedSave
  {
    [Serializable(true, false)]
    private string engineData = "";
    private VMBaseEntity parentEntity;
    private bool isModified;

    public virtual void Initialize(VMBaseEntity parent) => this.parentEntity = parent;

    public virtual void Initialize(VMBaseEntity parent, IComponent component)
    {
      this.Initialize(parent);
    }

    public VMBaseEntity Parent => this.parentEntity;

    public string EngineData => this.engineData;

    public string Name => this.GetComponentTypeName();

    public virtual void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "APIName", this.Name);
    }

    public virtual void LoadFromXML(XmlElement xmlNode) => this.OnModify();

    public virtual void OnCreate()
    {
    }

    public virtual void AfterCreate()
    {
    }

    public virtual void AfterSaveLoading()
    {
    }

    protected void SetEngineData(string engineData)
    {
      if (engineData == null)
        return;
      this.engineData = engineData;
    }

    protected virtual bool InstanceValid => true;

    public virtual string GetComponentTypeName()
    {
      Logger.AddError(TypeUtility.GetTypeName(this.GetType()));
      string componentTypeName = "";
      object[] customAttributes = this.GetType().GetCustomAttributes(typeof (InfoAttribute), true);
      if (customAttributes.Length != 0)
        componentTypeName = ((InfoAttribute) customAttributes[0]).ApiName;
      if ("" == componentTypeName)
        Logger.AddError(string.Format("Component api name for component {0} not defined !", (object) this.GetType()));
      return componentTypeName;
    }

    public virtual void Clear()
    {
    }

    public void OnModify()
    {
      this.isModified = true;
      if (this.ModifiableParent == null)
        return;
      this.ModifiableParent.OnModify();
    }

    public bool Modified => this.isModified;

    public IRealTimeModifiable ModifiableParent
    {
      get
      {
        return this.Parent != null && typeof (IRealTimeModifiable).IsAssignableFrom(this.Parent.GetType()) ? (IRealTimeModifiable) this.Parent : (IRealTimeModifiable) null;
      }
    }

    public bool NeedSave => this.Modified;
  }
}
