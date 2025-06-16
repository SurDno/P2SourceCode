// Decompiled with JetBrains decompiler
// Type: DeferredShadingSetup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
public class DeferredShadingSetup : MonoBehaviour
{
  [SerializeField]
  private Shader deferredShader;

  private void OnEnable()
  {
    GraphicsSettings.SetCustomShader(BuiltinShaderType.DeferredShading, this.deferredShader);
    GraphicsSettings.SetShaderMode(BuiltinShaderType.DeferredShading, BuiltinShaderMode.UseCustom);
  }

  private void OnDisable()
  {
    GraphicsSettings.SetShaderMode(BuiltinShaderType.DeferredShading, BuiltinShaderMode.UseBuiltin);
    GraphicsSettings.SetCustomShader(BuiltinShaderType.DeferredShading, (Shader) null);
  }
}
