using Facepunch.Steamworks.Callbacks;
using SteamNative;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Facepunch.Steamworks
{
  public class Leaderboard : IDisposable
  {
    private static readonly int[] subEntriesBuffer = new int[512];
    internal ulong BoardId;
    internal Client client;
    private readonly Queue<Action> _onCreated = new Queue<Action>();
    public Leaderboard.Entry[] Results;
    public Action OnBoardInformation;
    [ThreadStatic]
    private static List<Leaderboard.Entry> _sEntryBuffer;

    internal Leaderboard(Client c) => this.client = c;

    public string Name { get; private set; }

    public int TotalEntries { get; private set; }

    public bool IsValid => this.BoardId > 0UL;

    public bool IsError { get; private set; }

    public bool IsQuerying { get; private set; }

    public void Dispose() => this.client = (Client) null;

    private void DispatchOnCreatedCallbacks()
    {
      while (this._onCreated.Count > 0)
        this._onCreated.Dequeue()();
    }

    private bool DeferOnCreated(Action onValid, FailureCallback onFailure = null)
    {
      if (this.IsValid || this.IsError)
        return false;
      this._onCreated.Enqueue((Action) (() =>
      {
        if (this.IsValid)
        {
          onValid();
        }
        else
        {
          FailureCallback failureCallback = onFailure;
          if (failureCallback == null)
            return;
          failureCallback(Facepunch.Steamworks.Callbacks.Result.Fail);
        }
      }));
      return true;
    }

    internal void OnBoardCreated(LeaderboardFindResult_t result, bool error)
    {
      if (error || result.LeaderboardFound == (byte) 0)
      {
        this.IsError = true;
      }
      else
      {
        this.BoardId = result.SteamLeaderboard;
        if (this.IsValid)
        {
          this.Name = this.client.native.userstats.GetLeaderboardName((SteamLeaderboard_t) this.BoardId);
          this.TotalEntries = this.client.native.userstats.GetLeaderboardEntryCount((SteamLeaderboard_t) this.BoardId);
          Action boardInformation = this.OnBoardInformation;
          if (boardInformation != null)
            boardInformation();
        }
      }
      this.DispatchOnCreatedCallbacks();
    }

    public bool AddScore(bool onlyIfBeatsOldScore, int score, params int[] subscores)
    {
      if (this.IsError)
        return false;
      if (!this.IsValid)
        return this.DeferOnCreated((Action) (() => this.AddScore(onlyIfBeatsOldScore, score, subscores)));
      LeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod = LeaderboardUploadScoreMethod.ForceUpdate;
      if (onlyIfBeatsOldScore)
        eLeaderboardUploadScoreMethod = LeaderboardUploadScoreMethod.KeepBest;
      this.client.native.userstats.UploadLeaderboardScore((SteamLeaderboard_t) this.BoardId, eLeaderboardUploadScoreMethod, score, subscores, subscores.Length);
      return true;
    }

    public bool AddScore(
      bool onlyIfBeatsOldScore,
      int score,
      int[] subscores = null,
      Leaderboard.AddScoreCallback onSuccess = null,
      FailureCallback onFailure = null)
    {
      if (this.IsError)
        return false;
      if (!this.IsValid)
        return this.DeferOnCreated((Action) (() => this.AddScore(onlyIfBeatsOldScore, score, subscores, onSuccess, onFailure)), onFailure);
      if (subscores == null)
        subscores = new int[0];
      LeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod = LeaderboardUploadScoreMethod.ForceUpdate;
      if (onlyIfBeatsOldScore)
        eLeaderboardUploadScoreMethod = LeaderboardUploadScoreMethod.KeepBest;
      this.client.native.userstats.UploadLeaderboardScore((SteamLeaderboard_t) this.BoardId, eLeaderboardUploadScoreMethod, score, subscores, subscores.Length, (Action<LeaderboardScoreUploaded_t, bool>) ((result, error) =>
      {
        if (!error && result.Success > (byte) 0)
        {
          Leaderboard.AddScoreCallback addScoreCallback = onSuccess;
          if (addScoreCallback == null)
            return;
          addScoreCallback(new Leaderboard.AddScoreResult()
          {
            Score = result.Score,
            ScoreChanged = result.ScoreChanged > (byte) 0,
            GlobalRankNew = result.GlobalRankNew,
            GlobalRankPrevious = result.GlobalRankPrevious
          });
        }
        else
        {
          FailureCallback failureCallback = onFailure;
          if (failureCallback != null)
            failureCallback(error ? Facepunch.Steamworks.Callbacks.Result.IOFailure : Facepunch.Steamworks.Callbacks.Result.Fail);
        }
      }));
      return true;
    }

    public bool AttachRemoteFile(
      RemoteFile file,
      Leaderboard.AttachRemoteFileCallback onSuccess = null,
      FailureCallback onFailure = null)
    {
      if (this.IsError)
        return false;
      if (!this.IsValid)
        return this.DeferOnCreated((Action) (() => this.AttachRemoteFile(file, onSuccess, onFailure)), onFailure);
      if (file.IsShared)
        return (ulong) this.client.native.userstats.AttachLeaderboardUGC((SteamLeaderboard_t) this.BoardId, file.UGCHandle, (Action<LeaderboardUGCSet_t, bool>) ((result, error) =>
        {
          if (!error && result.Result == SteamNative.Result.OK)
          {
            Leaderboard.AttachRemoteFileCallback remoteFileCallback = onSuccess;
            if (remoteFileCallback == null)
              return;
            remoteFileCallback();
          }
          else
          {
            FailureCallback failureCallback = onFailure;
            if (failureCallback != null)
              failureCallback(result.Result == (SteamNative.Result) 0 ? Facepunch.Steamworks.Callbacks.Result.IOFailure : (Facepunch.Steamworks.Callbacks.Result) result.Result);
          }
        })).CallResultHandle > 0UL;
      file.Share((RemoteFile.ShareCallback) (() =>
      {
        if (file.IsShared && this.AttachRemoteFile(file, onSuccess, onFailure))
          return;
        FailureCallback failureCallback = onFailure;
        if (failureCallback != null)
          failureCallback(Facepunch.Steamworks.Callbacks.Result.Fail);
      }), onFailure);
      return true;
    }

    public bool FetchScores(Leaderboard.RequestType RequestType, int start, int end)
    {
      if (!this.IsValid || this.IsQuerying)
        return false;
      this.client.native.userstats.DownloadLeaderboardEntries((SteamLeaderboard_t) this.BoardId, (LeaderboardDataRequest) RequestType, start, end, new Action<LeaderboardScoresDownloaded_t, bool>(this.OnScores));
      this.Results = (Leaderboard.Entry[]) null;
      this.IsQuerying = true;
      return true;
    }

    private unsafe void ReadScores(
      LeaderboardScoresDownloaded_t result,
      List<Leaderboard.Entry> dest)
    {
      for (int index = 0; index < result.CEntryCount; ++index)
      {
        fixed (int* pDetails = Leaderboard.subEntriesBuffer)
        {
          LeaderboardEntry_t pLeaderboardEntry = new LeaderboardEntry_t();
          if (this.client.native.userstats.GetDownloadedLeaderboardEntry((SteamLeaderboardEntries_t) result.SteamLeaderboardEntries, index, ref pLeaderboardEntry, (IntPtr) (void*) pDetails, Leaderboard.subEntriesBuffer.Length))
          {
            List<Leaderboard.Entry> entryList = dest;
            Leaderboard.Entry entry = new Leaderboard.Entry()
            {
              GlobalRank = pLeaderboardEntry.GlobalRank,
              Score = pLeaderboardEntry.Score,
              SteamId = pLeaderboardEntry.SteamIDUser,
              SubScores = pLeaderboardEntry.CDetails == 0 ? (int[]) null : ((IEnumerable<int>) Leaderboard.subEntriesBuffer).Take<int>(pLeaderboardEntry.CDetails).ToArray<int>(),
              Name = this.client.Friends.GetName(pLeaderboardEntry.SteamIDUser),
              AttachedFile = pLeaderboardEntry.UGC >> 32 == (ulong) uint.MaxValue ? (RemoteFile) null : new RemoteFile(this.client.RemoteStorage, (UGCHandle_t) pLeaderboardEntry.UGC)
            };
            entryList.Add(entry);
          }
        }
      }
    }

    public bool FetchScores(
      Leaderboard.RequestType RequestType,
      int start,
      int end,
      Leaderboard.FetchScoresCallback onSuccess,
      FailureCallback onFailure = null)
    {
      if (this.IsError)
        return false;
      if (!this.IsValid)
        return this.DeferOnCreated((Action) (() => this.FetchScores(RequestType, start, end, onSuccess, onFailure)), onFailure);
      this.client.native.userstats.DownloadLeaderboardEntries((SteamLeaderboard_t) this.BoardId, (LeaderboardDataRequest) RequestType, start, end, (Action<LeaderboardScoresDownloaded_t, bool>) ((result, error) =>
      {
        if (error)
        {
          FailureCallback failureCallback = onFailure;
          if (failureCallback == null)
            return;
          failureCallback(Facepunch.Steamworks.Callbacks.Result.IOFailure);
        }
        else
        {
          if (Leaderboard._sEntryBuffer == null)
            Leaderboard._sEntryBuffer = new List<Leaderboard.Entry>();
          else
            Leaderboard._sEntryBuffer.Clear();
          this.ReadScores(result, Leaderboard._sEntryBuffer);
          onSuccess(Leaderboard._sEntryBuffer.ToArray());
        }
      }));
      return true;
    }

    private void OnScores(LeaderboardScoresDownloaded_t result, bool error)
    {
      this.IsQuerying = false;
      if (this.client == null || error)
        return;
      List<Leaderboard.Entry> dest = new List<Leaderboard.Entry>();
      this.ReadScores(result, dest);
      this.Results = dest.ToArray();
    }

    public enum RequestType
    {
      Global,
      GlobalAroundUser,
      Friends,
    }

    public delegate void AddScoreCallback(Leaderboard.AddScoreResult result);

    public struct AddScoreResult
    {
      public int Score;
      public bool ScoreChanged;
      public int GlobalRankNew;
      public int GlobalRankPrevious;
    }

    public delegate void AttachRemoteFileCallback();

    public delegate void FetchScoresCallback(Leaderboard.Entry[] results);

    public struct Entry
    {
      public ulong SteamId;
      public int Score;
      public int[] SubScores;
      public int GlobalRank;
      public RemoteFile AttachedFile;
      public string Name;
    }
  }
}
