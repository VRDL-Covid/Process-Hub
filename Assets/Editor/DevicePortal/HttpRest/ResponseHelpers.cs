//----------------------------------------------------------------------------------------------
// <copyright file="ResponseHelpers.cs" company="Microsoft Corporation">
//     Licensed under the MIT License. See LICENSE.TXT in the project root license information.
// </copyright>
//----------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace Scripts.DevicePortal
{
    /// <content>
    /// Methods for working with Http responses.
    /// </content>
    public partial class DevicePortalWrapper
    {
        /// <summary>
        /// The prefix for the <see cref="SystemPerformanceInformation" /> JSON formatting error.
        /// </summary>
        private static readonly string SysPerfInfoErrorPrefix = "{\"Reason\" : \"";

        /// <summary>
        /// The postfix for the <see cref="SystemPerformanceInformation" /> JSON formatting error.
        /// </summary>
        private static readonly string SysPerfInfoErrorPostfix = "\"}";

        /// <summary>
        /// Reads dataStream as T.
        /// </summary>
        /// <typeparam name="T">Return type for the JSON message</typeparam>
        /// <param name="dataStream">The stream that contains the JSON message to be checked.</param>
        /// <param name="settings">Optional settings for JSON serialization.</param>
        /// <returns>Read data</returns>
        public static T ReadJsonStream<T>(Stream dataStream, DataContractJsonSerializerSettings settings = null)
        {
            T data = default(T);
            object response = null;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), settings);

            using (dataStream)
            {
                if ((dataStream != null) &&
                    (dataStream.Length != 0))
                {
                    try
                    {
                        response = serializer.ReadObject(dataStream);
                    }
                    catch (SerializationException)
                    {
                        // Assert on serialization failure.
                        Debug.Assert(false, "Serialization failure encountered. Check DataContract types for a possible mismatch between expectations and reality");
                        throw;
                    }

                    data = (T)response;
                }
            }

            return data;
        }

        #region Data contract

        /// <summary>
        /// A null response class when we don't care about the response
        /// body.
        /// </summary>
        [DataContract]
        private class NullResponse
        {
        }

        #endregion
    }
}
