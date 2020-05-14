//----------------------------------------------------------------------------------------------
// <copyright file="AppDeployment.cs" company="Microsoft Corporation">
//     Licensed under the MIT License. See LICENSE.TXT in the project root license information.
// </copyright>
//----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
#if !WINDOWS_UWP
using System.Net;
using System.Net.Http;
#endif // !WINDOWS_UWP
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using static Scripts.DevicePortal.DevicePortalException;
#if WINDOWS_UWP
using Windows.Foundation;
using Windows.Security.Credentials;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
#endif

namespace Scripts.DevicePortal
{

        /// <content>
        /// Wrappers for App Deployment methods.
        /// </content>
        public partial class DevicePortalWrapper
    { 
        /// <summary>
        /// API to retrieve list of installed packages.
        /// </summary>
        public static readonly string InstalledPackagesApi = "api/app/packagemanager/packages";

        /// <summary>
        /// API for package management.
        /// </summary>
        public static readonly string PackageManagerApi = "api/app/packagemanager/package";


        /// <summary>
        /// Gets the collection of applications installed on the device.
        /// </summary>
        /// <returns>AppPackages object containing the list of installed application packages.</returns>
        public async Task<AppPackages> GetInstalledAppPackagesAsync()
        {
            return await this.GetAsync<AppPackages>(InstalledPackagesApi);
        }

        

        #region Data contract
        /// <summary>
        /// Object representing a list of Application Packages
        /// </summary>
        [DataContract]
        public class AppPackages
        {
            /// <summary>
            /// Gets a list of the packages
            /// </summary>
            [DataMember(Name = "InstalledPackages")]
            public List<PackageInfo> Packages { get; private set; }

            /// <summary>
            /// Presents a user readable representation of a list of AppPackages
            /// </summary>
            /// <returns>User readable list of AppPackages.</returns>
            public override string ToString()
            {
                string output = "Packages:\n";
                foreach (PackageInfo package in this.Packages)
                {
                    output += package;
                }

                return output;
            }
        }

        /// <summary>
        /// Object representing the install state
        /// </summary>
        [DataContract]
        public class InstallState
        {
            /// <summary>
            /// Gets install state code
            /// </summary>
            [DataMember(Name = "Code")]
            public int Code { get; private set; }

            /// <summary>
            /// Gets message text
            /// </summary>
            [DataMember(Name = "CodeText")]
            public string CodeText { get; private set; }

            /// <summary>
            /// Gets reason for state
            /// </summary>
            [DataMember(Name = "Reason")]
            public string Reason { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this was successful
            /// </summary>
            [DataMember(Name = "Success")]
            public bool WasSuccessful { get; private set; }
        }

        /// <summary>
        /// object representing the package information
        /// </summary>
        [DataContract]
        public class PackageInfo
        {
            /// <summary>
            /// Gets package name
            /// </summary>
            [DataMember(Name = "Name")]
            public string Name { get; private set; }

            /// <summary>
            /// Gets package family name
            /// </summary>
            [DataMember(Name = "PackageFamilyName")]
            public string FamilyName { get; private set; }

            /// <summary>
            /// Gets package full name
            /// </summary>
            [DataMember(Name = "PackageFullName")]
            public string FullName { get; private set; }

            /// <summary>
            /// Gets package relative Id
            /// </summary>
            [DataMember(Name = "PackageRelativeId")]
            public string AppId { get; private set; }

            /// <summary>
            /// Gets package publisher
            /// </summary>
            [DataMember(Name = "Publisher")]
            public string Publisher { get; private set; }

            /// <summary>
            /// Gets package version
            /// </summary>
            [DataMember(Name = "Version")]
            public PackageVersion Version { get; private set; }

            /// <summary>
            /// Gets package origin, a measure of how the app was installed. 
            /// PackageOrigin_Unknown            = 0,
            /// PackageOrigin_Unsigned           = 1,
            /// PackageOrigin_Inbox              = 2,
            /// PackageOrigin_Store              = 3,
            /// PackageOrigin_DeveloperUnsigned  = 4,
            /// PackageOrigin_DeveloperSigned    = 5,
            /// PackageOrigin_LineOfBusiness     = 6
            /// </summary>
            [DataMember(Name = "PackageOrigin")]
            public int PackageOrigin { get; private set; }

            /// <summary>
            /// Helper method to determine if the app was sideloaded and therefore can be used with e.g. GetFolderContentsAsync
            /// </summary>
            /// <returns> True if the package is sideloaded. </returns>
            public bool IsSideloaded()
            {
                return this.PackageOrigin == 4 || this.PackageOrigin == 5;
            }
            
            /// <summary>
            /// Get a string representation of the package
            /// </summary>
            /// <returns>String representation</returns>
            public override string ToString()
            {
                return string.Format("\t{0}\n\t\t{1}\n", this.FullName, this.AppId);
            }
        }

        /// <summary>
        /// Object representing a package version
        /// </summary>
        [DataContract]
        public class PackageVersion
        {
            /// <summary>
            ///  Gets version build
            /// </summary>
            [DataMember(Name = "Build")]
            public int Build { get; private set; }

            /// <summary>
            /// Gets package Major number
            /// </summary>
            [DataMember(Name = "Major")]
            public int Major { get; private set; }

            /// <summary>
            /// Gets package minor number
            /// </summary>
            [DataMember(Name = "Minor")]
            public int Minor { get; private set; }

            /// <summary>
            /// Gets package revision
            /// </summary>
            [DataMember(Name = "Revision")]
            public int Revision { get; private set; }

            /// <summary>
            /// Gets package version
            /// </summary>
            public Version Version
            {
                get { return new Version(this.Major, this.Minor, this.Build, this.Revision); }
            }

            /// <summary>
            /// Get a string representation of a version
            /// </summary>
            /// <returns>String representation</returns>
            public override string ToString()
            {
                return Version.ToString();
            }
        }
        #endregion // Data contract
    }
}
