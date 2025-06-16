using Cofe.Loggers;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  public static class ParamsArray
  {
    public static Dictionary<string, object> Read(string dataStr)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      switch (dataStr)
      {
        case null:
          Logger.AddError(string.Format("Attempt to read null params data at {0}", (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
          return dictionary;
        case "":
        case "0":
          return dictionary;
        default:
          string[] separator = new string[1]{ "&NEXT&PAR&" };
          foreach (string paramInfoStr in dataStr.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          {
            KeyValuePair<string, object> keyValuePair = ParamsArray.ReadParamInfo(paramInfoStr);
            dictionary.Add(keyValuePair.Key, keyValuePair.Value);
          }
          return dictionary;
      }
    }

    private static KeyValuePair<string, object> ReadParamInfo(string paramInfoStr)
    {
      string[] separator = new string[1]
      {
        "&PARAM&INFO&PART&"
      };
      string[] strArray = paramInfoStr.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      string key = "";
      object obj = (object) null;
      try
      {
        if (strArray.Length != 3)
        {
          Logger.AddError(string.Format("Read param element in params array error: {0} is invalid param data at {1}", (object) paramInfoStr, (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
        }
        else
        {
          key = strArray[0];
          VMType type = VMTypePool.GetType(strArray[1]);
          obj = PLVirtualMachine.Common.Data.StringSerializer.ReadValue(strArray[2], type.BaseType);
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Read param element in params array error: {0} at {1}", (object) ex.ToString(), (object) EngineAPIManager.Instance.CurrentFSMStateInfo));
      }
      return new KeyValuePair<string, object>(key, obj);
    }
  }
}
