using System.Collections.Generic;
using System.Text;
using Cofe.Loggers;
using Cofe.Serializations.Converters;
using Engine.Common.Utility;
using PLVirtualMachine.Common.EngineAPI;

namespace PLVirtualMachine.Common
{
  public struct HierarchyGuid
  {
    public static readonly HierarchyGuid Empty;
    private readonly ulong[] guids;

    public HierarchyGuid(string data)
    {
      guids = null;
      HierarchyGuid result;
      if (!TryParse(data, out result))
        Logger.AddError("Invalid hierarchy guid format: no hierarchy splitter or incorrect symbols count in string, data : '" + data + "' , state : " + EngineAPIManager.Instance.CurrentFSMStateInfo);
      else
        guids = result.guids;
    }

    public static bool TryParse(string data, out HierarchyGuid result)
    {
      result = Empty;
      List<ulong> ulongList = new List<ulong>();
      if (data != "")
      {
        int index = 0;
        string result1 = "";
        while (StringUtility.NextSubstring(data, "H", ref index, ref result1))
        {
          ulong result2;
          if (!DefaultConverter.TryParseUlong(result1, out result2) || result2 == 0UL)
            return false;
          ulongList.Add(result2);
        }
        if (ulongList.Count == 0)
          return false;
      }
      result = new HierarchyGuid(ulongList.ToArray());
      return true;
    }

    private HierarchyGuid(ulong[] guids) => this.guids = guids;

    public HierarchyGuid(ulong templateGuid)
    {
      guids = new ulong[1]{ templateGuid };
    }

    public HierarchyGuid(HierarchyGuid parentGuid, ulong templateGuid)
    {
      guids = new ulong[parentGuid.guids.Length + 1];
      parentGuid.guids.CopyTo(guids, 0);
      guids[parentGuid.guids.Length] = templateGuid;
    }

    public ulong[] Guids => guids;

    public ulong TemplateGuid
    {
      get => guids != null && guids.Length != 0 ? guids[guids.Length - 1] : 0UL;
    }

    public bool IsEmpty => guids == null || guids.Length == 0;

    public bool IsHierarchy => guids != null && guids.Length > 1;

    public string Write()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (guids != null)
      {
        for (int index = 0; index < guids.Length; ++index)
        {
          stringBuilder.Append(guids[index].ToString());
          if (index < guids.Length - 1)
            stringBuilder.Append("H");
        }
      }
      return stringBuilder.ToString();
    }

    public string DataStr => Write();

    public override int GetHashCode()
    {
      int hashCode = 0;
      if (guids != null)
      {
        foreach (ulong guid in guids)
          hashCode ^= guid.GetHashCode();
      }
      return hashCode;
    }

    public override bool Equals(object a)
    {
      return a is HierarchyGuid hierarchyGuid && this == hierarchyGuid;
    }

    public static bool operator ==(HierarchyGuid a, HierarchyGuid b)
    {
      if (a.guids == null && b.guids == null)
        return true;
      if (a.guids == null || b.guids == null || a.guids.Length != b.guids.Length)
        return false;
      for (int index = 0; index < b.guids.Length; ++index)
      {
        if ((long) a.guids[index] != (long) b.guids[index])
          return false;
      }
      return true;
    }

    public static bool operator !=(HierarchyGuid a, HierarchyGuid b) => !(a == b);
  }
}
