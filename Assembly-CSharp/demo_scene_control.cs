using UnityEngine;

public class demo_scene_control : MonoBehaviour {
	public Transform c_point;
	public Transform c_point1;
	public Transform f_point;
	public Transform[] style1 = new Transform[8];
	public Transform[] style2 = new Transform[8];
	public Transform[] style3 = new Transform[8];
	public Transform[] style4 = new Transform[8];
	private GameObject current;
	private int style;
	private int cur_effect;
	private int max_n = 11;

	private void Start() {
		Restart();
		Application.targetFrameRate = 60;
	}

	private void Update() {
		transform.RotateAround(c_point1.transform.position, Vector3.up, 0.5f);
	}

	private void OnGUI() {
		var name = current.name;
		GUI.Label(new Rect(15f, 10f, 200f, 20f), name.Substring(0, name.Length - 7));
		if (GUI.Button(new Rect(290f, 30f, 90f, 30f), "Style1")) {
			style = 0;
			Restart();
		}

		if (GUI.Button(new Rect(390f, 30f, 90f, 30f), "Style 2")) {
			style = 1;
			Restart();
		}

		if (GUI.Button(new Rect(490f, 30f, 90f, 30f), "Style 3")) {
			style = 2;
			Restart();
		}

		if (GUI.Button(new Rect(590f, 30f, 90f, 30f), "Style 4")) {
			style = 3;
			Restart();
		}

		if (GUI.Button(new Rect(10f, 30f, 40f, 30f), "<-")) {
			if (cur_effect <= 0)
				cur_effect = max_n;
			else
				--cur_effect;
			Restart();
		}

		if (!GUI.Button(new Rect(60f, 30f, 40f, 30f), "->"))
			return;
		if (cur_effect >= max_n)
			cur_effect = 0;
		else
			++cur_effect;
		Restart();
	}

	private void Restart() {
		var cPoint = c_point;
		Destroy(current);
		if (cur_effect == 4 || cur_effect == 5) {
			cPoint.transform.eulerAngles = new Vector3(0.0f, -90f, 0.0f);
			cPoint.transform.position = c_point1.transform.position + new Vector3(1f, 1f, 0.0f);
		} else if (cur_effect == 6 || cur_effect == 7) {
			cPoint.transform.eulerAngles = c_point1.transform.eulerAngles;
			cPoint.transform.position = c_point1.transform.position + new Vector3(0.0f, 1f, 0.0f);
		} else if (cur_effect == 10 || cur_effect == 11) {
			cPoint.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
			cPoint.transform.position = c_point1.transform.position + new Vector3(0.0f, 1f, 0.0f);
		} else {
			cPoint.transform.eulerAngles = c_point1.transform.eulerAngles;
			cPoint.transform.position = c_point1.transform.position;
		}

		if (style == 0)
			current = Instantiate(style1[cur_effect], cPoint.transform.position, cPoint.transform.rotation).gameObject;
		if (style == 1)
			current = Instantiate(style2[cur_effect], cPoint.transform.position, cPoint.transform.rotation).gameObject;
		if (style == 2)
			current = Instantiate(style3[cur_effect], cPoint.transform.position, cPoint.transform.rotation).gameObject;
		if (style != 3)
			return;
		current = Instantiate(style4[cur_effect], cPoint.transform.position, cPoint.transform.rotation).gameObject;
	}
}