using System;
using System.Collections;
using System.Collections.Generic;
using AssetDatabases;
using Cofe.Meta;
using Engine.Common;
using Engine.Common.Components.Locations;
using Engine.Common.Components.Movable;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Regions;
using Engine.Source.Components.Utilities;
using UnityEngine;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindTourConsoleCommands
  {
    private static YieldInstruction wait = new WaitForSeconds(1f);

    [ConsoleCommand("tour_regions")]
    private static string TourRegionsCommand(string command, ConsoleParameter[] parameters)
    {
      CoroutineService.Instance.Route(TourRegions());
      return command;
    }

    [ConsoleCommand("tour_indoors")]
    private static string TourIndoorsCommand(string command, ConsoleParameter[] parameters)
    {
      CoroutineService.Instance.Route(TourIndoors());
      return command;
    }

    [ConsoleCommand("tour_all")]
    private static string TourAllCommand(string command, ConsoleParameter[] parameters)
    {
      CoroutineService.Instance.Route(TourAll());
      return command;
    }

    private static IEnumerator TourAll()
    {
      yield return TourRegions();
      yield return TourIndoors();
    }

    private static IEnumerator TourRegions()
    {
      Simulation simulation = ServiceLocator.GetService<Simulation>();
      IEntity player = simulation.Player;
      if (player == null)
      {
        Debug.LogError("Player not found");
      }
      else
      {
        List<RegionComponent> regions = [];
        Array regionValues = Enum.GetValues(typeof (RegionEnum));
        foreach (RegionEnum regionValue in regionValues)
        {
          RegionComponent region = RegionUtility.GetRegionByName(regionValue);
          if (region != null && !region.IsDisposed)
            regions.Add(region);
          region = null;
        }
        for (int index = 0; index < regions.Count; ++index)
        {
          RegionComponent region = regions[index];
          Debug.LogError("Region : " + region.Owner.Name + "   ( " + index + " / " + regions.Count + " )");
          yield return ComputeRegion(region, player);
          region = null;
        }
        Debug.LogError("Regions Complete");
      }
    }

    private static IEnumerator ComputeRegion(RegionComponent region, IEntity player)
    {
      Vector3 position = ((IEntityView) region.Owner).Position;
      position.y = Terrain.activeTerrain.SampleHeight(position);
      NavMeshUtility.SamplePosition(ref position, AreaEnum.All.ToMask());
      NavigationComponent navigation = player.GetComponent<NavigationComponent>();
      LocationComponent location = region.GetComponent<LocationComponent>();
      navigation.TeleportTo(location, position, Quaternion.identity);
      while (navigation.WaitTeleport)
        yield return wait;
      LocationItemComponent locationItem = player.GetComponent<LocationItemComponent>();
      while (locationItem.IsHibernation)
        yield return wait;
      while (!SceneController.CanLoad)
        yield return wait;
      yield return wait;
    }

    private static IEnumerator TourIndoors()
    {
      Simulation simulation = ServiceLocator.GetService<Simulation>();
      IEntity player = simulation.Player;
      if (player == null)
      {
        Debug.LogError("Player not found");
      }
      else
      {
        List<LocationComponent> indoors = [];
        LocationComponent location = simulation.Hierarchy.GetComponent<LocationComponent>();
        ComputeLocation(location, indoors);
        for (int index = 0; index < indoors.Count; ++index)
        {
          LocationComponent indoor = indoors[index];
          string name = indoor.Owner.Name;
          StaticModelComponent model = indoor.GetComponent<StaticModelComponent>();
          if (model != null)
          {
            IScene connection = model.Connection.Value;
            if (connection != null)
              name = connection.Name;
            connection = null;
          }
          Debug.LogError("Indoor : " + name + "   ( " + index + " / " + indoors.Count + " )");
          yield return ComputeIndoor(indoor, player);
          indoor = null;
          name = null;
          model = null;
        }
        Debug.LogError("Indoors Complete");
      }
    }

    private static void ComputeLocation(LocationComponent location, List<LocationComponent> indoors)
    {
      if (location.LocationType == LocationType.Indoor)
      {
        indoors.Add(location);
      }
      else
      {
        foreach (LocationComponent child in location.Childs)
          ComputeLocation(child, indoors);
      }
    }

    private static IEnumerator ComputeIndoor(LocationComponent indoor, IEntity player)
    {
      Vector3 position = ((IEntityView) indoor.Owner).Position;
      PositionComponent positionComponent = LocationItemUtility.FindParentComponent<PositionComponent>(indoor.Owner);
      if (positionComponent != null)
        position = positionComponent.Position;
      NavigationComponent navigation = player.GetComponent<NavigationComponent>();
      navigation.TeleportTo(indoor, position, Quaternion.identity);
      while (navigation.WaitTeleport)
        yield return wait;
      LocationItemComponent locationItem = player.GetComponent<LocationItemComponent>();
      while (locationItem.IsHibernation)
        yield return wait;
      while (!SceneController.CanLoad)
        yield return wait;
      yield return wait;
    }
  }
}
