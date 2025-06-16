// Decompiled with JetBrains decompiler
// Type: TOD_Billboard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class TOD_Billboard : MonoBehaviour
{
  public float Altitude = 0.0f;
  public float Azimuth = 0.0f;
  public float Distance = 1f;
  public float Size = 1f;

  private T GetComponentInParents<T>() where T : Component
  {
    Transform transform = this.transform;
    T component;
    for (component = transform.GetComponent<T>(); (Object) component == (Object) null && (Object) transform.parent != (Object) null; component = transform.GetComponent<T>())
      transform = transform.parent;
    return component;
  }
}
