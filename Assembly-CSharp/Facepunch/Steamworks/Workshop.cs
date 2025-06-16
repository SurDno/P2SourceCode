using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SteamNative;
using Result = Facepunch.Steamworks.Callbacks.Result;

namespace Facepunch.Steamworks
{
  public class Workshop : IDisposable
  {
    internal const ulong InvalidHandle = 18446744073709551615;
    internal SteamUGC ugc;
    internal Friends friends;
    internal BaseSteamworks steamworks;
    internal SteamRemoteStorage remoteStorage;

    public event Action<ulong, Result> OnFileDownloaded;

    public event Action<ulong> OnItemInstalled;

    internal Workshop(BaseSteamworks steamworks, SteamUGC ugc, SteamRemoteStorage remoteStorage)
    {
      this.ugc = ugc;
      this.steamworks = steamworks;
      this.remoteStorage = remoteStorage;
      DownloadItemResult_t.RegisterCallback(steamworks, onDownloadResult);
      ItemInstalled_t.RegisterCallback(steamworks, onItemInstalled);
    }

    public void Dispose()
    {
      ugc = null;
      steamworks = null;
      remoteStorage = null;
      friends = null;
      OnFileDownloaded = null;
      OnItemInstalled = null;
    }

    private void onItemInstalled(ItemInstalled_t obj, bool failed)
    {
      if (OnItemInstalled == null || (int) obj.AppID != (int) Client.Instance.AppId)
        return;
      OnItemInstalled(obj.PublishedFileId);
    }

    private void onDownloadResult(DownloadItemResult_t obj, bool failed)
    {
      if (OnFileDownloaded == null || (int) obj.AppID != (int) Client.Instance.AppId)
        return;
      OnFileDownloaded(obj.PublishedFileId, (Result) obj.Result);
    }

    public Query CreateQuery()
    {
      return new Query {
        AppId = steamworks.AppId,
        workshop = this,
        friends = friends
      };
    }

    public Editor CreateItem(ItemType type)
    {
      return CreateItem(steamworks.AppId, type);
    }

    public Editor CreateItem(uint workshopUploadAppId, ItemType type)
    {
      return new Editor {
        workshop = this,
        WorkshopUploadAppId = workshopUploadAppId,
        Type = type
      };
    }

    public Editor EditItem(ulong itemId)
    {
      return new Editor {
        workshop = this,
        Id = itemId
      };
    }

    public Item GetItem(ulong itemid) => new Item(itemid, this);

    public enum Order
    {
      RankedByVote,
      RankedByPublicationDate,
      AcceptedForGameRankedByAcceptanceDate,
      RankedByTrend,
      FavoritedByFriendsRankedByPublicationDate,
      CreatedByFriendsRankedByPublicationDate,
      RankedByNumTimesReported,
      CreatedByFollowedUsersRankedByPublicationDate,
      NotYetRated,
      RankedByTotalVotesAsc,
      RankedByVotesUp,
      RankedByTextSearch,
      RankedByTotalUniqueSubscriptions,
    }

    public enum QueryType
    {
      Items,
      MicrotransactionItems,
      SubscriptionItems,
      Collections,
      Artwork,
      Videos,
      Screenshots,
      AllGuides,
      WebGuides,
      IntegratedGuides,
      UsableInGame,
      ControllerBindings,
      GameManagedItems,
    }

    public enum ItemType
    {
      Community,
      Microtransaction,
      Collection,
      Art,
      Video,
      Screenshot,
      Game,
      Software,
      Concept,
      WebGuide,
      IntegratedGuide,
      Merch,
      ControllerBinding,
      SteamworksAccessInvite,
      SteamVideo,
      GameManagedItem,
    }

    public enum UserQueryType : uint
    {
      Published,
      VotedOn,
      VotedUp,
      VotedDown,
      WillVoteLater,
      Favorited,
      Subscribed,
      UsedOrPlayed,
      Followed,
    }

    public class Editor
    {
      internal Workshop workshop;
      internal CallbackHandle CreateItem;
      internal CallbackHandle SubmitItemUpdate;
      internal UGCUpdateHandle_t UpdateHandle;
      private int bytesUploaded;
      private int bytesTotal;

      public ulong Id { get; internal set; }

      public string Title { get; set; } = null;

      public string Description { get; set; } = null;

      public string Folder { get; set; } = null;

