using Cofe.Loggers;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common
{
  public static class CommonVariableUtility
  {
    public static bool CheckParamInfo(string data, VMType needType)
    {
      if (!(typeof (VMType) == needType.BaseType) || !data.Contains("PLVirtualMaсhine.Common.IObjRef%"))
        return true;
      Logger.AddError("!!! Такого быть не должно , type : " + data);
      return false;
    }

    public static bool IsLocalVariableData(string data)
    {
      return data.Contains("local_") || data.Contains("_message_") || data.Contains("_inputparam_");
    }
  }
}
