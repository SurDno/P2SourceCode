using System;
using System.Collections.Generic;
using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.Log;

namespace TriangleNet.IO;

internal static class DataReader {
	public static int Reconstruct(Mesh mesh, InputGeometry input, ITriangle[] triangles) {
		var newotri = new Otri();
		var o2_1 = new Otri();
		var otri1 = new Otri();
		var o2_2 = new Otri();
		var o2_3 = new Otri();
		var osub = new Osub();
		var numArray1 = new int[3];
		var numArray2 = new int[2];
		var length = triangles == null ? 0 : triangles.Length;
		var count = input.segments.Count;
		mesh.inelements = length;
		mesh.regions.AddRange(input.regions);
		for (var index = 0; index < mesh.inelements; ++index)
			mesh.MakeTriangle(ref newotri);
		if (mesh.behavior.Poly) {
			mesh.insegments = count;
			for (var index = 0; index < mesh.insegments; ++index)
				mesh.MakeSegment(ref osub);
		}

		var otriListArray = new List<Otri>[mesh.vertices.Count];
		for (var index = 0; index < mesh.vertices.Count; ++index) {
			var otri2 = new Otri();
			otri2.triangle = Mesh.dummytri;
			otriListArray[index] = new List<Otri>(3);
			otriListArray[index].Add(otri2);
		}

		var index1 = 0;
		foreach (var triangle in mesh.triangles.Values) {
			newotri.triangle = triangle;
			numArray1[0] = triangles[index1].P0;
			numArray1[1] = triangles[index1].P1;
			numArray1[2] = triangles[index1].P2;
			for (var index2 = 0; index2 < 3; ++index2)
				if (numArray1[index2] < 0 || numArray1[index2] >= mesh.invertices) {
					SimpleLog.Instance.Error("Triangle has an invalid vertex index.", "MeshReader.Reconstruct()");
					throw new Exception("Triangle has an invalid vertex index.");
				}

			newotri.triangle.region = triangles[index1].Region;
			if (mesh.behavior.VarArea)
				newotri.triangle.area = triangles[index1].Area;
			newotri.orient = 0;
			newotri.SetOrg(mesh.vertices[numArray1[0]]);
			newotri.SetDest(mesh.vertices[numArray1[1]]);
			newotri.SetApex(mesh.vertices[numArray1[2]]);
			for (newotri.orient = 0; newotri.orient < 3; ++newotri.orient) {
				var index3 = numArray1[newotri.orient];
				var index4 = otriListArray[index3].Count - 1;
				var otri3 = otriListArray[index3][index4];
				otriListArray[index3].Add(newotri);
				var o2_4 = otri3;
				if (o2_4.triangle != Mesh.dummytri) {
					var vertex1 = newotri.Dest();
					var vertex2 = newotri.Apex();
					do {
						var vertex3 = o2_4.Dest();
						var vertex4 = o2_4.Apex();
						if (vertex2 == vertex3) {
							newotri.Lprev(ref o2_1);
							o2_1.Bond(ref o2_4);
						}

						if (vertex1 == vertex4) {
							o2_4.Lprev(ref o2_2);
							newotri.Bond(ref o2_2);
						}

						--index4;
						o2_4 = otriListArray[index3][index4];
					} while (o2_4.triangle != Mesh.dummytri);
				}
			}

			++index1;
		}

		var num = 0;
		if (mesh.behavior.Poly) {
			var index5 = 0;
			foreach (var segment in mesh.subsegs.Values) {
				osub.seg = segment;
				numArray2[0] = input.segments[index5].P0;
				numArray2[1] = input.segments[index5].P1;
				var boundary = input.segments[index5].Boundary;
				for (var index6 = 0; index6 < 2; ++index6)
					if (numArray2[index6] < 0 || numArray2[index6] >= mesh.invertices) {
						SimpleLog.Instance.Error("Segment has an invalid vertex index.", "MeshReader.Reconstruct()");
						throw new Exception("Segment has an invalid vertex index.");
					}

				osub.orient = 0;
				var vertex5 = mesh.vertices[numArray2[0]];
				var vertex6 = mesh.vertices[numArray2[1]];
				osub.SetOrg(vertex5);
				osub.SetDest(vertex6);
				osub.SetSegOrg(vertex5);
				osub.SetSegDest(vertex6);
				osub.seg.boundary = boundary;
				for (osub.orient = 0; osub.orient < 2; ++osub.orient) {
					var index7 = numArray2[1 - osub.orient];
					var index8 = otriListArray[index7].Count - 1;
					var otri4 = otriListArray[index7][index8];
					var tri = otriListArray[index7][index8];
					var vertex7 = osub.Org();
					for (var flag = true; flag && tri.triangle != Mesh.dummytri; tri = otriListArray[index7][index8]) {
						var vertex8 = tri.Dest();
						if (vertex7 == vertex8) {
							otriListArray[index7].Remove(otri4);
							tri.SegBond(ref osub);
							tri.Sym(ref o2_3);
							if (o2_3.triangle == Mesh.dummytri) {
								mesh.InsertSubseg(ref tri, 1);
								++num;
							}

							flag = false;
						}

						--index8;
						otri4 = otriListArray[index7][index8];
					}
				}

				++index5;
			}
		}

		for (var index9 = 0; index9 < mesh.vertices.Count; ++index9) {
			var index10 = otriListArray[index9].Count - 1;
			Otri otri5;
			for (var tri = otriListArray[index9][index10]; tri.triangle != Mesh.dummytri; tri = otri5) {
				--index10;
				otri5 = otriListArray[index9][index10];
				tri.SegDissolve();
				tri.Sym(ref o2_3);
				if (o2_3.triangle == Mesh.dummytri) {
					mesh.InsertSubseg(ref tri, 1);
					++num;
				}
			}
		}

		return num;
	}
}