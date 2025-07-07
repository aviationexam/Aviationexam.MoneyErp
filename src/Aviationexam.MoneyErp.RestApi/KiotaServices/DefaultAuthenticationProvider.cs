using Aviationexam.MoneyErp.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Aviationexam.MoneyErp.RestApi.KiotaServices;

public sealed class DefaultAuthenticationProvider(
    [FromKeyedServices(CommonDependencyInjectionExtensions.MoneyErpServiceKey)]
    IAccessTokenProvider accessTokenProvider
) : BaseBearerTokenAuthenticationProvider(accessTokenProvider);
