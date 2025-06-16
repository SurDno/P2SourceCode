// Decompiled with JetBrains decompiler
// Type: EnableOnAwake
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class EnableOnAwake : MonoBehaviour
{
  [SerializeField]
  private GameObject[] objects;

  private void Awake()
  {
    foreach (GameObject gameObject in this.objects)
    {
      if ((Object) gameObject != (Object) null)
        gameObject.SetActive(true);
    }
  }
}
