// Decompiled with JetBrains decompiler
// Type: SRF.UI.InheritColour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace SRF.UI
{
  [RequireComponent(typeof (Graphic))]
  [ExecuteInEditMode]
  [AddComponentMenu("SRF/UI/Inherit Colour")]
  public class InheritColour : SRMonoBehaviour
  {
    private Graphic _graphic;
    public Graphic From;

    private Graphic Graphic
    {
      get
      {
        if ((Object) this._graphic == (Object) null)
          this._graphic = this.GetComponent<Graphic>();
        return this._graphic;
      }
    }

    private void Refresh()
    {
      if ((Object) this.From == (Object) null)
        return;
      this.Graphic.color = this.From.canvasRenderer.GetColor();
    }

    private void Update() => this.Refresh();

    private void Start() => this.Refresh();
  }
}
