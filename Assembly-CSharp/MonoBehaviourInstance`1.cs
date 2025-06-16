// Decompiled with JetBrains decompiler
// Type: MonoBehaviourInstance`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public abstract class MonoBehaviourInstance<T> : MonoBehaviour where T : MonoBehaviourInstance<T>
{
  public static T Instance { get; private set; }

  protected virtual void Awake()
  {
    if ((Object) MonoBehaviourInstance<T>.Instance != (Object) null)
      Debug.LogError((object) "Instance already created", (Object) this);
    MonoBehaviourInstance<T>.Instance = (T) this;
  }
}
