using System;
using Engine.Common.Services;
using Engine.Source.Settings;
using Inspectors;

namespace Engine.Source.Services
{
  [RuntimeService(typeof (BackerUnlocksService), typeof (IBackerUnlocksService))]
  public class BackerUnlocksService : IBackerUnlocksService
  {
    private const string ItemCorrectCode = "somethingold";
    private const string QuestCorrectCode = "herekittykitty";
    private const string PolyhedralRoomCorrectCode = "0";
    private IValue<string> itemCode = new StringValue(nameof (itemCode), string.Empty);
    private IValue<string> questCode = new StringValue(nameof (questCode), string.Empty);
    private IValue<string> polyhedralRoomCode = new StringValue(nameof (polyhedralRoomCode), string.Empty);

    public event Action AnyChangeEvent;

    bool IBackerUnlocksService.ItemUnlocked
    {
      get
      {
        bool itemUnlocked = ItemUnlocked;
        Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (BackerUnlocksService)).Append(" : ").Append("ItemUnlocked").Append(" , value : ").Append(itemUnlocked));
        return itemUnlocked;
      }
    }

    bool IBackerUnlocksService.QuestUnlocked
    {
      get
      {
        bool questUnlocked = QuestUnlocked;
        Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (BackerUnlocksService)).Append(" : ").Append("QuestUnlocked").Append(" , value : ").Append(questUnlocked));
        return questUnlocked;
      }
    }

    bool IBackerUnlocksService.PolyhedralRoomUnlocked
    {
      get
      {
        bool polyhedralRoomUnlocked = PolyhedralRoomUnlocked;
        Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (BackerUnlocksService)).Append(" : ").Append("PolyhedralRoomUnlocked").Append(" , value : ").Append(polyhedralRoomUnlocked));
        return polyhedralRoomUnlocked;
      }
    }

    [Inspected(Mutable = true)]
    public bool ItemUnlocked
    {
      get => itemCode.Value == "somethingold";
      private set
      {
        if (ItemUnlocked == value)
          return;
        itemCode.Value = value ? "somethingold" : string.Empty;
        Action anyChangeEvent = AnyChangeEvent;
        if (anyChangeEvent == null)
          return;
        anyChangeEvent();
      }
    }

    [Inspected(Mutable = true)]
    public bool QuestUnlocked
    {
      get => questCode.Value == "herekittykitty";
      private set
      {
        if (QuestUnlocked == value)
          return;
        questCode.Value = value ? "herekittykitty" : string.Empty;
        Action anyChangeEvent = AnyChangeEvent;
        if (anyChangeEvent == null)
          return;
        anyChangeEvent();
      }
    }

    [Inspected(Mutable = true)]
    public bool PolyhedralRoomUnlocked
    {
      get => polyhedralRoomCode.Value == "0";
      private set
      {
        if (PolyhedralRoomUnlocked == value)
          return;
        polyhedralRoomCode.Value = value ? "0" : string.Empty;
        Action anyChangeEvent = AnyChangeEvent;
        if (anyChangeEvent == null)
          return;
        anyChangeEvent();
      }
    }

    public Result TryAddCode(string code)
    {
      if (code.Equals("somethingold", StringComparison.InvariantCultureIgnoreCase))
      {
        if (ItemUnlocked)
          return Result.AlreadyUnlocked;
        ItemUnlocked = true;
        return Result.Success;
      }
      if (!code.Equals("herekittykitty", StringComparison.InvariantCultureIgnoreCase))
        return Result.Fail;
      if (QuestUnlocked)
        return Result.AlreadyUnlocked;
      QuestUnlocked = true;
      return Result.Success;
    }

    public enum Result
    {
      Fail,
      Success,
      AlreadyUnlocked,
    }
  }
}
