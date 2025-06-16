// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.CommonVariableUtility
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using PLVirtualMachine.Common.EngineAPI;

#nullable disable
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
