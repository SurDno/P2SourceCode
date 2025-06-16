using System;
using System.Collections.Generic;

namespace UnityEngine.PostProcessing
{
  public sealed class MaterialFactory : IDisposable
  {
    private Dictionary<string, Material> m_Materials;

    public MaterialFactory() => m_Materials = new Dictionary<string, Material>();

    public Material Get(string shaderName)
    {
      Material material1;
      if (!m_Materials.TryGetValue(shaderName, out material1))
      {
        Shader shader = Shader.Find(shaderName);
        Material material2 = !(shader == null) ? new Material(shader) : throw new ArgumentException(string.Format("Shader not found ({0})", shaderName));
        material2.name = string.Format("PostFX - {0}", shaderName.Substring(shaderName.LastIndexOf("/") + 1));
        material2.hideFlags = HideFlags.DontSave;
        material1 = material2;
        m_Materials.Add(shaderName, material1);
      }
      return material1;
    }

    public void Dispose()
    {
      Dictionary<string, Material>.Enumerator enumerator = m_Materials.GetEnumerator();
      while (enumerator.MoveNext())
        GraphicsUtils.Destroy(enumerator.Current.Value);
      m_Materials.Clear();
    }
  }
}
