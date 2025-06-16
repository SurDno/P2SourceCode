// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Workshop
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

#nullable disable
namespace Facepunch.Steamworks
{
  public class Workshop : IDisposable
  {
    internal const ulong InvalidHandle = 18446744073709551615;
    internal SteamUGC ugc;
    internal Friends friends;
    internal BaseSteamworks steamworks;
    internal SteamRemoteStorage remoteStorage;

    public event Action<ulong, Facepunch.Steamworks.Callbacks.Result> OnFileDownloaded;

    public event Action<ulong> OnItemInstalled;

    internal Workshop(BaseSteamworks steamworks, SteamUGC ugc, SteamRemoteStorage remoteStorage)
    {
      this.ugc = ugc;
      this.steamworks = steamworks;
      this.remoteStorage = remoteStorage;
      DownloadItemResult_t.RegisterCallback(steamworks, new Action<DownloadItemResult_t, bool>(this.onDownloadResult));
      ItemInstalled_t.RegisterCallback(steamworks, new Action<ItemInstalled_t, bool>(this.onItemInstalled));
    }

    public void Dispose()
    {
      this.ugc = (SteamUGC) null;
      this.steamworks = (BaseSteamworks) null;
      this.remoteStorage = (SteamRemoteStorage) null;
      this.friends = (Friends) null;
      this.OnFileDownloaded = (Action<ulong, Facepunch.Steamworks.Callbacks.Result>) null;
      this.OnItemInstalled = (Action<ulong>) null;
    }

    private void onItemInstalled(ItemInstalled_t obj, bool failed)
    {
      if (this.OnItemInstalled == null || (int) obj.AppID != (int) Client.Instance.AppId)
        return;
      this.OnItemInstalled(obj.PublishedFileId);
    }

    private void onDownloadResult(DownloadItemResult_t obj, bool failed)
    {
      if (this.OnFileDownloaded == null || (int) obj.AppID != (int) Client.Instance.AppId)
        return;
      this.OnFileDownloaded(obj.PublishedFileId, (Facepunch.Steamworks.Callbacks.Result) obj.Result);
    }

    public Workshop.Query CreateQuery()
    {
      return new Workshop.Query()
      {
        AppId = this.steamworks.AppId,
        workshop = this,
        friends = this.friends
      };
    }

    public Workshop.Editor CreateItem(Workshop.ItemType type)
    {
      return this.CreateItem(this.steamworks.AppId, type);
    }

    public Workshop.Editor CreateItem(uint workshopUploadAppId, Workshop.ItemType type)
    {
      return new Workshop.Editor()
      {
        workshop = this,
        WorkshopUploadAppId = workshopUploadAppId,
        Type = new Workshop.ItemType?(type)
      };
    }

    public Workshop.Editor EditItem(ulong itemId)
    {
      return new Workshop.Editor()
      {
        workshop = this,
        Id = itemId
      };
    }

    public Workshop.Item GetItem(ulong itemid) => new Workshop.Item(itemid, this);

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
      private int bytesUploaded = 0;
      private int bytesTotal = 0;

      public ulong Id { get; internal set; }

      public string Title { get; set; } = (string) null;

      public string Description { get; set; } = (string) null;

      public string Folder { get; set; } = (string) null;

      public string PreviewImage { get; set; } = (string) null;

      public List<string> Tags { get; set; } = new List<string>();

      public bool Publishing { get; internal set; }

      public Workshop.ItemType? Type { get; set; }

      public string Error { get; internal set; } = (string) null;

      public string ChangeNote { get; set; } = "";

      public uint WorkshopUploadAppId { get; set; }

      public Workshop.Editor.VisibilityType? Visibility { get; set; }

      public bool NeedToAgreeToWorkshopLegal { get; internal set; }

      public double Progress
      {
        get
        {
          int bytesTotal = this.BytesTotal;
          return bytesTotal == 0 ? 0.0 : (double) this.BytesUploaded / (double) bytesTotal;
        }
      }

      public int BytesUploaded
      {
        get
        {
          if (!this.Publishing || (ulong) this.UpdateHandle == 0UL)
            return this.bytesUploaded;
          ulong punBytesProcessed = 0;
          ulong punBytesTotal = 0;
          int itemUpdateProgress = (int) this.workshop.steamworks.native.ugc.GetItemUpdateProgress(this.UpdateHandle, out punBytesProcessed, out punBytesTotal);
          this.bytesUploaded = Math.Max(this.bytesUploaded, (int) punBytesProcessed);
          return this.bytesUploaded;
        }
      }

