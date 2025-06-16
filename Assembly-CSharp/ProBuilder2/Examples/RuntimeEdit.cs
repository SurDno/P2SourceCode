using System.Collections.Generic;
using ProBuilder2.Common;

namespace ProBuilder2.Examples
{
  public class RuntimeEdit : MonoBehaviour
  {
    private pb_Selection currentSelection;
    private pb_Selection previousSelection;
    private pb_Object preview;
    public Material previewMaterial;
    private Vector2 mousePosition_initial = Vector2.zero;
    private bool dragging;
    public float rotateSpeed = 100f;

    private void Awake() => SpawnCube();

    private void OnGUI()
    {
      if (!GUI.Button(new Rect(5f, (float) (Screen.height - 25), 80f, 20f), "Reset"))
        return;
      currentSelection.Destroy();
      Object.Destroy((Object) preview.gameObject);
      SpawnCube();
    }

    private void SpawnCube()
    {
      pb_Object _pb = pb_ShapeGenerator.CubeGenerator(Vector3.one);
      _pb.gameObject.AddComponent<MeshCollider>().convex = false;
      currentSelection = new pb_Selection(_pb, null);
    }

    public void LateUpdate()
    {
      if (!currentSelection.HasObject())
        return;
      if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
      {
        mousePosition_initial = (Vector2) Input.mousePosition;
        dragging = true;
      }
      if (dragging)
      {
        Vector2 vector2 = (Vector2) ((Vector3) mousePosition_initial - Input.mousePosition);
        currentSelection.pb.gameObject.transform.RotateAround(Vector3.zero, new Vector3(vector2.y, vector2.x, 0.0f), rotateSpeed * Time.deltaTime);
        if (currentSelection.IsValid())
          RefreshSelectedFacePreview();
      }
      if (!Input.GetMouseButtonUp(1) && !Input.GetMouseButtonUp(0))
        return;
      dragging = false;
    }

    public void Update()
    {
      if (!Input.GetMouseButtonUp(0) || Input.GetKey(KeyCode.LeftAlt) || !FaceCheck(Input.mousePosition) || !currentSelection.IsValid())
        return;
      if (!currentSelection.Equals(previousSelection))
      {
        previousSelection = new pb_Selection(currentSelection.pb, currentSelection.face);
        RefreshSelectedFacePreview();
      }
      else
      {
        Vector3 vector3 = pb_Math.Normal((IList<Vector3>) currentSelection.pb.vertices.ValuesWithIndices<Vector3>(currentSelection.face.distinctIndices));
        if (Input.GetKey(KeyCode.LeftShift))
          currentSelection.pb.TranslateVertices(currentSelection.face.distinctIndices, vector3.normalized * -0.5f);
        else
          currentSelection.pb.TranslateVertices(currentSelection.face.distinctIndices, vector3.normalized * 0.5f);
        currentSelection.pb.Refresh();
        RefreshSelectedFacePreview();
      }
    }

    public bool FaceCheck(Vector3 pos)
    {
      Ray ray = Camera.main.ScreenPointToRay(pos);
      RaycastHit hitInfo;
      if (!Physics.Raycast(ray.origin, ray.direction, out hitInfo))
        return false;
      pb_Object component = hitInfo.transform.gameObject.GetComponent<pb_Object>();
      if ((Object) component == (Object) null)
        return false;
      Mesh msh = component.msh;
      int[] tri = new int[3]
      {
        msh.triangles[hitInfo.triangleIndex * 3],
        msh.triangles[hitInfo.triangleIndex * 3 + 1],
        msh.triangles[hitInfo.triangleIndex * 3 + 2]
      };
      currentSelection.pb = component;
      return component.FaceWithTriangle(tri, out currentSelection.face);
    }

    private void RefreshSelectedFacePreview()
    {
      Vector3[] v = currentSelection.pb.VerticesInWorldSpace(currentSelection.face.indices);
      int[] i = new int[v.Length];
      for (int index = 0; index < i.Length; ++index)
        i[index] = index;
      Vector3 vector3 = pb_Math.Normal((IList<Vector3>) v);
      for (int index = 0; index < v.Length; ++index)
        v[index] += vector3.normalized * 0.01f;
      if ((bool) (Object) preview)
        Object.Destroy((Object) preview.gameObject);
      preview = pb_Object.CreateInstanceWithVerticesFaces(v, new pb_Face[1]
      {
        new pb_Face(i)
      });
      preview.SetFaceMaterial(preview.faces, previewMaterial);
      preview.ToMesh();
      preview.Refresh();
    }

    private class pb_Selection
    {
      public pb_Object pb;
      public pb_Face face;

      public pb_Selection(pb_Object _pb, pb_Face _face)
      {
        pb = _pb;
        face = _face;
      }

      public bool HasObject() => (Object) pb != (Object) null;

      public bool IsValid() => (Object) pb != (Object) null && face != null;

      public bool Equals(pb_Selection sel)
      {
        return sel != null && sel.IsValid() && (Object) pb == (Object) sel.pb && face == sel.face;
      }

      public void Destroy()
      {
        if (!((Object) pb != (Object) null))
          return;
        Object.Destroy((Object) pb.gameObject);
      }

      public override string ToString()
      {
        return "pb_Object: " + pb == null ? "Null" : pb.name + "\npb_Face: " + (face == null ? "Null" : face.ToString());
      }
    }
  }
}
