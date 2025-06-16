using System.Collections.Generic;
using System.Linq;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.MessangerStationary;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Settings;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Services;

[GameService(typeof(PostmanStaticTeleportService))]
public class PostmanStaticTeleportService : IInitialisable, IUpdatable {
	private const float hibernateTrialTime = 0.5f;
	private const float playerSeesMeUpdatedTrialTime = 10f;
	private const float trialRepeatTime = 30f;
	[Inspected] private HashSet<Spawnpoint> spawnpoints = new();
	[Inspected] private List<Slot> postmans = new();
	[Inspected] private List<Slot> teleports = new();

	public IEnumerable<Spawnpoint> Spawnpoints => spawnpoints;

	public void AddSpawnpoints(Spawnpoint point) {
		spawnpoints.Add(point);
	}

	public void RemoveSpawnpoints(Spawnpoint point) {
		spawnpoints.Remove(point);
	}

	public void Initialise() {
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
	}

	public void Terminate() {
		spawnpoints.Clear();
		postmans.Clear();
		teleports.Clear();
		InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
	}

	private bool IsRegistered(IEntity owner) {
		return postmans.FindIndex(s => s.Owner == owner) != -1;
	}

	public void RegisterPostman(IEntity owner, SpawnpointKindEnum spawnpointKind) {
		Debug.Log(ObjectInfoUtility.GetStream().Append("Register static postman, kind : ").Append(spawnpointKind)
			.Append(" , owner : ").GetInfo(owner));
		if (spawnpointKind == SpawnpointKindEnum.None)
			Debug.LogError(
				string.Format("Adding static postman with {0} {1}", SpawnpointKindEnum.None, owner.GetInfo()));
		if (IsRegistered(owner))
			Debug.LogError("Postman already register, owner : " + owner.GetInfo());
		else
			postmans.Add(new Slot {
				Owner = owner,
				TimeLeft = 0.0f,
				SpawnpointKind = spawnpointKind
			});
	}

	public void UnregisterPostman(IEntity owner) {
		Debug.Log(ObjectInfoUtility.GetStream().Append("Unregister static postman, owner : ").GetInfo(owner));
		if (!IsRegistered(owner))
			Debug.LogError("Postman already unregister, owner : " + owner.GetInfo());
		else {
			var index = postmans.FindIndex(s => s.Owner == owner);
			if (postmans[index].Spawnpoint != null)
				postmans[index].Spawnpoint.Locked = false;
			postmans.RemoveAt(index);
		}
	}

	public void ComputeUpdate() {
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused || postmans.Count == 0)
			return;
		var player = ServiceLocator.GetService<ISimulation>().Player;
		if (player == null)
			return;
		var gameObject = ((IEntityView)player).GameObject;
		if (gameObject == null)
			return;
		var component1 = player.GetComponent<LocationItemComponent>();
		if (component1 == null || component1.IsIndoor)
			return;
		var component2 = player.GetComponent<PlayerControllerComponent>();
		if (component2 != null && !component2.CanReceiveMail.Value)
			return;
		UpdatePostmans(Time.deltaTime, gameObject, component1);
	}

	private void UpdatePostmans(
		float deltaTime,
		GameObject playerGameObject,
		LocationItemComponent playerLocation) {
		teleports.Clear();
		foreach (var postman in postmans) {
			var location = postman.Owner.GetComponent<LocationItemComponent>()?.Location;
			if (location != null) {
				postman.TimeLeft -= Time.deltaTime;
				if (location.IsHibernation)
					postman.TimeLeft = Mathf.Min(postman.TimeLeft, 0.5f);
				else {
					var owner = (IEntityView)postman.Owner;
					if (owner.GameObject != null &&
					    PostmanTeleportUtility.IsPointVisibleByPlayer(playerGameObject,
						    owner.GameObject.transform.position)) {
						postman.TimeLeft = Mathf.Max(postman.TimeLeft, 10f);
						continue;
					}
				}

				if (postman.TimeLeft <= 0.0)
					teleports.Add(postman);
			}
		}

		foreach (var teleport in teleports) {
			Spawnpoint spawnpoint;
			if (GetRandomPoint(false, playerGameObject, out spawnpoint, teleport.SpawnpointKind)) {
				teleport.Owner.GetComponent<NavigationComponent>().TeleportTo(playerLocation.Location,
					spawnpoint.transform.position, spawnpoint.transform.rotation);
				teleport.TimeLeft = 30f;
				if (teleport.Spawnpoint != null)
					teleport.Spawnpoint.Locked = false;
				teleport.Spawnpoint = spawnpoint;
				teleport.Spawnpoint.Locked = true;
			}
		}
	}

	private bool GetRandomPoint(
		bool playerIndoor,
		GameObject playerGameObject,
		out Spawnpoint spawnpoint,
		SpawnpointKindEnum kind) {
		if (spawnpoints.Count == 0) {
			spawnpoint = null;
			return false;
		}

		var num = InstanceByRequest<GraphicsGameSettings>.Instance.FieldOfView.Value *
		          GameCamera.Instance.Camera.aspect;
		foreach (var spawnpoint1 in spawnpoints.OrderBy(p =>
			         Vector3.Distance(playerGameObject.transform.position, p.transform.position)))
			if (spawnpoint1.SpawnpointKind == kind && !spawnpoint1.Locked) {
				if (!playerIndoor) {
					var to = (spawnpoint1.transform.position - playerGameObject.transform.position) with {
						y = 0.0f
					};
					if (to.magnitude < 30.0 && Vector3.Angle(playerGameObject.transform.forward, to) < num / 2.0)
						continue;
				}

				spawnpoint = spawnpoint1;
				return true;
			}

		spawnpoint = null;
		return false;
	}

	public class Slot {
		[Inspected] public IEntity Owner;
		[Inspected] public float TimeLeft;
		[Inspected] public SpawnpointKindEnum SpawnpointKind;
		[Inspected] public Spawnpoint Spawnpoint;

		[Inspected(Header = true)] private string Header => Owner != null ? Owner.Name : null;
	}
}