using System.Collections;
using System.Collections.Generic;
using System;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
#if !WINDOWS_UWP
using System.Security.Cryptography.X509Certificates;
#endif // !WINDOWS_UWP
using WRHP = WebRequestHandlerProxy;

namespace Scripts.DevicePortal
{
    public partial class DevicePortalWrapper
    {
        /// <summary>
        /// Issuer for the device certificate.
        /// </summary>
        public static readonly string DevicePortalCertificateIssuer = "Microsoft Windows Web Management";

        /// <summary>
        /// Endpoint used to access the certificate.
        /// </summary>
        public static readonly string RootCertificateEndpoint = "config/rootcertificate";

        /// <summary>
        /// Expected number of OS version sections once the OS version is split by period characters
        /// </summary>
        private static readonly uint ExpectedOSVersionSections = 5;

        /// <summary>
        /// The target OS version section index once the OS version is split by periods 
        /// </summary>
        private static readonly uint TargetOSVersionSection = 3;

        /// <summary>
        /// Device connection object.
        /// </summary>
        private IDevicePortalConnection deviceConnection;

        public enum HttpMethods
        {
            /// <summary>
            /// The HTTP Get method.
            /// </summary>
            Get,

            /// <summary>
            /// The HTTP Put method.
            /// </summary>
            Put,

            /// <summary>
            /// The HTTP Post method.
            /// </summary>
            Post,

            /// <summary>
            /// The HTTP Delete method.
            /// </summary>
            Delete,

            /// <summary>
            /// The HTTP WebSocket method.
            /// </summary>
            WebSocket
        }

        /// <summary>
        /// Gets the device address.
        /// </summary>
        public string Address
        {
            get { return this.deviceConnection.Connection.Authority; }
        }

        /// <summary>
        /// Gets the status code for establishing our connection.
        /// </summary>
        public HttpStatusCode ConnectionHttpStatusCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a description of why the connection failed.
        /// </summary>
        public string ConnectionFailedDescription
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the device operating system family.
        /// </summary>
        public string DeviceFamily
        {
            get
            {
                return (this.deviceConnection.Family != null) ? this.deviceConnection.Family : string.Empty;
            }
        }

        /// <summary>
        /// Gets the operating system version.
        /// </summary>
        public string OperatingSystemVersion
        {
            get
            {
                return (this.deviceConnection.OsInfo != null) ? this.deviceConnection.OsInfo.OsVersionString : string.Empty;
            }
        }


        /// <summary>
        /// Gets the device platform.
        /// </summary>
        public DevicePortalPlatforms Platform
        {
            get
            {
                return (this.deviceConnection.OsInfo != null) ? this.deviceConnection.OsInfo.Platform : DevicePortalPlatforms.Unknown;
            }
        }

