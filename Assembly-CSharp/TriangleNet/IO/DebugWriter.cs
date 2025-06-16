using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using TriangleNet.Data;
using TriangleNet.Geometry;

namespace TriangleNet.IO;

internal class DebugWriter {
	private static NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;
	private static readonly DebugWriter instance = new();
	private int iteration;
	private string session;
	private StreamWriter stream;
	private string tmpFile;
	private int triangles;
	private int[] vertices;

	public static DebugWriter Session => instance;

	private DebugWriter() { }

	public void Start(string session) {
		iteration = 0;
		this.session = session;
		if (stream != null)
			throw new Exception("A session is active. Finish before starting a new.");
		tmpFile = Path.GetTempFileName();
		stream = new StreamWriter(tmpFile);
	}

	public void Write(Mesh mesh, bool skip = false) {
		WriteMesh(mesh, skip);
		triangles = mesh.Triangles.Count;
	}

	public void Finish() {
		Finish(session + ".mshx");
	}

	private void Finish(string path) {
		if (stream == null)
			return;
		stream.Flush();
		stream.Dispose();
		stream = null;
		using (var fileStream = new FileStream(path, FileMode.Create)) {
			using (var gzipStream = new GZipStream(fileStream, CompressionMode.Compress, false)) {
				var bytes = Encoding.UTF8.GetBytes("#!N" + iteration + Environment.NewLine);
				gzipStream.Write(bytes, 0, bytes.Length);
				var buffer = File.ReadAllBytes(tmpFile);
				gzipStream.Write(buffer, 0, buffer.Length);
			}
		}

		File.Delete(tmpFile);
	}

	private void WriteGeometry(InputGeometry geometry) {
		stream.WriteLine("#!G{0}", iteration++);
	}

	private void WriteMesh(Mesh mesh, bool skip) {
		if ((triangles == mesh.triangles.Count) & skip)
			return;
		stream.WriteLine("#!M{0}", iteration++);
		if (VerticesChanged(mesh)) {
			HashVertices(mesh);
			stream.WriteLine("{0}", mesh.vertices.Count);
			foreach (var vertex in mesh.vertices.Values)
				stream.WriteLine("{0} {1} {2} {3}", vertex.hash, vertex.x.ToString(nfi), vertex.y.ToString(nfi),
					vertex.mark);
		} else
			stream.WriteLine("0");

		stream.WriteLine("{0}", mesh.subsegs.Count);
		var osub = new Osub();
		osub.orient = 0;
		foreach (var segment in mesh.subsegs.Values)
			if (segment.hash > 0) {
				osub.seg = segment;
				var vertex1 = osub.Org();
				var vertex2 = osub.Dest();
				stream.WriteLine("{0} {1} {2} {3}", osub.seg.hash, vertex1.hash, vertex2.hash, osub.seg.boundary);
			}

		var otri = new Otri();
		var o2 = new Otri();
		otri.orient = 0;
		stream.WriteLine("{0}", mesh.triangles.Count);
		foreach (var triangle in mesh.triangles.Values) {
			otri.triangle = triangle;
			var vertex3 = otri.Org();
			var vertex4 = otri.Dest();
			var vertex5 = otri.Apex();
			var num1 = vertex3 == null ? -1 : vertex3.hash;
			var num2 = vertex4 == null ? -1 : vertex4.hash;
			var num3 = vertex5 == null ? -1 : vertex5.hash;
			stream.Write("{0} {1} {2} {3}", otri.triangle.hash, num1, num2, num3);
			otri.orient = 1;
			otri.Sym(ref o2);
			var hash1 = o2.triangle.hash;
			otri.orient = 2;
			otri.Sym(ref o2);
			var hash2 = o2.triangle.hash;
			otri.orient = 0;
			otri.Sym(ref o2);
			var hash3 = o2.triangle.hash;
			stream.WriteLine(" {0} {1} {2}", hash1, hash2, hash3);
		}
	}

	private bool VerticesChanged(Mesh mesh) {
		if (vertices == null || mesh.Vertices.Count != vertices.Length)
			return true;
		var num = 0;
		foreach (Point vertex in mesh.Vertices)
			if (vertex.id != vertices[num++])
				return true;
		return false;
	}

	private void HashVertices(Mesh mesh) {
		if (vertices == null || mesh.Vertices.Count != vertices.Length)
			vertices = new int[mesh.Vertices.Count];
		var num = 0;
		foreach (var vertex in mesh.Vertices)
			vertices[num++] = vertex.id;
	}
}