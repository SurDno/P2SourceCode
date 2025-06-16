using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Camera/Camera Motion Blur")]
  public class CameraMotionBlur : PostEffectsBase
  {
    private static float MAX_RADIUS = 10f;
    public MotionBlurFilter filterType = MotionBlurFilter.Reconstruction;
    public bool preview;
    public Vector3 previewScale = Vector3.one;
    public float movementScale;
    public float rotationScale = 1f;
    public float maxVelocity = 8f;
    public float minVelocity = 0.1f;
    public float velocityScale = 0.375f;
    public float softZDistance = 0.005f;
    public int velocityDownsample = 1;
    public LayerMask excludeLayers = 0;
    private GameObject tmpCam;
    public Shader shader;
    public Shader dx11MotionBlurShader;
    public Shader replacementClear;
    private Material motionBlurMaterial;
    private Material dx11MotionBlurMaterial;
    public Texture2D noiseTexture;
    public float jitter = 0.05f;
    public bool showVelocity;
    public float showVelocityScale = 1f;
    private Matrix4x4 currentViewProjMat;
    private Matrix4x4 prevViewProjMat;
    private int prevFrameCount;
    private bool wasActive;
    private Vector3 prevFrameForward = Vector3.forward;
    private Vector3 prevFrameUp = Vector3.up;
    private Vector3 prevFramePos = Vector3.zero;
    private Camera _camera;

    private void CalculateViewProjection()
    {
      Matrix4x4 worldToCameraMatrix = _camera.worldToCameraMatrix;
      currentViewProjMat = GL.GetGPUProjectionMatrix(_camera.projectionMatrix, true) * worldToCameraMatrix;
    }

    private new void Start()
    {
      CheckResources();
      if (_camera == null)
        _camera = GetComponent<Camera>();
      wasActive = gameObject.activeInHierarchy;
      CalculateViewProjection();
      Remember();
      wasActive = false;
    }

    private void OnEnable()
    {
      if (_camera == null)
        _camera = GetComponent<Camera>();
      _camera.depthTextureMode |= DepthTextureMode.Depth;
    }

    private void OnDisable()
    {
      if (null != motionBlurMaterial)
      {
        DestroyImmediate(motionBlurMaterial);
        motionBlurMaterial = null;
      }
      if (null != dx11MotionBlurMaterial)
      {
        DestroyImmediate(dx11MotionBlurMaterial);
        dx11MotionBlurMaterial = null;
      }
      if (!(null != tmpCam))
        return;
      DestroyImmediate(tmpCam);
      tmpCam = null;
    }

    public override bool CheckResources()
    {
      CheckSupport(true, true);
      motionBlurMaterial = CheckShaderAndCreateMaterial(shader, motionBlurMaterial);
      if (supportDX11 && filterType == MotionBlurFilter.ReconstructionDX11)
        dx11MotionBlurMaterial = CheckShaderAndCreateMaterial(dx11MotionBlurShader, dx11MotionBlurMaterial);
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
      {
        Graphics.Blit(source, destination);
      }
      else
      {
        if (filterType == MotionBlurFilter.CameraMotion)
          StartFrame();
        RenderTextureFormat format = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf) ? RenderTextureFormat.RGHalf : RenderTextureFormat.ARGBHalf;
        RenderTexture temporary1 = RenderTexture.GetTemporary(divRoundUp(source.width, velocityDownsample), divRoundUp(source.height, velocityDownsample), 0, format);
        this.maxVelocity = Mathf.Max(2f, this.maxVelocity);
        float maxVelocity = this.maxVelocity;
        bool flag = filterType == MotionBlurFilter.ReconstructionDX11 && dx11MotionBlurMaterial == null;
        int width;
        int height;
        float num1;
        if (filterType == MotionBlurFilter.Reconstruction | flag || filterType == MotionBlurFilter.ReconstructionDisc)
        {
          this.maxVelocity = Mathf.Min(this.maxVelocity, MAX_RADIUS);
          width = divRoundUp(temporary1.width, (int) this.maxVelocity);
          height = divRoundUp(temporary1.height, (int) this.maxVelocity);
          num1 = temporary1.width / width;
        }
        else
        {
          width = divRoundUp(temporary1.width, (int) this.maxVelocity);
          height = divRoundUp(temporary1.height, (int) this.maxVelocity);
          num1 = temporary1.width / width;
        }
        RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0, format);
        RenderTexture temporary3 = RenderTexture.GetTemporary(width, height, 0, format);
        temporary1.filterMode = FilterMode.Point;
        temporary2.filterMode = FilterMode.Point;
        temporary3.filterMode = FilterMode.Point;
        if ((bool) (Object) noiseTexture)
          noiseTexture.filterMode = FilterMode.Point;
        source.wrapMode = TextureWrapMode.Clamp;
        temporary1.wrapMode = TextureWrapMode.Clamp;
        temporary3.wrapMode = TextureWrapMode.Clamp;
        temporary2.wrapMode = TextureWrapMode.Clamp;
        CalculateViewProjection();
        if (gameObject.activeInHierarchy && !wasActive)
          Remember();
        wasActive = gameObject.activeInHierarchy;
        Matrix4x4 matrix4x4 = Matrix4x4.Inverse(currentViewProjMat);
        motionBlurMaterial.SetMatrix("_InvViewProj", matrix4x4);
        motionBlurMaterial.SetMatrix("_PrevViewProj", prevViewProjMat);
        motionBlurMaterial.SetMatrix("_ToPrevViewProjCombined", prevViewProjMat * matrix4x4);
        motionBlurMaterial.SetFloat("_MaxVelocity", num1);
        motionBlurMaterial.SetFloat("_MaxRadiusOrKInPaper", num1);
        motionBlurMaterial.SetFloat("_MinVelocity", minVelocity);
        motionBlurMaterial.SetFloat("_VelocityScale", velocityScale);
        motionBlurMaterial.SetFloat("_Jitter", jitter);
        motionBlurMaterial.SetTexture("_NoiseTex", noiseTexture);
        motionBlurMaterial.SetTexture("_VelTex", temporary1);
        motionBlurMaterial.SetTexture("_NeighbourMaxTex", temporary3);
        motionBlurMaterial.SetTexture("_TileTexDebug", temporary2);
        if (preview)
        {
          Matrix4x4 worldToCameraMatrix = _camera.worldToCameraMatrix;
          Matrix4x4 identity = Matrix4x4.identity;
          identity.SetTRS(previewScale * 0.3333f, Quaternion.identity, Vector3.one);
          prevViewProjMat = GL.GetGPUProjectionMatrix(_camera.projectionMatrix, true) * identity * worldToCameraMatrix;
          motionBlurMaterial.SetMatrix("_PrevViewProj", prevViewProjMat);
          motionBlurMaterial.SetMatrix("_ToPrevViewProjCombined", prevViewProjMat * matrix4x4);
        }
        if (filterType == MotionBlurFilter.CameraMotion)
        {
          Vector4 zero = Vector4.zero;
          float num2 = Vector3.Dot(transform.up, Vector3.up);
          Vector3 rhs = prevFramePos - transform.position;
          float magnitude = rhs.magnitude;
          float num3 = (float) (Vector3.Angle(transform.up, prevFrameUp) / (double) _camera.fieldOfView * (source.width * 0.75));
          zero.x = rotationScale * num3;
          float num4 = (float) (Vector3.Angle(transform.forward, prevFrameForward) / (double) _camera.fieldOfView * (source.width * 0.75));
          zero.y = rotationScale * num2 * num4;
          float num5 = (float) (Vector3.Angle(transform.forward, prevFrameForward) / (double) _camera.fieldOfView * (source.width * 0.75));
          zero.z = rotationScale * (1f - num2) * num5;
          if (magnitude > (double) Mathf.Epsilon && movementScale > (double) Mathf.Epsilon)
          {
            zero.w = (float) (movementScale * (double) Vector3.Dot(transform.forward, rhs) * (source.width * 0.5));
            zero.x += (float) (movementScale * (double) Vector3.Dot(transform.up, rhs) * (source.width * 0.5));
            zero.y += (float) (movementScale * (double) Vector3.Dot(transform.right, rhs) * (source.width * 0.5));
          }
          if (preview)
            motionBlurMaterial.SetVector("_BlurDirectionPacked", new Vector4(previewScale.y, previewScale.x, 0.0f, previewScale.z) * 0.5f * _camera.fieldOfView);
          else
            motionBlurMaterial.SetVector("_BlurDirectionPacked", zero);
        }
        else
        {
          Graphics.Blit(source, temporary1, motionBlurMaterial, 0);
          Camera camera = null;
          if (excludeLayers.value != 0)
            camera = GetTmpCam();
          if ((bool) (Object) camera && excludeLayers.value != 0 && (bool) (Object) replacementClear && replacementClear.isSupported)
          {
            camera.targetTexture = temporary1;
            camera.cullingMask = excludeLayers;
            camera.RenderWithShader(replacementClear, "");
          }
        }
        if (!preview && Time.frameCount != prevFrameCount)
        {
          prevFrameCount = Time.frameCount;
          Remember();
        }
        source.filterMode = FilterMode.Bilinear;
        if (showVelocity)
        {
          motionBlurMaterial.SetFloat("_DisplayVelocityScale", showVelocityScale);
          Graphics.Blit(temporary1, destination, motionBlurMaterial, 1);
        }
        else if (filterType == MotionBlurFilter.ReconstructionDX11 && !flag)
        {
          dx11MotionBlurMaterial.SetFloat("_MinVelocity", minVelocity);
          dx11MotionBlurMaterial.SetFloat("_VelocityScale", velocityScale);
          dx11MotionBlurMaterial.SetFloat("_Jitter", jitter);
          dx11MotionBlurMaterial.SetTexture("_NoiseTex", noiseTexture);
          dx11MotionBlurMaterial.SetTexture("_VelTex", temporary1);
          dx11MotionBlurMaterial.SetTexture("_NeighbourMaxTex", temporary3);
          dx11MotionBlurMaterial.SetFloat("_SoftZDistance", Mathf.Max(0.00025f, softZDistance));
          dx11MotionBlurMaterial.SetFloat("_MaxRadiusOrKInPaper", num1);
          Graphics.Blit(temporary1, temporary2, dx11MotionBlurMaterial, 0);
          Graphics.Blit(temporary2, temporary3, dx11MotionBlurMaterial, 1);
          Graphics.Blit(source, destination, dx11MotionBlurMaterial, 2);
        }
        else if (filterType == MotionBlurFilter.Reconstruction | flag)
        {
          motionBlurMaterial.SetFloat("_SoftZDistance", Mathf.Max(0.00025f, softZDistance));
          Graphics.Blit(temporary1, temporary2, motionBlurMaterial, 2);
          Graphics.Blit(temporary2, temporary3, motionBlurMaterial, 3);
          Graphics.Blit(source, destination, motionBlurMaterial, 4);
        }
        else if (filterType == MotionBlurFilter.CameraMotion)
          Graphics.Blit(source, destination, motionBlurMaterial, 6);
        else if (filterType == MotionBlurFilter.ReconstructionDisc)
        {
          motionBlurMaterial.SetFloat("_SoftZDistance", Mathf.Max(0.00025f, softZDistance));
          Graphics.Blit(temporary1, temporary2, motionBlurMaterial, 2);
          Graphics.Blit(temporary2, temporary3, motionBlurMaterial, 3);
          Graphics.Blit(source, destination, motionBlurMaterial, 7);
        }
        else
          Graphics.Blit(source, destination, motionBlurMaterial, 5);
        RenderTexture.ReleaseTemporary(temporary1);
        RenderTexture.ReleaseTemporary(temporary2);
        RenderTexture.ReleaseTemporary(temporary3);
      }
    }

    private void Remember()
    {
      prevViewProjMat = currentViewProjMat;
      prevFrameForward = transform.forward;
      prevFrameUp = transform.up;
      prevFramePos = transform.position;
    }

    private Camera GetTmpCam()
    {
      if (tmpCam == null)
      {
        string name = "_" + _camera.name + "_MotionBlurTmpCam";
        GameObject gameObject = GameObject.Find(name);
        if (null == gameObject)
          tmpCam = new GameObject(name, typeof (Camera));
        else
          tmpCam = gameObject;
      }
      tmpCam.hideFlags = HideFlags.DontSave;
      tmpCam.transform.position = _camera.transform.position;
      tmpCam.transform.rotation = _camera.transform.rotation;
      tmpCam.transform.localScale = _camera.transform.localScale;
      tmpCam.GetComponent<Camera>().CopyFrom(_camera);
      tmpCam.GetComponent<Camera>().enabled = false;
      tmpCam.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
      tmpCam.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
      return tmpCam.GetComponent<Camera>();
    }

    private void StartFrame()
    {
      prevFramePos = Vector3.Slerp(prevFramePos, transform.position, 0.75f);
    }

    private static int divRoundUp(int x, int d) => (x + d - 1) / d;

    public enum MotionBlurFilter
    {
      CameraMotion,
      LocalBlur,
      Reconstruction,
      ReconstructionDX11,
      ReconstructionDisc,
    }
  }
}
