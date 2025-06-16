using ProBuilder2.Common;

public class HighlightNearestFace : MonoBehaviour
{
  public float travel = 50f;
  public float speed = 0.2f;
  private pb_Object target;
  private pb_Face nearest = null;

  private void Start()
  {
    target = pb_ShapeGenerator.PlaneGenerator(travel, travel, 25, 25, Axis.Up, false);
    target.SetFaceMaterial(target.faces, pb_Constant.DefaultMaterial);
    target.transform.position = new Vector3(travel * 0.5f, 0.0f, travel * 0.5f);
    target.ToMesh();
    target.Refresh();
    Camera main = Camera.main;
    main.transform.position = new Vector3(25f, 40f, 0.0f);
    main.transform.localRotation = Quaternion.Euler(new Vector3(65f, 0.0f, 0.0f));
  }

  private void Update()
  {
    float num1 = Time.time * speed;
    this.transform.position = new Vector3(Mathf.PerlinNoise(num1, num1) * travel, 2f, Mathf.PerlinNoise(num1 + 1f, num1 + 1f) * travel);
    if ((Object) target == (Object) null)
    {
      Debug.LogWarning((object) "Missing the ProBuilder Mesh target!");
    }
    else
    {
      Vector3 a = target.transform.InverseTransformPoint(this.transform.position);
      if (nearest != null)
        target.SetFaceColor(nearest, Color.white);
      int length = target.faces.Length;
      float num2 = float.PositiveInfinity;
      nearest = target.faces[0];
      for (int index = 0; index < length; ++index)
      {
        float num3 = Vector3.Distance(a, FaceCenter(target, target.faces[index]));
        if (num3 < (double) num2)
        {
          num2 = num3;
          nearest = target.faces[index];
        }
      }
      target.SetFaceColor(nearest, Color.blue);
      target.RefreshColors();
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
    float length = face.distinctIndices.Length;
    zero.x /= length;
    zero.y /= length;
    zero.z /= length;
    return zero;
  }
}
