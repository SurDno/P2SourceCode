using System;
using System.Collections.Generic;
using System.Linq;
using ProBuilder2.Common;
using ProBuilder2.MeshOperations;

public class ExtrudeRandomEdges : MonoBehaviour
{
  private pb_Object pb;
  private pb_Face lastExtrudedFace;
  public float distance = 1f;

  private void Start()
  {
    pb = pb_ShapeGenerator.PlaneGenerator(1f, 1f, 0, 0, Axis.Up, false);
    pb.SetFaceMaterial(pb.faces, pb_Constant.DefaultMaterial);
    lastExtrudedFace = pb.faces[0];
  }

  private void OnGUI()
  {
    if (!GUILayout.Button("Extrude Random Edge"))
      return;
    ExtrudeEdge();
  }

  private void ExtrudeEdge()
  {
    pb_Face sourceFace = lastExtrudedFace;
    List<pb_Edge> list = pb_WingedEdge.GetWingedEdges(pb).Where(x => x.face == sourceFace).Where(x => x.opposite == null).Select(y => y.edge.local).ToList();
    int index = UnityEngine.Random.Range(0, list.Count);
    pb_Edge pbEdge = list[index];
    Vector3 vector3 = (pb.vertices[pbEdge.x] + pb.vertices[pbEdge.y]) * 0.5f - pb_Math.Average<int>((IList<int>) sourceFace.distinctIndices, (Func<int, Vector3>) (x => pb.vertices[x]), (IList<int>) null);
    vector3.Normalize();
    pb_Edge[] extrudedEdges;
    pb.Extrude(new pb_Edge[1]{ pbEdge }, 0.0f, false, true, out extrudedEdges);
    lastExtrudedFace = pb.faces.Last();
    pb.SetSelectedEdges(extrudedEdges);
    pb.TranslateVertices(pb.SelectedTriangles, vector3 * distance);
    pb.ToMesh();
    pb.Refresh();
  }
}
