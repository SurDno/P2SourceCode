using Engine.Common.Services;
using UnityEngine;

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

  private void Awake() => this.meshRenderer = this.GetComponent<MeshRenderer>();

  private void Update()
  {
    if (!this.connected)
      return;
    this.updateTimeLeft -= Time.deltaTime;
    if ((double) this.updateTimeLeft > 0.0)
      return;
    this.updateTimeLeft = 1f;
    int days = ServiceLocator.GetService<ITimeService>().GameTime.Days;
    if (this.currentDay == days)
      return;
    this.currentDay = days;
    this.UpdateMaterial(days);
  }

  protected override void OnConnectToEngine()
  {
    this.connected = (Object) this.meshRenderer != (Object) null;
  }

  protected override void OnDisconnectFromEngine() => this.connected = false;

  private void UpdateMaterial(int day)
  {
    if ((Object) this.meshRenderer == (Object) null)
      return;
    this.meshRenderer.material = this.GetMaterial(day);
  }

  private Material GetMaterial(int day)
  {
    for (int index = 0; index < this.afishaMaterialOverrides.Length; ++index)
    {
      if (this.afishaMaterialOverrides[index].Day == day)
        return this.afishaMaterialOverrides[index].AfishaMaterial;
    }
    return this.afishaMaterialDefault;
  }
}
