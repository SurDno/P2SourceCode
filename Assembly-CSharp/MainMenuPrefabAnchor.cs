﻿using Engine.Common.Services;
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
    if (spawned)
      return;
    spawned = true;
    if (prefab == null)
      return;
    instance = Instantiate(prefab, transform, false);
    instance.name = prefab.name;
  }

  private void Despawn()
  {
    if (!spawned)
      return;
    spawned = false;
    if (instance == null)
      return;
    Destroy(instance);
    instance = null;
  }

  protected override void OnConnectToEngine() => CheckMainMenu();

  protected override void OnDisconnectFromEngine() => Despawn();

  private void Update()
  {
    if (!Connected)
      return;
    CheckMainMenu();
  }

  private void CheckMainMenu()
  {
    if (uiService?.Active is IMainMenu)
      Spawn();
    else
      Despawn();
  }
}
