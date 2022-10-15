using System;
using System.Text;

using Microsoft.AspNetCore.Http;


namespace AS4.ParserService.Infrastructure
{
    internal static class CertificateInfoRetriever
    {
        internal static CertificatePwdInformation RetrieveCertificatePassword(HttpRequest request)
        {
            if (request.Headers.Authorization.Count == 0)
            {
                return null;
            }

            try
            {
                string basicParameter = Encoding.UTF8.GetString(Convert.FromBase64String(request.Headers.Authorization[0]));

                int index = basicParameter.IndexOf(':');

                if (index == -1)
                {
                    return null;
                }

                return Newtonsoft.Json.JsonConvert.DeserializeObject<CertificatePwdInformation>(basicParameter.Substring(index + 1));
            }
            catch (Exception)
            {
                // TODO: log what went wrong.
                return null;
            }
        }
    }

    internal class CertificatePwdInformation
    {
        public CertificatePwdInformation(string signingPwd, string decryptPwd)
        {
            SigningPassword = signingPwd;
            DecryptionPassword = decryptPwd;
        }

        public string SigningPassword { get; set; }
        public string DecryptionPassword { get; set; }
    }
}