// Decompiled with JetBrains decompiler
// Type: UIRoot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.UI.Menu.Main;
using UnityEngine;

#nullable disable
public class UIRoot : MonoBehaviour
{
  [SerializeField]
  private GameObject root;
  [SerializeField]
  private GameObject[] prefabs;
  [Space]
  [SerializeField]
  private SmallLoading smallLoading;
  [SerializeField]
  private GameObject virtualCursor;

  public SmallLoading SmallLoading => this.smallLoading;

  public GameObject VirtualCursor => this.virtualCursor;

  public GameObject Root => this.root;

  public void Initialize()
  {
    Transform transform = this.root?.transform;
    if ((Object) transform == (Object) null)
      transform = this.transform;
    foreach (GameObject prefab in this.prefabs)
    {
      if (!((Object) prefab == (Object) null))
      {
        GameObject gameObject = Object.Instantiate<GameObject>(prefab, transform, false);
        gameObject.SetActive(false);
        gameObject.name = prefab.name;
      }
    }
  }
}
