using ProBuilder2.Common;
using UnityEngine;

namespace ProBuilder2.Examples;

public class RuntimeEdit : MonoBehaviour {
	private pb_Selection currentSelection;
	private pb_Selection previousSelection;
	private pb_Object preview;
	public Material previewMaterial;
	private Vector2 mousePosition_initial = Vector2.zero;
	private bool dragging;
	public float rotateSpeed = 100f;

	private void Awake() {
		SpawnCube();
	}

	private void OnGUI() {
		if (!GUI.Button(new Rect(5f, Screen.height - 25, 80f, 20f), "Reset"))
			return;
		currentSelection.Destroy();
		Destroy(preview.gameObject);
		SpawnCube();
	}

	private void SpawnCube() {
		var _pb = pb_ShapeGenerator.CubeGenerator(Vector3.one);
		_pb.gameObject.AddComponent<MeshCollider>().convex = false;
		currentSelection = new pb_Selection(_pb, null);
	}

	public void LateUpdate() {
		if (!currentSelection.HasObject())
			return;
		if (Input.GetMouseButtonDown(1) || (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))) {
			mousePosition_initial = Input.mousePosition;
			dragging = true;
		}

		if (dragging) {
			Vector2 vector2 = (Vector3)mousePosition_initial - Input.mousePosition;
			currentSelection.pb.gameObject.transform.RotateAround(Vector3.zero, new Vector3(vector2.y, vector2.x, 0.0f),
				rotateSpeed * Time.deltaTime);
			if (currentSelection.IsValid())
				RefreshSelectedFacePreview();
		}

		if (!Input.GetMouseButtonUp(1) && !Input.GetMouseButtonUp(0))
			return;
		dragging = false;
	}

	public void Update() {
		if (!Input.GetMouseButtonUp(0) || Input.GetKey(KeyCode.LeftAlt) || !FaceCheck(Input.mousePosition) ||
		    !currentSelection.IsValid())
			return;
		if (!currentSelection.Equals(previousSelection)) {
			previousSelection = new pb_Selection(currentSelection.pb, currentSelection.face);
			RefreshSelectedFacePreview();
		} else {
			var vector3 =
				pb_Math.Normal(currentSelection.pb.vertices.ValuesWithIndices(currentSelection.face.distinctIndices));
			if (Input.GetKey(KeyCode.LeftShift))
				currentSelection.pb.TranslateVertices(currentSelection.face.distinctIndices,
					vector3.normalized * -0.5f);
			else
				currentSelection.pb.TranslateVertices(currentSelection.face.distinctIndices, vector3.normalized * 0.5f);
			currentSelection.pb.Refresh();
			RefreshSelectedFacePreview();
		}
	}

	public bool FaceCheck(Vector3 pos) {
		var ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray.origin, ray.direction, out hitInfo))
			return false;
		var component = hitInfo.transform.gameObject.GetComponent<pb_Object>();
		if (component == null)
			return false;
		var msh = component.msh;
		var tri = new int[3] {
			msh.triangles[hitInfo.triangleIndex * 3],
			msh.triangles[hitInfo.triangleIndex * 3 + 1],
			msh.triangles[hitInfo.triangleIndex * 3 + 2]
		};
		currentSelection.pb = component;
		return component.FaceWithTriangle(tri, out currentSelection.face);
	}

	private void RefreshSelectedFacePreview() {
		var v = currentSelection.pb.VerticesInWorldSpace(currentSelection.face.indices);
		var i = new int[v.Length];
		for (var index = 0; index < i.Length; ++index)
			i[index] = index;
		var vector3 = pb_Math.Normal(v);
		for (var index = 0; index < v.Length; ++index)
			v[index] += vector3.normalized * 0.01f;
		if ((bool)(Object)preview)
			Destroy(preview.gameObject);
		preview = pb_Object.CreateInstanceWithVerticesFaces(v, new pb_Face[1] {
			new(i)
		});
		preview.SetFaceMaterial(preview.faces, previewMaterial);
		preview.ToMesh();
		preview.Refresh();
	}

	private class pb_Selection {
		public pb_Object pb;
		public pb_Face face;

		public pb_Selection(pb_Object _pb, pb_Face _face) {
			pb = _pb;
			face = _face;
		}

		public bool HasObject() {
			return pb != null;
		}

		public bool IsValid() {
			return pb != null && face != null;
		}

		public bool Equals(pb_Selection sel) {
			return sel != null && sel.IsValid() && pb == sel.pb && face == sel.face;
		}

		public void Destroy() {
			if (!(pb != null))
				return;
			Object.Destroy(pb.gameObject);
		}

		public override string ToString() {
			return "pb_Object: " + pb == null
				? "Null"
				: pb.name + "\npb_Face: " + (face == null ? "Null" : face.ToString());
		}
	}
}