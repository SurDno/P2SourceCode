using System.Collections.Generic;

public class PlagueWebCell {
	public PlagueWebCellId Id;
	public List<PlagueWebPoint> Points = new();
	public PlagueWeb1 PlagueWeb;

	public void AddPoint(PlagueWebPoint point) {
		Points.Add(point);
	}

	public void PlacePoint(PlagueWebPoint point) {
		PlagueWeb.PlacePoint(point);
	}

	public void RemovePoint(PlagueWebPoint point) {
		Points.Remove(point);
		if (Points.Count != 0)
			return;
		PlagueWeb.RemoveCell(this);
	}
}