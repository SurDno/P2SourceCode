using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class ControlIconsManager : MonoBehaviour
  {
    public static ControlIconsManager Instance;
    [SerializeField]
    private StringSpriteMap xboxMap;
    [SerializeField]
    private StringSpriteMap psMap;

    private void Awake()
    {
      if ((Object) ControlIconsManager.Instance != (Object) null)
        Object.Destroy((Object) ControlIconsManager.Instance.gameObject);
      ControlIconsManager.Instance = this;
    }

    public Sprite GetIconSprite(GameActionType type, out bool isHold)
    {
      return this.xboxMap.GetValue(InputUtility.GetHotKeyNameByAction(type, InputService.Instance.JoystickUsed, out isHold));
    }

    public Sprite GetIconSprite(string name) => this.xboxMap.GetValue(name);
  }
}
