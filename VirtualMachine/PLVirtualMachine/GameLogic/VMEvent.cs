using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Common.VMSpecialAttributes;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;
using VirtualMachine.Data.Customs;

namespace PLVirtualMachine.GameLogic;

[TypeData(EDataType.TEvent)]
[DataFactory("Event")]
public class VMEvent :
	VMBaseObject,
	IStub,
	IEditorDataReader,
	IEvent,
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable,
	ILocalContext {
	[FieldData("Manual")] private bool isManual = true;

	[FieldData("Condition", DataFieldType.Reference)]
	private ICondition eventCondition;

	[FieldData("ChangeTo")] private bool changeTo = true;
	[FieldData("Repeated")] private bool repeated = true;

	[FieldData("GameTimeContext", DataFieldType.Reference)]
	private IGameMode gameTimeContext;

	[FieldData("MessagesInfo")] private List<NameTypeData> messageInfoData;
	[FieldData("EventRaisingType")] private EEventRaisingType eventRaisingType;

	[FieldData("EventParameter", DataFieldType.Reference)]
	private VMParameter eventParameter;

	[FieldData("EventTime")] private GameTime eventTime;
	private List<BaseMessage> messages;
	private IContainer owner;
	private bool atOnce;
	private bool afterLoaded;

	public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext) {
		while (xml.Read()) {
			if (xml.NodeType == XmlNodeType.Element)
				switch (xml.Name) {
					case "ChangeTo":
						changeTo = EditorDataReadUtility.ReadValue(xml, changeTo);
						continue;
					case "Condition":
						eventCondition = EditorDataReadUtility.ReadReference<ICondition>(xml, creator);
						continue;
					case "EventParameter":
						eventParameter = EditorDataReadUtility.ReadReference<VMParameter>(xml, creator);
						continue;
					case "EventRaisingType":
						eventRaisingType = EditorDataReadUtility.ReadEnum<EEventRaisingType>(xml);
						continue;
					case "EventTime":
						eventTime = EditorDataReadUtility.ReadSerializable<GameTime>(xml);
						continue;
					case "GameTimeContext":
						gameTimeContext = EditorDataReadUtility.ReadReference<IGameMode>(xml, creator);
						continue;
					case "Manual":
						isManual = EditorDataReadUtility.ReadValue(xml, isManual);
						continue;
					case "MessagesInfo":
						messageInfoData =
							EditorDataReadUtility.ReadEditorDataSerializableList(xml, creator, messageInfoData);
						continue;
					case "Name":
						name = EditorDataReadUtility.ReadValue(xml, name);
						continue;
					case "Parent":
						parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
						continue;
					case "Repeated":
						repeated = EditorDataReadUtility.ReadValue(xml, repeated);
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

	public VMEvent(ulong guid)
		: base(guid) { }

	public override EObjectCategory GetCategory() {
		return EObjectCategory.OBJECT_CATEGORY_EVENT;
	}

	public bool IsManual => isManual;

	public EEventRaisingType EventRaisingType => eventRaisingType;

	public string FunctionalName => Parent == null ? Name : Parent.Name + "." + Name;

	public override IContainer Owner => owner;

	public bool IsInitial(IObject obj) {
		try {
			if (!afterLoaded)
				OnAfterLoad();
			return owner != null && obj.IsEqual(owner) && !IsManual && Name ==
				EngineAPIManager.GetSpecialEventName(ESpecialEventName.SEN_START_OBJECT_FSM, typeof(VMCommon));
		} catch (Exception ex) {
			Logger.AddError(string.Format("Event {0} initial checking error: {1}", Name, ex));
		}

		return false;
	}

	public ICondition Condition => eventCondition;

	public IParam EventParameter => eventParameter;

	public GameTime EventTime => eventTime;

	public bool ChangeTo => changeTo;

	public bool Repeated => repeated;

	public bool AtOnce => atOnce;

	public IGameMode GameTimeContext => gameTimeContext;

	public List<BaseMessage> ReturnMessages {
		get {
			if (!IsUpdated)
				Update();
			return messages;
		}
	}

	public virtual List<IVariable> GetLocalContextVariables(
		EContextVariableCategory eContextVarCategory,
		IContextElement currentElement,
		int iCounter = 0) {
		return new List<IVariable>();
	}

	public IVariable GetLocalContextVariable(string variableUniName, IContextElement currentElement = null) {
		return null;
	}

	public void OnAfterLoad() {
		owner = IsManual ? Parent :
			!typeof(IFunctionalComponent).IsAssignableFrom(Parent.GetType()) ? Parent : Parent.Parent;
		if (owner == null)
			Logger.AddError(string.Format("Invalid event: id={0}", BaseGuid));
		else {
			if (messages == null)
				LoadEventMessages();
			afterLoaded = true;
		}
	}

	public bool IsAfterLoaded => afterLoaded;

	public override void Clear() {
		if (eventCondition != null) {
			((VMPartCondition)eventCondition).Clear();
			eventCondition = null;
		}

		gameTimeContext = null;
		if (messageInfoData != null) {
			messageInfoData.Clear();
			messageInfoData = null;
		}

		eventParameter = null;
		eventTime = null;
		owner = null;
		if (messages == null)
			return;
		foreach (ContextVariable message in messages)
			message.Clear();
		messages.Clear();
		messages = null;
	}

	private void LoadEventMessages() {
		messages = new List<BaseMessage>();
		if (!IsManual) {
			if (Parent == null)
				Logger.AddError("Standart messages loading requires parent component");
			else {
				var apiEventInfoByName = EngineAPIManager.GetAPIEventInfoByName(Parent.Name, Name);
				if (apiEventInfoByName == null)
					Logger.AddError(string.Format("Component {0} haven't info for event {1}", Parent.Name, Name));
				else {
					for (var index = 0; index < apiEventInfoByName.MessageParams.Count; ++index) {
						var type = apiEventInfoByName.MessageParams[index].Type;
						var name = Name + "_message_" + apiEventInfoByName.MessageParams[index].Name;
						var baseMessage = new BaseMessage();
						baseMessage.Initialize(name, type);
						messages.Add(baseMessage);
					}

					atOnce = apiEventInfoByName.AtOnce;
				}
			}
		} else {
			if (messageInfoData == null)
				return;
			foreach (var nameTypeData in messageInfoData) {
				var baseMessage = new BaseMessage();
				baseMessage.Initialize(nameTypeData.Name, nameTypeData.Type);
				messages.Add(baseMessage);
			}

			messageInfoData = null;
		}
	}
}