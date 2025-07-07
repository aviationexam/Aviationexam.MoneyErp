using Microsoft.Kiota.Abstractions;
using System.Runtime.CompilerServices;

namespace Aviationexam.MoneyErp.RestApi.Extensions;

public static class BaseRequestBuilderExtensions
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_RequestAdapter")]
    public static extern IRequestAdapter GetRequestAdapter(
        BaseRequestBuilder requestBuilder
    );
}
