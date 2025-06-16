// Decompiled with JetBrains decompiler
// Type: SRDebugger.Services.ISystemInformationService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace SRDebugger.Services
{
  public interface ISystemInformationService
  {
    IEnumerable<string> GetCategories();

    IList<ISystemInfo> GetInfo(string category);

    Dictionary<string, Dictionary<string, object>> CreateReport(bool includePrivate = false);

    void AddInfo(string category, ISystemInfo info);
  }
}
