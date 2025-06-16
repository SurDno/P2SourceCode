using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Button))]
public class OpenURLButton : MonoBehaviour
{
  [SerializeField]
  private string url;

  private void Awake()
  {
    GetComponent<Button>().onClick.AddListener(OnClick);
  }

  private void OnClick() => Application.OpenURL(url);
}
