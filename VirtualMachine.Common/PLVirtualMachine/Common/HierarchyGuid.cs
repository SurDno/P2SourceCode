// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.HierarchyGuid
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Cofe.Serializations.Converters;
using Engine.Common.Utility;
using PLVirtualMachine.Common.EngineAPI;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace PLVirtualMachine.Common
{
  public struct HierarchyGuid
  {
    public static readonly HierarchyGuid Empty;
    private readonly ulong[] guids;

    public HierarchyGuid(string data)
    {
      this.guids = (ulong[]) null;
      HierarchyGuid result;
      if (!HierarchyGuid.TryParse(data, out result))
        Logger.AddError("Invalid hierarchy guid format: no hierarchy splitter or incorrect symbols count in string, data : '" + data + "' , state : " + EngineAPIManager.Instance.CurrentFSMStateInfo);
      else
        this.guids = result.guids;
    }

    public static bool TryParse(string data, out HierarchyGuid result)
    {
      result = HierarchyGuid.Empty;
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
      this.guids = new ulong[1]{ templateGuid };
    }

    public HierarchyGuid(HierarchyGuid parentGuid, ulong templateGuid)
    {
      this.guids = new ulong[parentGuid.guids.Length + 1];
      parentGuid.guids.CopyTo((Array) this.guids, 0);
      this.guids[parentGuid.guids.Length] = templateGuid;
    }

    public ulong[] Guids => this.guids;

    public ulong TemplateGuid
    {
      get => this.guids != null && this.guids.Length != 0 ? this.guids[this.guids.Length - 1] : 0UL;
    }

    public bool IsEmpty => this.guids == null || this.guids.Length == 0;

    public bool IsHierarchy => this.guids != null && this.guids.Length > 1;

    public string Write()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.guids != null)
      {
        for (int index = 0; index < this.guids.Length; ++index)
        {
          stringBuilder.Append(this.guids[index].ToString());
          if (index < this.guids.Length - 1)
            stringBuilder.Append("H");
        }
      }
      return stringBuilder.ToString();
    }

    public string DataStr => this.Write();

    public override int GetHashCode()
    {
      int hashCode = 0;
      if (this.guids != null)
      {
        foreach (ulong guid in this.guids)
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
