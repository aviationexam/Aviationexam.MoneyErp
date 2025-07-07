using System.Text.Json.Serialization;

namespace Aviationexam.MoneyErp.Common.Oidc;

[JsonSerializable(typeof(TokenResponse))]
public partial class TokenResponseJsonContext : JsonSerializerContext;
