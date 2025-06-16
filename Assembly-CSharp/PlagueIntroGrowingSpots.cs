using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Source.Commons;
using Inspectors;

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
    IsGame = SceneManager.GetActiveScene().name != "PlagueIntro_Riot_Loader";
    collider = this.GetComponent<Collider>();
    audioSource = this.GetComponent<AudioSource>();
    AddMaterial(this.gameObject);
    if (subObjects != null)
    {
      for (int index = 0; index < subObjects.Length; ++index)
        AddMaterial(subObjects[index]);
    }
    ApplyLevel();
  }

  private void AddMaterial(GameObject gameObject)
  {
    MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
    if (!(bool) (Object) component)
      return;
    plagueSpotMaterials.Add(component.material);
  }

  private GameObject GetPlayerGameObject()
  {
    if (!IsGame)
      return GameObject.Find("FPSController");
    return ((IEntityView) ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
  }

  private void Update()
  {
    if (IsGame && InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    if (updateTimeLeft <= 0.0)
    {
      inside = AreYouLookingAtMe();
      updateTimeLeft = updateTime;
    }
    else
      updateTimeLeft -= Time.deltaTime;
    if (inside)
    {
      level += Time.deltaTime / growTime;
      ApplyLevel();
    }
    if (!((Object) audioSource != (Object) null))
      return;
    UpdateAudioSourceEnable();
    volume = !inside || level >= 1.0 ? Mathf.Clamp01(volume - Time.deltaTime / 1f) : Mathf.Clamp01(volume + Time.deltaTime / 1f);
    audioSource.volume = volume;
  }

  private void ApplyLevel()
  {
    for (int index = 0; index < plagueSpotMaterials.Count; ++index)
      plagueSpotMaterials[index].SetFloat("_Level", level);
  }

  private Camera GetCamera() => IsGame ? GameCamera.Instance.Camera : Camera.main;

  private bool AreYouLookingAtMe()
  {
    Camera camera = GetCamera();
    return (double) (collider.bounds.ClosestPoint(camera.transform.position) - camera.transform.position).magnitude <= startDistance && GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), collider.bounds);
  }

  private void UpdateAudioSourceEnable()
  {
    bool flag = (double) (this.transform.position - EngineApplication.PlayerPosition).sqrMagnitude < (double) (audioSource.maxDistance * audioSource.maxDistance) && volume != 0.0;
    if (audioSource.enabled == flag)
      return;
    audioSource.enabled = flag;
  }
}
