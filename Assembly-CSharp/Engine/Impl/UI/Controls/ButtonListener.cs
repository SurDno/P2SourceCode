// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ButtonListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  [RequireComponent(typeof (Button))]
  public class ButtonListener : MonoBehaviour
  {
    [SerializeField]
    private EventView view;

    private void Awake()
    {
      this.GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnClick));
    }

    private void OnClick() => this.view?.Invoke();
  }
}