      public int BytesTotal
      {
        get
        {
          if (!this.Publishing || (ulong) this.UpdateHandle == 0UL)
            return this.bytesTotal;
          ulong punBytesProcessed = 0;
          ulong punBytesTotal = 0;
          int itemUpdateProgress = (int) this.workshop.steamworks.native.ugc.GetItemUpdateProgress(this.UpdateHandle, out punBytesProcessed, out punBytesTotal);
          this.bytesTotal = Math.Max(this.bytesTotal, (int) punBytesTotal);
          return this.bytesTotal;
        }
      }

      public void Publish()
      {
        this.bytesUploaded = 0;
        this.bytesTotal = 0;
        this.Publishing = true;
        this.Error = (string) null;
        if (this.Id == 0UL)
          this.StartCreatingItem();
        else
          this.PublishChanges();
      }

      private void StartCreatingItem()
      {
        if (!this.Type.HasValue)
          throw new Exception("Editor.Type must be set when creating a new item!");
        this.CreateItem = this.workshop.ugc.CreateItem((AppId_t) this.WorkshopUploadAppId, (WorkshopFileType) this.Type.Value, new Action<CreateItemResult_t, bool>(this.OnItemCreated));
      }

      private void OnItemCreated(CreateItemResult_t obj, bool Failed)
      {
        this.NeedToAgreeToWorkshopLegal = obj.UserNeedsToAcceptWorkshopLegalAgreement;
        this.CreateItem.Dispose();
        this.CreateItem = (CallbackHandle) null;
        if (obj.Result == SteamNative.Result.OK && !Failed)
        {
          this.Id = obj.PublishedFileId;
          this.PublishChanges();
        }
        else
        {
          this.Error = "Error creating new file: " + obj.Result.ToString() + "(" + (object) obj.PublishedFileId + ")";
          this.Publishing = false;
        }
      }

      private void PublishChanges()
      {
        this.UpdateHandle = this.workshop.ugc.StartItemUpdate((AppId_t) this.WorkshopUploadAppId, (PublishedFileId_t) this.Id);
        if (this.Title != null)
          this.workshop.ugc.SetItemTitle(this.UpdateHandle, this.Title);
        if (this.Description != null)
          this.workshop.ugc.SetItemDescription(this.UpdateHandle, this.Description);
        if (this.Folder != null)
        {
          if (!new DirectoryInfo(this.Folder).Exists)
            throw new Exception("Folder doesn't exist (" + this.Folder + ")");
          this.workshop.ugc.SetItemContent(this.UpdateHandle, this.Folder);
        }
        if (this.Tags != null && this.Tags.Count > 0)
          this.workshop.ugc.SetItemTags(this.UpdateHandle, this.Tags.ToArray());
        Workshop.Editor.VisibilityType? visibility = this.Visibility;
        if (visibility.HasValue)
        {
          SteamUGC ugc = this.workshop.ugc;
          UGCUpdateHandle_t updateHandle = this.UpdateHandle;
          visibility = this.Visibility;
          int eVisibility = (int) visibility.Value;
          ugc.SetItemVisibility(updateHandle, (RemoteStoragePublishedFileVisibility) eVisibility);
        }
        if (this.PreviewImage != null)
        {
          FileInfo fileInfo = new FileInfo(this.PreviewImage);
          if (!fileInfo.Exists)
            throw new Exception("PreviewImage doesn't exist (" + this.PreviewImage + ")");
          if (fileInfo.Length >= 1048576L)
            throw new Exception(string.Format("PreviewImage should be under 1MB ({0})", (object) fileInfo.Length));
          this.workshop.ugc.SetItemPreview(this.UpdateHandle, this.PreviewImage);
        }
        this.SubmitItemUpdate = this.workshop.ugc.SubmitItemUpdate(this.UpdateHandle, this.ChangeNote, new Action<SubmitItemUpdateResult_t, bool>(this.OnChangesSubmitted));
      }

      private void OnChangesSubmitted(SubmitItemUpdateResult_t obj, bool Failed)
      {
        if (Failed)
          throw new Exception("CreateItemResult_t Failed");
        this.UpdateHandle = (UGCUpdateHandle_t) 0UL;
        this.SubmitItemUpdate = (CallbackHandle) null;
        this.NeedToAgreeToWorkshopLegal = obj.UserNeedsToAcceptWorkshopLegalAgreement;
        this.Publishing = false;
        if (obj.Result == SteamNative.Result.OK)
          return;
        this.Error = "Error publishing changes: " + obj.Result.ToString() + " (" + this.NeedToAgreeToWorkshopLegal.ToString() + ")";
      }

