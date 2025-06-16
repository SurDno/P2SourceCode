using Engine.Common.Services;
using Engine.Source.Commons;
using Inspectors;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlagueIntroGrowingSpots : MonoBehaviour
{
  [SerializeField]
  private GameObject[] subObjects;
  [Inspected]
  private bool IsGame;
  [SerializeField]
  private float growTime = 3f;
  [SerializeField]
  private float startDistance = 7f;
  private List<Material> plagueSpotMaterials = new List<Material>();
  [Inspected]
  private float level;
  [Inspected]
  private float volume;
  private float updateTime = 0.2f;
  private float updateTimeLeft;
  private bool inside;
  private Collider collider;
  private AudioSource audioSource;

  private void Start()
  {
    this.IsGame = SceneManager.GetActiveScene().name != "PlagueIntro_Riot_Loader";
    this.collider = this.GetComponent<Collider>();
    this.audioSource = this.GetComponent<AudioSource>();
    this.AddMaterial(this.gameObject);
    if (this.subObjects != null)
    {
      for (int index = 0; index < this.subObjects.Length; ++index)
        this.AddMaterial(this.subObjects[index]);
    }
    this.ApplyLevel();
  }

  private void AddMaterial(GameObject gameObject)
  {
    MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
    if (!(bool) (Object) component)
      return;
    this.plagueSpotMaterials.Add(component.material);
  }

  private GameObject GetPlayerGameObject()
  {
    if (!this.IsGame)
      return GameObject.Find("FPSController");
    return ((IEntityView) ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
  }

  private void Update()
  {
    if (this.IsGame && InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    if ((double) this.updateTimeLeft <= 0.0)
    {
      this.inside = this.AreYouLookingAtMe();
      this.updateTimeLeft = this.updateTime;
    }
    else
      this.updateTimeLeft -= Time.deltaTime;
    if (this.inside)
    {
      this.level += Time.deltaTime / this.growTime;
      this.ApplyLevel();
    }
    if (!((Object) this.audioSource != (Object) null))
      return;
    this.UpdateAudioSourceEnable();
    this.volume = !this.inside || (double) this.level >= 1.0 ? Mathf.Clamp01(this.volume - Time.deltaTime / 1f) : Mathf.Clamp01(this.volume + Time.deltaTime / 1f);
    this.audioSource.volume = this.volume;
  }

  private void ApplyLevel()
  {
    for (int index = 0; index < this.plagueSpotMaterials.Count; ++index)
      this.plagueSpotMaterials[index].SetFloat("_Level", this.level);
  }

  private Camera GetCamera() => this.IsGame ? GameCamera.Instance.Camera : Camera.main;

  private bool AreYouLookingAtMe()
  {
    Camera camera = this.GetCamera();
    return (double) (this.collider.bounds.ClosestPoint(camera.transform.position) - camera.transform.position).magnitude <= (double) this.startDistance && GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), this.collider.bounds);
  }

  private void UpdateAudioSourceEnable()
  {
    bool flag = (double) (this.transform.position - EngineApplication.PlayerPosition).sqrMagnitude < (double) (this.audioSource.maxDistance * this.audioSource.maxDistance) && (double) this.volume != 0.0;
    if (this.audioSource.enabled == flag)
      return;
    this.audioSource.enabled = flag;
  }
}
