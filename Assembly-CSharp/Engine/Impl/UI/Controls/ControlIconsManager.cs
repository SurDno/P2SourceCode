// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ControlIconsManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using InputServices;
using UnityEngine;

#nullable disable
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
