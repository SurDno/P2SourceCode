using System;
using System.Text;
using Cofe.Meta;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using UnityEngine;

namespace Engine.Source.Debugs
{
  [Initialisable]
  public static class TextureStreamingGroupDebug
  {
    private static string name = "[Texture Streaming]";
    private static KeyCode key = KeyCode.Y;
    private static KeyModifficator modifficators = KeyModifficator.Control | KeyModifficator.Shift;
    private static Color headerColor = Color.cyan;
    private static Color trueColor = Color.white;
    private static StringBuilder sb;
    private static GizmoService service;

    [Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() => GroupDebugService.RegisterGroup(name, key, modifficators, Update));
    }

    private static void Update()
    {
      service = ServiceLocator.GetService<GizmoService>();
      if (sb == null)
        sb = new StringBuilder();
      sb.Append('\n');
      sb.Append(name);
      sb.Append(" (");
      sb.Append(InputUtility.GetHotKeyText(key, modifficators));
      sb.Append(")");
      service.DrawText(sb.ToString(), headerColor);
      sb.Clear();
      AppendProperty("Current Texture Memory", Texture.currentTextureMemory);
      AppendProperty("Desired Texture Memory", Texture.desiredTextureMemory);
      AppendProperty("Non-Streaming Texture Count", Texture.nonStreamingTextureCount);
      AppendProperty("Non-Streaming Texture Memory", Texture.nonStreamingTextureMemory);
      AppendProperty("Streaming Mipmap Upload Count", Texture.streamingMipmapUploadCount);
      AppendProperty("Streaming Renderer Count", Texture.streamingRendererCount);
      AppendProperty("Streaming Texture Count", Texture.streamingTextureCount);
      AppendProperty("Streaming Texture Loading Count", Texture.streamingTextureLoadingCount);
      AppendProperty("Streaming Texture Pending Load Count", Texture.streamingTexturePendingLoadCount);
      AppendProperty("Target Texture Memory", Texture.targetTextureMemory);
      AppendProperty("Total Texture Memory", Texture.totalTextureMemory);
      service = null;
    }

    private static void AppendProperty(string name, ulong value)
    {
      sb.Append("  ");
      sb.Append(name);
      sb.Append(": ");
      sb.Append(value.ToString("N0"));
      service.DrawText(sb.ToString(), trueColor);
      sb.Clear();
    }
  }
}
