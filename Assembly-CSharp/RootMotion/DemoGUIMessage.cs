using UnityEngine;

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