      public string PreviewImage { get; set; } = null;

      public List<string> Tags { get; set; } = new List<string>();

      public bool Publishing { get; internal set; }

      public ItemType? Type { get; set; }

      public string Error { get; internal set; }

      public string ChangeNote { get; set; } = "";

      public uint WorkshopUploadAppId { get; set; }

      public VisibilityType? Visibility { get; set; }

      public bool NeedToAgreeToWorkshopLegal { get; internal set; }

      public double Progress
      {
        get
        {
          int bytesTotal = BytesTotal;
          return bytesTotal == 0 ? 0.0 : BytesUploaded / (double) bytesTotal;
        }
      }

      public int BytesUploaded
      {
        get
        {
          if (!Publishing || UpdateHandle == 0UL)
            return bytesUploaded;
          ulong punBytesProcessed = 0;
          ulong punBytesTotal = 0;
          int itemUpdateProgress = (int) workshop.steamworks.native.ugc.GetItemUpdateProgress(UpdateHandle, out punBytesProcessed, out punBytesTotal);
          bytesUploaded = Math.Max(bytesUploaded, (int) punBytesProcessed);
          return bytesUploaded;
        }
      }

      public int BytesTotal
      {
        get
        {
          if (!Publishing || UpdateHandle == 0UL)
            return bytesTotal;
          ulong punBytesProcessed = 0;
          ulong punBytesTotal = 0;
          int itemUpdateProgress = (int) workshop.steamworks.native.ugc.GetItemUpdateProgress(UpdateHandle, out punBytesProcessed, out punBytesTotal);
          bytesTotal = Math.Max(bytesTotal, (int) punBytesTotal);
          return bytesTotal;
        }
      }

      public void Publish()
      {
        bytesUploaded = 0;
        bytesTotal = 0;
        Publishing = true;
        Error = null;
        if (Id == 0UL)
          StartCreatingItem();
        else
          PublishChanges();
      }

      private void StartCreatingItem()
      {
        if (!Type.HasValue)
          throw new Exception("Editor.Type must be set when creating a new item!");
        CreateItem = workshop.ugc.CreateItem(WorkshopUploadAppId, (WorkshopFileType) Type.Value, OnItemCreated);
      }

      private void OnItemCreated(CreateItemResult_t obj, bool Failed)
      {
        NeedToAgreeToWorkshopLegal = obj.UserNeedsToAcceptWorkshopLegalAgreement;
        CreateItem.Dispose();
        CreateItem = null;
        if (obj.Result == SteamNative.Result.OK && !Failed)
        {
          Id = obj.PublishedFileId;
          PublishChanges();
        }
        else
        {
          Error = "Error creating new file: " + obj.Result + "(" + obj.PublishedFileId + ")";
          Publishing = false;
        }
      }

      private void PublishChanges()
      {
        UpdateHandle = workshop.ugc.StartItemUpdate(WorkshopUploadAppId, Id);
        if (Title != null)
          workshop.ugc.SetItemTitle(UpdateHandle, Title);
        if (Description != null)
          workshop.ugc.SetItemDescription(UpdateHandle, Description);
        if (Folder != null)
        {
          if (!new DirectoryInfo(Folder).Exists)
            throw new Exception("Folder doesn't exist (" + Folder + ")");
          workshop.ugc.SetItemContent(UpdateHandle, Folder);
        }
        if (Tags != null && Tags.Count > 0)
          workshop.ugc.SetItemTags(UpdateHandle, Tags.ToArray());
        VisibilityType? visibility = Visibility;
        if (visibility.HasValue)
        {
          SteamUGC ugc = workshop.ugc;
          UGCUpdateHandle_t updateHandle = UpdateHandle;
          visibility = Visibility;
          int eVisibility = (int) visibility.Value;
          ugc.SetItemVisibility(updateHandle, (RemoteStoragePublishedFileVisibility) eVisibility);
        }
        if (PreviewImage != null)
        {
          FileInfo fileInfo = new FileInfo(PreviewImage);
          if (!fileInfo.Exists)
            throw new Exception("PreviewImage doesn't exist (" + PreviewImage + ")");
          if (fileInfo.Length >= 1048576L)
            throw new Exception(string.Format("PreviewImage should be under 1MB ({0})", fileInfo.Length));
          workshop.ugc.SetItemPreview(UpdateHandle, PreviewImage);
        }
        SubmitItemUpdate = workshop.ugc.SubmitItemUpdate(UpdateHandle, ChangeNote, OnChangesSubmitted);
      }

