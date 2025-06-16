using System.Collections.Generic;
using UnityEngine;

public class PlagueWeb1 : PlagueWeb {
	public float ViewRadius = 25f;
	public float MaxLinkLength = 3f;
	public float StringsPerPointsPerSecond = 0.5f;
	public float CellSize = 8f;
	public LayerMask CollisionMask;
	[SerializeField] private PlagueWebLink linkPrototype;
	private List<PlagueWebCell> cellPool = new();
	private List<PlagueWebPoint> pointPool = new();
	private Dictionary<PlagueWebCellId, PlagueWebCell> cells = new();
	private List<PlagueWebPoint> searchBuffer = new();
	private List<PlagueWebLink> stringBuffer = new();
	private int activePointsCount;
	private int activeStringsCount;
	private int visiblePointsCount = 1;
	private float phase;

	public override Vector3 CameraPosition { get; set; }

	public override bool IsActive {
		get => enabled;
		set => enabled = value;
	}

	public override IPlagueWebPoint AddPoint(
		Vector3 position,
		Vector3 directionality,
		float strength) {
		PlagueWebPoint point;
		if (pointPool.Count > 0) {
			var index = pointPool.Count - 1;
			point = pointPool[index];
			pointPool.RemoveAt(index);
		} else
			point = new PlagueWebPoint();

		point.Position = position;
		point.Directionality = directionality;
		point.Strength = strength;
		PlacePoint(point);
		return point;
	}

	public void PlacePoint(PlagueWebPoint point) {
		var key = new PlagueWebCellId(point.Position, CellSize);
		var plagueWebCell = point.Cell;
		if (plagueWebCell != null) {
			if (key == plagueWebCell.Id)
				return;
			plagueWebCell.RemovePoint(point);
		}

		if (!cells.TryGetValue(key, out plagueWebCell)) {
			if (cellPool.Count > 0) {
				var index = cellPool.Count - 1;
				plagueWebCell = cellPool[index];
				cellPool.RemoveAt(index);
			} else
				plagueWebCell = new PlagueWebCell();

			plagueWebCell.Id = key;
			plagueWebCell.PlagueWeb = this;
			cells.Add(key, plagueWebCell);
		}

		point.Cell = plagueWebCell;
		plagueWebCell.AddPoint(point);
	}

	public PlagueWebLink AddString(PlagueWebPoint pointA, PlagueWebPoint pointB) {
		PlagueWebLink component;
		if (stringBuffer.Count > activeStringsCount)
			component = stringBuffer[activeStringsCount];
		else {
			component = Instantiate(linkPrototype.gameObject).GetComponent<PlagueWebLink>();
			component.transform.SetParent(transform, false);
			stringBuffer.Add(component);
		}

		component.BeginAnimation(this, pointA, pointB);
		++activeStringsCount;
		return component;
	}

	public bool Raycast(PlagueWebPoint pointA, PlagueWebPoint pointB) {
		var origin = pointA.Position + pointA.Directionality;
		var vector3 = pointB.Position + pointB.Directionality - origin;
		var magnitude = vector3.magnitude;
		var direction = vector3 / magnitude;
		return Physics.Raycast(origin, direction, magnitude, CollisionMask, QueryTriggerInteraction.Ignore);
	}

	public void RemoveCell(PlagueWebCell cell) {
		cells.Remove(cell.Id);
		cell.PlagueWeb = null;
		cellPool.Add(cell);
	}

	public override void RemovePoint(IPlagueWebPoint point) {
		if (!(point is PlagueWebPoint point1))
			return;
		for (var index = 0; index < activeStringsCount; ++index)
			stringBuffer[index].OnPointDisable(point1);
		point1.Cell.RemovePoint(point1);
		point1.Cell = null;
		pointPool.Add(point1);
	}

	public void RemoveLink(PlagueWebLink plagueString) {
		for (var index = 0; index < activeStringsCount; ++index)
			if (stringBuffer[index] == plagueString) {
				stringBuffer[index] = stringBuffer[activeStringsCount - 1];
				stringBuffer[activeStringsCount - 1] = plagueString;
				--activeStringsCount;
				break;
			}
	}

	public void GetPointsInRadius(List<PlagueWebPoint> targetList, Vector3 position, float radius) {
		var num1 = Mathf.FloorToInt((position.x - radius) / CellSize);
		var num2 = Mathf.FloorToInt((position.z - radius) / CellSize);
		var num3 = Mathf.FloorToInt((position.x + radius) / CellSize);
		var num4 = Mathf.FloorToInt((position.z + radius) / CellSize);
		for (var x = num1; x <= num3; ++x) {
			for (var z = num2; z <= num4; ++z) {
				PlagueWebCell plagueWebCell;
				if (cells.TryGetValue(new PlagueWebCellId(x, z), out plagueWebCell))
					for (var index = 0; index < plagueWebCell.Points.Count; ++index) {
						var point = plagueWebCell.Points[index];
						if (point.Strength > 0.0 && Vector3.Distance(position, point.Position) <= (double)radius)
							targetList.Add(point);
					}
			}
		}
	}

	private void Update() {
		phase += Time.deltaTime * visiblePointsCount * StringsPerPointsPerSecond;
		if (phase < 1.0)
			return;
		phase = 0.0f;
		GetPointsInRadius(searchBuffer, CameraPosition, ViewRadius);
		visiblePointsCount = searchBuffer.Count > 0 ? searchBuffer.Count : 1;
		if (searchBuffer.Count > 1) {
			var plagueWebPoint1 = searchBuffer[Random.Range(0, searchBuffer.Count)];
			searchBuffer.Clear();
			GetPointsInRadius(searchBuffer, plagueWebPoint1.Position, MaxLinkLength);
			var index = Random.Range(0, searchBuffer.Count);
			if (searchBuffer[index] == plagueWebPoint1) {
				++index;
				if (index == searchBuffer.Count)
					index = 0;
			}

			var plagueWebPoint2 = searchBuffer[index];
			if (!Raycast(plagueWebPoint1, plagueWebPoint2)) {
				if (plagueWebPoint1.Strength >= (double)plagueWebPoint2.Strength)
					AddString(plagueWebPoint1, plagueWebPoint2);
				else
					AddString(plagueWebPoint2, plagueWebPoint1);
			}
		}

		searchBuffer.Clear();
	}
}