      public void Delete()
      {
        this.workshop.remoteStorage.DeletePublishedFile((PublishedFileId_t) this.Id);
        this.Id = 0UL;
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
      private int YourVote = 0;
      private string _ownerName = (string) null;

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

      internal static Workshop.Item From(SteamUGCDetails_t details, Workshop workshop)
      {
        return new Workshop.Item(details.PublishedFileId, workshop)
        {
          Title = details.Title,
          Description = details.Description,
          OwnerId = details.SteamIDOwner,
          Tags = ((IEnumerable<string>) details.Tags.Split(',')).Select<string, string>((Func<string, string>) (x => x.ToLower())).ToArray<string>(),
          Score = details.Score,
          VotesUp = details.VotesUp,
          VotesDown = details.VotesDown,
          Modified = Utility.Epoch.ToDateTime((Decimal) details.TimeUpdated),
          Created = Utility.Epoch.ToDateTime((Decimal) details.TimeCreated)
        };
      }

      public void Download(bool highPriority = true)
      {
        if (this.Installed || this.Downloading)
          return;
        if (!this.workshop.ugc.DownloadItem((PublishedFileId_t) this.Id, highPriority))
        {
          Console.WriteLine("Download Failed");
        }
        else
        {
          this.workshop.OnFileDownloaded += new Action<ulong, Facepunch.Steamworks.Callbacks.Result>(this.OnFileDownloaded);
          this.workshop.OnItemInstalled += new Action<ulong>(this.OnItemInstalled);
        }
      }

      public void Subscribe()
      {
        this.workshop.ugc.SubscribeItem((PublishedFileId_t) this.Id);
        ++this.SubscriptionCount;
      }

      public void UnSubscribe()
      {
        this.workshop.ugc.UnsubscribeItem((PublishedFileId_t) this.Id);
        --this.SubscriptionCount;
      }

      private void OnFileDownloaded(ulong fileid, Facepunch.Steamworks.Callbacks.Result result)
      {
        if ((long) fileid != (long) this.Id)
          return;
        this.workshop.OnFileDownloaded -= new Action<ulong, Facepunch.Steamworks.Callbacks.Result>(this.OnFileDownloaded);
      }

      private void OnItemInstalled(ulong fileid)
      {
        if ((long) fileid != (long) this.Id)
          return;
        this.workshop.OnItemInstalled -= new Action<ulong>(this.OnItemInstalled);
      }

      public ulong BytesDownloaded
      {
        get
        {
          this.UpdateDownloadProgress();
          return this._BytesDownloaded;
        }
      }

      public ulong BytesTotalDownload
      {
        get
        {
          this.UpdateDownloadProgress();
          return this._BytesTotal;
        }
      }

      public double DownloadProgress
      {
        get
        {
          this.UpdateDownloadProgress();
          return this._BytesTotal == 0UL ? 0.0 : (double) this._BytesDownloaded / (double) this._BytesTotal;
        }
      }

      public bool Installed => (this.State & ItemState.Installed) != 0;

      public bool Downloading => (this.State & ItemState.Downloading) != 0;

      public bool DownloadPending => (this.State & ItemState.DownloadPending) != 0;

      public bool Subscribed => (this.State & ItemState.Subscribed) != 0;

      public bool NeedsUpdate => (this.State & ItemState.NeedsUpdate) != 0;

      private ItemState State
      {
        get => (ItemState) this.workshop.ugc.GetItemState((PublishedFileId_t) this.Id);
      }

      public DirectoryInfo Directory
      {
        get
        {
          if (this._directory != null)
            return this._directory;
          if (!this.Installed)
            return (DirectoryInfo) null;
          ulong punSizeOnDisk;
          string pchFolder;
          if (this.workshop.ugc.GetItemInstallInfo((PublishedFileId_t) this.Id, out punSizeOnDisk, out pchFolder, out uint _))
          {
            this._directory = new DirectoryInfo(pchFolder);
            this.Size = punSizeOnDisk;
            if (this._directory.Exists)
              ;
          }
          return this._directory;
        }
      }

      public ulong Size { get; private set; }

      internal void UpdateDownloadProgress()
      {
        this.workshop.ugc.GetItemDownloadInfo((PublishedFileId_t) this.Id, out this._BytesDownloaded, out this._BytesTotal);
      }

      public void VoteUp()
      {
        if (this.YourVote == 1)
          return;
        if (this.YourVote == -1)
          --this.VotesDown;
        ++this.VotesUp;
        this.workshop.ugc.SetUserItemVote((PublishedFileId_t) this.Id, true);
        this.YourVote = 1;
      }

      public void VoteDown()
      {
        if (this.YourVote == -1)
          return;
        if (this.YourVote == 1)
          --this.VotesUp;
        ++this.VotesDown;
        this.workshop.ugc.SetUserItemVote((PublishedFileId_t) this.Id, false);
        this.YourVote = -1;
      }

      public Workshop.Editor Edit() => this.workshop.EditItem(this.Id);

      public string Url
      {
        get
        {
          return string.Format("http://steamcommunity.com/sharedfiles/filedetails/?source=Facepunch.Steamworks&id={0}", (object) this.Id);
        }
      }

      public string ChangelogUrl
      {
        get
        {
          return string.Format("http://steamcommunity.com/sharedfiles/filedetails/changelog/{0}", (object) this.Id);
        }
      }

      public string CommentsUrl
      {
        get
        {
          return string.Format("http://steamcommunity.com/sharedfiles/filedetails/comments/{0}", (object) this.Id);
        }
      }

      public string DiscussUrl
      {
        get
        {
          return string.Format("http://steamcommunity.com/sharedfiles/filedetails/discussions/{0}", (object) this.Id);
        }
      }

      public string StartsUrl
      {
        get
        {
          return string.Format("http://steamcommunity.com/sharedfiles/filedetails/stats/{0}", (object) this.Id);
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
          if (this._ownerName == null && this.workshop.friends != null)
          {
            this._ownerName = this.workshop.friends.GetName(this.OwnerId);
            if (this._ownerName == "[unknown]")
            {
              this._ownerName = (string) null;
              return string.Empty;
            }
          }
          return this._ownerName == null ? string.Empty : this._ownerName;
        }
      }
    }

