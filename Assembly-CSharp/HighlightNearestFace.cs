// Decompiled with JetBrains decompiler
// Type: HighlightNearestFace
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ProBuilder2.Common;
using UnityEngine;

#nullable disable
public class HighlightNearestFace : MonoBehaviour
{
  public float travel = 50f;
  public float speed = 0.2f;
  private pb_Object target;
  private pb_Face nearest = (pb_Face) null;

  private void Start()
  {
    this.target = pb_ShapeGenerator.PlaneGenerator(this.travel, this.travel, 25, 25, ProBuilder2.Common.Axis.Up, false);
    this.target.SetFaceMaterial(this.target.faces, pb_Constant.DefaultMaterial);
    this.target.transform.position = new Vector3(this.travel * 0.5f, 0.0f, this.travel * 0.5f);
    this.target.ToMesh();
    this.target.Refresh();
    Camera main = Camera.main;
    main.transform.position = new Vector3(25f, 40f, 0.0f);
    main.transform.localRotation = Quaternion.Euler(new Vector3(65f, 0.0f, 0.0f));
  }

  private void Update()
  {
    float num1 = Time.time * this.speed;
    this.transform.position = new Vector3(Mathf.PerlinNoise(num1, num1) * this.travel, 2f, Mathf.PerlinNoise(num1 + 1f, num1 + 1f) * this.travel);
    if ((Object) this.target == (Object) null)
    {
      Debug.LogWarning((object) "Missing the ProBuilder Mesh target!");
    }
    else
    {
      Vector3 a = this.target.transform.InverseTransformPoint(this.transform.position);
      if (this.nearest != null)
        this.target.SetFaceColor(this.nearest, Color.white);
      int length = this.target.faces.Length;
      float num2 = float.PositiveInfinity;
      this.nearest = this.target.faces[0];
      for (int index = 0; index < length; ++index)
      {
        float num3 = Vector3.Distance(a, this.FaceCenter(this.target, this.target.faces[index]));
        if ((double) num3 < (double) num2)
        {
          num2 = num3;
          this.nearest = this.target.faces[index];
        }
      }
      this.target.SetFaceColor(this.nearest, Color.blue);
      this.target.RefreshColors();
    }
  }

  private Vector3 FaceCenter(pb_Object pb, pb_Face face)
  {
    Vector3[] vertices = pb.vertices;
    Vector3 zero = Vector3.zero;
    foreach (int distinctIndex in face.distinctIndices)
    {
      zero.x += vertices[distinctIndex].x;
      zero.y += vertices[distinctIndex].y;
      zero.z += vertices[distinctIndex].z;
    }
    float length = (float) face.distinctIndices.Length;
    zero.x /= length;
    zero.y /= length;
    zero.z /= length;
    return zero;
  }
}
