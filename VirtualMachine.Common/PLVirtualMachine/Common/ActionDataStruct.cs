// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.ActionDataStruct
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Common
{
  public class ActionDataStruct : 
    IAbstractAction,
    IBaseAction,
    IFunctionalPoint,
    IStaticUpdateable,
    IVMStringSerializable
  {
    private EMathOperationType mathOperationType;
    private EActionType type;
    private string targetFunctionName = "";
    private string targetObjFunctionalName = "";
    private string targetParamName = "";
    private CommonVariable targetObject;
    private CommonVariable targetParam;
    private List<CommonVariable> sourceParams = new List<CommonVariable>();
    private AbstractActionInfo actionInfo;
    private ILocalContext localContext;

    public ActionDataStruct(string data, ILocalContext localContext)
    {
      this.localContext = localContext;
      this.actionInfo = new AbstractActionInfo((IAbstractAction) this);
      this.Read(data);
      this.MakeTarget();
    }

    public ILocalContext LocalContext => this.localContext;

    public EActionType ActionType => this.type;

    public EMathOperationType MathOperationType => this.mathOperationType;

    public string TargetFunction => this.targetFunctionName;

    public string TargetEvent => this.targetFunctionName;

    public BaseFunction TargetFunctionInstance => this.actionInfo.TargetFunctionInstance;

    public IParam SourceConstant => (IParam) null;

    public CommonVariable TargetObject => this.targetObject;

    public CommonVariable TargetParam => this.targetParam;

    public List<CommonVariable> SourceParams => this.sourceParams;

    public bool IsValid => this.actionInfo.IsValid;

    public void Update()
    {
      if (this.targetObject == null)
        this.MakeTarget();
      if (this.actionInfo == null)
        return;
      this.actionInfo.Update();
    }

    public bool IsUpdated => this.actionInfo == null || this.actionInfo.IsValid;

    public void Read(string data)
    {
      switch (data)
      {
        case null:
          Logger.AddError(string.Format("Attempt to read null abstract action info data at {0}", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          break;
        case "":
          break;
        default:
          string[] separator = new string[1]
          {
            "&ACTION&PART&"
          };
          string[] strArray = data.Split(separator, StringSplitOptions.None);
          if (strArray.Length < 4)
          {
            Logger.AddError(string.Format("Cannot read abstract action info: {0} isn't valid abstract action serialize data at {1}", (object) data, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
            break;
          }
          string str1 = strArray[0];
          string str2 = strArray[1];
          string str3 = strArray[2];
          string str4 = strArray[3];
          foreach (object obj in Enum.GetValues(typeof (EActionType)))
          {
            if (obj.ToString() == str1)
            {
              this.type = (EActionType) obj;
              break;
            }
          }
          this.targetObjFunctionalName = str2;
          this.targetFunctionName = str3;
          if (this.type == EActionType.ACTION_TYPE_SET_PARAM)
            this.targetParamName = str3;
          this.sourceParams.Clear();
          string str5 = str4;
          char[] chArray = new char[1]{ ',' };
          foreach (string sourceParam in str5.Split(chArray))
          {
            string data1 = ActionDataStruct.DeserializeSrcParam(sourceParam);
            CommonVariable commonVariable = new CommonVariable();
            commonVariable.Read(data1);
            this.sourceParams.Add(commonVariable);
          }
          this.MakeTarget();
          break;
      }
    }

    public string Write()
    {
      Logger.AddError("Not allowed serialization data struct in virtual machine!");
      return string.Empty;
    }

    private void MakeTarget()
    {
      this.targetObject = new CommonVariable();
      this.targetObject.InitialiseFromLocalVariable("group_" + this.targetObjFunctionalName, new VMType(typeof (IObjRef), this.targetObjFunctionalName));
      if (this.type != EActionType.ACTION_TYPE_SET_PARAM)
        return;
      string data = ActionDataStruct.DeserializeSrcParam(this.targetParamName);
      this.targetParam = new CommonVariable();
      this.targetParam.Read(data);
    }

    private static string DeserializeSrcParam(string sourceParam)
    {
      return sourceParam.Replace("&VAR&INFO&", "%");
    }
  }
}
