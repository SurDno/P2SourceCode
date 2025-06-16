using System;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  public class ProgressImagesTiled : ProgressView
  {
    [SerializeField]
    private Tile[] tiles;

    protected override void ApplyProgress()
    {
      if (tiles == null)
        return;
      foreach (Tile tile in tiles)
      {
        if (tile.image != null && tile.mapping.x != (double) tile.mapping.y)
          tile.image.fillAmount = (float) ((Progress - (double) tile.mapping.x) / (tile.mapping.y - (double) tile.mapping.x));
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
