using System;
using System.Security.Cryptography.X509Certificates;

namespace Aviationexam.MoneyErp.Common;

public interface IEndpointCertificateProvider : IDisposable
{
    X509Certificate2? EndpointCertificate { get; }
}