        /// <summary>
        /// Gets the device platform name.
        /// </summary>
        public string PlatformName
        {
            get
            {
                return (this.deviceConnection.OsInfo != null) ? this.deviceConnection.OsInfo.PlatformName : "Unknown";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DevicePortalWrapper" /> class.
        /// </summary>
        /// <param name="connection">Implementation of a connection object.</param>
        public DevicePortalWrapper(IDevicePortalConnection connection)
        {
            this.deviceConnection = connection;
        }

        /// <summary>
        /// Handler for reporting connection status.
        /// </summary>
        public event DeviceConnectionStatusEventHandler ConnectionStatus;

        public static readonly string API_FileQuery = @"http://{0}/api/filesystem/apps/file";

        public struct ConnectInfo
        {
            public string IP;
            public string User;
            public string Password;

            public ConnectInfo(string ip, string user, string password)
            {
                IP = ip;
                User = user;
                Password = password;
            }
        }

        public async static void PutFile(ConnectInfo conInfo, string knownFolderID, string packageName, string path, string filePath)
        {
            string query = string.Format(API_FileQuery, conInfo.IP);
            query += "?knowfolderid=" + Uri.EscapeUriString(knownFolderID);
            query += "&packageName=" + Uri.EscapeUriString(packageName);
            query += "&path=" + Uri.EscapeUriString(path);
            var httpRequest = new HttpClient();
            httpRequest.DefaultRequestHeaders.Clear();
            httpRequest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", EncodeToBase64(conInfo.User + ":" + conInfo.Password));

            byte[] data = System.IO.File.ReadAllBytes(filePath);
            ByteArrayContent byteContent = new ByteArrayContent(data);

            HttpResponseMessage resp = await httpRequest.PostAsync(query, byteContent);
            var responseMessage = await resp.Content.ReadAsStringAsync();
        }

        public static string EncodeToBase64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes(toEncode);
            string retval = Convert.ToBase64String(toEncodeAsBytes);
            return retval;
        }

        /// <summary>
        /// Calls the specified API with the provided payload.
        /// </summary>
        /// <typeparam name="T">Return type for the GET call</typeparam>
        /// <param name="apiPath">The relative portion of the uri path that specifies the API to call.</param>
        /// <param name="payload">The query string portion of the uri path that provides the parameterized data.</param>
        /// <returns>An object of the specified type containing the data returned by the request.</returns>
        public async Task<T> GetAsync<T>(
            string apiPath,
            string payload = null) where T : new()
        {
            Uri uri = Utilities.BuildEndpoint(
                this.deviceConnection.Connection,
                apiPath,
                payload);
            WRHP.WebRequest wr = new WRHP.WebRequest();
            int res = wr.Test(9);
            return ReadJsonStream<T>(await wr.GetAsync(uri, this.deviceConnection.Credentials).ConfigureAwait(false));
        }

        public async Task<System.IO.Stream> GetAsync(
            Uri uri)
        {
            WRHP.WebRequest wr = new WRHP.WebRequest();

            return await wr.GetAsync(uri, this.deviceConnection.Credentials);
        }

        /// <summary>.
        /// Connects to the device pointed to by IDevicePortalConnection provided in the constructor.
        /// </summary>
        /// <param name="ssid">Optional network SSID.</param>
        /// <param name="ssidKey">Optional network key.</param>
        /// <param name="updateConnection">Indicates whether we should update this connection's IP address after connecting.</param>
        /// <param name="manualCertificate">A manually provided X509 Certificate for trust validation against this device.</param>
        /// <remarks>Connect sends ConnectionStatus events to indicate the current progress in the connection process.
        /// Some applications may opt to not register for the ConnectionStatus event and await on Connect.</remarks>
        /// <returns>Task for tracking the connect.</returns>
        [method: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "manualCertificate param doesn't really span multiple lines, it just has a different type for UWP and .NET implementations.")]
        public async Task ConnectAsync(
            string ssid = null,
            string ssidKey = null,
            bool updateConnection = false,
#if WINDOWS_UWP
            Certificate manualCertificate = null)
#else
            X509Certificate2 manualCertificate = null)
#endif
        {
#if WINDOWS_UWP
            this.ConnectionHttpStatusCode = HttpStatusCode.Ok;
#else
            this.ConnectionHttpStatusCode = HttpStatusCode.OK;
#endif // WINDOWS_UWP
            string connectionPhaseDescription = string.Empty;

            if (manualCertificate != null)
            {
                this.SetManualCertificate(manualCertificate);
            }

            try
            {
                // Get the device family and operating system information.
                connectionPhaseDescription = "Requesting operating system information";
                this.SendConnectionStatus(
                    DeviceConnectionStatus.Connecting,
                    DeviceConnectionPhase.RequestingOperatingSystemInformation,
                    connectionPhaseDescription);
                this.deviceConnection.Family = await this.GetDeviceFamilyAsync().ConfigureAwait(false);
                this.deviceConnection.OsInfo = await this.GetOperatingSystemInformationAsync().ConfigureAwait(false);
                //this.deviceConnection.Packages = await this.GetInstalledAppPackagesAsync().ConfigureAwait(false);

                // Default to using whatever was specified in the connection.
                bool requiresHttps = this.IsUsingHttps();

                // HoloLens is the only device that supports the GetIsHttpsRequired method.
                if (this.deviceConnection.OsInfo.Platform == DevicePortalPlatforms.HoloLens)
                {
                    // Check to see if HTTPS is required to communicate with this device.
                    connectionPhaseDescription = "Checking secure connection requirements";
                    this.SendConnectionStatus(
                        DeviceConnectionStatus.Connecting,
                        DeviceConnectionPhase.DeterminingConnectionRequirements,
                        connectionPhaseDescription);
                    requiresHttps = await this.GetIsHttpsRequiredAsync().ConfigureAwait(false);
                }

                // Connect the device to the specified network.
                /*if (!string.IsNullOrWhiteSpace(ssid))
                {
                    connectionPhaseDescription = string.Format("Connecting to {0} network", ssid);
                    this.SendConnectionStatus(
                        DeviceConnectionStatus.Connecting,
                        DeviceConnectionPhase.ConnectingToTargetNetwork,
                        connectionPhaseDescription);
                    WifiInterfaces wifiInterfaces = await this.GetWifiInterfacesAsync().ConfigureAwait(false);

                    // TODO - consider what to do if there is more than one wifi interface on a device
                    await this.ConnectToWifiNetworkAsync(wifiInterfaces.Interfaces[0].Guid, ssid, ssidKey).ConfigureAwait(false);
                }*/

                this.SendConnectionStatus(
                    DeviceConnectionStatus.Connected,
                    DeviceConnectionPhase.Idle,
                    "Device connection established");
            }
            catch (Exception e)
            {
                DevicePortalException dpe = e as DevicePortalException;

                if (dpe != null)
                {
                    this.ConnectionHttpStatusCode = dpe.StatusCode;
                    this.ConnectionFailedDescription = dpe.Message;
                }
                else
                {
                    this.ConnectionHttpStatusCode = HttpStatusCode.Conflict;

                    // Get to the innermost exception for our return message.
                    Exception innermostException = e;
                    while (innermostException.InnerException != null)
                    {
                        innermostException = innermostException.InnerException;
                        await Task.Yield();
                    }

                    this.ConnectionFailedDescription = innermostException.Message;
                }

                this.SendConnectionStatus(
                    DeviceConnectionStatus.Failed,
                    DeviceConnectionPhase.Idle,
                    string.Format("Device connection failed: {0}, {1}", connectionPhaseDescription, this.ConnectionFailedDescription));
            }
        }

        /// <summary>
        /// Sends the connection status back to the caller
        /// </summary>
        /// <param name="status">Status of the connect attempt.</param>
        /// <param name="phase">Current phase of the connection attempt.</param>
        /// <param name="message">Optional message describing the connection status.</param>
        private void SendConnectionStatus(
            DeviceConnectionStatus status,
            DeviceConnectionPhase phase,
            string message = "")
        {
            this.ConnectionStatus?.Invoke(this, new DeviceConnectionStatusEventArgs(status, phase, message));
        }

        /// <summary>
        /// Helper method to determine if our connection is using HTTPS
        /// </summary>
        /// <returns>Whether we are using HTTPS</returns>
        private bool IsUsingHttps()
        {
            return this.deviceConnection.Connection.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase);
        }
    }       
}
    
