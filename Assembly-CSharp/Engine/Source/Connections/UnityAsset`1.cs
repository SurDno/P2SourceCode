using System;
using Inspectors;
using Object = UnityEngine.Object;

namespace Engine.Source.Connections
{
  public struct UnityAsset<T>(Guid id) : IUnityAsset
    where T : Object 
  {
    private Guid id = id;

    [Inspected]
    public Guid Id => id;

    public T Value => UnityAssetUtility.GetValue<T>(id);

    public override int GetHashCode() => id.GetHashCode();

    public override bool Equals(object a) => a is UnityAsset<T> unityAsset && this == unityAsset;

    public static bool operator ==(UnityAsset<T> a, UnityAsset<T> b) => a.id == b.id;

    public static bool operator !=(UnityAsset<T> a, UnityAsset<T> b) => !(a == b);
  }
}
