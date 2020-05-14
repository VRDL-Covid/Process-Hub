//----------------------------------------------------------------------------------------------
// <copyright file="RestPut.cs" company="Microsoft Corporation">
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
    /// .net 4.x implementation of HTTP PutAsync
    /// </content>
    public partial class DevicePortalWrapper
    {
        /// <summary>
        /// Submits the http put request to the specified uri.
        /// </summary>
        /// <param name="uri">The uri to which the put request will be issued.</param>
        /// <param name="body">The HTTP content comprising the body of the request.</param>
        /// <returns>Task tracking the PUT completion.</returns>
        public async Task<Stream> PutAsync(Uri uri, HttpContent body = null)
        {
            MemoryStream dataStream = null;
            WRHP.WebRequest wr = new WRHP.WebRequest();
            dataStream = await wr.PostAsync(uri, body, this.deviceConnection.Credentials) as System.IO.MemoryStream;
            return dataStream;
        }
    }
}
