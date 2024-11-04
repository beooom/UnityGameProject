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
    public UIButton twiceButton;
    public List<UIButton> buttons = new List<UIButton>();
    public Text isPower;
    private void Update()
    {
        ColorBlock colorBlock = twiceButton.colors;
        isPower.text = $"°ø°Ý·Â : {GameManager.Instance.player.power}";
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
        twiceButton.colors = colorBlock;
    }
    public void UpgradeButton(int index)
    {
        if (DataManager.Instance.totalGold < (DataManager.Instance.upgradeLevels[index] + 1) * 10)
            return;
        DataManager.Instance.totalGold -= (DataManager.Instance.upgradeLevels[index] + 1) * 10;
        DataManager.Instance.Upgrade(index);
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
