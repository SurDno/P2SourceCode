namespace Engine.Source.UI.Menu.Protagonist.LockPicking
{
  [CreateAssetMenu(fileName = "Lock Picking Settings", menuName = "Data/Lock Picking Settings")]
  public class LockPickingSettingsData : ScriptableObject
  {
    [SerializeField]
    private LockPickingSettings settings;

    public LockPickingSettings Settings => settings;
  }
}
