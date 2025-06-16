using Cofe.Meta;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services.Gizmos;
using Engine.Source.Utility;
using System;
using System.Text;
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

    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += (Action) (() => GroupDebugService.RegisterGroup(TextureStreamingGroupDebug.name, TextureStreamingGroupDebug.key, TextureStreamingGroupDebug.modifficators, new Action(TextureStreamingGroupDebug.Update)));
    }

    private static void Update()
    {
      TextureStreamingGroupDebug.service = ServiceLocator.GetService<GizmoService>();
      if (TextureStreamingGroupDebug.sb == null)
        TextureStreamingGroupDebug.sb = new StringBuilder();
      TextureStreamingGroupDebug.sb.Append('\n');
      TextureStreamingGroupDebug.sb.Append(TextureStreamingGroupDebug.name);
      TextureStreamingGroupDebug.sb.Append(" (");
      TextureStreamingGroupDebug.sb.Append(InputUtility.GetHotKeyText(TextureStreamingGroupDebug.key, TextureStreamingGroupDebug.modifficators));
      TextureStreamingGroupDebug.sb.Append(")");
      TextureStreamingGroupDebug.service.DrawText(TextureStreamingGroupDebug.sb.ToString(), TextureStreamingGroupDebug.headerColor);
      TextureStreamingGroupDebug.sb.Clear();
      TextureStreamingGroupDebug.AppendProperty("Current Texture Memory", Texture.currentTextureMemory);
      TextureStreamingGroupDebug.AppendProperty("Desired Texture Memory", Texture.desiredTextureMemory);
      TextureStreamingGroupDebug.AppendProperty("Non-Streaming Texture Count", Texture.nonStreamingTextureCount);
      TextureStreamingGroupDebug.AppendProperty("Non-Streaming Texture Memory", Texture.nonStreamingTextureMemory);
      TextureStreamingGroupDebug.AppendProperty("Streaming Mipmap Upload Count", Texture.streamingMipmapUploadCount);
      TextureStreamingGroupDebug.AppendProperty("Streaming Renderer Count", Texture.streamingRendererCount);
      TextureStreamingGroupDebug.AppendProperty("Streaming Texture Count", Texture.streamingTextureCount);
      TextureStreamingGroupDebug.AppendProperty("Streaming Texture Loading Count", Texture.streamingTextureLoadingCount);
      TextureStreamingGroupDebug.AppendProperty("Streaming Texture Pending Load Count", Texture.streamingTexturePendingLoadCount);
      TextureStreamingGroupDebug.AppendProperty("Target Texture Memory", Texture.targetTextureMemory);
      TextureStreamingGroupDebug.AppendProperty("Total Texture Memory", Texture.totalTextureMemory);
      TextureStreamingGroupDebug.service = (GizmoService) null;
    }

    private static void AppendProperty(string name, ulong value)
    {
      TextureStreamingGroupDebug.sb.Append("  ");
      TextureStreamingGroupDebug.sb.Append(name);
      TextureStreamingGroupDebug.sb.Append(": ");
      TextureStreamingGroupDebug.sb.Append(value.ToString("N0"));
      TextureStreamingGroupDebug.service.DrawText(TextureStreamingGroupDebug.sb.ToString(), TextureStreamingGroupDebug.trueColor);
      TextureStreamingGroupDebug.sb.Clear();
    }
  }
}
