// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.ObjectCombinationVariant
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Cofe.Loggers;
using Cofe.Serializations.Converters;
using Engine.Common.Types;
using PLVirtualMachine.Common.Data;
using System;

#nullable disable
namespace PLVirtualMachine.Common
{
  public class ObjectCombinationVariant
  {
    public ulong ObjectGuid;
    public int MinCount;
    public int MaxCount;
    public int Weight;
    public CombinationItemParams CIParams = new CombinationItemParams();

    public ObjectCombinationVariant()
    {
      this.ObjectGuid = 0UL;
      this.MinCount = 1;
      this.MaxCount = 1;
      this.Weight = 1;
    }

    public ObjectCombinationVariant(ulong objGuid, int minCount, int maxCount)
    {
      this.ObjectGuid = objGuid;
      this.MinCount = minCount;
      this.MaxCount = maxCount;
      this.Weight = 1;
    }

    public ObjectCombinationVariant(string dataStr)
    {
      this.ObjectGuid = 0UL;
      this.MinCount = 0;
      this.MaxCount = 0;
      this.Weight = 1;
      string[] separator = new string[1]{ "END&PAR" };
      string[] strArray = dataStr.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length != 0)
        this.ObjectGuid = this.ReadObjGuid(strArray[0]);
      if (strArray.Length > 1)
        this.MinCount = StringUtility.ToInt32(strArray[1]);
      if (strArray.Length > 2)
        this.MaxCount = StringUtility.ToInt32(strArray[2]);
      if (strArray.Length > 3)
        this.Weight = StringUtility.ToInt32(strArray[3]);
      if (strArray.Length <= 4)
        return;
      this.CIParams = new CombinationItemParams();
      this.CIParams.Read(strArray[4]);
    }

    public bool ContainsItem(IBlueprint item)
    {
      return this.ObjectGuid != 0UL && (long) item.BaseGuid == (long) this.ObjectGuid;
    }

    private ulong ReadObjGuid(string data)
    {
      if (GuidUtility.GetGuidFormat(data) == EGuidFormat.GT_BASE)
        return DefaultConverter.ParseUlong(data);
      Logger.AddError(string.Format("Invalid template base guid format {0} at object combination variant reading", (object) data));
      return 0;
    }
  }
}
