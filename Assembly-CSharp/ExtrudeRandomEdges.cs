// Decompiled with JetBrains decompiler
// Type: ExtrudeRandomEdges
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ProBuilder2.Common;
using ProBuilder2.MeshOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
public class ExtrudeRandomEdges : MonoBehaviour
{
  private pb_Object pb;
  private pb_Face lastExtrudedFace = (pb_Face) null;
  public float distance = 1f;

  private void Start()
  {
    this.pb = pb_ShapeGenerator.PlaneGenerator(1f, 1f, 0, 0, ProBuilder2.Common.Axis.Up, false);
    this.pb.SetFaceMaterial(this.pb.faces, pb_Constant.DefaultMaterial);
    this.lastExtrudedFace = this.pb.faces[0];
  }

  private void OnGUI()
  {
    if (!GUILayout.Button("Extrude Random Edge"))
      return;
    this.ExtrudeEdge();
  }

  private void ExtrudeEdge()
  {
    pb_Face sourceFace = this.lastExtrudedFace;
    List<pb_Edge> list = pb_WingedEdge.GetWingedEdges(this.pb, false).Where<pb_WingedEdge>((Func<pb_WingedEdge, bool>) (x => x.face == sourceFace)).Where<pb_WingedEdge>((Func<pb_WingedEdge, bool>) (x => x.opposite == null)).Select<pb_WingedEdge, pb_Edge>((Func<pb_WingedEdge, pb_Edge>) (y => y.edge.local)).ToList<pb_Edge>();
    int index = UnityEngine.Random.Range(0, list.Count);
    pb_Edge pbEdge = list[index];
    Vector3 vector3 = (this.pb.vertices[pbEdge.x] + this.pb.vertices[pbEdge.y]) * 0.5f - pb_Math.Average<int>((IList<int>) sourceFace.distinctIndices, (Func<int, Vector3>) (x => this.pb.vertices[x]), (IList<int>) null);
    vector3.Normalize();
    pb_Edge[] extrudedEdges;
    this.pb.Extrude(new pb_Edge[1]{ pbEdge }, 0.0f, false, true, out extrudedEdges);
    this.lastExtrudedFace = ((IEnumerable<pb_Face>) this.pb.faces).Last<pb_Face>();
    this.pb.SetSelectedEdges((IEnumerable<pb_Edge>) extrudedEdges);
    this.pb.TranslateVertices(this.pb.SelectedTriangles, vector3 * this.distance);
    this.pb.ToMesh();
    this.pb.Refresh();
  }
}
