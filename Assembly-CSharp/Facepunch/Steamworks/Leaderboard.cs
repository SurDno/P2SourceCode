﻿using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Steamworks.Callbacks;
using SteamNative;
using Result = Facepunch.Steamworks.Callbacks.Result;

namespace Facepunch.Steamworks
{
  public class Leaderboard : IDisposable
  {
    private static readonly int[] subEntriesBuffer = new int[512];
    internal ulong BoardId;
    internal Client client;
    private readonly Queue<Action> _onCreated = new Queue<Action>();
    public Entry[] Results;
    public Action OnBoardInformation;
    [ThreadStatic]
    private static List<Entry> _sEntryBuffer;

    internal Leaderboard(Client c) => client = c;

    public string Name { get; private set; }

    public int TotalEntries { get; private set; }

    public bool IsValid => BoardId > 0UL;

    public bool IsError { get; private set; }

    public bool IsQuerying { get; private set; }

    public void Dispose() => client = null;

    private void DispatchOnCreatedCallbacks()
    {
      while (_onCreated.Count > 0)
        _onCreated.Dequeue()();
    }

    private bool DeferOnCreated(Action onValid, FailureCallback onFailure = null)
    {
      if (IsValid || IsError)
        return false;
      _onCreated.Enqueue((Action) (() =>
      {
        if (IsValid)
        {
          onValid();
        }
        else
        {
          FailureCallback failureCallback = onFailure;
          if (failureCallback == null)
            return;
          failureCallback(Result.Fail);
        }
      }));
      return true;
    }

    internal void OnBoardCreated(LeaderboardFindResult_t result, bool error)
    {
      if (error || result.LeaderboardFound == 0)
      {
        IsError = true;
      }
      else
      {
        BoardId = result.SteamLeaderboard;
        if (IsValid)
        {
          Name = client.native.userstats.GetLeaderboardName(BoardId);
          TotalEntries = client.native.userstats.GetLeaderboardEntryCount(BoardId);
          Action boardInformation = OnBoardInformation;
          if (boardInformation != null)
            boardInformation();
        }
      }
      DispatchOnCreatedCallbacks();
    }

    public bool AddScore(bool onlyIfBeatsOldScore, int score, params int[] subscores)
    {
      if (IsError)
        return false;
      if (!IsValid)
        return DeferOnCreated(() => AddScore(onlyIfBeatsOldScore, score, subscores));
      LeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod = LeaderboardUploadScoreMethod.ForceUpdate;
      if (onlyIfBeatsOldScore)
        eLeaderboardUploadScoreMethod = LeaderboardUploadScoreMethod.KeepBest;
      client.native.userstats.UploadLeaderboardScore(BoardId, eLeaderboardUploadScoreMethod, score, subscores, subscores.Length);
      return true;
    }

    public bool AddScore(
      bool onlyIfBeatsOldScore,
      int score,
      int[] subscores = null,
      AddScoreCallback onSuccess = null,
      FailureCallback onFailure = null)
    {
      if (IsError)
        return false;
      if (!IsValid)
        return DeferOnCreated(() => AddScore(onlyIfBeatsOldScore, score, subscores, onSuccess, onFailure), onFailure);
      if (subscores == null)
        subscores = new int[0];
      LeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod = LeaderboardUploadScoreMethod.ForceUpdate;
      if (onlyIfBeatsOldScore)
        eLeaderboardUploadScoreMethod = LeaderboardUploadScoreMethod.KeepBest;
      client.native.userstats.UploadLeaderboardScore(BoardId, eLeaderboardUploadScoreMethod, score, subscores, subscores.Length, (result, error) =>
      {
        if (!error && result.Success > 0)
        {
          AddScoreCallback addScoreCallback = onSuccess;
          if (addScoreCallback == null)
            return;
          addScoreCallback(new AddScoreResult {
            Score = result.Score,
            ScoreChanged = result.ScoreChanged > 0,
            GlobalRankNew = result.GlobalRankNew,
            GlobalRankPrevious = result.GlobalRankPrevious
          });
        }
        else
        {
          FailureCallback failureCallback = onFailure;
          if (failureCallback != null)
            failureCallback(error ? Result.IOFailure : Result.Fail);
        }
      });
      return true;
    }

