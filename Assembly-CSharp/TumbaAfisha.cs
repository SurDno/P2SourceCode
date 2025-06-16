using Engine.Common.Services;

internal class TumbaAfisha : EngineDependent
{
  [SerializeField]
  private Material afishaMaterialDefault;
  [Tooltip("Day numeration starts with 1")]
  [SerializeField]
  private TumbaAfishaEntry[] afishaMaterialOverrides;
  private MeshRenderer meshRenderer;
  private bool connected;
  private float updateTimeLeft;
  private int currentDay = -1;

  private void Awake() => meshRenderer = this.GetComponent<MeshRenderer>();

  private void Update()
  {
    if (!connected)
      return;
    updateTimeLeft -= Time.deltaTime;
    if (updateTimeLeft > 0.0)
      return;
    updateTimeLeft = 1f;
    int days = ServiceLocator.GetService<ITimeService>().GameTime.Days;
    if (currentDay == days)
      return;
    currentDay = days;
    UpdateMaterial(days);
  }

  protected override void OnConnectToEngine()
  {
    connected = (Object) meshRenderer != (Object) null;
  }

  protected override void OnDisconnectFromEngine() => connected = false;

  private void UpdateMaterial(int day)
  {
    if ((Object) meshRenderer == (Object) null)
      return;
    meshRenderer.material = GetMaterial(day);
  }

  private Material GetMaterial(int day)
  {
    for (int index = 0; index < afishaMaterialOverrides.Length; ++index)
    {
      if (afishaMaterialOverrides[index].Day == day)
        return afishaMaterialOverrides[index].AfishaMaterial;
    }
    return afishaMaterialDefault;
  }
}
