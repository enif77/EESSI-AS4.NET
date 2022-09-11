using System;
using System.Globalization;
//using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Eu.EDelivery.AS4.Repositories;

//using Newtonsoft.Json.Linq;

//using Org.BouncyCastle.Utilities;


namespace Eu.EDelivery.AS4.Security.References
{
    /// <summary>
    /// Security Token Reference Strategy for the Key Identifier
    /// </summary>
    internal sealed class KeyIdentifierSecurityTokenReference : SecurityTokenReference
    {
        private readonly ICertificateRepository _certificateRepository;
        private string _certificateSubjectKeyIdentifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyIdentifierSecurityTokenReference"/> class. 
        /// </summary>
        /// <param name="certificate">The Certificate for which a SecurityTokenReference must be created.</param>
        public KeyIdentifierSecurityTokenReference(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            Certificate = certificate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyIdentifierSecurityTokenReference"/> class.
        /// </summary>
        /// <param name="envelope">XML Element that contains a Key Identifier Security Token Reference.</param>
        /// <param name="certificateRepository">Repository to obtain the certificate needed to embed it into the Key Identifier Security Token Reference.</param>
        public KeyIdentifierSecurityTokenReference(XmlElement envelope, ICertificateRepository certificateRepository)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            if (certificateRepository == null)
            {
                throw new ArgumentNullException(nameof(certificateRepository));
            }

            _certificateRepository = certificateRepository;
            LoadXml(envelope);
        }

        protected override X509Certificate2 LoadCertificate()
        {
            if (String.IsNullOrWhiteSpace(_certificateSubjectKeyIdentifier))
            {
                throw new InvalidOperationException("Unable to retrieve Certificate: No SubjectKeyIdentifier available.");
            }

            if (_certificateRepository == null)
            {
                throw new InvalidOperationException("Unable to retrieve Certificate: No CertificateRepository defined.");
            }

            return _certificateRepository.GetCertificate(
                X509FindType.FindBySubjectKeyIdentifier, 
                _certificateSubjectKeyIdentifier);
        }

        /// <summary>
        /// Load the <see cref="X509Certificate2" />
        /// from the given <paramref name="element" />
        /// </summary>
        /// <param name="element"></param>
        public override void LoadXml(XmlElement element)
        {
            var ns = new XmlNamespaceManager(new NameTable());
            ns.AddNamespace("wsse", Constants.Namespaces.WssSecuritySecExt);

            var xmlKeyIdentifier = element.SelectSingleNode("//wsse:SecurityTokenReference/wsse:KeyIdentifier", ns) as XmlElement;
            if (xmlKeyIdentifier == null)
            {
                throw new XmlException(
                    "No <wsse:KeyIdentifier/> element found in <wsse:SecurityTokenReference/> element");
            }

            byte[] base64Bytes = Convert.FromBase64String(xmlKeyIdentifier.InnerText);
            //var soapHexBinary = new SoapHexBinary(base64Bytes);

            _certificateSubjectKeyIdentifier = ToSoapHexString(base64Bytes);
        }


        // https://github.com/mono/mono/blob/main/mcs/class/corlib/System.Runtime.Remoting.Metadata.W3cXsd2001/SoapHexBinary.cs
        private string ToSoapHexString(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length);

            foreach (byte b in bytes)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        /// <summary>
        /// Get the Xml for the Key Identifier
        /// </summary>
        /// <returns></returns>
        public override XmlElement GetXml()
        {
            var xmlDocument = new XmlDocument { PreserveWhitespace = true };

            XmlElement securityTokenReferenceElement = xmlDocument.CreateElement(
                prefix: "wsse",
                localName: "SecurityTokenReference",
                namespaceURI: Constants.Namespaces.WssSecuritySecExt);

            XmlElement keyIdentifierElement = xmlDocument.CreateElement(
                prefix: "wsse",
                localName: "KeyIdentifier",
                namespaceURI: Constants.Namespaces.WssSecuritySecExt);

            keyIdentifierElement.SetAttribute("EncodingType", Constants.Namespaces.Base64Binary);
            keyIdentifierElement.SetAttribute("ValueType", Constants.Namespaces.SubjectKeyIdentifier);
            keyIdentifierElement.InnerText = GetSubjectKeyIdentifier();

            securityTokenReferenceElement.AppendChild(keyIdentifierElement);
            return securityTokenReferenceElement;
        }

        private string GetSubjectKeyIdentifier()
        {
            if (!String.IsNullOrWhiteSpace(_certificateSubjectKeyIdentifier))
            {
                return _certificateSubjectKeyIdentifier;
            }

            foreach (X509Extension extension in Certificate.Extensions)
            {
                if (!String.Equals(extension.Oid.FriendlyName, "Subject Key Identifier"))
                {
                    continue;
                }

                var x509SubjectKeyIdentifierExtension = (X509SubjectKeyIdentifierExtension)extension;
                //SoapHexBinary base64Binary = SoapHexBinary.Parse(x509SubjectKeyIdentifierExtension.SubjectKeyIdentifier);

                //return Convert.ToBase64String(base64Binary.Value);
                return Convert.ToBase64String(SoapHexBinaryFromBinHexString(x509SubjectKeyIdentifierExtension.SubjectKeyIdentifier));
            }

            throw new CryptographicException(
                "No extension with the name 'Subject Key Identifier' was found in the certificate extensions");
        }


        // https://github.com/mono/mono/blob/main/mcs/class/corlib/System.Runtime.Remoting.Metadata.W3cXsd2001/SoapHexBinary.cs
        internal static byte[] SoapHexBinaryFromBinHexString(string value)
        {
            char[] chars = value.ToCharArray();
            byte[] buffer = new byte[chars.Length / 2 + chars.Length % 2];
            int charLength = chars.Length;

            if (charLength % 2 != 0)
                throw CreateInvalidValueException(value);

            int bufIndex = 0;
            for (int i = 0; i < charLength - 1; i += 2)
            {
                buffer[bufIndex] = FromHex(chars[i], value);
                buffer[bufIndex] <<= 4;
                buffer[bufIndex] += FromHex(chars[i + 1], value);
                bufIndex++;
            }

            return buffer;
        }

        static byte FromHex(char hexDigit, string value)
        {
            try
            {
                return byte.Parse(hexDigit.ToString(),
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw CreateInvalidValueException(value);
            }
        }


        static Exception CreateInvalidValueException(string value)
        {
            //return new RemotingException(string.Format(
            return new Exception(string.Format(
                CultureInfo.InvariantCulture,
                "Invalid value '{0}' for xsd:hexBinary.",
                value));
        }
    }
}