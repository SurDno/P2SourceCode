using Engine.Common.Services;
using Engine.Source.Commons;
using Inspectors;
using System;
using System.Collections.Generic;
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
    this.IsGame = SceneManager.GetActiveScene().name != "PlagueIntro_Riot_Loader";
    this.spawnRootGO = new GameObject("[PlagueIntroRatsManager Root]");
    for (int index = 0; index < this.ratPoolCount; ++index)
    {
      PlagueIntroRat component = UnityEngine.Object.Instantiate<GameObject>(this.ratPrefab, this.spawnRootGO.transform).GetComponent<PlagueIntroRat>();
      component.Init(this);
      this.ratPool.Add(component);
    }
    for (int index = 0; index < this.transform.childCount; ++index)
    {
      NavMeshHit hit;
      if (NavMesh.SamplePosition(this.transform.GetChild(index).transform.position, out hit, 0.5f, -1))
        this.allSpawnPoints.Add(hit.position);
    }
  }

  private void OnDestroy()
  {
    for (int index = 0; index < this.ratPoolCount; ++index)
    {
      if ((UnityEngine.Object) this.ratPool[index] != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.ratPool[index].gameObject);
    }
    UnityEngine.Object.Destroy((UnityEngine.Object) this.spawnRootGO);
  }

  private GameObject GetPlayerGameObject()
  {
    if (!this.IsGame)
      return GameObject.Find("FPSController");
    return ((IEntityView) ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
  }

  private void Update()
  {
    this.UpdateSpawnPoints();
    if (this.ratCount == this.ratPoolCount || this.spawnPointsIndices.Count < 1)
      return;
    int index = UnityEngine.Random.Range(0, this.spawnPointsIndices.Count);
    int spawnPointsIndex = this.spawnPointsIndices[index];
    if (this.lockedPoints.Contains(spawnPointsIndex))
      return;
    Vector3 allSpawnPoint1 = this.allSpawnPoints[spawnPointsIndex];
    if (this.ratCount == this.ratPoolCount)
      return;
    Vector3 allSpawnPoint2 = this.allSpawnPoints[this.spawnPointsIndices[(index + 1 + UnityEngine.Random.Range(0, this.spawnPointsIndices.Count)) % this.spawnPointsIndices.Count]];
    this.Spawn(spawnPointsIndex, allSpawnPoint1, allSpawnPoint2);
  }

  private void UpdateSpawnPoints()
  {
    GameObject playerGameObject = this.GetPlayerGameObject();
    if (!(bool) (UnityEngine.Object) playerGameObject)
      return;
    Vector3 position = playerGameObject.transform.position;
    this.spawnPointsIndices.Clear();
    for (int index = 0; index < this.allSpawnPoints.Count; ++index)
    {
      if ((double) (position - this.allSpawnPoints[index]).magnitude < (double) this.spawnPointCollectRadius)
        this.spawnPointsIndices.Add(index);
    }
  }

  public void Spawn(int lockIndex, Vector3 from, Vector3 to)
  {
    PlagueIntroRat plagueIntroRat = this.ratPool[this.ratCount];
    plagueIntroRat.gameObject.SetActive(true);
    plagueIntroRat.Spawn(lockIndex, from, to);
    ++this.ratCount;
    this.lockedPoints.Add(lockIndex);
  }

  public void Dispose(PlagueIntroRat rat, int lockIndex)
  {
    rat.gameObject.SetActive(false);
    this.lockedPoints.Remove(lockIndex);
    if (this.ratCount > 1)
    {
      this.ratPool[this.ratPool.FindIndex(0, (Predicate<PlagueIntroRat>) (v => (UnityEngine.Object) v == (UnityEngine.Object) rat))] = this.ratPool[this.ratCount - 1];
      this.ratPool[this.ratCount - 1] = rat;
    }
    --this.ratCount;
  }
}
