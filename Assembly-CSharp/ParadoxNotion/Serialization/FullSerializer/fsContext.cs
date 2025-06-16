// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.fsContext
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer
{
  public sealed class fsContext
  {
    private readonly Dictionary<Type, object> _contextObjects = new Dictionary<Type, object>();

    public void Reset() => this._contextObjects.Clear();

    public void Set<T>(T obj) => this._contextObjects[typeof (T)] = (object) obj;

    public bool Has<T>() => this._contextObjects.ContainsKey(typeof (T));

    public T Get<T>()
    {
      object obj;
      if (this._contextObjects.TryGetValue(typeof (T), out obj))
        return (T) obj;
      throw new InvalidOperationException("There is no context object of type " + (object) typeof (T));
    }
  }
}
