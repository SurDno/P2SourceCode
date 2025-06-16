// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.EffectInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;

#nullable disable
namespace Engine.Source.Components
{
  public struct EffectInfo
  {
    [Inspected(Header = true)]
    public string Type;
    [Inspected(Header = true)]
    public string Name;
    [Inspected(Header = true)]
    public int Count;
    [Inspected(Header = true)]
    public int Compute;
  }
}
