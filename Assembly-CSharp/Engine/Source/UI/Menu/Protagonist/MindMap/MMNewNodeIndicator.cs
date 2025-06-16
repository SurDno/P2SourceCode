using UnityEngine;

namespace Engine.Source.UI.Menu.Protagonist.MindMap
{
  public class MMNewNodeIndicator : MonoBehaviour
  {
    [SerializeField]
    private CanvasGroup image0;
    [SerializeField]
    private CanvasGroup image1;
    [SerializeField]
    private float loopLength = 1f;
    [SerializeField]
    private float farScale = 1.2f;
    private float time = 0.0f;

    private void OnEnable() => this.time = Random.value * this.loopLength;

    private void Update()
    {
      this.time += Time.unscaledDeltaTime;
      this.ApplyTime();
    }

    private void ApplyTime()
    {
      float f = this.time / this.loopLength;
      float phase1 = f - Mathf.Floor(f);
      this.ApplyPhase(this.image0, phase1);
      float phase2 = phase1 + 0.5f;
      if ((double) phase2 >= 1.0)
        --phase2;
      this.ApplyPhase(this.image1, phase2);
    }

    private void ApplyPhase(CanvasGroup image, float phase)
    {
      float f = phase * 3.14159274f;
      float num1 = Mathf.Sin(f);
      float num2 = (float) (1.0 + ((double) this.farScale - 1.0) * ((double) Mathf.Cos(f) * 0.5 + 0.5));
      image.alpha = num1;
      image.transform.localScale = new Vector3(num2, num2, num2);
    }
  }
}
