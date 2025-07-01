using System.Text.Json.Serialization;

namespace Aviationexam.MoneyErp.KiotaServices;

[JsonSerializable(typeof(TokenResponse))]
public partial class TokenResponseJsonContext : JsonSerializerContext;
