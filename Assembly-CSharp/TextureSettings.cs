[CreateAssetMenu(menuName = "Data/Texture Settings")]
public class TextureSettings : ScriptableObject
{
  [SerializeField]
  private float memoryBudget = 512f;
  [SerializeField]
  private int maxLevelReduction = 2;
  [SerializeField]
  private int maxFileIORequests = 1024;
  [SerializeField]
  private int memoryRequirement = 1024;

  public void Apply()
  {
    QualitySettings.streamingMipmapsMemoryBudget = memoryBudget;
    QualitySettings.streamingMipmapsMaxLevelReduction = maxLevelReduction;
    QualitySettings.streamingMipmapsMaxFileIORequests = maxFileIORequests;
  }

  public bool CheckMemoryRequirement() => memoryRequirement <= SystemInfo.graphicsMemorySize;
}
