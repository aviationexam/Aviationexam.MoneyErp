using System;
using System.Security.Cryptography.X509Certificates;

namespace Aviationexam.MoneyErp.Common;

public sealed class PemEndpointCertificateProvider(
    ReadOnlySpan<char> pemCertificate
) : IEndpointCertificateProvider
{
    public void Dispose()
    {
        EndpointCertificate.Dispose();
    }

    public X509Certificate2 EndpointCertificate { get; } = X509Certificate2.CreateFromPem(pemCertificate);
}
