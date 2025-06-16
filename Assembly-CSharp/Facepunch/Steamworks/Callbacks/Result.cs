// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Callbacks.Result
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace Facepunch.Steamworks.Callbacks
{
  public enum Result
  {
    OK = 1,
    Fail = 2,
    NoConnection = 3,
    InvalidPassword = 5,
    LoggedInElsewhere = 6,
    InvalidProtocolVer = 7,
    InvalidParam = 8,
    FileNotFound = 9,
    Busy = 10, // 0x0000000A
    InvalidState = 11, // 0x0000000B
    InvalidName = 12, // 0x0000000C
    InvalidEmail = 13, // 0x0000000D
    DuplicateName = 14, // 0x0000000E
    AccessDenied = 15, // 0x0000000F
    Timeout = 16, // 0x00000010
    Banned = 17, // 0x00000011
    AccountNotFound = 18, // 0x00000012
    InvalidSteamID = 19, // 0x00000013
    ServiceUnavailable = 20, // 0x00000014
    NotLoggedOn = 21, // 0x00000015
    Pending = 22, // 0x00000016
    EncryptionFailure = 23, // 0x00000017
    InsufficientPrivilege = 24, // 0x00000018
    LimitExceeded = 25, // 0x00000019
    Revoked = 26, // 0x0000001A
    Expired = 27, // 0x0000001B
    AlreadyRedeemed = 28, // 0x0000001C
    DuplicateRequest = 29, // 0x0000001D
    AlreadyOwned = 30, // 0x0000001E
    IPNotFound = 31, // 0x0000001F
    PersistFailed = 32, // 0x00000020
    LockingFailed = 33, // 0x00000021
    LogonSessionReplaced = 34, // 0x00000022
    ConnectFailed = 35, // 0x00000023
    HandshakeFailed = 36, // 0x00000024
    IOFailure = 37, // 0x00000025
    RemoteDisconnect = 38, // 0x00000026
    ShoppingCartNotFound = 39, // 0x00000027
    Blocked = 40, // 0x00000028
    Ignored = 41, // 0x00000029
    NoMatch = 42, // 0x0000002A
    AccountDisabled = 43, // 0x0000002B
    ServiceReadOnly = 44, // 0x0000002C
    AccountNotFeatured = 45, // 0x0000002D
    AdministratorOK = 46, // 0x0000002E
    ContentVersion = 47, // 0x0000002F
    TryAnotherCM = 48, // 0x00000030
    PasswordRequiredToKickSession = 49, // 0x00000031
    AlreadyLoggedInElsewhere = 50, // 0x00000032
    Suspended = 51, // 0x00000033
    Cancelled = 52, // 0x00000034
    DataCorruption = 53, // 0x00000035
    DiskFull = 54, // 0x00000036
    RemoteCallFailed = 55, // 0x00000037
    PasswordUnset = 56, // 0x00000038
    ExternalAccountUnlinked = 57, // 0x00000039
    PSNTicketInvalid = 58, // 0x0000003A
    ExternalAccountAlreadyLinked = 59, // 0x0000003B
    RemoteFileConflict = 60, // 0x0000003C
    IllegalPassword = 61, // 0x0000003D
    SameAsPreviousValue = 62, // 0x0000003E
    AccountLogonDenied = 63, // 0x0000003F
    CannotUseOldPassword = 64, // 0x00000040
    InvalidLoginAuthCode = 65, // 0x00000041
    AccountLogonDeniedNoMail = 66, // 0x00000042
    HardwareNotCapableOfIPT = 67, // 0x00000043
    IPTInitError = 68, // 0x00000044
    ParentalControlRestricted = 69, // 0x00000045
    FacebookQueryError = 70, // 0x00000046
    ExpiredLoginAuthCode = 71, // 0x00000047
    IPLoginRestrictionFailed = 72, // 0x00000048
    AccountLockedDown = 73, // 0x00000049
    AccountLogonDeniedVerifiedEmailRequired = 74, // 0x0000004A
    NoMatchingURL = 75, // 0x0000004B
    BadResponse = 76, // 0x0000004C
    RequirePasswordReEntry = 77, // 0x0000004D
    ValueOutOfRange = 78, // 0x0000004E
    UnexpectedError = 79, // 0x0000004F
    Disabled = 80, // 0x00000050
    InvalidCEGSubmission = 81, // 0x00000051
    RestrictedDevice = 82, // 0x00000052
    RegionLocked = 83, // 0x00000053
    RateLimitExceeded = 84, // 0x00000054
    AccountLoginDeniedNeedTwoFactor = 85, // 0x00000055
    ItemDeleted = 86, // 0x00000056
    AccountLoginDeniedThrottle = 87, // 0x00000057
    TwoFactorCodeMismatch = 88, // 0x00000058
    TwoFactorActivationCodeMismatch = 89, // 0x00000059
    AccountAssociatedToMultiplePartners = 90, // 0x0000005A
    NotModified = 91, // 0x0000005B
    NoMobileDevice = 92, // 0x0000005C
    TimeNotSynced = 93, // 0x0000005D
    SmsCodeFailed = 94, // 0x0000005E
    AccountLimitExceeded = 95, // 0x0000005F
    AccountActivityLimitExceeded = 96, // 0x00000060
    PhoneActivityLimitExceeded = 97, // 0x00000061
    RefundToWallet = 98, // 0x00000062
    EmailSendFailure = 99, // 0x00000063
    NotSettled = 100, // 0x00000064
    NeedCaptcha = 101, // 0x00000065
    GSLTDenied = 102, // 0x00000066
    GSOwnerDenied = 103, // 0x00000067
    InvalidItemType = 104, // 0x00000068
    IPBanned = 105, // 0x00000069
  }
}
