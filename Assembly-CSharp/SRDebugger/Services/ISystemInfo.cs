// Decompiled with JetBrains decompiler
// Type: SRDebugger.Services.ISystemInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace SRDebugger.Services
{
  public interface ISystemInfo
  {
    string Title { get; }

    object Value { get; }

    bool IsPrivate { get; }
  }
}
