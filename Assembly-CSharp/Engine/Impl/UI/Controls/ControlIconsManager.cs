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
      if (Instance != null)
        Destroy(Instance.gameObject);
      Instance = this;
    }

    public Sprite GetIconSprite(GameActionType type, out bool isHold)
    {
      return xboxMap.GetValue(InputUtility.GetHotKeyNameByAction(type, InputService.Instance.JoystickUsed, out isHold));
    }

    public Sprite GetIconSprite(string name) => xboxMap.GetValue(name);
  }
}
