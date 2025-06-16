using System.Collections.Generic;

namespace PLVirtualMachine.Common.EngineAPI
{
  public static class VMTypePool
  {
    private static Dictionary<string, VMType> types = new Dictionary<string, VMType>();

    public static VMType GetType(string text)
    {
      lock (types)
      {
        VMType type1;
        if (types.TryGetValue(text, out type1))
          return type1;
        VMType type2 = new VMType();
        type2.Read(text);
        types.Add(text, type2);
        return type2;
      }
    }

    public static void Clear()
    {
      lock (types)
        types.Clear();
    }
  }
}
