// Decompiled with JetBrains decompiler
// Type: Engine.Source.Connections.UnitySubAsset`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Inspectors;
using System;

#nullable disable
namespace Engine.Source.Connections
{
  public struct UnitySubAsset<T> : IUnityAsset where T : UnityEngine.Object
  {
    private Guid id;
    private string name;

    [Inspected]
    public Guid Id => this.id;

    [Inspected]
    public string Name => this.name;

    public UnitySubAsset(Guid id, string name)
    {
      this.id = id;
      this.name = name;
    }

    public T Value => UnityAssetUtility.GetValue<T>(this.id, this.name);

    public override int GetHashCode()
    {
      int hashCode = this.id.GetHashCode();
      if (!this.name.IsNullOrEmpty())
        hashCode ^= this.name.GetHashCode();
      return hashCode;
    }

    public override bool Equals(object a)
    {
      return a is UnitySubAsset<T> unitySubAsset && this == unitySubAsset;
    }

    public static bool operator ==(UnitySubAsset<T> a, UnitySubAsset<T> b)
    {
      return a.id == b.id && (a.name.IsNullOrEmpty() == b.name.IsNullOrEmpty() || a.name == b.name);
    }

    public static bool operator !=(UnitySubAsset<T> a, UnitySubAsset<T> b) => !(a == b);
  }
}