      private void OnChangesSubmitted(SubmitItemUpdateResult_t obj, bool Failed)
      {
        if (Failed)
          throw new Exception("CreateItemResult_t Failed");
        UpdateHandle = 0UL;
        SubmitItemUpdate = null;
        NeedToAgreeToWorkshopLegal = obj.UserNeedsToAcceptWorkshopLegalAgreement;
        Publishing = false;
        if (obj.Result == SteamNative.Result.OK)
          return;
        Error = "Error publishing changes: " + obj.Result + " (" + NeedToAgreeToWorkshopLegal + ")";
      }

      public void Delete()
      {
        workshop.remoteStorage.DeletePublishedFile(Id);
        Id = 0UL;
      }

      public enum VisibilityType
      {
        Public,
        FriendsOnly,
        Private,
      }
    }

    public class Item
    {
      internal Workshop workshop;
      private DirectoryInfo _directory;
      private ulong _BytesDownloaded;
      private ulong _BytesTotal;
      private int YourVote;
      private string _ownerName;

      public string Description { get; private set; }

      public ulong Id { get; private set; }

      public ulong OwnerId { get; private set; }

      public float Score { get; private set; }

      public string[] Tags { get; private set; }

      public string Title { get; private set; }

      public uint VotesDown { get; private set; }

      public uint VotesUp { get; private set; }

      public DateTime Modified { get; private set; }

      public DateTime Created { get; private set; }

      public Item(ulong Id, Workshop workshop)
      {
        this.Id = Id;
        this.workshop = workshop;
      }

      internal static Item From(SteamUGCDetails_t details, Workshop workshop)
      {
        return new Item(details.PublishedFileId, workshop)
        {
          Title = details.Title,
          Description = details.Description,
          OwnerId = details.SteamIDOwner,
          Tags = details.Tags.Split(',').Select(x => x.ToLower()).ToArray(),
          Score = details.Score,
          VotesUp = details.VotesUp,
          VotesDown = details.VotesDown,
          Modified = Utility.Epoch.ToDateTime(details.TimeUpdated),
          Created = Utility.Epoch.ToDateTime(details.TimeCreated)
        };
      }

      public void Download(bool highPriority = true)
      {
        if (Installed || Downloading)
          return;
        if (!workshop.ugc.DownloadItem(Id, highPriority))
        {
          Console.WriteLine("Download Failed");
        }
        else
        {
          workshop.OnFileDownloaded += OnFileDownloaded;
          workshop.OnItemInstalled += OnItemInstalled;
        }
      }

      public void Subscribe()
      {
        workshop.ugc.SubscribeItem(Id);
        ++SubscriptionCount;
      }

      public void UnSubscribe()
      {
        workshop.ugc.UnsubscribeItem(Id);
        --SubscriptionCount;
      }

      private void OnFileDownloaded(ulong fileid, Result result)
      {
        if ((long) fileid != (long) Id)
          return;
        workshop.OnFileDownloaded -= OnFileDownloaded;
      }

      private void OnItemInstalled(ulong fileid)
      {
        if ((long) fileid != (long) Id)
          return;
        workshop.OnItemInstalled -= OnItemInstalled;
      }

      public ulong BytesDownloaded
      {
        get
        {
          UpdateDownloadProgress();
          return _BytesDownloaded;
        }
      }

      public ulong BytesTotalDownload
      {
        get
        {
          UpdateDownloadProgress();
          return _BytesTotal;
        }
      }

      public double DownloadProgress
      {
        get
        {
          UpdateDownloadProgress();
          return _BytesTotal == 0UL ? 0.0 : _BytesDownloaded / (double) _BytesTotal;
        }
      }

      public bool Installed => (State & ItemState.Installed) != 0;

      public bool Downloading => (State & ItemState.Downloading) != 0;

      public bool DownloadPending => (State & ItemState.DownloadPending) != 0;

      public bool Subscribed => (State & ItemState.Subscribed) != 0;

      public bool NeedsUpdate => (State & ItemState.NeedsUpdate) != 0;

      private ItemState State
      {
        get => (ItemState) workshop.ugc.GetItemState(Id);
      }

