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
    private List<Pathfinder.Node> pool;
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
        if (Pathfinder.main == null)
          Pathfinder.main = new Pathfinder();
        return Pathfinder.main;
      }
      set => Pathfinder.main = value;
    }

    private Pathfinder.Node Dequeue(out int indexInPool)
    {
      int index = this.frontier.Count - 1;
      indexInPool = this.frontier[index];
      Pathfinder.Node node = this.pool[indexInPool];
      this.frontier.RemoveAt(index);
      node.IndexInFrontier = -1;
      if (this.log)
      {
        this.logBuilder.Append("- Dequeue: " + (object) node.Estimation);
        this.logBuilder.Append(", " + (object) node.Cell);
        this.logBuilder.Append(", " + (object) node.Portal);
        this.logBuilder.Append(", " + (object) node.Distance);
        this.logBuilder.Append(", " + (object) node.Loss);
        this.logBuilder.AppendLine();
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
      if ((double) distance > (double) this.maxDistance)
        return;
      int index;
      if ((Object) portal == (Object) null)
        index = this.destinationIndex;
      else if (!this.dictionary.TryGetValue(portal, out index))
        index = -1;
      Pathfinder.Node freeNode;
      if (index != -1)
      {
        freeNode = this.pool[index];
        if (freeNode.IndexInFrontier < 0 || (double) freeNode.Estimation <= (double) estimation)
          return;
      }
      else
      {
        int poolUsage = this.poolUsage;
        freeNode = this.GetFreeNode();
        if ((Object) portal == (Object) null)
          this.destinationIndex = poolUsage;
        else
          this.dictionary.Add(portal, poolUsage);
        freeNode.IndexInFrontier = this.frontier.Count;
        this.frontier.Add(poolUsage);
      }
      freeNode.Portal = portal;
      freeNode.Cell = cell;
      freeNode.Position = position;
      freeNode.Direction = direction;
      freeNode.Distance = distance;
      freeNode.Loss = loss;
      freeNode.Estimation = estimation;
      freeNode.PreviousNodeIndex = previousNodeIndex;
      if (this.log)
      {
        this.logBuilder.Append("+ Enqueue: " + (object) freeNode.Estimation);
        this.logBuilder.Append(", " + (object) freeNode.Cell);
        this.logBuilder.Append(", " + (object) freeNode.Portal);
        this.logBuilder.Append(", " + (object) freeNode.Distance);
        this.logBuilder.Append(", " + (object) freeNode.Loss);
        this.logBuilder.AppendLine();
      }
      this.SortInFrontier(freeNode.IndexInFrontier);
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
      if ((Object) portal == (Object) null)
        output = this.destPosition;
      else if (!portal.ClosestPointToSegment(prevPosition, this.destPosition, out output))
        return;
      Vector3 vector1 = output - prevPosition;
      float num1 = Math.Normalize(ref vector1);
      if (prevNodeIndex == -1)
        prevDirection = Math.DirectionalityToDirection(prevDirection, vector1);
      vector1 = Vector3.MoveTowards(prevDirection, vector1, this.MaxTurnPerDistance * num1);
      float num2 = Vector3.Distance(prevDirection, vector1) * this.LossPerTurn;
      if ((Object) cell != (Object) null && (Object) cell.Profile != (Object) null)
        num2 += cell.LossPerMeter * num1;
      if ((Object) portal != (Object) null)
        num2 += portal.Loss;
      float loss = num2 + prevLoss;
      float distance = num1 + prevDistance;
      Vector3 vector2 = this.destPosition - output;
      float num3 = Math.Normalize(ref vector2);
      Vector3 vector3 = Vector3.MoveTowards(vector1, vector2, num3 * this.MaxTurnPerDistance);
      float num4 = Vector3.Distance(vector1, vector3) * this.LossPerTurn;
      Vector3 direction = Math.DirectionalityToDirection(this.destDirectionality, vector3);
      float num5 = num4 + Vector3.Distance(vector3, direction) * this.LossPerTurn;
      float estimation = (num3 + distance) * Mathf.Pow(2f, num5 + loss);
      this.Enqueue(portal, cell, output, vector1, distance, loss, estimation, prevNodeIndex);
    }

    private void FillPath(
      SPCell originCell,
      Vector3 originPosition,
      Vector3 originDirectionality,
      Pathfinder.Node node,
      List<PathPoint> output)
    {
      PathPoint pathPoint1;
      while (true)
      {
        Pathfinder.Node node1 = node.PreviousNodeIndex != -1 ? this.pool[node.PreviousNodeIndex] : (Pathfinder.Node) null;
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
      pathPoint1.Portal = (SPPortal) null;
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
        pathPoint4.Direction = Vector3.MoveTowards(pathPoint3.Direction, pathPoint4.Direction, pathPoint4.StepLength * this.MaxTurnPerDistance);
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
        if (this.logBuilder == null)
          this.logBuilder = new StringBuilder();
        else
          this.logBuilder.Remove(0, this.logBuilder.Length);
      }
      if ((Object) originCell == (Object) destCell)
        this.Enqueue(-1, originPosition, originDirectionality, 0.0f, 0.0f, originCell, (SPPortal) null);
      if ((Object) originCell != (Object) null)
      {
        foreach (SPPortal portal in originCell.Portals)
          this.Enqueue(-1, originPosition, originDirectionality, 0.0f, 0.0f, originCell, portal);
      }
      else if ((Object) destCell != (Object) null && (Object) destCell.Group != (Object) null)
      {
        foreach (SPPortal outerPortal in destCell.Group.OuterPortals)
          this.Enqueue(-1, originPosition, originDirectionality, 0.0f, 0.0f, originCell, outerPortal);
      }
      while (this.frontier.Count > 0)
      {
        int indexInPool;
        Pathfinder.Node node = this.Dequeue(out indexInPool);
        if ((Object) node.Portal == (Object) null)
        {
          this.FillPath(originCell, originPosition, originDirectionality, node, output);
          this.Reset();
          return true;
        }
        SPCell nextCell = this.GetNextCell(node.Cell, node.Portal);
        if ((Object) nextCell == (Object) destCell)
          this.Enqueue(indexInPool, node.Position, node.Direction, node.Distance, node.Loss, nextCell, (SPPortal) null);
        if ((Object) nextCell != (Object) null)
        {
          SPPortal[] portals = nextCell.Portals;
          for (int index = 0; index < portals.Length; ++index)
          {
            if ((Object) portals[index] != (Object) node.Portal)
              this.Enqueue(indexInPool, node.Position, node.Direction, node.Distance, node.Loss, nextCell, portals[index]);
          }
        }
        else if ((Object) destCell != (Object) null && (Object) destCell.Group != (Object) null)
        {
          SPPortal[] outerPortals = destCell.Group.OuterPortals;
          for (int index = 0; index < outerPortals.Length; ++index)
          {
            if ((Object) outerPortals[index] != (Object) node.Portal)
              this.Enqueue(indexInPool, node.Position, node.Direction, node.Distance, node.Loss, nextCell, outerPortals[index]);
          }
        }
      }
      this.Reset();
      return false;
    }

    private SPCell GetNextCell(SPCell cell, SPPortal portal)
    {
      return (Object) portal.CellA == (Object) cell ? portal.CellB : portal.CellA;
    }

    private Pathfinder.Node GetFreeNode()
    {
      Pathfinder.Node freeNode;
      if (this.pool.Count > this.poolUsage)
      {
        freeNode = this.pool[this.poolUsage];
      }
      else
      {
        freeNode = new Pathfinder.Node();
        this.pool.Add(freeNode);
      }
      ++this.poolUsage;
      return freeNode;
    }

    public string Log => this.logBuilder != null ? this.logBuilder.ToString() : string.Empty;

    private void Reset()
    {
      this.dictionary.Clear();
      for (int index = 0; index < this.poolUsage; ++index)
        this.pool[index].ClearReferences();
      this.frontier.Clear();
      this.poolUsage = 0;
      this.destinationIndex = -1;
    }

    private void SortInFrontier(int frontierIndex)
    {
      for (; frontierIndex > 0 && (double) this.pool[this.frontier[frontierIndex]].Estimation > (double) this.pool[this.frontier[frontierIndex - 1]].Estimation; --frontierIndex)
        this.SwapInFrontier(frontierIndex, frontierIndex - 1);
      for (; frontierIndex < this.frontier.Count - 1 && (double) this.pool[this.frontier[frontierIndex]].Estimation < (double) this.pool[this.frontier[frontierIndex + 1]].Estimation; ++frontierIndex)
        this.SwapInFrontier(frontierIndex, frontierIndex + 1);
    }

    public Pathfinder()
    {
      this.pool = new List<Pathfinder.Node>();
      this.dictionary = new Dictionary<SPPortal, int>();
      this.frontier = new List<int>();
      this.poolUsage = 0;
      this.destinationIndex = -1;
    }

    private void SwapInFrontier(int indexA, int indexB)
    {
      int num = this.frontier[indexA];
      this.frontier[indexA] = this.frontier[indexB];
      this.frontier[indexB] = num;
      this.pool[this.frontier[indexA]].IndexInFrontier = indexA;
      this.pool[this.frontier[indexB]].IndexInFrontier = indexB;
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
        this.Portal = (SPPortal) null;
        this.Cell = (SPCell) null;
      }
    }
  }
}
