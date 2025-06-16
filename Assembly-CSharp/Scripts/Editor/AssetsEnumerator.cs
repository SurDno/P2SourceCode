// Decompiled with JetBrains decompiler
// Type: Scripts.Editor.AssetsEnumerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Scripts.Editor
{
  public struct AssetsEnumerator : IEnumerable<string>, IEnumerable
  {
    private IEnumerable<string> assets;
    private string title;

    public AssetsEnumerator(string title, IEnumerable<string> assets)
    {
      this.title = title;
      this.assets = assets;
    }

    public IEnumerator<string> GetEnumerator() => this.assets.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) ((IEnumerable<string>) this).GetEnumerator();
    }
  }
}
