using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.GameLogic;

[TypeData(EDataType.TParameter)]
[DataFactory("Parameter")]
public class VMParameter :
	IStub,
	IEditorDataReader,
	IObject,
	IEditorBaseTemplate,
	IParam,
	IVariable,
	INamed,
	IContext {
	private ulong guid;
	[FieldData("Name")] private string name = "";

	[FieldData("OwnerComponent", DataFieldType.Reference)]
	private IFunctionalComponent ownerComponent;

	[FieldData("Type")] private VMType valueType;
	[FieldData("Value")] private object defValue;
	[FieldData("Implicit")] private bool isImplicit;
	[FieldData("Custom")] private bool isCustom;

	[FieldData("Parent", DataFieldType.Reference)]
	private IObject parent;

	private bool isAfterLoaded;
	private List<string> typeFunctionalList;

	public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext) {
		while (xml.Read()) {
			if (xml.NodeType == XmlNodeType.Element)
				switch (xml.Name) {
					case "Custom":
						isCustom = EditorDataReadUtility.ReadValue(xml, isCustom);
						continue;
					case "Implicit":
						isImplicit = EditorDataReadUtility.ReadValue(xml, isImplicit);
						continue;
					case "Name":
						name = EditorDataReadUtility.ReadValue(xml, name);
						continue;
					case "OwnerComponent":
						ownerComponent = EditorDataReadUtility.ReadReference<IFunctionalComponent>(xml, creator);
						continue;
					case "Parent":
						parent = EditorDataReadUtility.ReadReference<IObject>(xml, creator);
						continue;
					case "Type":
						valueType = EditorDataReadUtility.ReadTypeSerializable(xml);
						continue;
					case "Value":
						defValue = EditorDataReadUtility.ReadObjectValue(xml);
						continue;
					default:
						if (XMLDataLoader.Logs.Add(typeContext + " : " + xml.Name))
							Logger.AddError(typeContext + " : " + xml.Name);
						XmlReaderUtility.SkipNode(xml);
						continue;
				}

			if (xml.NodeType == XmlNodeType.EndElement)
				break;
		}
	}

	public VMParameter(ulong guid) {
		this.guid = guid;
	}

	public ulong BaseGuid => guid;

	public virtual bool IsEqual(IVariable other) {
		return other != null && typeof(VMParameter) == other.GetType() &&
		       (long)BaseGuid == (long)((VMParameter)other).BaseGuid;
	}

	public virtual bool IsEqual(IObject other) {
		return other != null && typeof(VMParameter) == other.GetType() &&
		       (long)BaseGuid == (long)((VMParameter)other).BaseGuid;
	}

	public EContextVariableCategory Category => EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM;

	public bool IsCustom => isCustom;

	public string Name => name;

	public string ComponentName => ownerComponent == null ? "" : ownerComponent.Name;

	public VMType Type => valueType;

	public object Value {
		get {
			if (!isAfterLoaded)
				OnAfterLoad();
			return defValue;
		}
		set => Logger.AddError("!!! Такого быть не должно !!!");
	}

	public bool Implicit => isImplicit;

	public IObject Parent => parent;

	public void OnAfterLoad() {
		if (isAfterLoaded)
			return;
		if (defValue != null && defValue.GetType() == typeof(string))
			if (valueType.BaseType != typeof(string))
				try {
					defValue = StringSerializer.ReadValue((string)defValue, valueType.BaseType);
				} catch (Exception ex) {
					Logger.AddError(ex.ToString());
				}

		isAfterLoaded = true;
	}

	public bool IsDynamicObject => typeof(IObjRef).IsAssignableFrom(valueType.BaseType);

	public IEnumerable<string> GetComponentNames() {
		if (typeFunctionalList == null)
			LoadTypeFunctionals();
		return typeFunctionalList;
	}

	public IEnumerable<IVariable> GetContextVariables(EContextVariableCategory contextVarCategory) {
		return TypedBlueprint != null
			? TypedBlueprint.GetContextVariables(contextVarCategory)
			: LoadContextVariables(contextVarCategory);
	}

	public IVariable GetContextVariable(string variableName) {
		foreach (var componentName in GetComponentNames()) {
			foreach (var contextVariable in EngineAPIManager.GetAbstractVariablesByFunctionalName(componentName,
				         EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_APIFUNCTION))
				if (contextVariable.Name == variableName)
					return contextVariable;
			foreach (var contextVariable in EngineAPIManager.GetAbstractVariablesByFunctionalName(componentName,
				         EContextVariableCategory.CONTEXT_VARIABLE_CATEGORY_PARAM))
				if (contextVariable.Name == variableName)
					return contextVariable;
		}

		return TypedBlueprint != null ? TypedBlueprint.GetContextVariable(variableName) : null;
	}

	public bool IsFunctionalSupport(string componentName) {
		return GetComponentNames().Contains(componentName);
	}

	public bool IsFunctionalSupport(IEnumerable<string> functionals) {
		var componentNames = GetComponentNames();
		foreach (var functional in functionals)
			if (!componentNames.Contains(functional))
				return false;
		return true;
	}

	public IBlueprint TypedBlueprint => Type.IsComplexSpecial ? Type.SpecialTypeBlueprint : null;

	public IGameObjectContext OwnerContext {
		get {
			var parent = Parent;
			while (parent != null) {
				if (typeof(IGameObjectContext).IsAssignableFrom(parent.GetType()))
					return (IGameObjectContext)parent;
				if (typeof(INamedElement).IsAssignableFrom(parent.GetType()))
					parent = ((INamedElement)parent).Parent;
			}

			return null;
		}
	}

	private IEnumerable<IVariable> LoadContextVariables(EContextVariableCategory contextVarCategory) {
		foreach (var componentName in GetComponentNames()) {
			foreach (var variable in EngineAPIManager.GetAbstractVariablesByFunctionalName(componentName,
				         contextVarCategory))
				yield return variable;
		}
	}

	private void LoadTypeFunctionals() {
		typeFunctionalList = new List<string>();
		if (defValue != null && typeof(IObjRef).IsAssignableFrom(defValue.GetType()) &&
		    ((IObjRef)defValue).Object != null)
			typeFunctionalList.AddRange(((IObjRef)defValue).Object.GetComponentNames());
		else {
			if (Type == null || !Type.IsFunctionalSpecial)
				return;
			typeFunctionalList.AddRange(Type.GetFunctionalParts());
		}
	}

	public string GuidStr => guid.ToString();

	public void Clear() {
		ownerComponent = null;
		valueType.Clear();
		defValue = null;
		parent = null;
		if (typeFunctionalList == null)
			return;
		typeFunctionalList.Clear();
		typeFunctionalList = null;
	}
}