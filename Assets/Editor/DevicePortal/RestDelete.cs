//----------------------------------------------------------------------------------------------
// <copyright file="RestDelete.cs" company="Microsoft Corporation">
//     Licensed under the MIT License. See LICENSE.TXT in the project root license information.
// </copyright>
//----------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WRHP = WebRequestHandlerProxy;

namespace Scripts.DevicePortal
{
    /// <content>
    /// .net 4.x implementation of HTTP DeleteAsync
    /// </content>
    public partial class DevicePortalWrapper
    {
        /// <summary>
        /// Submits the http delete request to the specified uri.
        /// </summary>
        /// <param name="uri">The uri to which the delete request will be issued.</param>
        /// <returns>Task tracking HTTP completion</returns>
        public async Task<Stream> DeleteAsync(Uri uri)
        {
            MemoryStream dataStream = null;
            WRHP.WebRequest wr = new WRHP.WebRequest();

            dataStream = await wr.DeleteAsync(uri, this.deviceConnection.Credentials) as System.IO.MemoryStream;

            return dataStream;
        }
    }
}
