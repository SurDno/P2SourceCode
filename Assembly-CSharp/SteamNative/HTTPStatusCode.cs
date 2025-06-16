namespace SteamNative
{
  internal enum HTTPStatusCode
  {
    Invalid = 0,
    HTTPStatusCode100Continue = 100, // 0x00000064
    HTTPStatusCode101SwitchingProtocols = 101, // 0x00000065
    HTTPStatusCode200OK = 200, // 0x000000C8
    HTTPStatusCode201Created = 201, // 0x000000C9
    HTTPStatusCode202Accepted = 202, // 0x000000CA
    HTTPStatusCode203NonAuthoritative = 203, // 0x000000CB
    HTTPStatusCode204NoContent = 204, // 0x000000CC
    HTTPStatusCode205ResetContent = 205, // 0x000000CD
    HTTPStatusCode206PartialContent = 206, // 0x000000CE
    HTTPStatusCode300MultipleChoices = 300, // 0x0000012C
    HTTPStatusCode301MovedPermanently = 301, // 0x0000012D
    HTTPStatusCode302Found = 302, // 0x0000012E
    HTTPStatusCode303SeeOther = 303, // 0x0000012F
    HTTPStatusCode304NotModified = 304, // 0x00000130
    HTTPStatusCode305UseProxy = 305, // 0x00000131
    HTTPStatusCode307TemporaryRedirect = 307, // 0x00000133
    HTTPStatusCode400BadRequest = 400, // 0x00000190
    HTTPStatusCode401Unauthorized = 401, // 0x00000191
    HTTPStatusCode402PaymentRequired = 402, // 0x00000192
    HTTPStatusCode403Forbidden = 403, // 0x00000193
    HTTPStatusCode404NotFound = 404, // 0x00000194
    HTTPStatusCode405MethodNotAllowed = 405, // 0x00000195
    HTTPStatusCode406NotAcceptable = 406, // 0x00000196
    HTTPStatusCode407ProxyAuthRequired = 407, // 0x00000197
    HTTPStatusCode408RequestTimeout = 408, // 0x00000198
    HTTPStatusCode409Conflict = 409, // 0x00000199
    HTTPStatusCode410Gone = 410, // 0x0000019A
    HTTPStatusCode411LengthRequired = 411, // 0x0000019B
    HTTPStatusCode412PreconditionFailed = 412, // 0x0000019C
    HTTPStatusCode413RequestEntityTooLarge = 413, // 0x0000019D
    HTTPStatusCode414RequestURITooLong = 414, // 0x0000019E
    HTTPStatusCode415UnsupportedMediaType = 415, // 0x0000019F
    HTTPStatusCode416RequestedRangeNotSatisfiable = 416, // 0x000001A0
    HTTPStatusCode417ExpectationFailed = 417, // 0x000001A1
    HTTPStatusCode4xxUnknown = 418, // 0x000001A2
    HTTPStatusCode429TooManyRequests = 429, // 0x000001AD
    HTTPStatusCode500InternalServerError = 500, // 0x000001F4
    HTTPStatusCode501NotImplemented = 501, // 0x000001F5
    HTTPStatusCode502BadGateway = 502, // 0x000001F6
    HTTPStatusCode503ServiceUnavailable = 503, // 0x000001F7
    HTTPStatusCode504GatewayTimeout = 504, // 0x000001F8
    HTTPStatusCode505HTTPVersionNotSupported = 505, // 0x000001F9
    HTTPStatusCode5xxUnknown = 599, // 0x00000257
  }
}
