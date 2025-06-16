using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class VolumetricLightRenderer : MonoBehaviour {
	private static Mesh _pointLightMesh;
	private static Mesh _spotLightMesh;
	private static Material _lightMaterial;
	private Camera _camera;
	private CommandBuffer _preLightPass;
	private Matrix4x4 _viewProj;
	private Material _blitAddMaterial;
	private Material _bilateralBlurMaterial;
	private RenderTexture _volumeLightTexture;
	private RenderTexture _halfVolumeLightTexture;
	private RenderTexture _quarterVolumeLightTexture;
	private static Texture _defaultSpotCookie;
	private RenderTexture _halfDepthBuffer;
	private RenderTexture _quarterDepthBuffer;
	private VolumtericResolution _currentResolution = VolumtericResolution.Half;
	private Texture2D _ditheringTexture;
	private Texture3D _noiseTexture;
	public VolumtericResolution Resolution = VolumtericResolution.Half;
	public Texture DefaultSpotCookie;

	public static event Action<VolumetricLightRenderer, Matrix4x4> PreRenderEvent;

	public static event Action PostRenderEvent;

	public CommandBuffer GlobalCommandBuffer => _preLightPass;

	public static Material GetLightMaterial() {
		return _lightMaterial;
	}

	public static Mesh GetPointLightMesh() {
		return _pointLightMesh;
	}

	public static Mesh GetSpotLightMesh() {
		return _spotLightMesh;
	}

	public RenderTexture GetVolumeLightBuffer() {
		if (Resolution == VolumtericResolution.Quarter)
			return _quarterVolumeLightTexture;
		return Resolution == VolumtericResolution.Half ? _halfVolumeLightTexture : _volumeLightTexture;
	}

	public RenderTexture GetVolumeLightDepthBuffer() {
		if (Resolution == VolumtericResolution.Quarter)
			return _quarterDepthBuffer;
		return Resolution == VolumtericResolution.Half ? _halfDepthBuffer : null;
	}

	public static Texture GetDefaultSpotCookie() {
		return _defaultSpotCookie;
	}

	private void Awake() {
		_camera = GetComponent<Camera>();
		if (_camera.actualRenderingPath == RenderingPath.Forward)
			_camera.depthTextureMode = DepthTextureMode.Depth;
		_currentResolution = Resolution;
		var shader1 = Shader.Find("Hidden/BlitAdd");
		_blitAddMaterial = !(shader1 == null)
			? new Material(shader1)
			: throw new Exception(
				"Critical Error: \"Hidden/BlitAdd\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
		var shader2 = Shader.Find("Hidden/BilateralBlur");
		_bilateralBlurMaterial = !(shader2 == null)
			? new Material(shader2)
			: throw new Exception(
				"Critical Error: \"Hidden/BilateralBlur\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
		_preLightPass = new CommandBuffer();
		_preLightPass.name = "PreLight";
		ChangeResolution();
		if (_pointLightMesh == null) {
			var primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			_pointLightMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
			Destroy(primitive);
		}

		if (_spotLightMesh == null)
			_spotLightMesh = CreateSpotLightMesh();
		if (_lightMaterial == null) {
			var shader3 = Shader.Find("Sandbox/VolumetricLight");
			_lightMaterial = !(shader3 == null)
				? new Material(shader3)
				: throw new Exception(
					"Critical Error: \"Sandbox/VolumetricLight\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
		}

		if (_defaultSpotCookie == null)
			_defaultSpotCookie = DefaultSpotCookie;
		LoadNoise3dTexture();
		GenerateDitherTexture();
	}

	private void OnEnable() {
		if (_camera.actualRenderingPath == RenderingPath.Forward)
			_camera.AddCommandBuffer(CameraEvent.AfterDepthTexture, _preLightPass);
		else
			_camera.AddCommandBuffer(CameraEvent.BeforeLighting, _preLightPass);
	}

	private void OnDisable() {
		if (_camera.actualRenderingPath == RenderingPath.Forward)
			_camera.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, _preLightPass);
		else
			_camera.RemoveCommandBuffer(CameraEvent.BeforeLighting, _preLightPass);
	}

	private void ChangeResolution() {
		var width1 = Mathf.Max(1, _camera.pixelWidth);
		var height1 = Mathf.Max(1, _camera.pixelHeight);
		if (_volumeLightTexture != null)
			Destroy(_volumeLightTexture);
		_volumeLightTexture = new RenderTexture(width1, height1, 0, RenderTextureFormat.ARGBHalf);
		_volumeLightTexture.name = "VolumeLightBuffer";
		_volumeLightTexture.filterMode = FilterMode.Bilinear;
		if (_halfDepthBuffer != null)
			Destroy(_halfDepthBuffer);
		if (_halfVolumeLightTexture != null)
			Destroy(_halfVolumeLightTexture);
		if (Resolution == VolumtericResolution.Half || Resolution == VolumtericResolution.Quarter) {
			var width2 = Mathf.Max(1, width1 / 2);
			var height2 = Mathf.Max(1, height1 / 2);
			_halfVolumeLightTexture = new RenderTexture(width2, height2, 0, RenderTextureFormat.ARGBHalf);
			_halfVolumeLightTexture.name = "VolumeLightBufferHalf";
			_halfVolumeLightTexture.filterMode = FilterMode.Bilinear;
			_halfDepthBuffer = new RenderTexture(width2, height2, 0, RenderTextureFormat.RFloat);
			_halfDepthBuffer.name = "VolumeLightHalfDepth";
			_halfDepthBuffer.Create();
			_halfDepthBuffer.filterMode = FilterMode.Point;
		}

		if (_quarterVolumeLightTexture != null)
			Destroy(_quarterVolumeLightTexture);
		if (_quarterDepthBuffer != null)
			Destroy(_quarterDepthBuffer);
		if (Resolution != VolumtericResolution.Quarter)
			return;
		var width3 = Mathf.Max(1, width1 / 4);
		var height3 = Mathf.Max(1, height1 / 4);
		_quarterVolumeLightTexture = new RenderTexture(width3, height3, 0, RenderTextureFormat.ARGBHalf);
		_quarterVolumeLightTexture.name = "VolumeLightBufferQuarter";
		_quarterVolumeLightTexture.filterMode = FilterMode.Bilinear;
		_quarterDepthBuffer = new RenderTexture(width3, height3, 0, RenderTextureFormat.RFloat);
		_quarterDepthBuffer.name = "VolumeLightQuarterDepth";
		_quarterDepthBuffer.Create();
		_quarterDepthBuffer.filterMode = FilterMode.Point;
	}

	public void OnPostRender() {
		var postRenderEvent = PostRenderEvent;
		if (postRenderEvent == null)
			return;
		postRenderEvent();
	}

	public void OnPreRender() {
		if (_currentResolution != Resolution) {
			_currentResolution = Resolution;
			ChangeResolution();
		} else if (_volumeLightTexture.width != _camera.pixelWidth || _volumeLightTexture.height != _camera.pixelHeight)
			ChangeResolution();

		_viewProj = GL.GetGPUProjectionMatrix(
			            Matrix4x4.Perspective(_camera.fieldOfView, _camera.aspect, 0.01f, _camera.farClipPlane), true) *
		            _camera.worldToCameraMatrix;
		_preLightPass.Clear();
		var flag = SystemInfo.graphicsShaderLevel > 40;
		if (Resolution == VolumtericResolution.Quarter) {
			Texture source = null;
			_preLightPass.Blit(source, (RenderTargetIdentifier)(Texture)_halfDepthBuffer, _bilateralBlurMaterial,
				flag ? 4 : 10);
			_preLightPass.Blit(source, (RenderTargetIdentifier)(Texture)_quarterDepthBuffer, _bilateralBlurMaterial,
				flag ? 6 : 11);
			_preLightPass.SetRenderTarget((RenderTargetIdentifier)(Texture)_quarterVolumeLightTexture);
		} else if (Resolution == VolumtericResolution.Half) {
			_preLightPass.Blit(null, (RenderTargetIdentifier)(Texture)_halfDepthBuffer, _bilateralBlurMaterial,
				flag ? 4 : 10);
			_preLightPass.SetRenderTarget((RenderTargetIdentifier)(Texture)_halfVolumeLightTexture);
		} else
			_preLightPass.SetRenderTarget((RenderTargetIdentifier)(Texture)_volumeLightTexture);

		_preLightPass.ClearRenderTarget(false, true, new Color(0.0f, 0.0f, 0.0f, 1f));
		UpdateMaterialParameters();
		var preRenderEvent = PreRenderEvent;
		if (preRenderEvent == null)
			return;
		preRenderEvent(this, _viewProj);
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (Resolution == VolumtericResolution.Quarter) {
			var temporary = RenderTexture.GetTemporary(_quarterVolumeLightTexture.width,
				_quarterVolumeLightTexture.height, 0, RenderTextureFormat.ARGBHalf);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(_quarterVolumeLightTexture, temporary, _bilateralBlurMaterial, 8);
			Graphics.Blit(temporary, _quarterVolumeLightTexture, _bilateralBlurMaterial, 9);
			Graphics.Blit(_quarterVolumeLightTexture, _volumeLightTexture, _bilateralBlurMaterial, 7);
			RenderTexture.ReleaseTemporary(temporary);
		} else if (Resolution == VolumtericResolution.Half) {
			var temporary = RenderTexture.GetTemporary(_halfVolumeLightTexture.width, _halfVolumeLightTexture.height, 0,
				RenderTextureFormat.ARGBHalf);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(_halfVolumeLightTexture, temporary, _bilateralBlurMaterial, 2);
			Graphics.Blit(temporary, _halfVolumeLightTexture, _bilateralBlurMaterial, 3);
			Graphics.Blit(_halfVolumeLightTexture, _volumeLightTexture, _bilateralBlurMaterial, 5);
			RenderTexture.ReleaseTemporary(temporary);
		} else {
			var temporary = RenderTexture.GetTemporary(_volumeLightTexture.width, _volumeLightTexture.height, 0,
				RenderTextureFormat.ARGBHalf);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(_volumeLightTexture, temporary, _bilateralBlurMaterial, 0);
			Graphics.Blit(temporary, _volumeLightTexture, _bilateralBlurMaterial, 1);
			RenderTexture.ReleaseTemporary(temporary);
		}

		_blitAddMaterial.SetTexture("_Source", source);
		Graphics.Blit(_volumeLightTexture, destination, _blitAddMaterial, 0);
	}

	private void UpdateMaterialParameters() {
		_bilateralBlurMaterial.SetTexture("_HalfResDepthBuffer", _halfDepthBuffer);
		_bilateralBlurMaterial.SetTexture("_HalfResColor", _halfVolumeLightTexture);
		_bilateralBlurMaterial.SetTexture("_QuarterResDepthBuffer", _quarterDepthBuffer);
		_bilateralBlurMaterial.SetTexture("_QuarterResColor", _quarterVolumeLightTexture);
		Shader.SetGlobalTexture("_DitherTexture", _ditheringTexture);
		Shader.SetGlobalTexture("_NoiseTexture", _noiseTexture);
	}

	private void LoadNoise3dTexture() {
		var textAsset = Resources.Load("NoiseVolume") as TextAsset;
		var bytes = textAsset.bytes;
		var uint32_1 = BitConverter.ToUInt32(textAsset.bytes, 12);
		var uint32_2 = BitConverter.ToUInt32(textAsset.bytes, 16);
		var uint32_3 = BitConverter.ToUInt32(textAsset.bytes, 20);
		var uint32_4 = BitConverter.ToUInt32(textAsset.bytes, 24);
		var uint32_5 = BitConverter.ToUInt32(textAsset.bytes, 80);
		BitConverter.ToUInt32(textAsset.bytes, 84);
		var num1 = BitConverter.ToUInt32(textAsset.bytes, 88);
		if (num1 == 0U)
			num1 = uint32_3 / uint32_2 * 8U;
		_noiseTexture = new Texture3D((int)uint32_2, (int)uint32_1, (int)uint32_4, TextureFormat.RGBA32, false);
		_noiseTexture.name = "3D Noise";
		var colors = new Color[(int)uint32_2 * (int)uint32_1 * (int)uint32_4];
		uint startIndex = 128;
		if (textAsset.bytes[84] == 68 && textAsset.bytes[85] == 88 && textAsset.bytes[86] == 49 &&
		    textAsset.bytes[87] == 48 && (uint32_5 & 4U) > 0U) {
			var uint32_6 = BitConverter.ToUInt32(textAsset.bytes, (int)startIndex);
			if (uint32_6 >= 60U && uint32_6 <= 65U)
				num1 = 8U;
			else if (uint32_6 >= 48U && uint32_6 <= 52U)
				num1 = 16U;
			else if (uint32_6 >= 27U && uint32_6 <= 32U)
				num1 = 32U;
			startIndex += 20U;
		}

		var num2 = num1 / 8U;
		var num3 = (uint)((int)uint32_2 * (int)num1 + 7) / 8U;
		for (var index1 = 0; index1 < uint32_4; ++index1) {
			for (var index2 = 0; index2 < uint32_1; ++index2) {
				for (var index3 = 0; index3 < uint32_2; ++index3) {
					var num4 = bytes[startIndex + index3 * num2] / (float)byte.MaxValue;
					colors[index3 + index2 * uint32_2 + index1 * uint32_2 * uint32_1] =
						new Color(num4, num4, num4, num4);
				}

				startIndex += num3;
			}
		}

		_noiseTexture.SetPixels(colors);
		_noiseTexture.Apply();
	}

	private void GenerateDitherTexture() {
		if (_ditheringTexture != null)
			return;
		var num1 = 8;
		_ditheringTexture = new Texture2D(num1, num1, TextureFormat.Alpha8, false, true);
		_ditheringTexture.filterMode = FilterMode.Point;
		var colors = new Color32[num1 * num1];
		var num2 = 0;
		byte num3 = 3;
		var color32Array1 = colors;
		var index1 = num2;
		var num4 = index1 + 1;
		var color32_1 = new Color32(num3, num3, num3, num3);
		color32Array1[index1] = color32_1;
		byte num5 = 192;
		var color32Array2 = colors;
		var index2 = num4;
		var num6 = index2 + 1;
		var color32_2 = new Color32(num5, num5, num5, num5);
		color32Array2[index2] = color32_2;
		byte num7 = 51;
		var color32Array3 = colors;
		var index3 = num6;
		var num8 = index3 + 1;
		var color32_3 = new Color32(num7, num7, num7, num7);
		color32Array3[index3] = color32_3;
		byte num9 = 239;
		var color32Array4 = colors;
		var index4 = num8;
		var num10 = index4 + 1;
		var color32_4 = new Color32(num9, num9, num9, num9);
		color32Array4[index4] = color32_4;
		byte num11 = 15;
		var color32Array5 = colors;
		var index5 = num10;
		var num12 = index5 + 1;
		var color32_5 = new Color32(num11, num11, num11, num11);
		color32Array5[index5] = color32_5;
		byte num13 = 204;
		var color32Array6 = colors;
		var index6 = num12;
		var num14 = index6 + 1;
		var color32_6 = new Color32(num13, num13, num13, num13);
		color32Array6[index6] = color32_6;
		byte num15 = 62;
		var color32Array7 = colors;
		var index7 = num14;
		var num16 = index7 + 1;
		var color32_7 = new Color32(num15, num15, num15, num15);
		color32Array7[index7] = color32_7;
		byte num17 = 251;
		var color32Array8 = colors;
		var index8 = num16;
		var num18 = index8 + 1;
		var color32_8 = new Color32(num17, num17, num17, num17);
		color32Array8[index8] = color32_8;
		byte num19 = 129;
		var color32Array9 = colors;
		var index9 = num18;
		var num20 = index9 + 1;
		var color32_9 = new Color32(num19, num19, num19, num19);
		color32Array9[index9] = color32_9;
		byte num21 = 66;
		var color32Array10 = colors;
		var index10 = num20;
		var num22 = index10 + 1;
		var color32_10 = new Color32(num21, num21, num21, num21);
		color32Array10[index10] = color32_10;
		byte num23 = 176;
		var color32Array11 = colors;
		var index11 = num22;
		var num24 = index11 + 1;
		var color32_11 = new Color32(num23, num23, num23, num23);
		color32Array11[index11] = color32_11;
		byte num25 = 113;
		var color32Array12 = colors;
		var index12 = num24;
		var num26 = index12 + 1;
		var color32_12 = new Color32(num25, num25, num25, num25);
		color32Array12[index12] = color32_12;
		byte num27 = 141;
		var color32Array13 = colors;
		var index13 = num26;
		var num28 = index13 + 1;
		var color32_13 = new Color32(num27, num27, num27, num27);
		color32Array13[index13] = color32_13;
		byte num29 = 78;
		var color32Array14 = colors;
		var index14 = num28;
		var num30 = index14 + 1;
		var color32_14 = new Color32(num29, num29, num29, num29);
		color32Array14[index14] = color32_14;
		byte num31 = 188;
		var color32Array15 = colors;
		var index15 = num30;
		var num32 = index15 + 1;
		var color32_15 = new Color32(num31, num31, num31, num31);
		color32Array15[index15] = color32_15;
		byte num33 = 125;
		var color32Array16 = colors;
		var index16 = num32;
		var num34 = index16 + 1;
		var color32_16 = new Color32(num33, num33, num33, num33);
		color32Array16[index16] = color32_16;
		byte num35 = 35;
		var color32Array17 = colors;
		var index17 = num34;
		var num36 = index17 + 1;
		var color32_17 = new Color32(num35, num35, num35, num35);
		color32Array17[index17] = color32_17;
		byte num37 = 223;
		var color32Array18 = colors;
		var index18 = num36;
		var num38 = index18 + 1;
		var color32_18 = new Color32(num37, num37, num37, num37);
		color32Array18[index18] = color32_18;
		byte num39 = 19;
		var color32Array19 = colors;
		var index19 = num38;
		var num40 = index19 + 1;
		var color32_19 = new Color32(num39, num39, num39, num39);
		color32Array19[index19] = color32_19;
		byte num41 = 207;
		var color32Array20 = colors;
		var index20 = num40;
		var num42 = index20 + 1;
		var color32_20 = new Color32(num41, num41, num41, num41);
		color32Array20[index20] = color32_20;
		byte num43 = 47;
		var color32Array21 = colors;
		var index21 = num42;
		var num44 = index21 + 1;
		var color32_21 = new Color32(num43, num43, num43, num43);
		color32Array21[index21] = color32_21;
		byte num45 = 235;
		var color32Array22 = colors;
		var index22 = num44;
		var num46 = index22 + 1;
		var color32_22 = new Color32(num45, num45, num45, num45);
		color32Array22[index22] = color32_22;
		byte num47 = 31;
		var color32Array23 = colors;
		var index23 = num46;
		var num48 = index23 + 1;
		var color32_23 = new Color32(num47, num47, num47, num47);
		color32Array23[index23] = color32_23;
		byte num49 = 219;
		var color32Array24 = colors;
		var index24 = num48;
		var num50 = index24 + 1;
		var color32_24 = new Color32(num49, num49, num49, num49);
		color32Array24[index24] = color32_24;
		byte num51 = 160;
		var color32Array25 = colors;
		var index25 = num50;
		var num52 = index25 + 1;
		var color32_25 = new Color32(num51, num51, num51, num51);
		color32Array25[index25] = color32_25;
		byte num53 = 98;
		var color32Array26 = colors;
		var index26 = num52;
		var num54 = index26 + 1;
		var color32_26 = new Color32(num53, num53, num53, num53);
		color32Array26[index26] = color32_26;
		byte num55 = 145;
		var color32Array27 = colors;
		var index27 = num54;
		var num56 = index27 + 1;
		var color32_27 = new Color32(num55, num55, num55, num55);
		color32Array27[index27] = color32_27;
		byte num57 = 82;
		var color32Array28 = colors;
		var index28 = num56;
		var num58 = index28 + 1;
		var color32_28 = new Color32(num57, num57, num57, num57);
		color32Array28[index28] = color32_28;
		byte num59 = 172;
		var color32Array29 = colors;
		var index29 = num58;
		var num60 = index29 + 1;
		var color32_29 = new Color32(num59, num59, num59, num59);
		color32Array29[index29] = color32_29;
		byte num61 = 109;
		var color32Array30 = colors;
		var index30 = num60;
		var num62 = index30 + 1;
		var color32_30 = new Color32(num61, num61, num61, num61);
		color32Array30[index30] = color32_30;
		byte num63 = 156;
		var color32Array31 = colors;
		var index31 = num62;
		var num64 = index31 + 1;
		var color32_31 = new Color32(num63, num63, num63, num63);
		color32Array31[index31] = color32_31;
		byte num65 = 94;
		var color32Array32 = colors;
		var index32 = num64;
		var num66 = index32 + 1;
		var color32_32 = new Color32(num65, num65, num65, num65);
		color32Array32[index32] = color32_32;
		byte num67 = 11;
		var color32Array33 = colors;
		var index33 = num66;
		var num68 = index33 + 1;
		var color32_33 = new Color32(num67, num67, num67, num67);
		color32Array33[index33] = color32_33;
		byte num69 = 200;
		var color32Array34 = colors;
		var index34 = num68;
		var num70 = index34 + 1;
		var color32_34 = new Color32(num69, num69, num69, num69);
		color32Array34[index34] = color32_34;
		byte num71 = 58;
		var color32Array35 = colors;
		var index35 = num70;
		var num72 = index35 + 1;
		var color32_35 = new Color32(num71, num71, num71, num71);
		color32Array35[index35] = color32_35;
		byte num73 = 247;
		var color32Array36 = colors;
		var index36 = num72;
		var num74 = index36 + 1;
		var color32_36 = new Color32(num73, num73, num73, num73);
		color32Array36[index36] = color32_36;
		byte num75 = 7;
		var color32Array37 = colors;
		var index37 = num74;
		var num76 = index37 + 1;
		var color32_37 = new Color32(num75, num75, num75, num75);
		color32Array37[index37] = color32_37;
		byte num77 = 196;
		var color32Array38 = colors;
		var index38 = num76;
		var num78 = index38 + 1;
		var color32_38 = new Color32(num77, num77, num77, num77);
		color32Array38[index38] = color32_38;
		byte num79 = 54;
		var color32Array39 = colors;
		var index39 = num78;
		var num80 = index39 + 1;
		var color32_39 = new Color32(num79, num79, num79, num79);
		color32Array39[index39] = color32_39;
		byte num81 = 243;
		var color32Array40 = colors;
		var index40 = num80;
		var num82 = index40 + 1;
		var color32_40 = new Color32(num81, num81, num81, num81);
		color32Array40[index40] = color32_40;
		byte num83 = 137;
		var color32Array41 = colors;
		var index41 = num82;
		var num84 = index41 + 1;
		var color32_41 = new Color32(num83, num83, num83, num83);
		color32Array41[index41] = color32_41;
		byte num85 = 74;
		var color32Array42 = colors;
		var index42 = num84;
		var num86 = index42 + 1;
		var color32_42 = new Color32(num85, num85, num85, num85);
		color32Array42[index42] = color32_42;
		byte num87 = 184;
		var color32Array43 = colors;
		var index43 = num86;
		var num88 = index43 + 1;
		var color32_43 = new Color32(num87, num87, num87, num87);
		color32Array43[index43] = color32_43;
		byte num89 = 121;
		var color32Array44 = colors;
		var index44 = num88;
		var num90 = index44 + 1;
		var color32_44 = new Color32(num89, num89, num89, num89);
		color32Array44[index44] = color32_44;
		byte num91 = 133;
		var color32Array45 = colors;
		var index45 = num90;
		var num92 = index45 + 1;
		var color32_45 = new Color32(num91, num91, num91, num91);
		color32Array45[index45] = color32_45;
		byte num93 = 70;
		var color32Array46 = colors;
		var index46 = num92;
		var num94 = index46 + 1;
		var color32_46 = new Color32(num93, num93, num93, num93);
		color32Array46[index46] = color32_46;
		byte num95 = 180;
		var color32Array47 = colors;
		var index47 = num94;
		var num96 = index47 + 1;
		var color32_47 = new Color32(num95, num95, num95, num95);
		color32Array47[index47] = color32_47;
		byte num97 = 117;
		var color32Array48 = colors;
		var index48 = num96;
		var num98 = index48 + 1;
		var color32_48 = new Color32(num97, num97, num97, num97);
		color32Array48[index48] = color32_48;
		byte num99 = 43;
		var color32Array49 = colors;
		var index49 = num98;
		var num100 = index49 + 1;
		var color32_49 = new Color32(num99, num99, num99, num99);
		color32Array49[index49] = color32_49;
		byte num101 = 231;
		var color32Array50 = colors;
		var index50 = num100;
		var num102 = index50 + 1;
		var color32_50 = new Color32(num101, num101, num101, num101);
		color32Array50[index50] = color32_50;
		byte num103 = 27;
		var color32Array51 = colors;
		var index51 = num102;
		var num104 = index51 + 1;
		var color32_51 = new Color32(num103, num103, num103, num103);
		color32Array51[index51] = color32_51;
		byte num105 = 215;
		var color32Array52 = colors;
		var index52 = num104;
		var num106 = index52 + 1;
		var color32_52 = new Color32(num105, num105, num105, num105);
		color32Array52[index52] = color32_52;
		byte num107 = 39;
		var color32Array53 = colors;
		var index53 = num106;
		var num108 = index53 + 1;
		var color32_53 = new Color32(num107, num107, num107, num107);
		color32Array53[index53] = color32_53;
		byte num109 = 227;
		var color32Array54 = colors;
		var index54 = num108;
		var num110 = index54 + 1;
		var color32_54 = new Color32(num109, num109, num109, num109);
		color32Array54[index54] = color32_54;
		byte num111 = 23;
		var color32Array55 = colors;
		var index55 = num110;
		var num112 = index55 + 1;
		var color32_55 = new Color32(num111, num111, num111, num111);
		color32Array55[index55] = color32_55;
		byte num113 = 211;
		var color32Array56 = colors;
		var index56 = num112;
		var num114 = index56 + 1;
		var color32_56 = new Color32(num113, num113, num113, num113);
		color32Array56[index56] = color32_56;
		byte num115 = 168;
		var color32Array57 = colors;
		var index57 = num114;
		var num116 = index57 + 1;
		var color32_57 = new Color32(num115, num115, num115, num115);
		color32Array57[index57] = color32_57;
		byte num117 = 105;
		var color32Array58 = colors;
		var index58 = num116;
		var num118 = index58 + 1;
		var color32_58 = new Color32(num117, num117, num117, num117);
		color32Array58[index58] = color32_58;
		byte num119 = 153;
		var color32Array59 = colors;
		var index59 = num118;
		var num120 = index59 + 1;
		var color32_59 = new Color32(num119, num119, num119, num119);
		color32Array59[index59] = color32_59;
		byte num121 = 90;
		var color32Array60 = colors;
		var index60 = num120;
		var num122 = index60 + 1;
		var color32_60 = new Color32(num121, num121, num121, num121);
		color32Array60[index60] = color32_60;
		byte num123 = 164;
		var color32Array61 = colors;
		var index61 = num122;
		var num124 = index61 + 1;
		var color32_61 = new Color32(num123, num123, num123, num123);
		color32Array61[index61] = color32_61;
		byte num125 = 102;
		var color32Array62 = colors;
		var index62 = num124;
		var num126 = index62 + 1;
		var color32_62 = new Color32(num125, num125, num125, num125);
		color32Array62[index62] = color32_62;
		byte num127 = 149;
		var color32Array63 = colors;
		var index63 = num126;
		var num128 = index63 + 1;
		var color32_63 = new Color32(num127, num127, num127, num127);
		color32Array63[index63] = color32_63;
		byte num129 = 86;
		var color32Array64 = colors;
		var index64 = num128;
		var num130 = index64 + 1;
		var color32_64 = new Color32(num129, num129, num129, num129);
		color32Array64[index64] = color32_64;
		_ditheringTexture.SetPixels32(colors);
		_ditheringTexture.Apply();
	}

	private Mesh CreateSpotLightMesh() {
		var spotLightMesh = new Mesh();
		var vector3Array = new Vector3[50];
		var color32Array = new Color32[50];
		vector3Array[0] = new Vector3(0.0f, 0.0f, 0.0f);
		vector3Array[1] = new Vector3(0.0f, 0.0f, 1f);
		var f = 0.0f;
		var num1 = 0.3926991f;
		var z = 0.9f;
		for (var index = 0; index < 16; ++index) {
			vector3Array[index + 2] = new Vector3(-Mathf.Cos(f) * z, Mathf.Sin(f) * z, z);
			color32Array[index + 2] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			vector3Array[index + 2 + 16] = new Vector3(-Mathf.Cos(f), Mathf.Sin(f), 1f);
			color32Array[index + 2 + 16] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
			vector3Array[index + 2 + 32] = new Vector3(-Mathf.Cos(f) * z, Mathf.Sin(f) * z, 1f);
			color32Array[index + 2 + 32] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			f += num1;
		}

		spotLightMesh.vertices = vector3Array;
		spotLightMesh.colors32 = color32Array;
		var numArray1 = new int[288];
		var num2 = 0;
		for (var index1 = 2; index1 < 17; ++index1) {
			var numArray2 = numArray1;
			var index2 = num2;
			var num3 = index2 + 1;
			numArray2[index2] = 0;
			var numArray3 = numArray1;
			var index3 = num3;
			var num4 = index3 + 1;
			var num5 = index1;
			numArray3[index3] = num5;
			var numArray4 = numArray1;
			var index4 = num4;
			num2 = index4 + 1;
			var num6 = index1 + 1;
			numArray4[index4] = num6;
		}

		var numArray5 = numArray1;
		var index5 = num2;
		var num7 = index5 + 1;
		numArray5[index5] = 0;
		var numArray6 = numArray1;
		var index6 = num7;
		var num8 = index6 + 1;
		numArray6[index6] = 17;
		var numArray7 = numArray1;
		var index7 = num8;
		var num9 = index7 + 1;
		numArray7[index7] = 2;
		for (var index8 = 2; index8 < 17; ++index8) {
			var numArray8 = numArray1;
			var index9 = num9;
			var num10 = index9 + 1;
			var num11 = index8;
			numArray8[index9] = num11;
			var numArray9 = numArray1;
			var index10 = num10;
			var num12 = index10 + 1;
			var num13 = index8 + 16;
			numArray9[index10] = num13;
			var numArray10 = numArray1;
			var index11 = num12;
			var num14 = index11 + 1;
			var num15 = index8 + 1;
			numArray10[index11] = num15;
			var numArray11 = numArray1;
			var index12 = num14;
			var num16 = index12 + 1;
			var num17 = index8 + 1;
			numArray11[index12] = num17;
			var numArray12 = numArray1;
			var index13 = num16;
			var num18 = index13 + 1;
			var num19 = index8 + 16;
			numArray12[index13] = num19;
			var numArray13 = numArray1;
			var index14 = num18;
			num9 = index14 + 1;
			var num20 = index8 + 16 + 1;
			numArray13[index14] = num20;
		}

		var numArray14 = numArray1;
		var index15 = num9;
		var num21 = index15 + 1;
		numArray14[index15] = 2;
		var numArray15 = numArray1;
		var index16 = num21;
		var num22 = index16 + 1;
		numArray15[index16] = 17;
		var numArray16 = numArray1;
		var index17 = num22;
		var num23 = index17 + 1;
		numArray16[index17] = 18;
		var numArray17 = numArray1;
		var index18 = num23;
		var num24 = index18 + 1;
		numArray17[index18] = 18;
		var numArray18 = numArray1;
		var index19 = num24;
		var num25 = index19 + 1;
		numArray18[index19] = 17;
		var numArray19 = numArray1;
		var index20 = num25;
		var num26 = index20 + 1;
		numArray19[index20] = 33;
		for (var index21 = 18; index21 < 33; ++index21) {
			var numArray20 = numArray1;
			var index22 = num26;
			var num27 = index22 + 1;
			var num28 = index21;
			numArray20[index22] = num28;
			var numArray21 = numArray1;
			var index23 = num27;
			var num29 = index23 + 1;
			var num30 = index21 + 16;
			numArray21[index23] = num30;
			var numArray22 = numArray1;
			var index24 = num29;
			var num31 = index24 + 1;
			var num32 = index21 + 1;
			numArray22[index24] = num32;
			var numArray23 = numArray1;
			var index25 = num31;
			var num33 = index25 + 1;
			var num34 = index21 + 1;
			numArray23[index25] = num34;
			var numArray24 = numArray1;
			var index26 = num33;
			var num35 = index26 + 1;
			var num36 = index21 + 16;
			numArray24[index26] = num36;
			var numArray25 = numArray1;
			var index27 = num35;
			num26 = index27 + 1;
			var num37 = index21 + 16 + 1;
			numArray25[index27] = num37;
		}

		var numArray26 = numArray1;
		var index28 = num26;
		var num38 = index28 + 1;
		numArray26[index28] = 18;
		var numArray27 = numArray1;
		var index29 = num38;
		var num39 = index29 + 1;
		numArray27[index29] = 33;
		var numArray28 = numArray1;
		var index30 = num39;
		var num40 = index30 + 1;
		numArray28[index30] = 34;
		var numArray29 = numArray1;
		var index31 = num40;
		var num41 = index31 + 1;
		numArray29[index31] = 34;
		var numArray30 = numArray1;
		var index32 = num41;
		var num42 = index32 + 1;
		numArray30[index32] = 33;
		var numArray31 = numArray1;
		var index33 = num42;
		var num43 = index33 + 1;
		numArray31[index33] = 49;
		for (var index34 = 34; index34 < 49; ++index34) {
			var numArray32 = numArray1;
			var index35 = num43;
			var num44 = index35 + 1;
			numArray32[index35] = 1;
			var numArray33 = numArray1;
			var index36 = num44;
			var num45 = index36 + 1;
			var num46 = index34 + 1;
			numArray33[index36] = num46;
			var numArray34 = numArray1;
			var index37 = num45;
			num43 = index37 + 1;
			var num47 = index34;
			numArray34[index37] = num47;
		}

		var numArray35 = numArray1;
		var index38 = num43;
		var num48 = index38 + 1;
		numArray35[index38] = 1;
		var numArray36 = numArray1;
		var index39 = num48;
		var num49 = index39 + 1;
		numArray36[index39] = 34;
		var numArray37 = numArray1;
		var index40 = num49;
		var num50 = index40 + 1;
		numArray37[index40] = 49;
		spotLightMesh.triangles = numArray1;
		spotLightMesh.RecalculateBounds();
		return spotLightMesh;
	}

	public enum VolumtericResolution {
		Full,
		Half,
		Quarter
	}
}