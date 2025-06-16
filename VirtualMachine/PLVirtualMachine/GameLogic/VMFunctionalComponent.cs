using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using Engine.Common.Reflection;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.GameLogic;

[TypeData(EDataType.TFunctionalComponent)]
[DataFactory("FunctionalComponent")]
public class VMFunctionalComponent :
	VMBaseObject,
	IStub,
	IEditorDataReader,
	IFunctionalComponent,
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable {
	[FieldData("Events", DataFieldType.Reference)]
	private List<IEvent> eventsList = new();

	[FieldData("Main")] private bool isMain;
	[FieldData("LoadPriority")] private long loadPriority = long.MaxValue;
	private string dependedComponentName;
	private List<BaseFunction> functions = new();
	private Type componentType;
	private bool afterLoaded;

	public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext) {
		while (xml.Read()) {
			if (xml.NodeType == XmlNodeType.Element)
				switch (xml.Name) {
					case "Events":
						eventsList = EditorDataReadUtility.ReadReferenceList(xml, creator, eventsList);
						continue;
					case "Main":
						isMain = EditorDataReadUtility.ReadValue(xml, isMain);
						continue;
					case "LoadPriority":
						loadPriority = EditorDataReadUtility.ReadValue(xml, loadPriority);
						continue;
					case "Name":
						name = EditorDataReadUtility.ReadValue(xml, name);
						continue;
					case "Parent":
						parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
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

	public VMFunctionalComponent(ulong guid)
		: base(guid) { }

	public override EObjectCategory GetCategory() {
		return EObjectCategory.OBJECT_CATEGORY_FUNC_COMPONENT;
	}

	public string DependedComponentName => dependedComponentName;

	public Type ComponentType => componentType;

	public List<IEvent> EngineEvents => eventsList;

	public List<BaseFunction> EngineFunctions => functions;

	public bool Main => isMain;

	public bool Inited => componentType != null;

	public long LoadPriority => loadPriority;

	public void OnAfterLoad() {
		componentType = EngineAPIManager.GetComponentTypeByName(name);
		if (null == componentType)
			return;
		var componentInfo = InfoAttribute.GetComponentInfo(Name);
		if (componentInfo != null)
			dependedComponentName = componentInfo.DependedComponentName;
		LoadAPIFunctions();
		for (var index = 0; index < eventsList.Count; ++index)
			((VMEvent)eventsList[index]).OnAfterLoad();
		afterLoaded = true;
	}

	public bool IsAfterLoaded => afterLoaded;

	public override void Clear() {
		base.Clear();
		if (eventsList != null) {
			foreach (IContainer events in eventsList)
				events.Clear();
			eventsList.Clear();
			eventsList = null;
		}

		if (functions == null)
			return;
		foreach (var function in functions)
			function.Clear();
		functions.Clear();
		functions = null;
	}

	private void LoadAPIFunctions() {
		functions.Clear();
		var functionalComponentByName = EngineAPIManager.GetFunctionalComponentByName(Name);
		for (var index = 0; index < functionalComponentByName.Methods.Count; ++index) {
			var baseFunction = new BaseFunction(functionalComponentByName.Methods[index].MethodName, this);
			baseFunction.InitParams(functionalComponentByName.Methods[index].InputParams,
				functionalComponentByName.Methods[index].ReturnParam);
			functions.Add(baseFunction);
		}
	}
}