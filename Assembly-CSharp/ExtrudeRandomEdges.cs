using System.Collections.Generic;
using System.Linq;
using ProBuilder2.Common;
using ProBuilder2.MeshOperations;
using UnityEngine;

public class ExtrudeRandomEdges : MonoBehaviour {
	private pb_Object pb;
	private pb_Face lastExtrudedFace;
	public float distance = 1f;

	private void Start() {
		pb = pb_ShapeGenerator.PlaneGenerator(1f, 1f, 0, 0, Axis.Up, false);
		pb.SetFaceMaterial(pb.faces, pb_Constant.DefaultMaterial);
		lastExtrudedFace = pb.faces[0];
	}

	private void OnGUI() {
		if (!GUILayout.Button("Extrude Random Edge"))
			return;
		ExtrudeEdge();
	}

	private void ExtrudeEdge() {
		var sourceFace = lastExtrudedFace;
		var list = pb_WingedEdge.GetWingedEdges(pb).Where(x => x.face == sourceFace).Where(x => x.opposite == null)
			.Select(y => y.edge.local).ToList();
		var index = Random.Range(0, list.Count);
		var pbEdge = list[index];
		var vector3 = (pb.vertices[pbEdge.x] + pb.vertices[pbEdge.y]) * 0.5f -
		              sourceFace.distinctIndices.Average(x => pb.vertices[x]);
		vector3.Normalize();
		pb_Edge[] extrudedEdges;
		pb.Extrude(new pb_Edge[1] { pbEdge }, 0.0f, false, true, out extrudedEdges);
		lastExtrudedFace = pb.faces.Last();
		pb.SetSelectedEdges(extrudedEdges);
		pb.TranslateVertices(pb.SelectedTriangles, vector3 * distance);
		pb.ToMesh();
		pb.Refresh();
	}
}