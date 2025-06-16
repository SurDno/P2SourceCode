using System;
using System.Collections.Generic;
using Inspectors;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class KnifePostProcessEffect : MonoBehaviour {
	[SerializeField] private Texture scarTexture;
	[SerializeField] private Color bloodColor;
	private AudioSource audioSource;
	private List<Scar> scars = new();
	private Material material;
	private Scar zero;

	[Inspected]
	public void AddRandomScar() {
		var vector3_1 = gameObject.transform.right - gameObject.transform.up;
		var vector3_2 = gameObject.transform.position + gameObject.transform.forward + Vector3.up;
	}

	public void AddScar(
		Vector3 velocityInWorldSpace,
		Vector3 positionInWorldSpace,
		float strength,
		float time) {
		var vector3 = gameObject.transform.InverseTransformVector(velocityInWorldSpace);
		var magnitude = vector3.magnitude;
		var num = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
		var viewportPoint = gameObject.GetComponent<Camera>().WorldToViewportPoint(positionInWorldSpace);
		if (viewportPoint.z < 0.0)
			viewportPoint *= -1f;
		var vector2 = new Vector2(Mathf.Clamp(viewportPoint.x, 0.1f, 0.9f), Mathf.Clamp(viewportPoint.y, 0.1f, 0.9f));
		scars.Add(new Scar {
			Position = vector2,
			Scale = 1f,
			Rotation = -num,
			Strength = strength,
			Time = time,
			TimeLeft = time
		});
	}

	public void AddScar(Vector2 position, float scale, float rotation, float strength, float time) {
		scars.Add(new Scar {
			Position = position,
			Scale = scale,
			Rotation = rotation,
			Strength = strength,
			Time = time,
			TimeLeft = time
		});
	}

	private void Awake() {
		audioSource = GetComponent<AudioSource>();
		material = new Material(Shader.Find("Hidden/KnifePostProcessEffectShader"));
		zero = new Scar {
			Position = Vector2.zero,
			Scale = 1f,
			Rotation = 0.0f,
			Strength = 0.0f,
			Time = 1f,
			TimeLeft = 0.0f
		};
		material.SetTexture("_ScarTex", scarTexture);
		material.SetColor("_BloodColor", bloodColor);
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest) {
		material.SetFloat("_Aspect", src.width / (float)src.height);
		Graphics.Blit(src, dest, material);
	}

	private void OnPreRender() {
		var index = 0;
		while (index < scars.Count)
			if (scars[index].TimeLeft < (double)Time.deltaTime)
				scars.RemoveAt(index);
			else {
				scars[index].TimeLeft -= Time.deltaTime;
				++index;
			}

		while (scars.Count < 3)
			scars.Add(zero);
		material.SetFloat("_StrengthScar1", scars[0].Strength * scars[0].TimeLeft / scars[0].Time);
		material.SetFloat("_StrengthScar2", scars[1].Strength * scars[1].TimeLeft / scars[1].Time);
		material.SetFloat("_StrengthScar3", scars[2].Strength * scars[2].TimeLeft / scars[2].Time);
		material.SetVector("_PositionScar1", new Vector4 {
			x = scars[0].Position.x,
			y = scars[0].Position.y,
			z = Mathf.Sin(scars[0].Rotation * ((float)Math.PI / 180f)) / scars[0].Scale,
			w = Mathf.Cos(scars[0].Rotation * ((float)Math.PI / 180f)) / scars[0].Scale
		});
		material.SetVector("_PositionScar2", new Vector4 {
			x = scars[1].Position.x,
			y = scars[1].Position.y,
			z = Mathf.Sin(scars[1].Rotation * ((float)Math.PI / 180f)) / scars[1].Scale,
			w = Mathf.Cos(scars[1].Rotation * ((float)Math.PI / 180f)) / scars[1].Scale
		});
		material.SetVector("_PositionScar3", new Vector4 {
			x = scars[2].Position.x,
			y = scars[2].Position.y,
			z = Mathf.Sin(scars[2].Rotation * ((float)Math.PI / 180f)) / scars[2].Scale,
			w = Mathf.Cos(scars[2].Rotation * ((float)Math.PI / 180f)) / scars[2].Scale
		});
	}

	public class Scar {
		public Vector2 Position;
		public float Scale;
		public float Rotation;
		public float Strength;
		public float Time;
		public float TimeLeft;
	}
}