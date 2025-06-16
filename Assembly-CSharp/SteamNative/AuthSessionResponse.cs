﻿namespace SteamNative
{
  internal enum AuthSessionResponse
  {
    OK,
    UserNotConnectedToSteam,
    NoLicenseOrExpired,
    VACBanned,
    LoggedInElseWhere,
    VACCheckTimedOut,
    AuthTicketCanceled,
    AuthTicketInvalidAlreadyUsed,
    AuthTicketInvalid,
    PublisherIssuedBan,
  }
}