      public DirectoryInfo Directory
      {
        get
        {
          if (_directory != null)
            return _directory;
          if (!Installed)
            return null;
          ulong punSizeOnDisk;
          string pchFolder;
          if (workshop.ugc.GetItemInstallInfo(Id, out punSizeOnDisk, out pchFolder, out uint _))
          {
            _directory = new DirectoryInfo(pchFolder);
            Size = punSizeOnDisk;
            if (_directory.Exists)
              ;
          }
          return _directory;
        }
      }

      public ulong Size { get; private set; }

      internal void UpdateDownloadProgress()
      {
        workshop.ugc.GetItemDownloadInfo(Id, out _BytesDownloaded, out _BytesTotal);
      }

      public void VoteUp()
      {
        if (YourVote == 1)
          return;
        if (YourVote == -1)
          --VotesDown;
        ++VotesUp;
        workshop.ugc.SetUserItemVote(Id, true);
        YourVote = 1;
      }

      public void VoteDown()
      {
        if (YourVote == -1)
          return;
        if (YourVote == 1)
          --VotesUp;
        ++VotesDown;
        workshop.ugc.SetUserItemVote(Id, false);
        YourVote = -1;
      }

      public Editor Edit() => workshop.EditItem(Id);

      public string Url
      {
        get
        {
          return string.Format("http://steamcommunity.com/sharedfiles/filedetails/?source=Facepunch.Steamworks&id={0}", Id);
        }
      }

      public string ChangelogUrl
      {
        get
        {
          return string.Format("http://steamcommunity.com/sharedfiles/filedetails/changelog/{0}", Id);
        }
      }

      public string CommentsUrl
      {
        get
        {
          return string.Format("http://steamcommunity.com/sharedfiles/filedetails/comments/{0}", Id);
        }
      }

      public string DiscussUrl
      {
        get
        {
          return string.Format("http://steamcommunity.com/sharedfiles/filedetails/discussions/{0}", Id);
        }
      }

      public string StartsUrl
      {
        get
        {
          return string.Format("http://steamcommunity.com/sharedfiles/filedetails/stats/{0}", Id);
        }
      }

      public int SubscriptionCount { get; internal set; }

      public int FavouriteCount { get; internal set; }

      public int FollowerCount { get; internal set; }

      public int WebsiteViews { get; internal set; }

      public int ReportScore { get; internal set; }

      public string PreviewImageUrl { get; internal set; }

      public string OwnerName
      {
        get
        {
          if (_ownerName == null && workshop.friends != null)
          {
            _ownerName = workshop.friends.GetName(OwnerId);
            if (_ownerName == "[unknown]")
            {
              _ownerName = null;
              return string.Empty;
            }
          }
          return _ownerName == null ? string.Empty : _ownerName;
        }
      }
    }

    public class Query : IDisposable
    {
      internal const int SteamResponseSize = 50;
      internal UGCQueryHandle_t Handle;
      internal CallbackHandle Callback;
      public Action<Query> OnResult;
      internal Workshop workshop;
      internal Friends friends;
      private int _resultPage;
      private int _resultsRemain;
      private int _resultSkip;
      private List<Item> _results;

      public uint AppId { get; set; }

      public uint UploaderAppId { get; set; }

      public QueryType QueryType { get; set; } = QueryType.Items;

      public Order Order { get; set; } = Order.RankedByVote;

      public string SearchText { get; set; }

      public Item[] Items { get; set; }

      public int TotalResults { get; set; }

      public ulong? UserId { get; set; }

      public int RankedByTrendDays { get; set; }

      public UserQueryType UserQueryType { get; set; } = UserQueryType.Published;

      public int Page { get; set; } = 1;

      public int PerPage { get; set; } = 50;

      public void Run()
      {
        if (Callback != null)
          return;
        if (Page <= 0)
          throw new Exception("Page should be 1 or above");
        int num = (Page - 1) * PerPage;
        TotalResults = 0;
        _resultSkip = num % 50;
        _resultsRemain = PerPage;
        _resultPage = (int) Math.Floor(num / 50.0);
        _results = new List<Item>();
        RunInternal();
      }

