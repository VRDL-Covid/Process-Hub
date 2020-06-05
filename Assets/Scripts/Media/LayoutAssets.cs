using UnityEngine;
 using UnityEngine.UI;
 using System.Collections;
using Microsoft.MixedReality.Toolkit.UI;

 public class LayoutAssets : MonoBehaviour
{
    public int rows = 4;
    public int columns = 4;
    private float buttonWidth;                                        
    private float buttonHeight;                                      
    public PressableButtonHoloLens2 prefab;
    private PressableButtonHoloLens2 button;

    IEnumerator Start()
    {
        yield return null;
        RectTransform myRect = GetComponent<RectTransform>();        
        buttonHeight = myRect.rect.height / (float)rows;            
        buttonWidth = myRect.rect.width / (float)columns;            
        GridLayoutGroup grid = this.GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(buttonWidth, buttonHeight);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                button = (PressableButtonHoloLens2)Instantiate(prefab);
                button.transform.SetParent(transform);       
            }
        }
    }
}