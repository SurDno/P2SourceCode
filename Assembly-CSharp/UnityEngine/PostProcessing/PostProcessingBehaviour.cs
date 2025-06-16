using System;
using System.Collections.Generic;

namespace UnityEngine.PostProcessing
{
  [ImageEffectAllowedInSceneView]
  [RequireComponent(typeof (Camera))]
  [DisallowMultipleComponent]
  [ExecuteInEditMode]
  [AddComponentMenu("Effects/Post-Processing Behaviour", -1)]
  public class PostProcessingBehaviour : MonoBehaviour
  {
    public PostProcessingProfile profile;
    public Func<Vector2, Matrix4x4> jitteredMatrixFunc;
    private Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>> m_CommandBuffers;
    private List<PostProcessingComponentBase> m_Components;
    private Dictionary<PostProcessingComponentBase, bool> m_ComponentStates;
    private MaterialFactory m_MaterialFactory;
    private RenderTextureFactory m_RenderTextureFactory;
    private PostProcessingContext m_Context;
    private Camera m_Camera;
    private PostProcessingProfile m_PreviousProfile;
    private bool m_RenderingInSceneView = false;
    private BuiltinDebugViewsComponent m_DebugViews;
    private AmbientOcclusionComponent m_AmbientOcclusion;
    private ScreenSpaceReflectionComponent m_ScreenSpaceReflection;
    private FogComponent m_FogComponent;
    private MotionBlurComponent m_MotionBlur;
    private TaaComponent m_Taa;
    private EyeAdaptationComponent m_EyeAdaptation;
    private DepthOfFieldComponent m_DepthOfField;
    private BloomComponent m_Bloom;
    private ChromaticAberrationComponent m_ChromaticAberration;
    private ColorGradingComponent m_ColorGrading;
    private UserLutComponent m_UserLut;
    private GrainComponent m_Grain;
    private VignetteComponent m_Vignette;
    private DitheringComponent m_Dithering;
    private FxaaComponent m_Fxaa;
    private List<PostProcessingComponentBase> m_ComponentsToEnable = new List<PostProcessingComponentBase>();
    private List<PostProcessingComponentBase> m_ComponentsToDisable = new List<PostProcessingComponentBase>();

    private void OnEnable()
    {
      m_CommandBuffers = new Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>>();
      m_MaterialFactory = new MaterialFactory();
      m_RenderTextureFactory = new RenderTextureFactory();
      m_Context = new PostProcessingContext();
      m_Components = new List<PostProcessingComponentBase>();
      m_DebugViews = AddComponent(new BuiltinDebugViewsComponent());
      m_AmbientOcclusion = AddComponent(new AmbientOcclusionComponent());
      m_ScreenSpaceReflection = AddComponent(new ScreenSpaceReflectionComponent());
      m_FogComponent = AddComponent(new FogComponent());
      m_MotionBlur = AddComponent(new MotionBlurComponent());
      m_Taa = AddComponent(new TaaComponent());
      m_EyeAdaptation = AddComponent(new EyeAdaptationComponent());
      m_DepthOfField = AddComponent(new DepthOfFieldComponent());
      m_Bloom = AddComponent(new BloomComponent());
      m_ChromaticAberration = AddComponent(new ChromaticAberrationComponent());
      m_ColorGrading = AddComponent(new ColorGradingComponent());
      m_UserLut = AddComponent(new UserLutComponent());
      m_Grain = AddComponent(new GrainComponent());
      m_Vignette = AddComponent(new VignetteComponent());
      m_Dithering = AddComponent(new DitheringComponent());
      m_Fxaa = AddComponent(new FxaaComponent());
      m_ComponentStates = new Dictionary<PostProcessingComponentBase, bool>();
      foreach (PostProcessingComponentBase component in m_Components)
        m_ComponentStates.Add(component, false);
      this.useGUILayout = false;
    }

    private void OnPreCull()
    {
      m_Camera = this.GetComponent<Camera>();
      if ((UnityEngine.Object) profile == (UnityEngine.Object) null || (UnityEngine.Object) m_Camera == (UnityEngine.Object) null)
        return;
      PostProcessingContext pcontext = m_Context.Reset();
      pcontext.profile = profile;
      pcontext.renderTextureFactory = m_RenderTextureFactory;
      pcontext.materialFactory = m_MaterialFactory;
      pcontext.camera = m_Camera;
      m_DebugViews.Init(pcontext, profile.debugViews);
      m_AmbientOcclusion.Init(pcontext, profile.ambientOcclusion);
      m_ScreenSpaceReflection.Init(pcontext, profile.screenSpaceReflection);
      m_FogComponent.Init(pcontext, profile.fog);
      m_MotionBlur.Init(pcontext, profile.motionBlur);
      m_Taa.Init(pcontext, profile.antialiasing);
      m_EyeAdaptation.Init(pcontext, profile.eyeAdaptation);
      m_DepthOfField.Init(pcontext, profile.depthOfField);
      m_Bloom.Init(pcontext, profile.bloom);
      m_ChromaticAberration.Init(pcontext, profile.chromaticAberration);
      m_ColorGrading.Init(pcontext, profile.colorGrading);
      m_UserLut.Init(pcontext, profile.userLut);
      m_Grain.Init(pcontext, profile.grain);
      m_Vignette.Init(pcontext, profile.vignette);
      m_Dithering.Init(pcontext, profile.dithering);
      m_Fxaa.Init(pcontext, profile.antialiasing);
      if ((UnityEngine.Object) m_PreviousProfile != (UnityEngine.Object) profile)
      {
        DisableComponents();
        m_PreviousProfile = profile;
      }
      CheckObservers();
      DepthTextureMode depthTextureMode = DepthTextureMode.None;
      foreach (PostProcessingComponentBase component in m_Components)
      {
        if (component.active)
          depthTextureMode |= component.GetCameraFlags();
      }
      pcontext.camera.depthTextureMode = depthTextureMode;
      if (m_RenderingInSceneView || !m_Taa.active || profile.debugViews.willInterrupt)
        return;
      m_Taa.SetProjectionMatrix(jitteredMatrixFunc);
    }

