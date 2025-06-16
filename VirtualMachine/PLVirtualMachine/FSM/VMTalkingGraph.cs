using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using PLVirtualMachine.GameLogic;
using PLVirtualMachine.Objects;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.FSM;

[TypeData(EDataType.TTalking)]
[DataFactory("Talking")]
public class VMTalkingGraph :
	FiniteStateMachine,
	IStub,
	IEditorDataReader,
	ITalkingGraph,
	IFiniteStateMachine,
	IState,
	IGraphObject,
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable,
	ILocalContext,
	IGraph {
	private IGameObjectContext talkingContext;

	public override void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext) {
		while (xml.Read()) {
			if (xml.NodeType == XmlNodeType.Element)
				switch (xml.Name) {
					case "EntryPoints":
						entryPoints = EditorDataReadUtility.ReadReferenceList(xml, creator, entryPoints);
						continue;
					case "EventLinks":
						eventLinks = EditorDataReadUtility.ReadReferenceList(xml, creator, eventLinks);
						continue;
					case "GraphType":
						graphType = EditorDataReadUtility.ReadEnum<EGraphType>(xml);
						continue;
					case "IgnoreBlock":
						ignoreBlock = EditorDataReadUtility.ReadValue(xml, ignoreBlock);
						continue;
					case "Initial":
						initial = EditorDataReadUtility.ReadValue(xml, initial);
						continue;
					case "InputLinks":
						inputLinks = EditorDataReadUtility.ReadReferenceList(xml, creator, inputLinks);
						continue;
					case "InputParamsInfo":
						inputParamsInfo =
							EditorDataReadUtility.ReadEditorDataSerializableList(xml, creator, inputParamsInfo);
						continue;
					case "Name":
						name = EditorDataReadUtility.ReadValue(xml, name);
						continue;
					case "OutputLinks":
						outputLinks = EditorDataReadUtility.ReadReferenceList(xml, creator, outputLinks);
						continue;
					case "Owner":
						owner = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
						continue;
					case "Parent":
						parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
						continue;
					case "States":
						states = EditorDataReadUtility.ReadReferenceList(xml, creator, states);
						continue;
					case "SubstituteGraph":
						substituteGraph = EditorDataReadUtility.ReadReference<IFiniteStateMachine>(xml, creator);
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

	public VMTalkingGraph(ulong guid)
		: base(guid) { }

	public override bool IgnoreBlock => true;

	public bool OnlyOnce => ignoreBlock;

	public IVariable GetSpeechAuthorInfo(ulong id) {
		if (talkingContext == null) {
			Logger.AddError("Cannot load authors list: talking context not defined");
			return null;
		}

		if (((VMLogicObject)talkingContext).IsFunctionalSupport("Speaking"))
			return (long)talkingContext.BaseGuid == (long)id ? ((VMLogicObject)talkingContext).GetSelf() : null;
		if (talkingContext.GetCategory() == EObjectCategory.OBJECT_CATEGORY_CLASS &&
		    ((VMLogicObject)talkingContext).IsFunctionalSupport("Speaking"))
			return (long)talkingContext.BaseGuid == (long)id ? ((VMLogicObject)talkingContext).GetSelf() : null;
		var staticObjects1 = ((VMLogicObject)talkingContext).GetStaticObjects();
		if (staticObjects1 != null)
			for (var index = 0; index < staticObjects1.Count; ++index)
				if (((VMLogicObject)staticObjects1[index]).IsFunctionalSupport("Speaking") &&
				    (long)staticObjects1[index].Object.BaseGuid == (long)id)
					return ((VMLogicObject)staticObjects1[index].Object).GetSelf();
		foreach (var contextVariable in ((VMLogicObject)talkingContext).GetContextVariables(EContextVariableCategory
			         .CONTEXT_VARIABLE_CATEGORY_PARAM))
			if (typeof(IObjRef).IsAssignableFrom(contextVariable.Type.BaseType)) {
				var speechAuthorInfo = (VMParameter)contextVariable;
				if (speechAuthorInfo.Value != null) {
					var objRef = (IObjRef)((VMParameter)contextVariable).Value;
					if (objRef.Object != null && !((VMLogicObject)objRef.Object).Static &&
					    (long)speechAuthorInfo.BaseGuid == (long)id)
						return speechAuthorInfo;
				}
			}

		var staticObjects2 = ((VMLogicObject)IStaticDataContainer.StaticDataContainer.GameRoot).GetStaticObjects();
		if (staticObjects2 != null)
			for (var index = 0; index < staticObjects2.Count; ++index)
				if (((VMLogicObject)staticObjects2[index].Object).IsFunctionalSupport("Speaking") &&
				    (long)staticObjects2[index].Object.BaseGuid == (long)id)
					return ((VMLogicObject)staticObjects2[index].Object).GetSelf();
		Logger.AddError("В контексте диалога нет ни одного персонажа. Диалог не имеет авторов!");
		return null;
	}

	public override void OnAfterLoad() {
		if (IsAfterLoaded)
			return;
		talkingContext = (IGameObjectContext)((VMBaseObject)Parent).Owner;
		base.OnAfterLoad();
	}

	public override void Clear() {
		base.Clear();
		talkingContext = null;
	}
}