      private void RunInternal()
      {
        if (FileId.Count != 0)
        {
          PublishedFileId_t[] array = FileId.Select((Func<ulong, PublishedFileId_t>) (x => x)).ToArray();
          _resultsRemain = array.Length;
          Handle = workshop.ugc.CreateQueryUGCDetailsRequest(array);
        }
        else
          Handle = !UserId.HasValue ? workshop.ugc.CreateQueryAllUGCRequest((UGCQuery) Order, (UGCMatchingUGCType) QueryType, UploaderAppId, AppId, (uint) (_resultPage + 1)) : workshop.ugc.CreateQueryUserUGCRequest((uint) (UserId.Value & uint.MaxValue), (UserUGCList) UserQueryType, (UGCMatchingUGCType) QueryType, UserUGCListSortOrder.LastUpdatedDesc, UploaderAppId, AppId, (uint) (_resultPage + 1));
        if (!string.IsNullOrEmpty(SearchText))
          workshop.ugc.SetSearchText(Handle, SearchText);
        foreach (string requireTag in RequireTags)
          workshop.ugc.AddRequiredTag(Handle, requireTag);
        if (RequireTags.Count > 0)
          workshop.ugc.SetMatchAnyTag(Handle, !RequireAllTags);
        if (RankedByTrendDays > 0)
          workshop.ugc.SetRankedByTrendDays(Handle, (uint) RankedByTrendDays);
        foreach (string excludeTag in ExcludeTags)
          workshop.ugc.AddExcludedTag(Handle, excludeTag);
        Callback = workshop.ugc.SendQueryUGCRequest(Handle, ResultCallback);
      }

      private void ResultCallback(SteamUGCQueryCompleted_t data, bool bFailed)
      {
        if (bFailed)
          throw new Exception("bFailed!");
        int num = 0;
        for (int index = 0; index < data.NumResultsReturned; ++index)
        {
          if (_resultSkip > 0)
          {
            --_resultSkip;
          }
          else
          {
            SteamUGCDetails_t details = new SteamUGCDetails_t();
            if (workshop.ugc.GetQueryUGCResult(data.Handle, (uint) index, ref details) && !_results.Any(x => (long) x.Id == (long) details.PublishedFileId))
            {
              Item obj = Item.From(details, workshop);
              obj.SubscriptionCount = GetStat(data.Handle, index, ItemStatistic.NumSubscriptions);
              obj.FavouriteCount = GetStat(data.Handle, index, ItemStatistic.NumFavorites);
              obj.FollowerCount = GetStat(data.Handle, index, ItemStatistic.NumFollowers);
              obj.WebsiteViews = GetStat(data.Handle, index, ItemStatistic.NumUniqueWebsiteViews);
              obj.ReportScore = GetStat(data.Handle, index, ItemStatistic.ReportScore);
              string pchURL = null;
              if (workshop.ugc.GetQueryUGCPreviewURL(data.Handle, (uint) index, out pchURL))
                obj.PreviewImageUrl = pchURL;
              _results.Add(obj);
              --_resultsRemain;
              ++num;
              if (_resultsRemain <= 0)
                break;
            }
          }
        }
        TotalResults = TotalResults > data.TotalMatchingResults ? TotalResults : (int) data.TotalMatchingResults;
        Callback.Dispose();
        Callback = null;
        ++_resultPage;
        if (_resultsRemain > 0 && num > 0)
        {
          RunInternal();
        }
        else
        {
          Items = _results.ToArray();
          if (OnResult != null)
            OnResult(this);
        }
      }

      private int GetStat(ulong handle, int index, ItemStatistic stat)
      {
        ulong pStatValue = 0;
        return !workshop.ugc.GetQueryUGCStatistic(handle, (uint) index, (SteamNative.ItemStatistic) stat, out pStatValue) ? 0 : (int) pStatValue;
      }

      public bool IsRunning => Callback != null;

      public List<string> RequireTags { get; set; } = new List<string>();

      public bool RequireAllTags { get; set; } = false;

      public List<string> ExcludeTags { get; set; } = new List<string>();

      public List<ulong> FileId { get; set; } = new List<ulong>();

      public void Block()
      {
        workshop.steamworks.Update();
        while (IsRunning)
        {
          Thread.Sleep(10);
          workshop.steamworks.Update();
        }
      }

      public void Dispose()
      {
      }
    }

    private enum ItemStatistic : uint
    {
      NumSubscriptions,
      NumFavorites,
      NumFollowers,
      NumUniqueSubscriptions,
      NumUniqueFavorites,
      NumUniqueFollowers,
      NumUniqueWebsiteViews,
      ReportScore,
    }
  }
}
