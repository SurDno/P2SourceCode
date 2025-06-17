using System;
using System.Collections.Generic;
using Cofe.Loggers;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;

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
          Logger.AddError(string.Format("Attempt to read null params data at {0}", EngineAPIManager.Instance.CurrentFSMStateInfo));
          return dictionary;
        case "":
        case "0":
          return dictionary;
        default:
          string[] separator = ["&NEXT&PAR&"];
          foreach (string paramInfoStr in dataStr.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          {
            KeyValuePair<string, object> keyValuePair = ReadParamInfo(paramInfoStr);
            dictionary.Add(keyValuePair.Key, keyValuePair.Value);
          }
          return dictionary;
      }
    }

    private static KeyValuePair<string, object> ReadParamInfo(string paramInfoStr)
    {
      string[] separator = [
        "&PARAM&INFO&PART&"
      ];
      string[] strArray = paramInfoStr.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      string key = "";
      object obj = null;
      try
      {
        if (strArray.Length != 3)
        {
          Logger.AddError(string.Format("Read param element in params array error: {0} is invalid param data at {1}", paramInfoStr, EngineAPIManager.Instance.CurrentFSMStateInfo));
        }
        else
        {
          key = strArray[0];
          VMType type = VMTypePool.GetType(strArray[1]);
          obj = StringSerializer.ReadValue(strArray[2], type.BaseType);
        }
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("Read param element in params array error: {0} at {1}", ex, EngineAPIManager.Instance.CurrentFSMStateInfo));
      }
      return new KeyValuePair<string, object>(key, obj);
    }
  }
}
