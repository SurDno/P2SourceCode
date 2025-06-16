using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using PLVirtualMachine.GameLogic;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.FSM;

[TypeData(EDataType.TSpeech)]
[DataFactory("Speech")]
public class VMSpeech :
	VMState,
	IStub,
	IEditorDataReader,
	ISpeech,
	IState,
	IGraphObject,
	IContainer,
	IObject,
	IEditorBaseTemplate,
	INamedElement,
	INamed,
	IStaticUpdateable,
	ILocalContext {
	[FieldData("Replyes", DataFieldType.Reference)]
	private List<ISpeechReply> exitPoints = new();

	[FieldData("Text", DataFieldType.Reference)]
	private VMGameString text;

	[FieldData("AuthorGuid")] private ulong speechAuthorObjGuid;
	[FieldData("OnlyOnce")] private bool onlyOnce;
	[FieldData("IsTrade")] private bool isTrade;

	[FieldData("ParamText", DataFieldType.Reference)]
	private VMParameter speechParam;

	private IVariable speechAuthor;

	public override void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext) {
		while (xml.Read()) {
			if (xml.NodeType == XmlNodeType.Element)
				switch (xml.Name) {
					case "AuthorGuid":
						speechAuthorObjGuid = EditorDataReadUtility.ReadValue(xml, speechAuthorObjGuid);
						continue;
					case "EntryPoints":
						entryPoints = EditorDataReadUtility.ReadReferenceList(xml, creator, entryPoints);
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
					case "IsTrade":
						isTrade = EditorDataReadUtility.ReadValue(xml, isTrade);
						continue;
					case "Name":
						name = EditorDataReadUtility.ReadValue(xml, name);
						continue;
					case "OnlyOnce":
						onlyOnce = EditorDataReadUtility.ReadValue(xml, onlyOnce);
						continue;
					case "OutputLinks":
						outputLinks = EditorDataReadUtility.ReadReferenceList(xml, creator, outputLinks);
						continue;
					case "Owner":
						owner = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
						continue;
					case "ParamText":
						speechParam = EditorDataReadUtility.ReadReference<VMParameter>(xml, creator);
						continue;
					case "Parent":
						parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
						continue;
					case "Replyes":
						exitPoints = EditorDataReadUtility.ReadReferenceList(xml, creator, exitPoints);
						continue;
					case "Text":
						text = EditorDataReadUtility.ReadReference<VMGameString>(xml, creator);
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

	public VMSpeech(ulong guid)
		: base(guid) { }

	public override EObjectCategory GetCategory() {
		return EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT;
	}

	public override EStateType StateType => EStateType.STATE_TYPE_SPEECH;

	public IGameString Text => text;

	public IParam TextParam => speechParam;

	public IObjRef Author {
		get {
			if (this.speechAuthor != null) {
				if (typeof(VMParameter).IsAssignableFrom(this.speechAuthor.GetType())) {
					var speechAuthor = (VMParameter)this.speechAuthor;
					if (typeof(IObjRef).IsAssignableFrom(speechAuthor.Type.BaseType))
						return (IObjRef)speechAuthor.Value;
				} else if (typeof(IObjRef).IsAssignableFrom(this.speechAuthor.GetType()))
					return (IObjRef)this.speechAuthor;
			}

			return null;
		}
	}

	public ulong SpeechAuthorObjGuid => speechAuthorObjGuid;

	public bool OnlyOnce => onlyOnce;

	public bool IsTrade => isTrade;

	public override int GetExitPointsCount() {
		return exitPoints.Count;
	}

	public List<ISpeechReply> Replies => exitPoints;

	public IActionLine ActionLine => entryPoints.Count > 0 ? entryPoints[0].ActionLine : null;

	public override bool IgnoreBlock => !IsTrade || base.IgnoreBlock;

	public override void Update() { }

	public override void OnAfterLoad() {
		if (IsAfterLoaded)
			return;
		if (!VMBaseObjectUtility.CheckOrders(exitPoints))
			Logger.AddError(string.Format("Speech line id={0} has invalid replyes ordering", BaseGuid));
		base.OnAfterLoad();
		BindAuthor();
		for (var index = 0; index < Replies.Count; ++index) {
			var actionLine = Replies[index].ActionLine;
			if (actionLine != null)
				MakeLocalContextElementsDependencys(actionLine);
		}
	}

	public override void Clear() {
		base.Clear();
		if (exitPoints != null) {
			foreach (var exitPoint in exitPoints)
				if (typeof(VMSpeechReply) == exitPoint.GetType())
					((VMBaseObject)exitPoint).Clear();
			exitPoints.Clear();
			exitPoints = null;
		}

		text = null;
		speechParam = null;
		speechAuthor = null;
	}

	protected void BindAuthor() {
		Parent.Update();
		speechAuthor = ((VMTalkingGraph)Parent).GetSpeechAuthorInfo(speechAuthorObjGuid);
		if (speechAuthor != null)
			return;
		Logger.AddError("Cannot bind speech author: unknown author object guid: " + speechAuthorObjGuid);
	}
}