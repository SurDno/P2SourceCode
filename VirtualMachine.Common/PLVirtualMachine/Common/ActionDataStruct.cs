using System;
using System.Collections.Generic;
using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common;

public class ActionDataStruct :
	IAbstractAction,
	IBaseAction,
	IFunctionalPoint,
	IStaticUpdateable,
	IVMStringSerializable {
	private EMathOperationType mathOperationType;
	private EActionType type;
	private string targetFunctionName = "";
	private string targetObjFunctionalName = "";
	private string targetParamName = "";
	private CommonVariable targetObject;
	private CommonVariable targetParam;
	private List<CommonVariable> sourceParams = new();
	private AbstractActionInfo actionInfo;
	private ILocalContext localContext;

	public ActionDataStruct(string data, ILocalContext localContext) {
		this.localContext = localContext;
		actionInfo = new AbstractActionInfo(this);
		Read(data);
		MakeTarget();
	}

	public ILocalContext LocalContext => localContext;

	public EActionType ActionType => type;

	public EMathOperationType MathOperationType => mathOperationType;

	public string TargetFunction => targetFunctionName;

	public string TargetEvent => targetFunctionName;

	public BaseFunction TargetFunctionInstance => actionInfo.TargetFunctionInstance;

	public IParam SourceConstant => null;

	public CommonVariable TargetObject => targetObject;

	public CommonVariable TargetParam => targetParam;

	public List<CommonVariable> SourceParams => sourceParams;

	public bool IsValid => actionInfo.IsValid;

	public void Update() {
		if (targetObject == null)
			MakeTarget();
		if (actionInfo == null)
			return;
		actionInfo.Update();
	}

	public bool IsUpdated => actionInfo == null || actionInfo.IsValid;

	public void Read(string data) {
		switch (data) {
			case null:
				Logger.AddError(string.Format("Attempt to read null abstract action info data at {0}",
					EngineAPIManager.Instance.CurrentFSMStateInfo));
				break;
			case "":
				break;
			default:
				var separator = new string[1] {
					"&ACTION&PART&"
				};
				var strArray = data.Split(separator, StringSplitOptions.None);
				if (strArray.Length < 4) {
					Logger.AddError(string.Format(
						"Cannot read abstract action info: {0} isn't valid abstract action serialize data at {1}", data,
						EngineAPIManager.Instance.CurrentFSMStateInfo));
					break;
				}

				var str1 = strArray[0];
				var str2 = strArray[1];
				var str3 = strArray[2];
				var str4 = strArray[3];
				foreach (var obj in Enum.GetValues(typeof(EActionType)))
					if (obj.ToString() == str1) {
						type = (EActionType)obj;
						break;
					}

				targetObjFunctionalName = str2;
				targetFunctionName = str3;
				if (type == EActionType.ACTION_TYPE_SET_PARAM)
					targetParamName = str3;
				sourceParams.Clear();
				var str5 = str4;
				var chArray = new char[1] { ',' };
				foreach (var sourceParam in str5.Split(chArray)) {
					var data1 = DeserializeSrcParam(sourceParam);
					var commonVariable = new CommonVariable();
					commonVariable.Read(data1);
					sourceParams.Add(commonVariable);
				}

				MakeTarget();
				break;
		}
	}

	public string Write() {
		Logger.AddError("Not allowed serialization data struct in virtual machine!");
		return string.Empty;
	}

	private void MakeTarget() {
		targetObject = new CommonVariable();
		targetObject.InitialiseFromLocalVariable("group_" + targetObjFunctionalName,
			new VMType(typeof(IObjRef), targetObjFunctionalName));
		if (type != EActionType.ACTION_TYPE_SET_PARAM)
			return;
		var data = DeserializeSrcParam(targetParamName);
		targetParam = new CommonVariable();
		targetParam.Read(data);
	}

	private static string DeserializeSrcParam(string sourceParam) {
		return sourceParam.Replace("&VAR&INFO&", "%");
	}
}