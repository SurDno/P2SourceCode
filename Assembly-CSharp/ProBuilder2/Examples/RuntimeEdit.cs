// Decompiled with JetBrains decompiler
// Type: ProBuilder2.Examples.RuntimeEdit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ProBuilder2.Common;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace ProBuilder2.Examples
{
  public class RuntimeEdit : MonoBehaviour
  {
    private RuntimeEdit.pb_Selection currentSelection;
    private RuntimeEdit.pb_Selection previousSelection;
    private pb_Object preview;
    public Material previewMaterial;
    private Vector2 mousePosition_initial = Vector2.zero;
    private bool dragging = false;
    public float rotateSpeed = 100f;

    private void Awake() => this.SpawnCube();

    private void OnGUI()
    {
      if (!GUI.Button(new Rect(5f, (float) (Screen.height - 25), 80f, 20f), "Reset"))
        return;
      this.currentSelection.Destroy();
      Object.Destroy((Object) this.preview.gameObject);
      this.SpawnCube();
    }

    private void SpawnCube()
    {
      pb_Object _pb = pb_ShapeGenerator.CubeGenerator(Vector3.one);
      _pb.gameObject.AddComponent<MeshCollider>().convex = false;
      this.currentSelection = new RuntimeEdit.pb_Selection(_pb, (pb_Face) null);
    }

    public void LateUpdate()
    {
      if (!this.currentSelection.HasObject())
        return;
      if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
      {
        this.mousePosition_initial = (Vector2) Input.mousePosition;
        this.dragging = true;
      }
      if (this.dragging)
      {
        Vector2 vector2 = (Vector2) ((Vector3) this.mousePosition_initial - Input.mousePosition);
        this.currentSelection.pb.gameObject.transform.RotateAround(Vector3.zero, new Vector3(vector2.y, vector2.x, 0.0f), this.rotateSpeed * Time.deltaTime);
        if (this.currentSelection.IsValid())
          this.RefreshSelectedFacePreview();
      }
      if (!Input.GetMouseButtonUp(1) && !Input.GetMouseButtonUp(0))
        return;
      this.dragging = false;
    }

    public void Update()
    {
      if (!Input.GetMouseButtonUp(0) || Input.GetKey(KeyCode.LeftAlt) || !this.FaceCheck(Input.mousePosition) || !this.currentSelection.IsValid())
        return;
      if (!this.currentSelection.Equals(this.previousSelection))
      {
        this.previousSelection = new RuntimeEdit.pb_Selection(this.currentSelection.pb, this.currentSelection.face);
        this.RefreshSelectedFacePreview();
      }
      else
      {
        Vector3 vector3 = pb_Math.Normal((IList<Vector3>) this.currentSelection.pb.vertices.ValuesWithIndices<Vector3>(this.currentSelection.face.distinctIndices));
        if (Input.GetKey(KeyCode.LeftShift))
          this.currentSelection.pb.TranslateVertices(this.currentSelection.face.distinctIndices, vector3.normalized * -0.5f);
        else
          this.currentSelection.pb.TranslateVertices(this.currentSelection.face.distinctIndices, vector3.normalized * 0.5f);
        this.currentSelection.pb.Refresh();
        this.RefreshSelectedFacePreview();
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
      this.currentSelection.pb = component;
      return component.FaceWithTriangle(tri, out this.currentSelection.face);
    }

    private void RefreshSelectedFacePreview()
    {
      Vector3[] v = this.currentSelection.pb.VerticesInWorldSpace(this.currentSelection.face.indices);
      int[] i = new int[v.Length];
      for (int index = 0; index < i.Length; ++index)
        i[index] = index;
      Vector3 vector3 = pb_Math.Normal((IList<Vector3>) v);
      for (int index = 0; index < v.Length; ++index)
        v[index] += vector3.normalized * 0.01f;
      if ((bool) (Object) this.preview)
        Object.Destroy((Object) this.preview.gameObject);
      this.preview = pb_Object.CreateInstanceWithVerticesFaces(v, new pb_Face[1]
      {
        new pb_Face(i)
      });
      this.preview.SetFaceMaterial(this.preview.faces, this.previewMaterial);
      this.preview.ToMesh();
      this.preview.Refresh();
    }

    private class pb_Selection
    {
      public pb_Object pb;
      public pb_Face face;

      public pb_Selection(pb_Object _pb, pb_Face _face)
      {
        this.pb = _pb;
        this.face = _face;
      }

      public bool HasObject() => (Object) this.pb != (Object) null;

      public bool IsValid() => (Object) this.pb != (Object) null && this.face != null;

      public bool Equals(RuntimeEdit.pb_Selection sel)
      {
        return sel != null && sel.IsValid() && (Object) this.pb == (Object) sel.pb && this.face == sel.face;
      }

      public void Destroy()
      {
        if (!((Object) this.pb != (Object) null))
          return;
        Object.Destroy((Object) this.pb.gameObject);
      }

      public override string ToString()
      {
        return "pb_Object: " + (object) this.pb == null ? "Null" : this.pb.name + "\npb_Face: " + (this.face == null ? "Null" : ((object) this.face).ToString());
      }
    }
  }
}
