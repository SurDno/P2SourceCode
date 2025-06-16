using Engine.Source.Services.Inputs;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class GameActionView : MonoBehaviour
  {
    public abstract void SetValue(GameActionType value, bool instant);

    public abstract GameActionType GetValue();
  }
}
