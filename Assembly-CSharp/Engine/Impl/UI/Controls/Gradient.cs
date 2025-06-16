using System;

namespace Engine.Impl.UI.Controls
{
  [AddComponentMenu("UI/Gradient")]
  public class Gradient : Graphic
  {
    private static UIVertex[] vertexBuffer = new UIVertex[4];
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private GradientDirection direction;
    [SerializeField]
    [Range(0.0f, 1f)]
    private float startPosition;
    [SerializeField]
    private Color startColor = Color.white;
    [SerializeField]
    [Range(0.0f, 1f)]
    private float endPosition = 1f;
    [SerializeField]
    private Color endColor = Color.white;

    public Sprite Sprite
    {
      get => sprite;
      set
      {
        if ((UnityEngine.Object) sprite == (UnityEngine.Object) value)
          return;
        sprite = value;
        this.SetVerticesDirty();
      }
    }

    public GradientDirection Direction
    {
      get => direction;
      set
      {
        if (direction == value)
          return;
        direction = value;
        this.SetVerticesDirty();
      }
    }

    public float StartPosition
    {
      get => startPosition;
      set
      {
        value = Mathf.Clamp01(value);
        if (startPosition == (double) value)
          return;
        startPosition = value;
        this.SetVerticesDirty();
      }
    }

    public float EndPosition
    {
      get => endPosition;
      set
      {
        value = Mathf.Clamp01(value);
        if (endPosition == (double) value)
          return;
        endPosition = value;
        this.SetVerticesDirty();
      }
    }

    public Color StartColor
    {
      get => startColor;
      set
      {
        if (startColor == value)
          return;
        startColor = value;
        this.SetVerticesDirty();
      }
    }

    public Color EndColor
    {
      get => endColor;
      set
      {
        if (endColor == value)
          return;
        endColor = value;
        this.SetVerticesDirty();
      }
    }

    protected Gradient() => this.useLegacyMeshGeneration = false;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
      vh.Clear();
      Vector4 drawingDimensions = GetDrawingDimensions();
      if ((double) drawingDimensions.x >= (double) drawingDimensions.z || (double) drawingDimensions.y >= (double) drawingDimensions.w || startPosition >= (double) endPosition)
        return;
      Vector4 corners = (UnityEngine.Object) sprite != (UnityEngine.Object) null ? DataUtility.GetOuterUV(sprite) : Vector4.zero;
      Vector4 vector4_1 = Crop(drawingDimensions);
      Vector4 vector4_2 = Crop(corners);
      Color color1 = startColor * this.color;
      Color color2 = endColor * this.color;
      vh.AddVert(new Vector3(vector4_1.x, vector4_1.y), (Color32) color1, new Vector2(vector4_2.x, vector4_2.y));
      vh.AddVert(new Vector3(vector4_1.x, vector4_1.w), (Color32) (direction == GradientDirection.Horizontal ? color1 : color2), new Vector2(vector4_2.x, vector4_2.w));
      vh.AddVert(new Vector3(vector4_1.z, vector4_1.w), (Color32) color2, new Vector2(vector4_2.z, vector4_2.w));
      vh.AddVert(new Vector3(vector4_1.z, vector4_1.y), (Color32) (direction == GradientDirection.Horizontal ? color2 : color1), new Vector2(vector4_2.z, vector4_2.y));
      vh.AddTriangle(0, 1, 2);
      vh.AddTriangle(2, 3, 0);
    }

    protected Vector4 Crop(Vector4 corners)
    {
      Vector4 vector4 = corners;
      if (direction == GradientDirection.Horizontal)
      {
        vector4.x = (float) ((double) corners.x * (1.0 - startPosition) + (double) corners.z * startPosition);
        vector4.z = (float) ((double) corners.x * (1.0 - endPosition) + (double) corners.z * endPosition);
      }
      else
      {
        vector4.y = (float) ((double) corners.y * (1.0 - startPosition) + (double) corners.w * startPosition);
        vector4.w = (float) ((double) corners.y * (1.0 - endPosition) + (double) corners.w * endPosition);
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
        return (UnityEngine.Object) sprite == (UnityEngine.Object) null ? (Texture) Graphic.s_WhiteTexture : (Texture) sprite.texture;
      }
    }

    private Vector4 GetDrawingDimensions()
    {
      Rect pixelAdjustedRect = this.GetPixelAdjustedRect();
      Vector4 vector4 = (UnityEngine.Object) sprite == (UnityEngine.Object) null ? Vector4.zero : DataUtility.GetPadding(sprite);
      Vector2 vector2 = (UnityEngine.Object) sprite == (UnityEngine.Object) null ? new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) : new Vector2(sprite.rect.width, sprite.rect.height);
      int num1 = Mathf.RoundToInt(vector2.x);
      int num2 = Mathf.RoundToInt(vector2.y);
      Vector4 drawingDimensions = new Vector4(vector4.x / (float) num1, vector4.y / (float) num2, ((float) num1 - vector4.z) / num1, ((float) num2 - vector4.w) / num2);
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
