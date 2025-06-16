// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.OwnHashInfoEqualityComparer
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public class OwnHashInfoEqualityComparer : IEqualityComparer<OwnHashInfo>
  {
    public static readonly OwnHashInfoEqualityComparer Instance = new OwnHashInfoEqualityComparer();

    public bool Equals(OwnHashInfo x, OwnHashInfo y)
    {
      return x.OwnerId == y.OwnerId && (long) x.EventId == (long) y.EventId && x.SendingFSMGuid == y.SendingFSMGuid;
    }

    public int GetHashCode(OwnHashInfo obj)
    {
      Guid guid = obj.OwnerId;
      int num = guid.GetHashCode() ^ obj.EventId.GetHashCode();
      guid = obj.SendingFSMGuid;
      int hashCode = guid.GetHashCode();
      return num ^ hashCode;
    }
  }
}
