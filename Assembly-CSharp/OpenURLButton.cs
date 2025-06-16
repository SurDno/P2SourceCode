using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof (Button))]
public class OpenURLButton : MonoBehaviour
{
  [SerializeField]
  private string url;

  private void Awake()
  {
    this.GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnClick));
  }

  private void OnClick() => Application.OpenURL(this.url);
}
