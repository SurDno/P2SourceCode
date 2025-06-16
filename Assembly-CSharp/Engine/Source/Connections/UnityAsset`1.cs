// Decompiled with JetBrains decompiler
// Type: Engine.Source.Connections.UnityAsset`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;
using System;

#nullable disable
namespace Engine.Source.Connections
{
  public struct UnityAsset<T> : IUnityAsset where T : UnityEngine.Object
  {
    private Guid id;

    [Inspected]
    public Guid Id => this.id;

    public UnityAsset(Guid id) => this.id = id;

    public T Value => UnityAssetUtility.GetValue<T>(this.id);

    public override int GetHashCode() => this.id.GetHashCode();

    public override bool Equals(object a) => a is UnityAsset<T> unityAsset && this == unityAsset;

    public static bool operator ==(UnityAsset<T> a, UnityAsset<T> b) => a.id == b.id;

    public static bool operator !=(UnityAsset<T> a, UnityAsset<T> b) => !(a == b);
  }
}
