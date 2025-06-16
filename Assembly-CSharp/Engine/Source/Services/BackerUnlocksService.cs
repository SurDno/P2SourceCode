using Engine.Common.Services;
using Engine.Source.Settings;
using Inspectors;
using System;
using UnityEngine;

namespace Engine.Source.Services
{
  [RuntimeService(new System.Type[] {typeof (BackerUnlocksService), typeof (IBackerUnlocksService)})]
  public class BackerUnlocksService : IBackerUnlocksService
  {
    private const string ItemCorrectCode = "somethingold";
    private const string QuestCorrectCode = "herekittykitty";
    private const string PolyhedralRoomCorrectCode = "0";
    private IValue<string> itemCode = (IValue<string>) new StringValue(nameof (itemCode), string.Empty);
    private IValue<string> questCode = (IValue<string>) new StringValue(nameof (questCode), string.Empty);
    private IValue<string> polyhedralRoomCode = (IValue<string>) new StringValue(nameof (polyhedralRoomCode), string.Empty);

    public event Action AnyChangeEvent;

    bool IBackerUnlocksService.ItemUnlocked
    {
      get
      {
        bool itemUnlocked = this.ItemUnlocked;
        Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (BackerUnlocksService)).Append(" : ").Append("ItemUnlocked").Append(" , value : ").Append(itemUnlocked));
        return itemUnlocked;
      }
    }

    bool IBackerUnlocksService.QuestUnlocked
    {
      get
      {
        bool questUnlocked = this.QuestUnlocked;
        Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (BackerUnlocksService)).Append(" : ").Append("QuestUnlocked").Append(" , value : ").Append(questUnlocked));
        return questUnlocked;
      }
    }

    bool IBackerUnlocksService.PolyhedralRoomUnlocked
    {
      get
      {
        bool polyhedralRoomUnlocked = this.PolyhedralRoomUnlocked;
        Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (BackerUnlocksService)).Append(" : ").Append("PolyhedralRoomUnlocked").Append(" , value : ").Append(polyhedralRoomUnlocked));
        return polyhedralRoomUnlocked;
      }
    }

    [Inspected(Mutable = true)]
    public bool ItemUnlocked
    {
      get => this.itemCode.Value == "somethingold";
      private set
      {
        if (this.ItemUnlocked == value)
          return;
        this.itemCode.Value = value ? "somethingold" : string.Empty;
        Action anyChangeEvent = this.AnyChangeEvent;
        if (anyChangeEvent == null)
          return;
        anyChangeEvent();
      }
    }

    [Inspected(Mutable = true)]
    public bool QuestUnlocked
    {
      get => this.questCode.Value == "herekittykitty";
      private set
      {
        if (this.QuestUnlocked == value)
          return;
        this.questCode.Value = value ? "herekittykitty" : string.Empty;
        Action anyChangeEvent = this.AnyChangeEvent;
        if (anyChangeEvent == null)
          return;
        anyChangeEvent();
      }
    }

    [Inspected(Mutable = true)]
    public bool PolyhedralRoomUnlocked
    {
      get => this.polyhedralRoomCode.Value == "0";
      private set
      {
        if (this.PolyhedralRoomUnlocked == value)
          return;
        this.polyhedralRoomCode.Value = value ? "0" : string.Empty;
        Action anyChangeEvent = this.AnyChangeEvent;
        if (anyChangeEvent == null)
          return;
        anyChangeEvent();
      }
    }

    public BackerUnlocksService.Result TryAddCode(string code)
    {
      if (code.Equals("somethingold", StringComparison.InvariantCultureIgnoreCase))
      {
        if (this.ItemUnlocked)
          return BackerUnlocksService.Result.AlreadyUnlocked;
        this.ItemUnlocked = true;
        return BackerUnlocksService.Result.Success;
      }
      if (!code.Equals("herekittykitty", StringComparison.InvariantCultureIgnoreCase))
        return BackerUnlocksService.Result.Fail;
      if (this.QuestUnlocked)
        return BackerUnlocksService.Result.AlreadyUnlocked;
      this.QuestUnlocked = true;
      return BackerUnlocksService.Result.Success;
    }

    public enum Result
    {
      Fail,
      Success,
      AlreadyUnlocked,
    }
  }
}
