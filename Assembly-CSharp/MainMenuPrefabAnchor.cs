using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.UI;
using UnityEngine;

public class MainMenuPrefabAnchor : EngineDependent
{
  [SerializeField]
  private GameObject prefab;
  [FromLocator]
  private UIService uiService;
  private bool spawned;
  private GameObject instance;

  private void Spawn()
  {
    if (this.spawned)
      return;
    this.spawned = true;
    if ((Object) this.prefab == (Object) null)
      return;
    this.instance = Object.Instantiate<GameObject>(this.prefab, this.transform, false);
    this.instance.name = this.prefab.name;
  }

  private void Despawn()
  {
    if (!this.spawned)
      return;
    this.spawned = false;
    if ((Object) this.instance == (Object) null)
      return;
    Object.Destroy((Object) this.instance);
    this.instance = (GameObject) null;
  }

  protected override void OnConnectToEngine() => this.CheckMainMenu();

  protected override void OnDisconnectFromEngine() => this.Despawn();

  private void Update()
  {
    if (!this.Connected)
      return;
    this.CheckMainMenu();
  }

  private void CheckMainMenu()
  {
    if (this.uiService?.Active is IMainMenu)
      this.Spawn();
    else
      this.Despawn();
  }
}
