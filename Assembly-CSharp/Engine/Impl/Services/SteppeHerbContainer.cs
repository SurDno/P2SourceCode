using System;
using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Regions;
using Inspectors;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Engine.Impl.Services;

public class SteppeHerbContainer : IDisposable {
	private const float spawnRadius = 30f;
	private const float pointsPartToProcessPerUpdate = 0.1f;
	private IEntity template;
	private List<SteppeHerbEntry> SteppeHerbEntries = new();
	private List<int> SteppeHerbIndices = new();
	private List<int> SteppeHerbIndicesLeft = new();
	private List<int> NearPlayerIndices = new();
	private int processingIndex;
	private int amount;

	public SteppeHerbContainer(IEntity template, GameObject prefab) {
		this.template = template;
		var gameObject = Object.Instantiate(prefab);
		for (var index = 0; index < gameObject.transform.childCount; ++index) {
			var position = gameObject.transform.GetChild(index).position;
			SteppeHerbIndicesLeft.Add(SteppeHerbEntries.Count);
			SteppeHerbEntries.Add(new SteppeHerbEntry {
				Position = position,
				Entity = null,
				On = false
			});
		}

		Object.Destroy(gameObject);
	}

	[Inspected]
	public int Amount {
		get => amount;
		set {
			amount = Mathf.Clamp(value, 0, SteppeHerbEntries.Count);
			Syncronize();
		}
	}

	[Inspected]
	public void Reset() {
		foreach (var steppeHerbEntry in SteppeHerbEntries)
			steppeHerbEntry.Collected = false;
	}

	public void Dispose() {
		foreach (var steppeHerbIndex in SteppeHerbIndices) {
			var steppeHerbEntry = SteppeHerbEntries[steppeHerbIndex];
			if (steppeHerbEntry.Entity != null) {
				steppeHerbEntry.Entity.Dispose();
				steppeHerbEntry.Entity = null;
				steppeHerbEntry.On = false;
			}
		}
	}

	private void Syncronize() {
		if (amount == SteppeHerbIndices.Count)
			return;
		if (amount < SteppeHerbIndices.Count) {
			var num = SteppeHerbIndices.Count - amount;
			for (var index1 = 0; index1 < num; ++index1) {
				var index2 = Random.Range(0, SteppeHerbIndices.Count);
				var steppeHerbIndex = SteppeHerbIndices[index2];
				SteppeHerbEntries[steppeHerbIndex].On = false;
				SteppeHerbIndices.RemoveAt(index2);
				SteppeHerbIndicesLeft.Add(steppeHerbIndex);
			}
		} else {
			var num = amount - SteppeHerbIndices.Count;
			for (var index3 = 0; index3 < num; ++index3) {
				var index4 = Random.Range(0, SteppeHerbIndicesLeft.Count);
				var index5 = SteppeHerbIndicesLeft[index4];
				SteppeHerbEntries[index5].On = true;
				SteppeHerbIndicesLeft.RemoveAt(index4);
				SteppeHerbIndices.Add(index5);
			}
		}
	}

	private IEntity SpawnHerb(SteppeHerbEntry entry, LocationComponent locationComponent) {
		var entity = ServiceCache.Factory.Instantiate(template);
		((Entity)entity).DontSave = true;
		var service = ServiceLocator.GetService<Simulation>();
		service.Add(entity, service.Objects);
		var component = entity.GetComponent<NavigationComponent>();
		if (component != null) {
			var rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 6.28318548f), 0.0f);
			component.TeleportTo(locationComponent, entry.Position, rotation);
		} else
			Debug.LogError("NavigationComponent not found : " + entity.GetInfo());

		return entity;
	}

	private bool IsCollected(IEntity entity) {
		var component = entity.GetComponent<ParametersComponent>();
		if (component == null)
			return true;
		var byName = component.GetByName<bool>(ParameterNameEnum.Collected);
		return byName == null || byName.Value;
	}

	public void Update(Vector3 playerPosition) {
		if (SteppeHerbEntries.Count == 0)
			return;
		var regionByName = RegionUtility.GetRegionByName(RegionEnum.Steppe);
		if (regionByName == null)
			Debug.LogError("region == null");
		else {
			var component = regionByName.GetComponent<LocationComponent>();
			var index1 = 0;
			Vector3 vector3;
			while (index1 < NearPlayerIndices.Count) {
				vector3 = SteppeHerbEntries[NearPlayerIndices[index1]].Position - playerPosition;
				if (vector3.magnitude > 30.0) {
					var steppeHerbEntry = SteppeHerbEntries[NearPlayerIndices[index1]];
					if (steppeHerbEntry.Entity != null) {
						steppeHerbEntry.Collected = IsCollected(steppeHerbEntry.Entity);
						steppeHerbEntry.Entity.Dispose();
						steppeHerbEntry.Entity = null;
					}

					NearPlayerIndices.RemoveAt(index1);
				} else
					++index1;
			}

			var num = (int)(SteppeHerbEntries.Count * 0.10000000149011612) + 1;
			for (var index2 = 0; index2 < num; ++index2) {
				processingIndex = (processingIndex + 1) % SteppeHerbEntries.Count;
				vector3 = SteppeHerbEntries[processingIndex].Position - playerPosition;
				if (vector3.magnitude < 30.0 && !NearPlayerIndices.Contains(processingIndex)) {
					var steppeHerbEntry = SteppeHerbEntries[processingIndex];
					if (steppeHerbEntry.On && !steppeHerbEntry.Collected) {
						if (steppeHerbEntry.Entity != null)
							Debug.LogError("Entity can't be null here");
						else
							steppeHerbEntry.Entity = SpawnHerb(steppeHerbEntry, component);
					}

					NearPlayerIndices.Add(processingIndex);
				}
			}
		}
	}
}