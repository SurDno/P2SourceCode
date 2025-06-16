using System;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  [AddComponentMenu("UI/Gradient")]
  public class Gradient : Graphic
  {
    private static UIVertex[] vertexBuffer = new UIVertex[4];
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private Gradient.GradientDirection direction;
    [SerializeField]
    [Range(0.0f, 1f)]
    private float startPosition = 0.0f;
    [SerializeField]
    private Color startColor = Color.white;
    [SerializeField]
    [Range(0.0f, 1f)]
    private float endPosition = 1f;
    [SerializeField]
    private Color endColor = Color.white;

    public Sprite Sprite
    {
      get => this.sprite;
      set
      {
        if ((UnityEngine.Object) this.sprite == (UnityEngine.Object) value)
          return;
        this.sprite = value;
        this.SetVerticesDirty();
      }
    }

    public Gradient.GradientDirection Direction
    {
      get => this.direction;
      set
      {
        if (this.direction == value)
          return;
        this.direction = value;
        this.SetVerticesDirty();
      }
    }

    public float StartPosition
    {
      get => this.startPosition;
      set
      {
        value = Mathf.Clamp01(value);
        if ((double) this.startPosition == (double) value)
          return;
        this.startPosition = value;
        this.SetVerticesDirty();
      }
    }

    public float EndPosition
    {
      get => this.endPosition;
      set
      {
        value = Mathf.Clamp01(value);
        if ((double) this.endPosition == (double) value)
          return;
        this.endPosition = value;
        this.SetVerticesDirty();
      }
    }

    public Color StartColor
    {
      get => this.startColor;
      set
      {
        if (this.startColor == value)
          return;
        this.startColor = value;
        this.SetVerticesDirty();
      }
    }

    public Color EndColor
    {
      get => this.endColor;
      set
      {
        if (this.endColor == value)
          return;
        this.endColor = value;
        this.SetVerticesDirty();
      }
    }

    protected Gradient() => this.useLegacyMeshGeneration = false;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
      vh.Clear();
      Vector4 drawingDimensions = this.GetDrawingDimensions();
      if ((double) drawingDimensions.x >= (double) drawingDimensions.z || (double) drawingDimensions.y >= (double) drawingDimensions.w || (double) this.startPosition >= (double) this.endPosition)
        return;
      Vector4 corners = (UnityEngine.Object) this.sprite != (UnityEngine.Object) null ? DataUtility.GetOuterUV(this.sprite) : Vector4.zero;
      Vector4 vector4_1 = this.Crop(drawingDimensions);
      Vector4 vector4_2 = this.Crop(corners);
      Color color1 = this.startColor * this.color;
      Color color2 = this.endColor * this.color;
      vh.AddVert(new Vector3(vector4_1.x, vector4_1.y), (Color32) color1, new Vector2(vector4_2.x, vector4_2.y));
      vh.AddVert(new Vector3(vector4_1.x, vector4_1.w), (Color32) (this.direction == Gradient.GradientDirection.Horizontal ? color1 : color2), new Vector2(vector4_2.x, vector4_2.w));
      vh.AddVert(new Vector3(vector4_1.z, vector4_1.w), (Color32) color2, new Vector2(vector4_2.z, vector4_2.w));
      vh.AddVert(new Vector3(vector4_1.z, vector4_1.y), (Color32) (this.direction == Gradient.GradientDirection.Horizontal ? color2 : color1), new Vector2(vector4_2.z, vector4_2.y));
      vh.AddTriangle(0, 1, 2);
      vh.AddTriangle(2, 3, 0);
    }

    protected Vector4 Crop(Vector4 corners)
    {
      Vector4 vector4 = corners;
      if (this.direction == Gradient.GradientDirection.Horizontal)
      {
        vector4.x = (float) ((double) corners.x * (1.0 - (double) this.startPosition) + (double) corners.z * (double) this.startPosition);
        vector4.z = (float) ((double) corners.x * (1.0 - (double) this.endPosition) + (double) corners.z * (double) this.endPosition);
      }
      else
      {
        vector4.y = (float) ((double) corners.y * (1.0 - (double) this.startPosition) + (double) corners.w * (double) this.startPosition);
        vector4.w = (float) ((double) corners.y * (1.0 - (double) this.endPosition) + (double) corners.w * (double) this.endPosition);
      }
      return vector4;
    }

    public virtual void OnBeforeSerialize()
    {
    }

    public virtual void OnAfterDeserialize()
    {
    }

    public override Texture mainTexture
    {
      get
      {
        return (UnityEngine.Object) this.sprite == (UnityEngine.Object) null ? (Texture) Graphic.s_WhiteTexture : (Texture) this.sprite.texture;
      }
    }

    private Vector4 GetDrawingDimensions()
    {
      Rect pixelAdjustedRect = this.GetPixelAdjustedRect();
      Vector4 vector4 = (UnityEngine.Object) this.sprite == (UnityEngine.Object) null ? Vector4.zero : DataUtility.GetPadding(this.sprite);
      Vector2 vector2 = (UnityEngine.Object) this.sprite == (UnityEngine.Object) null ? new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) : new Vector2(this.sprite.rect.width, this.sprite.rect.height);
      int num1 = Mathf.RoundToInt(vector2.x);
      int num2 = Mathf.RoundToInt(vector2.y);
      Vector4 drawingDimensions = new Vector4(vector4.x / (float) num1, vector4.y / (float) num2, ((float) num1 - vector4.z) / (float) num1, ((float) num2 - vector4.w) / (float) num2);
      drawingDimensions = new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * drawingDimensions.x, pixelAdjustedRect.y + pixelAdjustedRect.height * drawingDimensions.y, pixelAdjustedRect.x + pixelAdjustedRect.width * drawingDimensions.z, pixelAdjustedRect.y + pixelAdjustedRect.height * drawingDimensions.w);
      return drawingDimensions;
    }

    [Serializable]
    public enum GradientDirection
    {
      Horizontal,
      Vertical,
    }
  }
}
