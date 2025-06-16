public class Rootmotion45 : MonoBehaviour
{
  [Header("Agent speed")]
  [Tooltip("Желательно подобрать эту скорость максимально близко с скорости цикла шага. Навигационный агент будет передвигаться на такой скорости во время бега.")]
  public float WalkSpeed;
  [Tooltip("Желательно подобрать эту скорость максимально близко с скорости цикла бега. Навигационный агент будет передвигаться на такой скорости во время бега.")]
  public float RunSpeed;
  [Header("Exact positioning data (Walk)")]
  [Tooltip("Скейл вызванный ретаргетом (например, маленький мальчик ходит на анимациях девочки)")]
  public float RetargetLegScale = 1f;
  [Tooltip("Длина цикла шага в метрах")]
  public float WalkCycleLength = 1f;
  [Tooltip("Длина левого стопа в метрах")]
  public float WalkLeftLegStopLength = 1f;
  [Tooltip("Длина правого стопа в метрах")]
  public float WalkRightLegStopLength = 1f;
  [Header("Exact positioning data (Run)")]
  [Tooltip("Длина цикла шага в метрах")]
  public float RunCycleLength = 2f;
  [Tooltip("Длина левого стопа в метрах")]
  public float RunLeftLegStopLength = 1f;
  [Tooltip("Длина правого стопа в метрах")]
  public float RunRightLegStopLength = 1f;
  [Header("Right")]
  public bool AngleR000ToLeft;
  public bool AngleR045ToLeft;
  public bool AngleR090ToLeft;
  public bool AngleR135ToLeft;
  public bool AngleR180ToLeft;
  [Header("Left")]
  public bool AngleL000ToLeft;
  public bool AngleL045ToLeft;
  public bool AngleL090ToLeft;
  public bool AngleL135ToLeft;
  public bool AngleL180ToLeft;
  [Header("Run Right")]
  public bool AngleRunR000ToLeft = false;
  public bool AngleRunR090ToLeft = false;
  public bool AngleRunR180ToLeft = false;
  [Header("Run Left")]
  public bool AngleRunL000ToLeft = false;
  public bool AngleRunL090ToLeft = true;
  public bool AngleRunL180ToLeft = true;
}
