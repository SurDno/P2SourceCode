// Decompiled with JetBrains decompiler
// Type: PlagueWeb
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public abstract class PlagueWeb : MonoBehaviour
{
  public static PlagueWeb Instance { get; private set; }

  private void Awake() => PlagueWeb.Instance = this;

  public abstract Vector3 CameraPosition { get; set; }

  public abstract bool IsActive { get; set; }

  public abstract IPlagueWebPoint AddPoint(
    Vector3 position,
    Vector3 directionality,
    float strength);

  public abstract void RemovePoint(IPlagueWebPoint point);
}
