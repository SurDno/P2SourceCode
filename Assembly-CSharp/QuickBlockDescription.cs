[CreateAssetMenu(fileName = "QuickBlock", menuName = "Pathologic2/States/QuickBlock", order = 101)]
public class QuickBlockDescription : ScriptableObject
{
  [Header("Быстрый блок")]
  [Tooltip("Вероятность (0-1) заблокировать удар")]
  public float QuickBlockProbability = 0.3f;
  [Tooltip("Вероятность (0-1) увернуться движением (выбрасывается уже после того как решено блогировать)")]
  public float DodgeProbability = 0.7f;
}
