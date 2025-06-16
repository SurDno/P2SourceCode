using Inspectors;
using System;

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