    public class Query : IDisposable
    {
      internal const int SteamResponseSize = 50;
      internal UGCQueryHandle_t Handle;
      internal CallbackHandle Callback;
      public Action<Workshop.Query> OnResult;
      internal Workshop workshop;
      internal Friends friends;
      private int _resultPage = 0;
      private int _resultsRemain = 0;
      private int _resultSkip = 0;
      private List<Workshop.Item> _results;

      public uint AppId { get; set; }

      public uint UploaderAppId { get; set; }

      public Workshop.QueryType QueryType { get; set; } = Workshop.QueryType.Items;

      public Workshop.Order Order { get; set; } = Workshop.Order.RankedByVote;

      public string SearchText { get; set; }

      public Workshop.Item[] Items { get; set; }

      public int TotalResults { get; set; }

      public ulong? UserId { get; set; }

      public int RankedByTrendDays { get; set; }

      public Workshop.UserQueryType UserQueryType { get; set; } = Workshop.UserQueryType.Published;

      public int Page { get; set; } = 1;

      public int PerPage { get; set; } = 50;

      public void Run()
      {
        if (this.Callback != null)
          return;
        if (this.Page <= 0)
          throw new Exception("Page should be 1 or above");
        int num = (this.Page - 1) * this.PerPage;
        this.TotalResults = 0;
        this._resultSkip = num % 50;
        this._resultsRemain = this.PerPage;
        this._resultPage = (int) Math.Floor((double) num / 50.0);
        this._results = new List<Workshop.Item>();
        this.RunInternal();
      }

