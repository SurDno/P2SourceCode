using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using UnityEngine;

public class CurrentGameNameView : MonoBehaviour
{
  [SerializeField]
  private StringView view;

  private void OnEnable()
  {
    this.view.StringValue = InstanceByRequest<GameDataService>.Instance.GetCurrentGameData().GameName;
  }
}
