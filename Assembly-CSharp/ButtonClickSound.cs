using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof (Button))]
public class ButtonClickSound : MonoBehaviour
{
  private void Awake()
  {
    this.GetComponent<Button>().onClick.AddListener(new UnityAction(this.PlaySound));
  }

  private void PlaySound() => MonoBehaviourInstance<UISounds>.Instance.PlayClickSound();
}
