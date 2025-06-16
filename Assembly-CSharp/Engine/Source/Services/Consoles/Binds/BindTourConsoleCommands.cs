// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.Binds.BindTourConsoleCommands
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using AssetDatabases;
using Cofe.Meta;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Locations;
using Engine.Common.Components.Movable;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Impl.Services.Simulations;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Regions;
using Engine.Source.Components.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindTourConsoleCommands
  {
    private static YieldInstruction wait = (YieldInstruction) new WaitForSeconds(1f);

    [ConsoleCommand("tour_regions")]
    private static string TourRegionsCommand(string command, ConsoleParameter[] parameters)
    {
      CoroutineService.Instance.Route(BindTourConsoleCommands.TourRegions());
      return command;
    }

    [ConsoleCommand("tour_indoors")]
    private static string TourIndoorsCommand(string command, ConsoleParameter[] parameters)
    {
      CoroutineService.Instance.Route(BindTourConsoleCommands.TourIndoors());
      return command;
    }

    [ConsoleCommand("tour_all")]
    private static string TourAllCommand(string command, ConsoleParameter[] parameters)
    {
      CoroutineService.Instance.Route(BindTourConsoleCommands.TourAll());
      return command;
    }

    private static IEnumerator TourAll()
    {
      yield return (object) BindTourConsoleCommands.TourRegions();
      yield return (object) BindTourConsoleCommands.TourIndoors();
    }

    private static IEnumerator TourRegions()
    {
      Simulation simulation = ServiceLocator.GetService<Simulation>();
      IEntity player = simulation.Player;
      if (player == null)
      {
        Debug.LogError((object) "Player not found");
      }
      else
      {
        List<RegionComponent> regions = new List<RegionComponent>();
        Array regionValues = Enum.GetValues(typeof (RegionEnum));
        foreach (RegionEnum regionValue in regionValues)
        {
          RegionComponent region = RegionUtility.GetRegionByName(regionValue);
          if (region != null && !region.IsDisposed)
            regions.Add(region);
          region = (RegionComponent) null;
        }
        for (int index = 0; index < regions.Count; ++index)
        {
          RegionComponent region = regions[index];
          Debug.LogError((object) ("Region : " + region.Owner.Name + "   ( " + (object) index + " / " + (object) regions.Count + " )"));
          yield return (object) BindTourConsoleCommands.ComputeRegion(region, player);
          region = (RegionComponent) null;
        }
        Debug.LogError((object) "Regions Complete");
      }
    }

    private static IEnumerator ComputeRegion(RegionComponent region, IEntity player)
    {
      Vector3 position = ((IEntityView) region.Owner).Position;
      position.y = Terrain.activeTerrain.SampleHeight(position);
      NavMeshUtility.SamplePosition(ref position, AreaEnum.All.ToMask());
      NavigationComponent navigation = player.GetComponent<NavigationComponent>();
      LocationComponent location = region.GetComponent<LocationComponent>();
      navigation.TeleportTo((ILocationComponent) location, position, Quaternion.identity);
      while (navigation.WaitTeleport)
        yield return (object) BindTourConsoleCommands.wait;
      LocationItemComponent locationItem = player.GetComponent<LocationItemComponent>();
      while (locationItem.IsHibernation)
        yield return (object) BindTourConsoleCommands.wait;
      while (!SceneController.CanLoad)
        yield return (object) BindTourConsoleCommands.wait;
      yield return (object) BindTourConsoleCommands.wait;
    }

    private static IEnumerator TourIndoors()
    {
      Simulation simulation = ServiceLocator.GetService<Simulation>();
      IEntity player = simulation.Player;
      if (player == null)
      {
        Debug.LogError((object) "Player not found");
      }
      else
      {
        List<LocationComponent> indoors = new List<LocationComponent>();
        LocationComponent location = simulation.Hierarchy.GetComponent<LocationComponent>();
        BindTourConsoleCommands.ComputeLocation(location, indoors);
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
            connection = (IScene) null;
          }
          Debug.LogError((object) ("Indoor : " + name + "   ( " + (object) index + " / " + (object) indoors.Count + " )"));
          yield return (object) BindTourConsoleCommands.ComputeIndoor(indoor, player);
          indoor = (LocationComponent) null;
          name = (string) null;
          model = (StaticModelComponent) null;
        }
        Debug.LogError((object) "Indoors Complete");
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
          BindTourConsoleCommands.ComputeLocation(child, indoors);
      }
    }

    private static IEnumerator ComputeIndoor(LocationComponent indoor, IEntity player)
    {
      Vector3 position = ((IEntityView) indoor.Owner).Position;
      PositionComponent positionComponent = LocationItemUtility.FindParentComponent<PositionComponent>(indoor.Owner);
      if (positionComponent != null)
        position = positionComponent.Position;
      NavigationComponent navigation = player.GetComponent<NavigationComponent>();
      navigation.TeleportTo((ILocationComponent) indoor, position, Quaternion.identity);
      while (navigation.WaitTeleport)
        yield return (object) BindTourConsoleCommands.wait;
      LocationItemComponent locationItem = player.GetComponent<LocationItemComponent>();
      while (locationItem.IsHibernation)
        yield return (object) BindTourConsoleCommands.wait;
      while (!SceneController.CanLoad)
        yield return (object) BindTourConsoleCommands.wait;
      yield return (object) BindTourConsoleCommands.wait;
    }
  }
}
