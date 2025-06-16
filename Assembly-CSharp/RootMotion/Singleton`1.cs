// Decompiled with JetBrains decompiler
// Type: RootMotion.Singleton`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion
{
  public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
  {
    private static T sInstance = default (T);

    public static T instance => Singleton<T>.sInstance;

    protected virtual void Awake()
    {
      if ((Object) Singleton<T>.sInstance != (Object) null)
        Debug.LogError((object) (this.name + "error: already initialized"), (Object) this);
      Singleton<T>.sInstance = (T) this;
    }
  }
}
