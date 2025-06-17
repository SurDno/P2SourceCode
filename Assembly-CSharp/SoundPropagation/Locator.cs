using System.Collections.Generic;
using UnityEngine;

namespace SoundPropagation
{
  public class Locator(Pathfinder pathfinder = null) {
    private static Locator main;
    private Pathfinder customPathfinder = pathfinder;
    public List<PathPoint> path = [];
    public float MaxTurnPerDistance = 2f;
    public float OcclusionPerTurn = 0.5f;

    public static Locator Main
    {
      get
      {
        if (main == null)
          main = new Locator();
        return main;
      }
      set => main = value;
    }

    public Pathfinder ActivePathfinder
    {
      get => customPathfinder == null ? Pathfinder.Main : customPathfinder;
      set => customPathfinder = value;
    }

    public List<PathPoint> Path => path;

    private Location LocationFromPath(List<PathPoint> path, Vector3 destDirectionality)
    {
      Location location = new Location {
        Filtering = new Filtering(),
        NearestCorner = path[path.Count - 1].Position,
        PathLength = 0.0f,
        PathFound = true
      };
      PathPoint pathPoint1 = path[0];
      Vector3 rhs = location.NearestCorner - pathPoint1.Position;
      rhs.Normalize();
      for (int index = path.Count - 2; index >= 0; --index)
      {
        PathPoint pathPoint2 = path[index];
        PathPoint pathPoint3 = path[index + 1];
        location.PathLength += pathPoint2.StepLength;
        if (pathPoint2.Cell != null)
          location.Filtering.AddFiltering(pathPoint2.Cell.FilteringPerMeter, pathPoint2.StepLength);
        if (pathPoint2.Portal != null)
          location.Filtering.AddOcclusion(pathPoint2.Portal.Occlusion);
        Vector3 vector3 = pathPoint2.Direction - pathPoint3.Direction;
        location.Filtering.AddOcclusion(vector3.magnitude * OcclusionPerTurn);
        if (index > 0)
        {
          Vector3 lhs = pathPoint2.Position - pathPoint1.Position;
          lhs.Normalize();
          if (Vector3.Dot(lhs, rhs) < 0.999)
          {
            location.NearestCorner = pathPoint2.Position;
            rhs = lhs;
          }
        }
      }
      float occlusion = Math.Normalize(ref destDirectionality) * (Vector3.Distance(destDirectionality, pathPoint1.Direction) * OcclusionPerTurn);
      location.Filtering.AddOcclusion(occlusion);
      return location;
    }

    public Location GetLocation(
      SPCell originCell,
      Vector3 originPosition,
      Vector3 originDirectionality,
      SPCell destCell,
      Vector3 destPosition,
      Vector3 destDirectionality,
      float roughMaxCost,
      bool logPathfinding = false)
    {
      path.Clear();
      Pathfinder activePathfinder = ActivePathfinder;
      activePathfinder.MaxTurnPerDistance = MaxTurnPerDistance;
      activePathfinder.LossPerTurn = OcclusionPerTurn;
      if (activePathfinder.GetReversedPath(originCell, originPosition, originDirectionality, destCell, destPosition, destDirectionality, path, roughMaxCost, logPathfinding))
        return LocationFromPath(path, destDirectionality);
      return new Location { PathFound = false };
    }
  }
}
