using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.GameLogic;

[TypeData(EDataType.TAction)]
[DataFactory("Action")]
public class VMGameAction :
	IStub,
	IEditorDataReader,
	ISingleAction,
	IGameAction,
	IOrderedChild,
	IContextElement,
	IObject,
	IEditorBaseTemplate,
	INamed,
	IBaseAction,
	IStaticUpdateable,
	IAbstractAction,
	IFunctionalPoint {
	protected ulong guid;
	[FieldData("Name")] private string name = "";
	[FieldData("ActionType")] private EActionType actionType;
	[FieldData("MathOperationType")] private EMathOperationType mathOperationType;
	[FieldData("TargetFuncName")] private string targetFunctionName;

	[FieldData("LocalContext", DataFieldType.Reference)]
	private ILocalContext localContext;

	[FieldData("SourceConst", DataFieldType.Reference)]
	private VMParameter sourceConst;

	[FieldData("SourceExpression", DataFieldType.Reference)]
	private VMExpression sourceExpression;

	[FieldData("OrderIndex")] private int orderIndex;
	[FieldData("TargetObject")] private CommonVariable targetObject;
	[FieldData("TargetParam")] private CommonVariable targetParam;
	[FieldData("SourceParams")] private List<CommonVariable> sourceParams = new();
	private AbstractActionInfo actionInfo;
	private bool isUpdated;

	public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext) {
		while (xml.Read()) {
			if (xml.NodeType == XmlNodeType.Element)
				switch (xml.Name) {
					case "ActionType":
						actionType = EditorDataReadUtility.ReadEnum<EActionType>(xml);
						continue;
					case "LocalContext":
						localContext = EditorDataReadUtility.ReadReference<ILocalContext>(xml, creator);
						continue;
					case "MathOperationType":
						mathOperationType = EditorDataReadUtility.ReadEnum<EMathOperationType>(xml);
						continue;
					case "Name":
						name = EditorDataReadUtility.ReadValue(xml, name);
						continue;
					case "OrderIndex":
						orderIndex = EditorDataReadUtility.ReadValue(xml, orderIndex);
						continue;
					case "SourceConst":
						sourceConst = EditorDataReadUtility.ReadReference<VMParameter>(xml, creator);
						continue;
					case "SourceExpression":
						sourceExpression = EditorDataReadUtility.ReadReference<VMExpression>(xml, creator);
						continue;
					case "SourceParams":
						sourceParams = EditorDataReadUtility.ReadSerializableList(xml, sourceParams);
						continue;
					case "TargetFuncName":
						targetFunctionName = EditorDataReadUtility.ReadValue(xml, targetFunctionName);
						continue;
					case "TargetObject":
						targetObject = EditorDataReadUtility.ReadSerializable<CommonVariable>(xml);
						continue;
					case "TargetParam":
						targetParam = EditorDataReadUtility.ReadSerializable<CommonVariable>(xml);
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

	public VMGameAction(ulong guid) {
		this.guid = guid;
		actionInfo = new AbstractActionInfo(this);
	}

	public ulong BaseGuid => guid;

	public string Name => name;

	public ILocalContext LocalContext => localContext;

	public int Order => orderIndex;

	public virtual List<IVariable> LocalContextVariables => new();

	public virtual IVariable GetLocalContextVariable(string varUniName) {
		return null;
	}

	public bool IsValid => actionInfo.IsValid;

	public EActionType ActionType => actionType;

	public EMathOperationType MathOperationType => mathOperationType;

	public string TargetFunction => targetFunctionName == null ? string.Empty : targetFunctionName;

	public string TargetEvent => targetFunctionName == null ? string.Empty : targetFunctionName;

	public IParam SourceConstant => sourceConst;

	public IExpression SourceExpression => sourceExpression;

	public void Update() {
		if (isUpdated && actionInfo.IsValid)
			return;
		actionInfo.Update();
		isUpdated = actionInfo.IsValid;
	}

	public bool IsUpdated => isUpdated;

	public BaseFunction TargetFunctionInstance => actionInfo.TargetFunctionInstance;

	public CommonVariable TargetObject => targetObject;

	public CommonVariable TargetParam => targetParam;

	public List<CommonVariable> SourceParams => sourceParams;

	public bool IsEqual(IObject other) {
		return other != null && typeof(VMGameAction) == other.GetType() &&
		       (long)BaseGuid == (long)((VMGameAction)other).BaseGuid;
	}

	public string GuidStr => guid.ToString();

	public void Clear() {
		localContext = null;
		if (sourceConst != null) {
			sourceConst.Clear();
			sourceConst = null;
		}

		if (sourceExpression != null) {
			sourceExpression.Clear();
			sourceExpression = null;
		}

		if (targetObject != null) {
			targetObject.Clear();
			targetObject = null;
		}

		if (targetParam != null) {
			targetParam.Clear();
			targetParam = null;
		}

		if (sourceParams != null) {
			foreach (ContextVariable sourceParam in sourceParams)
				sourceParam.Clear();
			sourceParams.Clear();
			sourceParams = null;
		}

		if (actionInfo == null)
			return;
		actionInfo.Clear();
		actionInfo = null;
	}
}