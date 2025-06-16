// Decompiled with JetBrains decompiler
// Type: Engine.Common.Types.Pair`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace Engine.Common.Types
{
  public struct Pair<T1, T2>
  {
    public Pair(T1 item1, T2 item2)
    {
      this.Item1 = item1;
      this.Item2 = item2;
    }

    public T1 Item1 { get; set; }

    public T2 Item2 { get; set; }
  }
}
