using System.Security.Cryptography.X509Certificates;

namespace Ultz.SimpleServer.Common
{
    public static class Pem
    {
        public static X509Certificate2 GetCertificate(string cert)
        {
            return new X509Certificate2(CommonExt.GetBytesFromPem(cert, "CERTIFICATE"));
        }

        public static X509Certificate2 GetCertificate(string cert, string key)
        {
            return GetCertificate(cert).CopyWithPrivateKey(
                CommonExt.DecodeRsaPrivateKey(CommonExt.GetBytesFromPem(key,
                    "RSA PRIVATE KEY")));
        }
    }
}