using Aviationexam.MoneyErp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Aviationexam.MoneyErp.KiotaServices;

public sealed class DefaultAuthenticationProvider(
    [FromKeyedServices(DependencyInjectionExtensions.MoneyErpServiceKey)]
    IAccessTokenProvider accessTokenProvider
) : BaseBearerTokenAuthenticationProvider(accessTokenProvider);