    public bool AttachRemoteFile(
      RemoteFile file,
      AttachRemoteFileCallback onSuccess = null,
      FailureCallback onFailure = null)
    {
      if (IsError)
        return false;
      if (!IsValid)
        return DeferOnCreated(() => AttachRemoteFile(file, onSuccess, onFailure), onFailure);
      if (file.IsShared)
        return client.native.userstats.AttachLeaderboardUGC(BoardId, file.UGCHandle, (result, error) =>
        {
          if (!error && result.Result == SteamNative.Result.OK)
          {
            AttachRemoteFileCallback remoteFileCallback = onSuccess;
            if (remoteFileCallback == null)
              return;
            remoteFileCallback();
          }
          else
          {
            FailureCallback failureCallback = onFailure;
            if (failureCallback != null)
              failureCallback(result.Result == 0 ? Result.IOFailure : (Result) result.Result);
          }
        }).CallResultHandle > 0UL;
      file.Share(() =>
      {
        if (file.IsShared && AttachRemoteFile(file, onSuccess, onFailure))
          return;
        FailureCallback failureCallback = onFailure;
        if (failureCallback != null)
          failureCallback(Result.Fail);
      }, onFailure);
      return true;
    }

    public bool FetchScores(RequestType RequestType, int start, int end)
    {
      if (!IsValid || IsQuerying)
        return false;
      client.native.userstats.DownloadLeaderboardEntries(BoardId, (LeaderboardDataRequest) RequestType, start, end, OnScores);
      Results = null;
      IsQuerying = true;
      return true;
    }

    private unsafe void ReadScores(
      LeaderboardScoresDownloaded_t result,
      List<Entry> dest)
    {
      for (int index = 0; index < result.CEntryCount; ++index)
      {
        fixed (int* pDetails = subEntriesBuffer)
        {
          LeaderboardEntry_t pLeaderboardEntry = new LeaderboardEntry_t();
          if (client.native.userstats.GetDownloadedLeaderboardEntry(result.SteamLeaderboardEntries, index, ref pLeaderboardEntry, (IntPtr) pDetails, subEntriesBuffer.Length))
          {
            List<Entry> entryList = dest;
            Entry entry = new Entry {
              GlobalRank = pLeaderboardEntry.GlobalRank,
              Score = pLeaderboardEntry.Score,
              SteamId = pLeaderboardEntry.SteamIDUser,
              SubScores = pLeaderboardEntry.CDetails == 0 ? null : subEntriesBuffer.Take(pLeaderboardEntry.CDetails).ToArray(),
              Name = client.Friends.GetName(pLeaderboardEntry.SteamIDUser),
              AttachedFile = pLeaderboardEntry.UGC >> 32 == uint.MaxValue ? null : new RemoteFile(client.RemoteStorage, pLeaderboardEntry.UGC)
            };
            entryList.Add(entry);
          }
        }
      }
    }

    public bool FetchScores(
      RequestType RequestType,
      int start,
      int end,
      FetchScoresCallback onSuccess,
      FailureCallback onFailure = null)
    {
      if (IsError)
        return false;
      if (!IsValid)
        return DeferOnCreated(() => FetchScores(RequestType, start, end, onSuccess, onFailure), onFailure);
      client.native.userstats.DownloadLeaderboardEntries(BoardId, (LeaderboardDataRequest) RequestType, start, end, (result, error) =>
      {
        if (error)
        {
          FailureCallback failureCallback = onFailure;
          if (failureCallback == null)
            return;
          failureCallback(Result.IOFailure);
        }
        else
        {
          if (_sEntryBuffer == null)
            _sEntryBuffer = new List<Entry>();
          else
            _sEntryBuffer.Clear();
          ReadScores(result, _sEntryBuffer);
          onSuccess(_sEntryBuffer.ToArray());
        }
      });
      return true;
    }

    private void OnScores(LeaderboardScoresDownloaded_t result, bool error)
    {
      IsQuerying = false;
      if (client == null || error)
        return;
      List<Entry> dest = new List<Entry>();
      ReadScores(result, dest);
      Results = dest.ToArray();
    }

    public enum RequestType
    {
      Global,
      GlobalAroundUser,
      Friends,
    }

    public delegate void AddScoreCallback(AddScoreResult result);

    public struct AddScoreResult
    {
      public int Score;
      public bool ScoreChanged;
      public int GlobalRankNew;
      public int GlobalRankPrevious;
    }

    public delegate void AttachRemoteFileCallback();

    public delegate void FetchScoresCallback(Entry[] results);

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
