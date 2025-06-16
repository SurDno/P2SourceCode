// Decompiled with JetBrains decompiler
// Type: OpenURLButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
[RequireComponent(typeof (Button))]
public class OpenURLButton : MonoBehaviour
{
  [SerializeField]
  private string url;

  private void Awake()
  {
    this.GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnClick));
  }

  private void OnClick() => Application.OpenURL(this.url);
}
