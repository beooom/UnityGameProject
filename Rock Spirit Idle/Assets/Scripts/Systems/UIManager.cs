using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using static Lean.Pool.LeanGameObjectPool;
using UIButton = UnityEngine.UI.Button;

public class UIManager : SingletonManager<UIManager>
{
    public bool m_IsButtonDowning;
    public UIButton twiceButton;
    public List<UIButton> buttons = new List<UIButton>();
    public Text isPower;

    public Image title;
    public UIButton titleStart;
    public UIButton titleQuit;

    public bool isGameStarted = false; // ���� ���� ���� Ȯ�ο� �÷���

    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 0f;
        title.enabled = true;
        titleStart.onClick.AddListener(() => StartCoroutine(StartGameAfterDelay(1f)));  // ���� ��ư Ŭ�� �� ���� ����
        titleQuit.onClick.AddListener(Application.Quit);  // ���� ��ư Ŭ�� �� ���� ����
    }


    private void Update()
    {
        if (!isGameStarted) return;

        ColorBlock colorBlock = twiceButton.colors;
        isPower.text = $"���ݷ� : {GameManager.Instance.player.GetCurrentPower()}";
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
        m_IsButtonDowning = !m_IsButtonDowning; // ���� ��ȯ
    }
    private IEnumerator StartGameAfterDelay(float delay)
    {
        title.enabled = false;     // Ÿ��Ʋ �̹��� ����
        titleStart.gameObject.SetActive(false);  // ���� ��ư ����
        titleQuit.gameObject.SetActive(false);   // ���� ��ư ����

        yield return new WaitForSecondsRealtime(delay);  // ���� (�ǽð� ���)

        isGameStarted = true; // ���� ���� �÷��� ����
        Time.timeScale = 1f;  // ���� ���� (�ð� �帧 �簳)
    }

}
