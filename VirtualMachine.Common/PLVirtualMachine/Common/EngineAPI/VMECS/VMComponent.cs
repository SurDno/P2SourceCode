using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.Serialization;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

public class VMComponent :
	ISerializeStateSave,
	IDynamicLoadSerializable,
	IRealTimeModifiable,
	INeedSave {
	[Serializable] private string engineData = "";
	private VMBaseEntity parentEntity;
	private bool isModified;

	public virtual void Initialize(VMBaseEntity parent) {
		parentEntity = parent;
	}

	public virtual void Initialize(VMBaseEntity parent, IComponent component) {
		Initialize(parent);
	}

	public VMBaseEntity Parent => parentEntity;

	public string EngineData => engineData;

	public string Name => GetComponentTypeName();

	public virtual void StateSave(IDataWriter writer) {
		SaveManagerUtility.Save(writer, "APIName", Name);
	}

	public virtual void LoadFromXML(XmlElement xmlNode) {
		OnModify();
	}

	public virtual void OnCreate() { }

	public virtual void AfterCreate() { }

	public virtual void AfterSaveLoading() { }

	protected void SetEngineData(string engineData) {
		if (engineData == null)
			return;
		this.engineData = engineData;
	}

	protected virtual bool InstanceValid => true;

	public virtual string GetComponentTypeName() {
		Logger.AddError(TypeUtility.GetTypeName(GetType()));
		var componentTypeName = "";
		var customAttributes = GetType().GetCustomAttributes(typeof(InfoAttribute), true);
		if (customAttributes.Length != 0)
			componentTypeName = ((InfoAttribute)customAttributes[0]).ApiName;
		if ("" == componentTypeName)
			Logger.AddError(string.Format("Component api name for component {0} not defined !", GetType()));
		return componentTypeName;
	}

	public virtual void Clear() { }

	public void OnModify() {
		isModified = true;
		if (ModifiableParent == null)
			return;
		ModifiableParent.OnModify();
	}

	public bool Modified => isModified;

	public IRealTimeModifiable ModifiableParent =>
		Parent != null && typeof(IRealTimeModifiable).IsAssignableFrom(Parent.GetType())
			? (IRealTimeModifiable)Parent
			: null;

	public bool NeedSave => Modified;
}