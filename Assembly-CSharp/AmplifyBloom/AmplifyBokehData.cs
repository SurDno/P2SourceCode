// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.AmplifyBokehData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace AmplifyBloom
{
  [Serializable]
  public class AmplifyBokehData
  {
    internal RenderTexture BokehRenderTexture;
    internal Vector4[] Offsets;

    public AmplifyBokehData(Vector4[] offsets) => this.Offsets = offsets;

    public void Destroy()
    {
      if ((UnityEngine.Object) this.BokehRenderTexture != (UnityEngine.Object) null)
      {
        AmplifyUtils.ReleaseTempRenderTarget(this.BokehRenderTexture);
        this.BokehRenderTexture = (RenderTexture) null;
      }
      this.Offsets = (Vector4[]) null;
    }
  }
}
