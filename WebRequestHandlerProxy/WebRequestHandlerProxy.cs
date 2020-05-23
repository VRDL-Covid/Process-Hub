using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace WebRequestHandlerProxy
{
    public class WebRequest
    {
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
        /// A manually provided certificate for trust validation.
        /// </summary>
        private X509Certificate2 manualCertificate = null;

        public int Test(int val)
        {
            int x = val;
            return x;
        }
        /// <summary>
        /// Handler for when an unvalidated cert is received.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="certificate">The server's certificate.</param>
        /// <param name="chain">The cert chain.</param>
        /// <param name="sslPolicyErrors">Policy Errors.</param>
        /// <returns>whether the cert should still pass validation.</returns>
        public delegate bool UnvalidatedCertEventHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors);
        
        /// <summary>
        /// Gets or sets handler for untrusted certificate handling
        /// </summary>
        public event UnvalidatedCertEventHandler UnvalidatedCert;

        public async Task<Stream> GetAsync(Uri uri, System.Net.NetworkCredential Credentials)
        {
            MemoryStream dataStream = null;
            HttpHeadersHelper headerHelper = new HttpHeadersHelper();
            WebRequestHandler handler = new WebRequestHandler();
            handler.UseDefaultCredentials = false;
            handler.Credentials = Credentials;
            //handler.ServerCertificateValidationCallback = this.ServerCertificateValidation;

            using (HttpClient client = new HttpClient(handler))
            {
                //headerHelper.ApplyHttpHeaders(client, HttpMethods.Get);

                using (HttpResponseMessage response = await client.GetAsync(uri).ConfigureAwait(false))
                {
                    //if (!response.IsSuccessStatusCode)
                    //{
                    //    throw await DevicePortalException.CreateAsync(response);
                    //}

                    using (HttpContent content = response.Content)
                    {
                        dataStream = new MemoryStream();

                        await content.CopyToAsync(dataStream).ConfigureAwait(false);

                        // Ensure we return with the stream pointed at the origin.
                        dataStream.Position = 0;
                    }
                }
            }
            return dataStream;
        }


        /// <summary>
        /// Submits the http post request to the specified uri.
        /// </summary>
        /// <param name="uri">The uri to which the post request will be issued.</param>
        /// <param name="requestContent">Optional content containing data for the request body.</param>
        /// <returns>Task tracking the completion of the POST request</returns>
        public async Task<Stream> PostAsync(Uri uri, HttpContent requestContent, System.Net.NetworkCredential Credentials)
        {
            MemoryStream responseDataStream = null;
            HttpHeadersHelper headerHelper = new HttpHeadersHelper();
            WebRequestHandler requestSettings = new WebRequestHandler();
            requestSettings.UseDefaultCredentials = false;
            requestSettings.Credentials = Credentials;
            requestSettings.ServerCertificateValidationCallback = this.ServerCertificateValidation;

            using (HttpClient client = new HttpClient(requestSettings))
            {
                headerHelper.ApplyHttpHeaders(client, HttpMethods.Post);

                using (HttpResponseMessage response = await client.PostAsync(uri, requestContent).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw await DevicePortalException.CreateAsync(response);
                    }

                    headerHelper.RetrieveCsrfToken(response);

                    if (response.Content != null)
                    {
                        using (HttpContent responseContent = response.Content)
                        {
                            responseDataStream = new MemoryStream();

                            await responseContent.CopyToAsync(responseDataStream).ConfigureAwait(false);

                            // Ensure we return with the stream pointed at the origin.
                            responseDataStream.Position = 0;
                        }
                    }
                }
            }

            return responseDataStream;
        }

        public async Task<Stream> DeleteAsync(Uri uri, System.Net.NetworkCredential Credentials)
        {
            MemoryStream dataStream = null;

            WebRequestHandler requestSettings = new WebRequestHandler();
            HttpHeadersHelper headerHelper = new HttpHeadersHelper();
            requestSettings.UseDefaultCredentials = false;
            requestSettings.Credentials = Credentials;
            requestSettings.ServerCertificateValidationCallback = this.ServerCertificateValidation;

            using (HttpClient client = new HttpClient(requestSettings))
            {
                headerHelper.ApplyHttpHeaders(client, HttpMethods.Delete);

                using (HttpResponseMessage response = await client.DeleteAsync(uri).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw await DevicePortalException.CreateAsync(response);
                    }

                    headerHelper.RetrieveCsrfToken(response);

                    if (response.Content != null)
                    {
                        using (HttpContent content = response.Content)
                        {
                            dataStream = new MemoryStream();

                            await content.CopyToAsync(dataStream).ConfigureAwait(false);

                            // Ensure we return with the stream pointed at the origin.
                            dataStream.Position = 0;
                        }
                    }
                }
            }

            return dataStream;
        }

        /// <summary>
        /// Validate the server certificate
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="certificate">The server's certificate</param>
        /// <param name="chain">The cert chain</param>
        /// <param name="sslPolicyErrors">Policy Errors</param>
        /// <returns>whether the cert passes validation</returns>
        private bool ServerCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (this.manualCertificate != null)
            {
                chain.ChainPolicy.ExtraStore.Add(this.manualCertificate);
            }

            X509Certificate2 certv2 = new X509Certificate2(certificate);
            bool isValid = chain.Build(certv2);

            // If chain validation failed but we have a manual cert, we can still
            // check the chain to see if the server cert chains up to our manual cert
            // (or matches it) in which case this is valid.
            if (!isValid && this.manualCertificate != null)
            {
                foreach (X509ChainElement element in chain.ChainElements)
                {
                    foreach (X509ChainStatus status in element.ChainElementStatus)
                    {
                        // Check if this is a failure that should cause the chain to be rejected
                        if (status.Status != X509ChainStatusFlags.NoError &&
                            status.Status != X509ChainStatusFlags.UntrustedRoot &&
                            status.Status != X509ChainStatusFlags.RevocationStatusUnknown)
                        {
                            return false;
                        }
                    }

                    // This cert chained to our provided cert. Continue walking
                    // the chain to ensure we don't hit a failure that would
                    // cause our chain to be rejected.
                    if (element.Certificate.Issuer == this.manualCertificate.Issuer &&
                        element.Certificate.Thumbprint == this.manualCertificate.Thumbprint)
                    {
                        isValid = true;
                        break;
                    }
                }
            }

            // If this still appears invalid, we give the app a chance via a handler
            // to override the trust decision.
            if (!isValid)
            {
                bool? overridenIsValid = this.UnvalidatedCert?.Invoke(this, certificate, chain, sslPolicyErrors);

                if (overridenIsValid != null && overridenIsValid == true)
                {
                    isValid = true;
                }
            }

            return isValid;
        }
    }
}
