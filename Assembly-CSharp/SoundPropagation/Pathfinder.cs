using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SoundPropagation
{
  public class Pathfinder
  {
    private static Pathfinder main;
    public float MaxTurnPerDistance = 2f;
    public float LossPerTurn = 0.5f;
    private List<Node> pool;
    private int poolUsage;
    private Dictionary<SPPortal, int> dictionary;
    private int destinationIndex;
    private List<int> frontier;
    private StringBuilder logBuilder;
    private Vector3 destPosition;
    private Vector3 destDirectionality;
    private float maxDistance;
    private bool log;

    public static Pathfinder Main
    {
      get
      {
        if (main == null)
          main = new Pathfinder();
        return main;
      }
      set => main = value;
    }

    private Node Dequeue(out int indexInPool)
    {
      int index = frontier.Count - 1;
      indexInPool = frontier[index];
      Node node = pool[indexInPool];
      frontier.RemoveAt(index);
      node.IndexInFrontier = -1;
      if (log)
      {
        logBuilder.Append("- Dequeue: " + node.Estimation);
        logBuilder.Append(", " + node.Cell);
        logBuilder.Append(", " + node.Portal);
        logBuilder.Append(", " + node.Distance);
        logBuilder.Append(", " + node.Loss);
        logBuilder.AppendLine();
      }
      return node;
    }

    private void Enqueue(
      SPPortal portal,
      SPCell cell,
      Vector3 position,
      Vector3 direction,
      float distance,
      float loss,
      float estimation,
      int previousNodeIndex)
    {
      if (distance > (double) maxDistance)
        return;
      int index;
      if (portal == null)
        index = destinationIndex;
      else if (!dictionary.TryGetValue(portal, out index))
        index = -1;
      Node freeNode;
      if (index != -1)
      {
        freeNode = pool[index];
        if (freeNode.IndexInFrontier < 0 || freeNode.Estimation <= (double) estimation)
          return;
      }
      else
      {
        int poolUsage = this.poolUsage;
        freeNode = GetFreeNode();
        if (portal == null)
          destinationIndex = poolUsage;
        else
          dictionary.Add(portal, poolUsage);
        freeNode.IndexInFrontier = frontier.Count;
        frontier.Add(poolUsage);
      }
      freeNode.Portal = portal;
      freeNode.Cell = cell;
      freeNode.Position = position;
      freeNode.Direction = direction;
      freeNode.Distance = distance;
      freeNode.Loss = loss;
      freeNode.Estimation = estimation;
      freeNode.PreviousNodeIndex = previousNodeIndex;
      if (log)
      {
        logBuilder.Append("+ Enqueue: " + freeNode.Estimation);
        logBuilder.Append(", " + freeNode.Cell);
        logBuilder.Append(", " + freeNode.Portal);
        logBuilder.Append(", " + freeNode.Distance);
        logBuilder.Append(", " + freeNode.Loss);
        logBuilder.AppendLine();
      }
      SortInFrontier(freeNode.IndexInFrontier);
    }

    private void Enqueue(
      int prevNodeIndex,
      Vector3 prevPosition,
      Vector3 prevDirection,
      float prevDistance,
      float prevLoss,
      SPCell cell,
      SPPortal portal)
    {
      Vector3 output;
      if (portal == null)
        output = destPosition;
      else if (!portal.ClosestPointToSegment(prevPosition, destPosition, out output))
        return;
      Vector3 vector1 = output - prevPosition;
      float num1 = Math.Normalize(ref vector1);
      if (prevNodeIndex == -1)
        prevDirection = Math.DirectionalityToDirection(prevDirection, vector1);
      vector1 = Vector3.MoveTowards(prevDirection, vector1, MaxTurnPerDistance * num1);
      float num2 = Vector3.Distance(prevDirection, vector1) * LossPerTurn;
      if (cell != null && cell.Profile != null)
        num2 += cell.LossPerMeter * num1;
      if (portal != null)
        num2 += portal.Loss;
      float loss = num2 + prevLoss;
      float distance = num1 + prevDistance;
      Vector3 vector2 = destPosition - output;
      float num3 = Math.Normalize(ref vector2);
      Vector3 vector3 = Vector3.MoveTowards(vector1, vector2, num3 * MaxTurnPerDistance);
      float num4 = Vector3.Distance(vector1, vector3) * LossPerTurn;
      Vector3 direction = Math.DirectionalityToDirection(destDirectionality, vector3);
      float num5 = num4 + Vector3.Distance(vector3, direction) * LossPerTurn;
      float estimation = (num3 + distance) * Mathf.Pow(2f, num5 + loss);
      Enqueue(portal, cell, output, vector1, distance, loss, estimation, prevNodeIndex);
    }

    private void FillPath(
      SPCell originCell,
      Vector3 originPosition,
      Vector3 originDirectionality,
      Node node,
      List<PathPoint> output)
    {
      PathPoint pathPoint1;
      while (true)
      {
        Node node1 = node.PreviousNodeIndex != -1 ? pool[node.PreviousNodeIndex] : null;
        if (output.Count > 0)
        {
          Vector3 pointB = node1 != null ? node1.Position : originPosition;
          Vector3 output1;
          if (node.Portal.ClosestPointToSegment(output[output.Count - 1].Position, pointB, out output1))
            node.Position = output1;
        }
        List<PathPoint> pathPointList = output;
        pathPoint1 = new PathPoint();
        pathPoint1.Cell = node.Cell;
        pathPoint1.Portal = node.Portal;
        pathPoint1.Position = node.Position;
        PathPoint pathPoint2 = pathPoint1;
        pathPointList.Add(pathPoint2);
        if (node1 != null)
          node = node1;
        else
          break;
      }
      pathPoint1 = new PathPoint();
      pathPoint1.Cell = originCell;
      pathPoint1.Portal = null;
      pathPoint1.Position = originPosition;
      pathPoint1.Direction = (output[output.Count - 1].Position - originPosition).normalized;
      pathPoint1.StepLength = 0.0f;
      PathPoint pathPoint3 = pathPoint1;
      pathPoint3.Direction = Math.DirectionalityToDirection(originDirectionality, pathPoint3.Direction);
      output.Add(pathPoint3);
      for (int index = output.Count - 2; index >= 0; --index)
      {
        PathPoint pathPoint4 = output[index];
        pathPoint4.Direction = pathPoint4.Position - pathPoint3.Position;
        pathPoint4.StepLength = Math.Normalize(ref pathPoint4.Direction);
        pathPoint4.Direction = Vector3.MoveTowards(pathPoint3.Direction, pathPoint4.Direction, pathPoint4.StepLength * MaxTurnPerDistance);
        output[index] = pathPoint4;
        pathPoint3 = pathPoint4;
      }
    }

    public bool GetReversedPath(
      SPCell originCell,
      Vector3 originPosition,
      Vector3 originDirectionality,
      SPCell destCell,
      Vector3 destPosition,
      Vector3 destDirectionality,
      List<PathPoint> output,
      float maxDistance,
      bool log = false)
    {
      output.Clear();
      this.destPosition = destPosition;
      this.destDirectionality = destDirectionality;
      this.maxDistance = maxDistance;
      this.log = log;
      if (log)
      {
        if (logBuilder == null)
          logBuilder = new StringBuilder();
        else
          logBuilder.Remove(0, logBuilder.Length);
      }
      if (originCell == destCell)
        Enqueue(-1, originPosition, originDirectionality, 0.0f, 0.0f, originCell, null);
      if (originCell != null)
      {
        foreach (SPPortal portal in originCell.Portals)
          Enqueue(-1, originPosition, originDirectionality, 0.0f, 0.0f, originCell, portal);
      }
      else if (destCell != null && destCell.Group != null)
      {
        foreach (SPPortal outerPortal in destCell.Group.OuterPortals)
          Enqueue(-1, originPosition, originDirectionality, 0.0f, 0.0f, originCell, outerPortal);
      }
      while (frontier.Count > 0)
      {
        int indexInPool;
        Node node = Dequeue(out indexInPool);
        if (node.Portal == null)
        {
          FillPath(originCell, originPosition, originDirectionality, node, output);
          Reset();
          return true;
        }
        SPCell nextCell = GetNextCell(node.Cell, node.Portal);
        if (nextCell == destCell)
          Enqueue(indexInPool, node.Position, node.Direction, node.Distance, node.Loss, nextCell, null);
        if (nextCell != null)
        {
          SPPortal[] portals = nextCell.Portals;
          for (int index = 0; index < portals.Length; ++index)
          {
            if (portals[index] != node.Portal)
              Enqueue(indexInPool, node.Position, node.Direction, node.Distance, node.Loss, nextCell, portals[index]);
          }
        }
        else if (destCell != null && destCell.Group != null)
        {
          SPPortal[] outerPortals = destCell.Group.OuterPortals;
          for (int index = 0; index < outerPortals.Length; ++index)
          {
            if (outerPortals[index] != node.Portal)
              Enqueue(indexInPool, node.Position, node.Direction, node.Distance, node.Loss, nextCell, outerPortals[index]);
          }
        }
      }
      Reset();
      return false;
    }

    private SPCell GetNextCell(SPCell cell, SPPortal portal)
    {
      return portal.CellA == cell ? portal.CellB : portal.CellA;
    }

    private Node GetFreeNode()
    {
      Node freeNode;
      if (pool.Count > poolUsage)
      {
        freeNode = pool[poolUsage];
      }
      else
      {
        freeNode = new Node();
        pool.Add(freeNode);
      }
      ++poolUsage;
      return freeNode;
    }

    public string Log => logBuilder != null ? logBuilder.ToString() : string.Empty;

    private void Reset()
    {
      dictionary.Clear();
      for (int index = 0; index < poolUsage; ++index)
        pool[index].ClearReferences();
      frontier.Clear();
      poolUsage = 0;
      destinationIndex = -1;
    }

    private void SortInFrontier(int frontierIndex)
    {
      for (; frontierIndex > 0 && pool[frontier[frontierIndex]].Estimation > (double) pool[frontier[frontierIndex - 1]].Estimation; --frontierIndex)
        SwapInFrontier(frontierIndex, frontierIndex - 1);
      for (; frontierIndex < frontier.Count - 1 && pool[frontier[frontierIndex]].Estimation < (double) pool[frontier[frontierIndex + 1]].Estimation; ++frontierIndex)
        SwapInFrontier(frontierIndex, frontierIndex + 1);
    }

    public Pathfinder()
    {
      pool = new List<Node>();
      dictionary = new Dictionary<SPPortal, int>();
      frontier = new List<int>();
      poolUsage = 0;
      destinationIndex = -1;
    }

    private void SwapInFrontier(int indexA, int indexB)
    {
      int num = frontier[indexA];
      frontier[indexA] = frontier[indexB];
      frontier[indexB] = num;
      pool[frontier[indexA]].IndexInFrontier = indexA;
      pool[frontier[indexB]].IndexInFrontier = indexB;
    }

    private class Node
    {
      public SPPortal Portal;
      public SPCell Cell;
      public Vector3 Position;
      public Vector3 Direction;
      public float Distance;
      public float Loss;
      public float Estimation;
      public int IndexInFrontier;
      public int PreviousNodeIndex;

      public void ClearReferences()
      {
        Portal = null;
        Cell = null;
      }
    }
  }
}
