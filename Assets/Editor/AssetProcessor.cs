using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

namespace Scripts.Editor
{
    public class AssetProcessor : MonoBehaviour
    {
        private static AssetProcessor instance = null;

        public static AssetProcessor Instance
        {
            get
            {
                return instance;
            }
        }

        public static void AddAsset(string localName, ref string message)
        {
            // now import into local asset database...
            //message = "";
            //AssetPostProcessor.message = message;
            Task checkTask = Task.Run(() =>
            {
                AssetDatabase.ImportAsset(localName, ImportAssetOptions.ForceUpdate);
            });

        }
    }
}
