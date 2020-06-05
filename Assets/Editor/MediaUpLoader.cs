using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.WindowsDevicePortal;
using WebRequestHandlerProxy;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Scripts.DevicePortal;
using System.Linq;

namespace Scripts.ProcessEditor
{
    public class MediaUpLoader : EditorWindow
    {
        public MediaUpLoader() { }
        //string userName = "User Name", passwordToEdit = "<Password>";
        string userName = "holomark", passwordToEdit = "Ptmdxl04~";

        string ipOverUsbAddress = "https://localhost:10080";
        string ipOverWiFiAddress = "https://192.168.0.9";
        string computername = "not set";
        bool useUSB = false;
        DevicePortalWrapper.ConnectInfo connInfo;
        DevicePortalWrapper portal = null;
        System.Uri targetDevice = null;

        // used to display data from the connected device (e.g. hololens);
        string portalMessage = "";
        
        public MediaUpLoader(string pathToResource)
        {
        }

        [MenuItem("Window/Media Manager")]
        public static void ShowWindow()
        {
            EditorWindow win = EditorWindow.GetWindow(typeof(MediaUpLoader));
            win.title = "Media Manager";
        }

        async private void OnGUI()
        {
            GUIStyle Style = EditorStyles.textField;
            Style.alignment = TextAnchor.UpperLeft;
            
            //Displays absolute root path
            EditorGUILayout.SelectableLabel("Manager media", EditorStyles.miniLabel, GUILayout.MaxHeight(16));

            userName = EditorGUILayout.TextField(new GUIContent("User Name"), userName);
            passwordToEdit = EditorGUILayout.PasswordField(new GUIContent("Password"), passwordToEdit);

            CheckConnectionMediumAsync(Style);
            //?????DeviceOsInfo doi = 
            CheckConnect(Style);
            GUIUtility.ExitGUI();
        }
        async System.Threading.Tasks.Task<DeviceOsInfo> CheckConnectionMediumAsync(GUIStyle Style)
        {
            //System.Threading.Tasks.Task<DeviceOsInfo> OsInfoTask = null;
            DeviceOsInfo OsInfo = null;

            useUSB = EditorGUILayout.Toggle("Use USB to connect", useUSB);
            if (useUSB)
            {
                ipOverUsbAddress = EditorGUILayout.TextField(new GUIContent("IP over USB"), ipOverUsbAddress, Style);
                GUILayout.BeginHorizontal("USB");
                if (GUILayout.Button("Connect USB"))
                {
                    DeviceInfo devInf = new DeviceInfo(ipOverUsbAddress, userName, passwordToEdit);
                    //OsInfoTask = DevicePortal.GetDeviceOsInfoAsync(devInf);
                    //OsInfo = await DevicePortal.GetDeviceOsInfoAsync(devInf);
                }
                GUILayout.EndHorizontal();

            }
            else
            {
                ipOverWiFiAddress = EditorGUILayout.TextField(new GUIContent("IP over Wi-Fi"), ipOverWiFiAddress, Style);
                GUILayout.BeginHorizontal("Wi-Fi");
                if (GUILayout.Button("Connect Wi-Fi"))
                {
                    connInfo.IP = ipOverWiFiAddress;
                    connInfo.User = userName;
                    connInfo.Password = passwordToEdit;
                    bool connected = TryOpenDevicePortalConnection(ref targetDevice,  connInfo, out portal);


                    DevicePortalWrapper.AppPackages ias =  await portal.GetInstalledAppPackagesAsync();
                    //ias.Packages.OrderBy(pkg => pkg.FamilyName).
                    //MachineName mn = await DevicePortal.GetMachineNameAsync(devInf);
                    //OsInfo = aaait DevicePortal.GetDeviceOsInfoAsync(devInf);
                    //EditorGUILayout.LabelField(OsInfo.ComputerName, Style);
                    //computername = mn.ComputerName;// OsInfo.ComputerName;
                }

                //EditorGUILayout.LabelField(computername, Style);
                GUILayout.EndHorizontal();
            }
            return OsInfo;
        }

        void CheckConnect(GUIStyle Style)
        {
            // get the order and store...

            GUILayout.BeginHorizontal();
            GUILayout.Label("Get Installed Apps:");
            if (GUILayout.Button("Connect"))
            {

            }
            GUILayout.EndHorizontal();

        }

        void HandlePortalEvents(object sender, DeviceConnectionStatusEventArgs args)
        {
            portalMessage = $"Phase: {args.Phase}, Status: {args.Status}, Message - {args.Message}";
        }

        private bool TryOpenDevicePortalConnection(ref System.Uri targetDevice, DevicePortalWrapper.ConnectInfo connInfo, out DevicePortalWrapper portal)
        {
            bool success = System.Uri.TryCreate(connInfo.IP, System.UriKind.Absolute, out targetDevice);

            portal = new DevicePortalWrapper(new DefaultDevicePortalConnection(targetDevice.ToString(), connInfo.User, connInfo.Password));
            portal.ConnectionStatus += this.HandlePortalEvents;
            try
            {
                // We need to handle this event otherwise remote connection will be rejected if
                // device isn't trusted by local PC
                portal.UnvalidatedCert += DoCertValidation;

                var connectTask = portal.ConnectAsync(updateConnection: false);
                connectTask.Wait();

                if (portal.ConnectionHttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (portal.ConnectionHttpStatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new System.UnauthorizedAccessException("Connection rejected due to missing/incorrect credentials; specify valid credentials with /user and /pwd switches");
                    }
                    else if (!string.IsNullOrEmpty(portal.ConnectionFailedDescription))
                    {
                        throw new System.OperationCanceledException(string.Format("WDP connection failed (HTTP {0}) : {1}", (int)portal.ConnectionHttpStatusCode, portal.ConnectionFailedDescription));
                    }
                    else
                    {
                        throw new System.OperationCanceledException(string.Format("WDP connection failed (HTTP {0}) : no additional information", (int)portal.ConnectionHttpStatusCode));
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to open DevicePortal connection to '" + portal.Address + "'\n" + ex.Message);

                success = false;
            }

            portal.ConnectionStatus -= this.HandlePortalEvents;

            return success;
        }

        private static bool DoCertValidation(DevicePortalWrapper sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // We're not validating the remote host
            return true;
        }
    }
}
