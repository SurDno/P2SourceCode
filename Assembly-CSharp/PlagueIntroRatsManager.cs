using System.Collections.Generic;
using Engine.Common.Services;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlagueIntroRatsManager : MonoBehaviour {
	[Inspected] private bool IsGame;
	[SerializeField] private int ratPoolCount = 30;
	[SerializeField] private int spawnPointCollectRadius = 30;
	[SerializeField] private GameObject ratPrefab;
	[Inspected] private List<PlagueIntroRat> ratPool = new();
	[Inspected] private int ratCount;
	[Inspected] private List<int> spawnPointsIndices = new();
	[Inspected] private List<Vector3> allSpawnPoints = new();
	[Inspected] private HashSet<int> lockedPoints = new();
	[Inspected] private GameObject spawnRootGO;

	private void Start() {
		IsGame = SceneManager.GetActiveScene().name != "PlagueIntro_Riot_Loader";
		spawnRootGO = new GameObject("[PlagueIntroRatsManager Root]");
		for (var index = 0; index < ratPoolCount; ++index) {
			var component = Instantiate(ratPrefab, spawnRootGO.transform).GetComponent<PlagueIntroRat>();
			component.Init(this);
			ratPool.Add(component);
		}

		for (var index = 0; index < transform.childCount; ++index) {
			NavMeshHit hit;
			if (NavMesh.SamplePosition(transform.GetChild(index).transform.position, out hit, 0.5f, -1))
				allSpawnPoints.Add(hit.position);
		}
	}

	private void OnDestroy() {
		for (var index = 0; index < ratPoolCount; ++index)
			if (ratPool[index] != null)
				Destroy(ratPool[index].gameObject);
		Destroy(spawnRootGO);
	}

	private GameObject GetPlayerGameObject() {
		if (!IsGame)
			return GameObject.Find("FPSController");
		return ((IEntityView)ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
	}

	private void Update() {
		UpdateSpawnPoints();
		if (ratCount == ratPoolCount || spawnPointsIndices.Count < 1)
			return;
		var index = Random.Range(0, spawnPointsIndices.Count);
		var spawnPointsIndex = spawnPointsIndices[index];
		if (lockedPoints.Contains(spawnPointsIndex))
			return;
		var allSpawnPoint1 = allSpawnPoints[spawnPointsIndex];
		if (ratCount == ratPoolCount)
			return;
		var allSpawnPoint2 =
			allSpawnPoints[
				spawnPointsIndices[(index + 1 + Random.Range(0, spawnPointsIndices.Count)) % spawnPointsIndices.Count]];
		Spawn(spawnPointsIndex, allSpawnPoint1, allSpawnPoint2);
	}

	private void UpdateSpawnPoints() {
		var playerGameObject = GetPlayerGameObject();
		if (!(bool)(Object)playerGameObject)
			return;
		var position = playerGameObject.transform.position;
		spawnPointsIndices.Clear();
		for (var index = 0; index < allSpawnPoints.Count; ++index)
			if ((position - allSpawnPoints[index]).magnitude < (double)spawnPointCollectRadius)
				spawnPointsIndices.Add(index);
	}

	public void Spawn(int lockIndex, Vector3 from, Vector3 to) {
		var plagueIntroRat = ratPool[ratCount];
		plagueIntroRat.gameObject.SetActive(true);
		plagueIntroRat.Spawn(lockIndex, from, to);
		++ratCount;
		lockedPoints.Add(lockIndex);
	}

	public void Dispose(PlagueIntroRat rat, int lockIndex) {
		rat.gameObject.SetActive(false);
		lockedPoints.Remove(lockIndex);
		if (ratCount > 1) {
			ratPool[ratPool.FindIndex(0, v => v == rat)] = ratPool[ratCount - 1];
			ratPool[ratCount - 1] = rat;
		}

		--ratCount;
	}
}