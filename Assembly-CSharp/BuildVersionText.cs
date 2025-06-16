using Engine.Source.Commons;

public class BuildVersionText : MonoBehaviour
{
  private void Start()
  {
    InstanceByRequest<LabelService>.Instance.OnInvalidate += OnInvalidate;
    OnInvalidate();
  }

  private void OnInvalidate()
  {
    this.GetComponent<Text>().text = InstanceByRequest<LabelService>.Instance.Label;
  }
}
