using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AssetBundles
{
    public class Utility
    {
        public const string AssetBundlesOutputPath = "AssetBundles";

        public static string GetPlatformName()
        {
#if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
            return GetPlatformForAssetBundles(Application.platform);
#endif
        }

#if UNITY_EDITOR
        private static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                default:
                    return null;
            }
        }
#endif

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                default:
                    return null;
            }
        }
        /// <summary>
        /// Constructs a fully formed endpoint uri.
        /// </summary>
        /// <param name="baseUri">The base uri </param>
        /// <param name="path">The path to </param>
        /// <param name="payload">Parameterized control data if required</param>
        /// <returns>Uri object containing the complete path and query string required to issue the call.</returns>
        public static System.Uri BuildEndpoint(
            System.Uri baseUri,
            string path,
            string payload = null)
        {
            string relativePart = !string.IsNullOrWhiteSpace(payload) ?
                                    string.Format("{0}?{1}", path, payload) : path;
            return new System.Uri(baseUri, relativePart);
        }
    }
}
