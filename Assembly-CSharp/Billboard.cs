// Decompiled with JetBrains decompiler
// Type: Billboard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Billboard : MonoBehaviour
{
  private Camera camera;

  private void Awake()
  {
    GameObject gameObjectWithTag = GameObject.FindGameObjectWithTag("MainCamera");
    if (!((Object) gameObjectWithTag != (Object) null))
      return;
    this.camera = gameObjectWithTag.GetComponent<Camera>();
  }

  private void Update()
  {
    if (!((Object) this.camera != (Object) null))
      return;
    this.transform.rotation = Quaternion.Euler(this.camera.transform.rotation.eulerAngles with
    {
      x = 0.0f,
      z = 0.0f
    });
  }
}
