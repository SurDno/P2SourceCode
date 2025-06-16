// Decompiled with JetBrains decompiler
// Type: PlagueSpotWithPlayerTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Engine.Source.Components.Utilities;
using Inspectors;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable
public class PlagueSpotWithPlayerTrigger : MonoBehaviour
{
  [SerializeField]
  private float growTime = 5f;
  [Inspected]
  private bool IsGame;
  private Material plagueSpotMaterial;
  [Inspected]
  private PlagueSpotWithPlayerTrigger.State state = PlagueSpotWithPlayerTrigger.State.Unknown;
  [Inspected]
  private float level = 0.0f;
  private AudioSource audioSource;
  private float volume;

  private void SetState(PlagueSpotWithPlayerTrigger.State state)
  {
    switch (state)
    {
      case PlagueSpotWithPlayerTrigger.State.Waiting:
        this.level = 0.0f;
        this.plagueSpotMaterial.SetFloat("_Level", 0.0f);
        break;
      case PlagueSpotWithPlayerTrigger.State.Growing:
        this.level = 0.0f;
        this.audioSource.PlayAndCheck();
        this.plagueSpotMaterial.SetFloat("_Level", 0.0f);
        break;
    }
    this.state = state;
  }

  private void UpdateState()
  {
    switch (this.state)
    {
      case PlagueSpotWithPlayerTrigger.State.Growing:
        this.level = Mathf.Clamp01(this.level + Time.deltaTime / this.growTime);
        if (Mathf.Approximately(this.level, 1f))
          this.SetState(PlagueSpotWithPlayerTrigger.State.Done);
        this.plagueSpotMaterial.SetFloat("_Level", Mathf.Clamp01(SmoothUtility.Smooth22(Mathf.Sqrt(this.level))));
        if (!this.audioSource.isPlaying)
          this.audioSource.PlayAndCheck();
        this.volume = Mathf.MoveTowards(this.volume, 1f, Time.deltaTime / 2f);
        this.audioSource.volume = this.volume;
        break;
      case PlagueSpotWithPlayerTrigger.State.Done:
        this.volume = Mathf.MoveTowards(this.volume, 0.0f, Time.deltaTime / 2f);
        this.audioSource.volume = this.volume;
        break;
    }
  }

  private void Start()
  {
    this.plagueSpotMaterial = this.gameObject.GetComponent<MeshRenderer>().material;
    this.IsGame = SceneManager.GetActiveScene().name != "PlagueIntro_Riot_Loader";
    this.audioSource = this.GetComponent<AudioSource>();
    this.SetState(PlagueSpotWithPlayerTrigger.State.Waiting);
  }

  private void Update() => this.UpdateState();

  private void OnTriggerEnter(Collider other)
  {
    GameObject playerGameObject = this.GetPlayerGameObject();
    if ((Object) playerGameObject == (Object) null || !((Object) playerGameObject == (Object) other.gameObject) || this.state != PlagueSpotWithPlayerTrigger.State.Waiting)
      return;
    this.SetState(PlagueSpotWithPlayerTrigger.State.Growing);
  }

  private GameObject GetPlayerGameObject()
  {
    if (!this.IsGame)
      return GameObject.Find("FPSController");
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    if (player == null)
      return (GameObject) null;
    return ((IEntityView) player)?.GameObject;
  }

  private enum State
  {
    Unknown,
    Waiting,
    Growing,
    Done,
  }
}
