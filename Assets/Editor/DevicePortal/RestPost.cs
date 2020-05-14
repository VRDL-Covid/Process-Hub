//----------------------------------------------------------------------------------------------
// <copyright file="RestPost.cs" company="Microsoft Corporation">
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
    /// .net 4.x implementation of HTTP PostAsync
    /// </content>
    public partial class DevicePortalWrapper
    {
        /// <summary>
        /// Submits the http post request to the specified uri.
        /// </summary>
        /// <param name="uri">The uri to which the post request will be issued.</param>
        /// <param name="requestStream">Optional stream containing data for the request body.</param>
        /// <param name="requestStreamContentType">The type of that request body data.</param>
        /// <returns>Task tracking the completion of the POST request</returns>
        public async Task<Stream> PostAsync(
            Uri uri,
            Stream requestStream = null,
            string requestStreamContentType = null)
        {
            StreamContent requestContent = null;

            if (requestStream != null)
            {
                requestContent = new StreamContent(requestStream);
                requestContent.Headers.Remove(ContentTypeHeaderName);
                requestContent.Headers.TryAddWithoutValidation(ContentTypeHeaderName, requestStreamContentType);
            }

            return await this.PostAsync(uri, requestContent);
        }

        /// <summary>
        /// Submits the http post request to the specified uri.
        /// </summary>
        /// <param name="uri">The uri to which the post request will be issued.</param>
        /// <param name="requestContent">Optional content containing data for the request body.</param>
        /// <returns>Task tracking the completion of the POST request</returns>
        public async Task<Stream> PostAsync(
            Uri uri,
            HttpContent requestContent)
        {
            MemoryStream responseDataStream = null;

            WRHP.WebRequest wr = new WRHP.WebRequest();

            responseDataStream = await wr.PostAsync(uri, requestContent, this.deviceConnection.Credentials) as System.IO.MemoryStream;

            return responseDataStream;
        }

    }
}
