using System.Security.Cryptography.X509Certificates;

namespace Aviationexam.MoneyErp.Common;

public sealed class DefaultEndpointCertificateProvider : IEndpointCertificateProvider
{
    public void Dispose()
    {
    }

    public X509Certificate2? EndpointCertificate => null;
}
