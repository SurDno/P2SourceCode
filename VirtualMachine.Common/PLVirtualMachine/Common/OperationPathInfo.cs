using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public class OperationPathInfo : IVMStringSerializable
  {
    private List<CommonVariable> operationRootsList = new List<CommonVariable>();
    public const string OperationPathRootName = "Pathologic";

    public OperationPathInfo(string data) => this.Read(data);

    public List<CommonVariable> RootInfoList => this.operationRootsList;

    public void Read(string data)
    {
      switch (data)
      {
        case null:
          Logger.AddError(string.Format("Attempt to read null operation path info at {0}", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          break;
        case "":
          break;
        case "0":
          break;
        default:
          if (data.Contains("&ROOT&PATH&VAR"))
          {
            string[] separator = new string[1]
            {
              "&ROOT&PATH&VAR"
            };
            this.ReadPathList(data.Split(separator, StringSplitOptions.RemoveEmptyEntries)[1]);
            break;
          }
          this.ReadPathList(data);
          break;
      }
    }

    public string Write()
    {
      Logger.AddError("Not allowed serialization data struct in virtual machine!");
      return string.Empty;
    }

    private void ReadPathList(string data)
    {
      this.operationRootsList.Clear();
      string[] separator = new string[1]{ "END&PATH" };
      foreach (string operationRootsListInfo in data.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        if ("" != operationRootsListInfo)
          this.operationRootsList.Add(this.ReadContextParamValue(operationRootsListInfo));
      }
    }

    private CommonVariable ReadContextParamValue(string operationRootsListInfo)
    {
      string str = "";
      string variableData = "";
      if (operationRootsListInfo.Contains("CONTEXT&PARAM"))
      {
        string[] separator = new string[1]
        {
          "CONTEXT&PARAM"
        };
        string[] strArray = operationRootsListInfo.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length > 1)
        {
          str = strArray[0];
          variableData = strArray[1];
        }
        else if (strArray.Length == 1)
        {
          str = strArray[0];
          variableData = strArray[0];
        }
      }
      else
        str = operationRootsListInfo;
      CommonVariable rootVarInfo = new CommonVariable();
      rootVarInfo.Initialise(str, variableData);
      if (!this.IsValidRootInfo(rootVarInfo) && str.Contains("/") && "" != IStaticDataContainer.StaticDataContainer.GameRoot.GetHierarchyGuidByHierarchyPath(str).Write())
      {
        rootVarInfo = new CommonVariable();
        rootVarInfo.Initialise(str, "");
      }
      return rootVarInfo;
    }

    private bool IsValidRootInfo(CommonVariable rootVarInfo)
    {
      IContext gameRoot = (IContext) IStaticDataContainer.StaticDataContainer.GameRoot;
      rootVarInfo.Bind(gameRoot);
      return rootVarInfo.IsBinded && rootVarInfo.VariableContext != null;
    }
  }
}