      private void RunInternal()
      {
        if (this.FileId.Count != 0)
        {
          PublishedFileId_t[] array = this.FileId.Select<ulong, PublishedFileId_t>((Func<ulong, PublishedFileId_t>) (x => (PublishedFileId_t) x)).ToArray<PublishedFileId_t>();
          this._resultsRemain = array.Length;
          this.Handle = this.workshop.ugc.CreateQueryUGCDetailsRequest(array);
        }
        else
          this.Handle = !this.UserId.HasValue ? this.workshop.ugc.CreateQueryAllUGCRequest((UGCQuery) this.Order, (UGCMatchingUGCType) this.QueryType, (AppId_t) this.UploaderAppId, (AppId_t) this.AppId, (uint) (this._resultPage + 1)) : this.workshop.ugc.CreateQueryUserUGCRequest((AccountID_t) (uint) (this.UserId.Value & (ulong) uint.MaxValue), (UserUGCList) this.UserQueryType, (UGCMatchingUGCType) this.QueryType, UserUGCListSortOrder.LastUpdatedDesc, (AppId_t) this.UploaderAppId, (AppId_t) this.AppId, (uint) (this._resultPage + 1));
        if (!string.IsNullOrEmpty(this.SearchText))
          this.workshop.ugc.SetSearchText(this.Handle, this.SearchText);
        foreach (string requireTag in this.RequireTags)
          this.workshop.ugc.AddRequiredTag(this.Handle, requireTag);
        if (this.RequireTags.Count > 0)
          this.workshop.ugc.SetMatchAnyTag(this.Handle, !this.RequireAllTags);
        if (this.RankedByTrendDays > 0)
          this.workshop.ugc.SetRankedByTrendDays(this.Handle, (uint) this.RankedByTrendDays);
        foreach (string excludeTag in this.ExcludeTags)
          this.workshop.ugc.AddExcludedTag(this.Handle, excludeTag);
        this.Callback = this.workshop.ugc.SendQueryUGCRequest(this.Handle, new Action<SteamUGCQueryCompleted_t, bool>(this.ResultCallback));
      }

      private void ResultCallback(SteamUGCQueryCompleted_t data, bool bFailed)
      {
        if (bFailed)
          throw new Exception("bFailed!");
        int num = 0;
        for (int index = 0; (long) index < (long) data.NumResultsReturned; ++index)
        {
          if (this._resultSkip > 0)
          {
            --this._resultSkip;
          }
          else
          {
            SteamUGCDetails_t details = new SteamUGCDetails_t();
            if (this.workshop.ugc.GetQueryUGCResult((UGCQueryHandle_t) data.Handle, (uint) index, ref details) && !this._results.Any<Workshop.Item>((Func<Workshop.Item, bool>) (x => (long) x.Id == (long) details.PublishedFileId)))
            {
              Workshop.Item obj = Workshop.Item.From(details, this.workshop);
              obj.SubscriptionCount = this.GetStat(data.Handle, index, Workshop.ItemStatistic.NumSubscriptions);
              obj.FavouriteCount = this.GetStat(data.Handle, index, Workshop.ItemStatistic.NumFavorites);
              obj.FollowerCount = this.GetStat(data.Handle, index, Workshop.ItemStatistic.NumFollowers);
              obj.WebsiteViews = this.GetStat(data.Handle, index, Workshop.ItemStatistic.NumUniqueWebsiteViews);
              obj.ReportScore = this.GetStat(data.Handle, index, Workshop.ItemStatistic.ReportScore);
              string pchURL = (string) null;
              if (this.workshop.ugc.GetQueryUGCPreviewURL((UGCQueryHandle_t) data.Handle, (uint) index, out pchURL))
                obj.PreviewImageUrl = pchURL;
              this._results.Add(obj);
              --this._resultsRemain;
              ++num;
              if (this._resultsRemain <= 0)
                break;
            }
          }
        }
        this.TotalResults = (long) this.TotalResults > (long) data.TotalMatchingResults ? this.TotalResults : (int) data.TotalMatchingResults;
        this.Callback.Dispose();
        this.Callback = (CallbackHandle) null;
        ++this._resultPage;
        if (this._resultsRemain > 0 && num > 0)
        {
          this.RunInternal();
        }
        else
        {
          this.Items = this._results.ToArray();
          if (this.OnResult != null)
            this.OnResult(this);
        }
      }

      private int GetStat(ulong handle, int index, Workshop.ItemStatistic stat)
      {
        ulong pStatValue = 0;
        return !this.workshop.ugc.GetQueryUGCStatistic((UGCQueryHandle_t) handle, (uint) index, (SteamNative.ItemStatistic) stat, out pStatValue) ? 0 : (int) pStatValue;
      }

      public bool IsRunning => this.Callback != null;

      public List<string> RequireTags { get; set; } = new List<string>();

      public bool RequireAllTags { get; set; } = false;

      public List<string> ExcludeTags { get; set; } = new List<string>();

      public List<ulong> FileId { get; set; } = new List<ulong>();

      public void Block()
      {
        this.workshop.steamworks.Update();
        while (this.IsRunning)
        {
          Thread.Sleep(10);
          this.workshop.steamworks.Update();
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
