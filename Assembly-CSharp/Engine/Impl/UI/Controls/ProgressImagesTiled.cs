using System;

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
        if ((UnityEngine.Object) tile.image != (UnityEngine.Object) null && (double) tile.mapping.x != (double) tile.mapping.y)
          tile.image.fillAmount = (float) ((Progress - (double) tile.mapping.x) / ((double) tile.mapping.y - (double) tile.mapping.x));
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
