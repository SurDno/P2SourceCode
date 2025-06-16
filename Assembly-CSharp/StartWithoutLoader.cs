// Decompiled with JetBrains decompiler
// Type: StartWithoutLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable
public class StartWithoutLoader : MonoBehaviour
{
  [SerializeField]
  private GameObject[] prefabs;

  private void Awake()
  {
    if (SceneManager.GetActiveScene() == this.gameObject.scene)
    {
      int siblingIndex = this.transform.GetSiblingIndex();
      foreach (GameObject prefab in this.prefabs)
      {
        if (!((Object) prefab == (Object) null))
        {
          GameObject gameObject = Object.Instantiate<GameObject>(prefab);
          gameObject.name = prefab.name;
          gameObject.transform.SetSiblingIndex(siblingIndex);
          ++siblingIndex;
        }
      }
    }
    Object.Destroy((Object) this.gameObject);
  }
}
