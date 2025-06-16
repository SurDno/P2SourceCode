// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ProgressImagesTiled
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ProgressImagesTiled : ProgressView
  {
    [SerializeField]
    private ProgressImagesTiled.Tile[] tiles;

    protected override void ApplyProgress()
    {
      if (this.tiles == null)
        return;
      foreach (ProgressImagesTiled.Tile tile in this.tiles)
      {
        if ((UnityEngine.Object) tile.image != (UnityEngine.Object) null && (double) tile.mapping.x != (double) tile.mapping.y)
          tile.image.fillAmount = (float) (((double) this.Progress - (double) tile.mapping.x) / ((double) tile.mapping.y - (double) tile.mapping.x));
      }
    }

    public override void SkipAnimation()
    {
    }

    [Serializable]
    public struct Tile
    {
      public Image image;
      public Vector2 mapping;
    }
  }
}
