using System;
using UnityEngine;
using UnityEngine.UI;

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
