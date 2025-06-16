using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlagueIntroRatsManager : MonoBehaviour
{
  [Inspected]
  private bool IsGame;
  [SerializeField]
  private int ratPoolCount = 30;
  [SerializeField]
  private int spawnPointCollectRadius = 30;
  [SerializeField]
  private GameObject ratPrefab;
  [Inspected]
  private List<PlagueIntroRat> ratPool = new List<PlagueIntroRat>();
  [Inspected]
  private int ratCount;
  [Inspected]
  private List<int> spawnPointsIndices = new List<int>();
  [Inspected]
  private List<Vector3> allSpawnPoints = new List<Vector3>();
  [Inspected]
  private HashSet<int> lockedPoints = new HashSet<int>();
  [Inspected]
  private GameObject spawnRootGO;

  private void Start()
  {
    IsGame = SceneManager.GetActiveScene().name != "PlagueIntro_Riot_Loader";
    spawnRootGO = new GameObject("[PlagueIntroRatsManager Root]");
    for (int index = 0; index < ratPoolCount; ++index)
    {
      PlagueIntroRat component = Instantiate(ratPrefab, spawnRootGO.transform).GetComponent<PlagueIntroRat>();
      component.Init(this);
      ratPool.Add(component);
    }
    for (int index = 0; index < transform.childCount; ++index)
    {
      NavMeshHit hit;
      if (NavMesh.SamplePosition(transform.GetChild(index).transform.position, out hit, 0.5f, -1))
        allSpawnPoints.Add(hit.position);
    }
  }

  private void OnDestroy()
  {
    for (int index = 0; index < ratPoolCount; ++index)
    {
      if (ratPool[index] != null)
        Destroy(ratPool[index].gameObject);
    }
    Destroy(spawnRootGO);
  }

  private GameObject GetPlayerGameObject()
  {
    if (!IsGame)
      return GameObject.Find("FPSController");
    return ((IEntityView) ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
  }

  private void Update()
  {
    UpdateSpawnPoints();
    if (ratCount == ratPoolCount || spawnPointsIndices.Count < 1)
      return;
    int index = Random.Range(0, spawnPointsIndices.Count);
    int spawnPointsIndex = spawnPointsIndices[index];
    if (lockedPoints.Contains(spawnPointsIndex))
      return;
    Vector3 allSpawnPoint1 = allSpawnPoints[spawnPointsIndex];
    if (ratCount == ratPoolCount)
      return;
    Vector3 allSpawnPoint2 = allSpawnPoints[spawnPointsIndices[(index + 1 + Random.Range(0, spawnPointsIndices.Count)) % spawnPointsIndices.Count]];
    Spawn(spawnPointsIndex, allSpawnPoint1, allSpawnPoint2);
  }

  private void UpdateSpawnPoints()
  {
    GameObject playerGameObject = GetPlayerGameObject();
    if (!(bool) (Object) playerGameObject)
      return;
    Vector3 position = playerGameObject.transform.position;
    spawnPointsIndices.Clear();
    for (int index = 0; index < allSpawnPoints.Count; ++index)
    {
      if ((position - allSpawnPoints[index]).magnitude < (double) spawnPointCollectRadius)
        spawnPointsIndices.Add(index);
    }
  }

  public void Spawn(int lockIndex, Vector3 from, Vector3 to)
  {
    PlagueIntroRat plagueIntroRat = ratPool[ratCount];
    plagueIntroRat.gameObject.SetActive(true);
    plagueIntroRat.Spawn(lockIndex, from, to);
    ++ratCount;
    lockedPoints.Add(lockIndex);
  }

  public void Dispose(PlagueIntroRat rat, int lockIndex)
  {
    rat.gameObject.SetActive(false);
    lockedPoints.Remove(lockIndex);
    if (ratCount > 1)
    {
      ratPool[ratPool.FindIndex(0, v => v == rat)] = ratPool[ratCount - 1];
      ratPool[ratCount - 1] = rat;
    }
    --ratCount;
  }
}
