using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UIButton = UnityEngine.UI.Button;

public class UIManager : SingletonManager<UIManager>
{
    public bool m_IsButtonDowning;
    public UIButton button;
    private void Update()
    {
        ColorBlock colorBlock = button.colors;
        if (m_IsButtonDowning)
        {
            colorBlock.selectedColor = Color.green;
            colorBlock.normalColor = Color.green;
            Time.timeScale = 2.0f;
        }
        else
        {
            colorBlock.selectedColor = Color.white;
            colorBlock.normalColor = Color.white;
            Time.timeScale = 1.0f;
        }
        button.colors = colorBlock;
    }

    public void PointerDown()
    {
        if (m_IsButtonDowning == false)
        {
            m_IsButtonDowning = true;
        }
        else
        {
            m_IsButtonDowning = false;
        }
    }
}
