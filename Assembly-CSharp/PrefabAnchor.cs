// Decompiled with JetBrains decompiler
// Type: PrefabAnchor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class PrefabAnchor : MonoBehaviour
{
  [SerializeField]
  private GameObject prefab;
  [SerializeField]
  private bool worldSpace = false;
  [SerializeField]
  private bool destroyOnDisable = false;
  private GameObject instance;

  private void OnEnable()
  {
    if ((Object) this.instance == (Object) null)
    {
      if (!((Object) this.prefab != (Object) null))
        return;
      this.instance = !this.worldSpace ? Object.Instantiate<GameObject>(this.prefab, this.transform, false) : Object.Instantiate<GameObject>(this.prefab);
      this.instance.name = this.prefab.name;
    }
    else
    {
      if (!this.worldSpace)
        return;
      this.instance.SetActive(true);
    }
  }

  private void OnDisable()
  {
    if ((Object) this.instance == (Object) null)
      return;
    if (this.destroyOnDisable)
    {
      Object.Destroy((Object) this.instance);
      this.instance = (GameObject) null;
    }
    else
    {
      if (!this.worldSpace)
        return;
      this.instance.SetActive(false);
    }
  }
}
