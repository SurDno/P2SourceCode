// Decompiled with JetBrains decompiler
// Type: RootMotion.DemoGUIMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace RootMotion
{
  public class DemoGUIMessage : MonoBehaviour
  {
    public string text;
    public Color color = Color.white;

    private void OnGUI()
    {
      GUI.color = this.color;
      GUILayout.Label(this.text);
      GUI.color = Color.white;
    }
  }
}
