// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMTypePool
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI
{
  public static class VMTypePool
  {
    private static Dictionary<string, VMType> types = new Dictionary<string, VMType>();

    public static VMType GetType(string text)
    {
      lock (VMTypePool.types)
      {
        VMType type1;
        if (VMTypePool.types.TryGetValue(text, out type1))
          return type1;
        VMType type2 = new VMType();
        type2.Read(text);
        VMTypePool.types.Add(text, type2);
        return type2;
      }
    }

    public static void Clear()
    {
      lock (VMTypePool.types)
        VMTypePool.types.Clear();
    }
  }
}
