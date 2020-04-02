using UnityEngine;
using Newtonsoft.Json;
using System.Text;
using Scripts.Json;
using System.IO;

//[System.Serializable]
public class ProcessMetaData
{
    public Categories categories { get; set; }
    //public IList<Hologram> holograms { get; set; }
}

public class ReadMetaData
{
    public static string fileName = "holoSetUp";

    public static ProcessMetaData ReadData(string json)
    {
        ProcessMetaData pmd = null;
        pmd = JsonConvert.DeserializeObject<ProcessMetaData>(json);
        return pmd;
    }

    public Categories ReadCategoriesData(string fileName)
    {

        Categories cats = new Categories();
        string path = string.Format("{0}/{1}.json", Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            byte[] data = File.ReadAllBytes(path);
            string json = Encoding.ASCII.GetString(data);

            cats = JsonConvert.DeserializeObject<Categories>(json);
        }
        return cats;
    }

    /*
    public Holograms ReadHologramData(string fileName)
    {
        Holograms hols = new Holograms();
        string path = string.Format("{0}/{1}.json", Application.persistentDataPath, fileName);
        if (UnityEngine.Windows.File.Exists(path))
        {
            byte[] data = UnityEngine.Windows.File.ReadAllBytes(path);
            string json = Encoding.ASCII.GetString(data);

            hols = JsonConvert.DeserializeObject<Holograms>(json);
        }
        return hols;
    }
*/
}






