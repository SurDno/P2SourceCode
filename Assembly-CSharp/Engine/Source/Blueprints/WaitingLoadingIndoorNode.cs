using System.Collections;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Locations;
using Engine.Common.Services;
using Engine.Source.Blueprints.Utility;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class WaitingLoadingIndoorNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () => StartCoroutine(WaitingLoading(output)));
    }

    private IEnumerator WaitingLoading(FlowOutput output)
    {
      WaitingLoadingUtility.Logs.Clear();
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
      {
        Debug.LogError("player == null");
        output.Call();
      }
      else
      {
        LocationItemComponent locationItem = player.GetComponent<LocationItemComponent>();
        if (locationItem == null)
        {
          Debug.LogError("locationItem == null");
          output.Call();
        }
        else
        {
          ILocationComponent location = locationItem.LogicLocation;
          if (location == null || ((LocationComponent) location).LocationType != LocationType.Indoor)
          {
            output.Call();
          }
          else
          {
            Debug.Log(ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(GetType())).Append(" , location ").GetInfo(location.Owner));
            do
            {
              yield return null;
            }
            while (!CheckLoading(location));
            output.Call();
          }
        }
      }
    }

    private bool CheckLoading(ILocationComponent location)
    {
      if (!location.Owner.IsEnabledInHierarchy)
        return true;
      if (location.IsHibernation)
      {
        if (WaitingLoadingUtility.Logs.Add(location.Owner))
          Debug.Log(ObjectInfoUtility.GetStream().Append("Waiting loading indoor : ").GetInfo(location.Owner));
        return false;
      }
      foreach (ILocationComponent child in ((LocationComponent) location).Childs)
      {
        if (!CheckLoading(child))
          return false;
      }
      return true;
    }
  }
}
