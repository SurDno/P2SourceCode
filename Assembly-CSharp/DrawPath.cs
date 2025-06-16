using Engine.Source.Components.Utilities;
using UnityEngine;
using UnityEngine.AI;

public class DrawPath : MonoBehaviour
{
  private void Update() => NavMeshUtility.DrawPath(this.gameObject.GetComponent<NavMeshAgent>());
}
