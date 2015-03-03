using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace MobileId
{
    //    public class WebClientConfig : ConfigurationSection (does not work)

    /// <summary>
    /// Encapsulation of all configuration parameters needed by WebClientImpl
    /// </summary>
    public class WebClientConfig
    {
        // mandatory input from caller
        string _apId = null;
        string _sslCertThumbprint = null;
        string _serviceUrlPrefix = null;

        // optional input from caller
        StoreLocation _sslKeyStore = StoreLocation.CurrentUser;
        UserLanguage _userLanguageDefault = UserLanguage.en;
        // string _dtbsPrefix = null;
        bool _srvSideValidation = true;
        int _requestTimeOutSeconds = 80;
        string _sslCaCertDN = "CN=Swisscom TEST Root CA 2, OU=Digital Certificate Services, O=Swisscom, C=ch";
        string _seedApTransId = "Some ASCII text to be used to build the unique AP_TransId in request";
        bool _enableSubscriberInfo = false;

        //// not used. doesn't work
        ///// <summary>
        ///// Build a configuration from NameValueCollection, such that from a section in app.config file.
        ///// </summary>
        ///// <param name="appCfg"></param>
        //public WebClientConfig(NameValueCollection appCfg)
        //{
        //    Trace.Assert(appCfg != null, "appCfg is null");
        //    // if (appCfg == null) throw new ArgumentNullException("appCfg is null");
        //    this.ApId = appCfg["ApId"];
        //}

        public static WebClientConfig CreateConfigFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("config fileName is null or empty");
                // TODO: construct a default filename
            }
            WebClientConfig cfg = new WebClientConfig();

            XmlReaderSettings xmlSetting = new XmlReaderSettings();
            xmlSetting.CloseInput = true;
            xmlSetting.IgnoreProcessingInstructions = true;
            xmlSetting.IgnoreWhitespace = true;
            using (XmlReader xml = XmlReader.Create(fileName, xmlSetting))
            {
                String s;
                while (xml.Read())
                {
                    // we process only the <mobileIdClient .../> element
                    if (xml.Name == "mobileIdClient")
                    {
                        cfg.ApId = xml["AP_ID"];
                        // cfg.DtbsPrefix = xml["DtbsPrefix"];
                        if (! string.IsNullOrEmpty(s = xml["RequestTimeOutSeconds"]))
                            cfg.RequestTimeOutSeconds = int.Parse(s);
                        cfg.ServiceUrlPrefix = xml["ServiceUrlPrefix"];
                        if (! string.IsNullOrEmpty(s = xml["SrvSideValidation"]))
                            cfg.SrvSideValidation = bool.Parse(s);
                        cfg.SslCertThumbprint = xml["SslCertThumbprint"];
                        if (! string.IsNullOrEmpty(s = xml["SslKeystore"]))
                            cfg.SslKeystore = Util.ParseKeyStoreLocation(s);
                        if (! string.IsNullOrEmpty(s = xml["SslRootCaCertDN"]))
                            cfg.SslRootCaCertDN = s;
                        if (! string.IsNullOrEmpty(s = xml["EnableSubscriberInfo"]))
                            cfg.EnableSubscriberInfo = Boolean.Parse(s);
                        break;
                    }
                }
                xml.Close();
            }

            return cfg;
        }

        // TODO: input validation of config parameters

        /// <summary>
        /// AP_ID of the client, s. Reference Guide of Mobile ID
        /// </summary>
        [ConfigurationProperty("ApId", IsRequired = true)]
        public string ApId { 
            get {
                return _apId;
            } 
            set {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("ApId is null or empty");
                if (! Util.IsXmlSafe(value))
                    throw new ArgumentException("ApId contains bad chars");
                _apId = value;
            }
        }

        public UserLanguage UserLanguageDefault 
        { 
            get { return _userLanguageDefault; }
            set { _userLanguageDefault = value; } 
        }

        ///// <summary>
        ///// Determines how verbose is the logging is. Can be 0 (only errors are logged), 1 (warning, error), 2 (info, warning, error), 3 (all messages are logged)
        ///// Default is 2.
        ///// </summary>
        //[ConfigurationProperty("LogVerbosity", IsRequired = false, DefaultValue = 2)]
        //public int LogVerbosity
        //{ 
        //    get { return _logVerbosity; }
        //    set { 
        //        int new_value = (value < 0) ? 0 : (value > 3) ? 3 : value ;
        //        if (_logVerbosity != new_value) {
        //            _logVerbosity = new_value;
        //            Log.SetLogVerbosity(_logVerbosity);
        //        }
        //    }
        //}

        [ConfigurationProperty("SslKeystore", IsRequired = false, DefaultValue = "CurrentUser")]
        public StoreLocation SslKeystore
        { 
            get {return _sslKeyStore;}
            set {
                switch (value.ToString())
                {
                    case "CurrentUser" : _sslKeyStore = StoreLocation.CurrentUser; break;
                    case "LocalMachine" : _sslKeyStore = StoreLocation.LocalMachine; break;
                    default: throw new ArgumentOutOfRangeException("SslKeystore is neither 'CurrentUser' nor 'LocalMachine'");
                }
            }
        }

        [ConfigurationProperty("SslCertThumbprint", IsRequired = true, DefaultValue = "CurrentUser")]
        public string SslCertThumbprint {
            get { return _sslCertThumbprint; }
            set { _sslCertThumbprint = value; }
        }

        /// <summary>
        /// Distinguished Name of Root CA Certificate in the CA Chain of the SSL Server Certificate for Mobile ID Service
        /// </summary>
        [ConfigurationProperty("SslRootCaCertDN", IsRequired = true)]
        public string SslRootCaCertDN {
            get { return _sslCaCertDN; }
            set { _sslCaCertDN = value;  }
        }

        public string ServiceUrlPrefix {
            get { return _serviceUrlPrefix; }
            set { _serviceUrlPrefix = value;}
        }

        //public string DtbsPrefix { 
        //    get { return _dtbsPrefix; }
        //    set { _dtbsPrefix = value; }
        //}

        public bool SrvSideValidation { 
            get { return _srvSideValidation; }
            set { _srvSideValidation = value; }
        }

        public string SeedApTransId {
            get { return _seedApTransId; }
            set { _seedApTransId = value; }
        }

        public bool EnableSubscriberInfo { 
            get { return _enableSubscriberInfo;}
            set { _enableSubscriberInfo = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(192);
            // sorted alphabetically in name
            sb.Append("ApId=").Append(_apId);
            // sb.Append(", DtbsPrefix=").Append(_dtbsPrefix);
            sb.Append(", EnabledSubscriberInfo=").Append(_enableSubscriberInfo);
            sb.Append(", RequestTimeOutSeconds=").Append(_requestTimeOutSeconds);
            sb.Append(", SeedApTransId=").Append(_seedApTransId);
            sb.Append(", ServiceUrlPrefix=").Append(_serviceUrlPrefix);
            sb.Append(", SrvSideValidation=").Append(_srvSideValidation);
            sb.Append(", SslKeystore=").Append(_sslKeyStore);
            sb.Append(", SslCertThumbprint=").Append(_sslCertThumbprint);
            sb.Append(", SslRootCaCertDN=").Append(_sslCaCertDN);
            sb.Append(", UserLanguageDefault=").Append(_userLanguageDefault);
            return sb.ToString();

        }

        public int RequestTimeOutSeconds {
            get { return _requestTimeOutSeconds; }
            set { _requestTimeOutSeconds = value; }
        }
    }
}
