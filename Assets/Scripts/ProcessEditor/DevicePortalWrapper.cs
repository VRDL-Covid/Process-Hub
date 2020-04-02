using System.Collections;
using System.Collections.Generic;
using System;
using System.Web;
using System.Net.Http;


public class DevicePortalWrapper
{
    public static readonly string API_FileQuery = @"http://{0}/api/filesystem/apps/file";

    public struct ConnectInfo
    {
        public string IP;
        public string User;
        public string Password;

        public ConnectInfo (string ip, string user, string password)
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
}
