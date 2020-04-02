using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardScript : MonoBehaviour
{
    UnityEngine.TouchScreenKeyboard keyboard;
    //UnityEngine.TouchScreenKeyboardType keyboardType;
    public string keyboardTitle = "Keyboard";
    public string keyboardText = "";
    public GameObject ObjectForId;

    // Start is called before the first frame update
    void Start()
    {
        /*switch(keyboardType)
        {
            case TouchScreenKeyboardType.
        }*/
        // Single-line textbox with title

    }

    public void RequestKeyBoard()
    {
        keyboard = TouchScreenKeyboard.Open(keyboardText, TouchScreenKeyboardType.Default, false, false, false, false, keyboardTitle);
        keyboard.active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (TouchScreenKeyboard.visible == false && keyboard != null)
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Done)
            {
                keyboardText = keyboard.text;
            }
        }
    }
}
