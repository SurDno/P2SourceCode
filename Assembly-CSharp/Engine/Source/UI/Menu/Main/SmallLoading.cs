using System;
using UnityEngine;

namespace Engine.Source.UI.Menu.Main
{
  public class SmallLoading : MonoBehaviour
  {
    private static bool _showBackground = false;
    private static Action<bool> onBackground;
    [SerializeField]
    private GameObject background;

    public static bool showBackground
    {
      get => SmallLoading._showBackground;
      set
      {
        SmallLoading._showBackground = value;
        Action<bool> onBackground = SmallLoading.onBackground;
        if (onBackground == null)
          return;
        onBackground(value);
      }
    }

    private void Awake()
    {
      SmallLoading.onBackground += new Action<bool>(this.OnBackground);
      this.OnBackground(SmallLoading._showBackground);
    }

    private void OnDestroy() => SmallLoading.onBackground -= new Action<bool>(this.OnBackground);

    private void OnBackground(bool active) => this.background.SetActive(active);
  }
}