    private void OnPreRender()
    {
      if ((UnityEngine.Object) profile == (UnityEngine.Object) null)
        return;
      TryExecuteCommandBuffer(m_DebugViews);
      TryExecuteCommandBuffer(m_AmbientOcclusion);
      TryExecuteCommandBuffer(m_ScreenSpaceReflection);
      if (m_RenderingInSceneView)
        return;
      TryExecuteCommandBuffer(m_MotionBlur);
    }

    private void OnPostRender()
    {
      if ((UnityEngine.Object) profile == (UnityEngine.Object) null || (UnityEngine.Object) m_Camera == (UnityEngine.Object) null || m_RenderingInSceneView || !m_Taa.active || profile.debugViews.willInterrupt)
        return;
      m_Context.camera.ResetProjectionMatrix();
    }

    [ImageEffectTransformsToLDR]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if ((UnityEngine.Object) profile == (UnityEngine.Object) null || (UnityEngine.Object) m_Camera == (UnityEngine.Object) null)
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        bool flag1 = false;
        bool active = m_Fxaa.active;
        bool antialiasCoC = m_Taa.active && !m_RenderingInSceneView;
        bool flag2 = m_DepthOfField.active && !m_RenderingInSceneView;
        Material material1 = m_MaterialFactory.Get("Hidden/Post FX/Uber Shader");
        material1.shaderKeywords = (string[]) null;
        RenderTexture renderTexture1 = source;
        RenderTexture renderTexture2 = destination;
        if (antialiasCoC)
        {
          RenderTexture destination1 = m_RenderTextureFactory.Get(renderTexture1);
          m_Taa.Render(renderTexture1, destination1);
          renderTexture1 = destination1;
        }
        Texture autoExposure = (Texture) GraphicsUtils.whiteTexture;
        if (m_EyeAdaptation.active)
        {
          flag1 = true;
          autoExposure = m_EyeAdaptation.Prepare(renderTexture1, material1);
        }
        material1.SetTexture("_AutoExposure", autoExposure);
        if (flag2)
        {
          flag1 = true;
          m_DepthOfField.Prepare(renderTexture1, material1, antialiasCoC, m_Taa.jitterVector, m_Taa.model.settings.taaSettings.motionBlending);
        }
        if (m_Bloom.active)
        {
          flag1 = true;
          m_Bloom.Prepare(renderTexture1, material1, autoExposure);
        }
        bool flag3 = flag1 | TryPrepareUberImageEffect(m_ChromaticAberration, material1) | TryPrepareUberImageEffect(m_ColorGrading, material1) | TryPrepareUberImageEffect(m_Vignette, material1) | TryPrepareUberImageEffect(m_UserLut, material1);
        Material material2 = active ? m_MaterialFactory.Get("Hidden/Post FX/FXAA") : (Material) null;
        if (active)
        {
          material2.shaderKeywords = (string[]) null;
          TryPrepareUberImageEffect(m_Grain, material2);
          TryPrepareUberImageEffect(m_Dithering, material2);
          if (flag3)
          {
            RenderTexture dest = m_RenderTextureFactory.Get(renderTexture1);
            Graphics.Blit((Texture) renderTexture1, dest, material1, 0);
            renderTexture1 = dest;
          }
          m_Fxaa.Render(renderTexture1, renderTexture2);
        }
        else
        {
          flag3 = flag3 | TryPrepareUberImageEffect(m_Grain, material1) | TryPrepareUberImageEffect(m_Dithering, material1);
          if (flag3)
          {
            if (!GraphicsUtils.isLinearColorSpace)
              material1.EnableKeyword("UNITY_COLORSPACE_GAMMA");
            Graphics.Blit((Texture) renderTexture1, renderTexture2, material1, 0);
          }
        }
        if (!flag3 && !active)
          Graphics.Blit((Texture) renderTexture1, renderTexture2);
        m_RenderTextureFactory.ReleaseAll();
      }
    }

    private void OnGUI()
    {
      if (Event.current.type != EventType.Repaint || (UnityEngine.Object) profile == (UnityEngine.Object) null || (UnityEngine.Object) m_Camera == (UnityEngine.Object) null)
        return;
      if (m_EyeAdaptation.active && profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.EyeAdaptation))
        m_EyeAdaptation.OnGUI();
      else if (m_ColorGrading.active && profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.LogLut))
      {
        m_ColorGrading.OnGUI();
      }
      else
      {
        if (!m_UserLut.active || !profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.UserLut))
          return;
        m_UserLut.OnGUI();
      }
    }

    private void OnDisable()
    {
      foreach (KeyValuePair<CameraEvent, CommandBuffer> keyValuePair in m_CommandBuffers.Values)
      {
        m_Camera.RemoveCommandBuffer(keyValuePair.Key, keyValuePair.Value);
        keyValuePair.Value.Dispose();
      }
      m_CommandBuffers.Clear();
      if ((UnityEngine.Object) profile != (UnityEngine.Object) null)
        DisableComponents();
      m_Components.Clear();
      if ((UnityEngine.Object) m_Camera != (UnityEngine.Object) null)
        m_Camera.depthTextureMode = DepthTextureMode.None;
      m_MaterialFactory.Dispose();
      m_RenderTextureFactory.Dispose();
      GraphicsUtils.Dispose();
    }

    public void ResetTemporalEffects()
    {
      m_Taa.ResetHistory();
      m_MotionBlur.ResetHistory();
      m_EyeAdaptation.ResetHistory();
    }

    private void CheckObservers()
    {
      foreach (KeyValuePair<PostProcessingComponentBase, bool> componentState in m_ComponentStates)
      {
        PostProcessingComponentBase key = componentState.Key;
        bool enabled = key.GetModel().enabled;
        if (enabled != componentState.Value)
        {
          if (enabled)
            m_ComponentsToEnable.Add(key);
          else
            m_ComponentsToDisable.Add(key);
        }
      }
      for (int index = 0; index < m_ComponentsToDisable.Count; ++index)
      {
        PostProcessingComponentBase key = m_ComponentsToDisable[index];
        m_ComponentStates[key] = false;
        key.OnDisable();
      }
      for (int index = 0; index < m_ComponentsToEnable.Count; ++index)
      {
        PostProcessingComponentBase key = m_ComponentsToEnable[index];
        m_ComponentStates[key] = true;
        key.OnEnable();
      }
      m_ComponentsToDisable.Clear();
      m_ComponentsToEnable.Clear();
    }

    private void DisableComponents()
    {
      foreach (PostProcessingComponentBase component in m_Components)
      {
        PostProcessingModel model = component.GetModel();
        if (model != null && model.enabled)
          component.OnDisable();
      }
    }

    private CommandBuffer AddCommandBuffer<T>(CameraEvent evt, string name) where T : PostProcessingModel
    {
      CommandBuffer commandBuffer = new CommandBuffer {
        name = name
      };
      KeyValuePair<CameraEvent, CommandBuffer> keyValuePair = new KeyValuePair<CameraEvent, CommandBuffer>(evt, commandBuffer);
      m_CommandBuffers.Add(typeof (T), keyValuePair);
      m_Camera.AddCommandBuffer(evt, keyValuePair.Value);
      return keyValuePair.Value;
    }

    private void RemoveCommandBuffer<T>() where T : PostProcessingModel
    {
      Type key = typeof (T);
      KeyValuePair<CameraEvent, CommandBuffer> keyValuePair;
      if (!m_CommandBuffers.TryGetValue(key, out keyValuePair))
        return;
      m_Camera.RemoveCommandBuffer(keyValuePair.Key, keyValuePair.Value);
      m_CommandBuffers.Remove(key);
      keyValuePair.Value.Dispose();
    }

    private CommandBuffer GetCommandBuffer<T>(CameraEvent evt, string name) where T : PostProcessingModel
    {
      KeyValuePair<CameraEvent, CommandBuffer> keyValuePair;
      CommandBuffer commandBuffer;
      if (!m_CommandBuffers.TryGetValue(typeof (T), out keyValuePair))
        commandBuffer = AddCommandBuffer<T>(evt, name);
      else if (keyValuePair.Key != evt)
      {
        RemoveCommandBuffer<T>();
        commandBuffer = AddCommandBuffer<T>(evt, name);
      }
      else
        commandBuffer = keyValuePair.Value;
      return commandBuffer;
    }

    private void TryExecuteCommandBuffer<T>(PostProcessingComponentCommandBuffer<T> component) where T : PostProcessingModel
    {
      if (component.active)
      {
        CommandBuffer commandBuffer = GetCommandBuffer<T>(component.GetCameraEvent(), component.GetName());
        commandBuffer.Clear();
        component.PopulateCommandBuffer(commandBuffer);
      }
      else
        RemoveCommandBuffer<T>();
    }

    private bool TryPrepareUberImageEffect<T>(
      PostProcessingComponentRenderTexture<T> component,
      Material material)
      where T : PostProcessingModel
    {
      if (!component.active)
        return false;
      component.Prepare(material);
      return true;
    }

    private T AddComponent<T>(T component) where T : PostProcessingComponentBase
    {
      m_Components.Add(component);
      return component;
    }
  }
}
