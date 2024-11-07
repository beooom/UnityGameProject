using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static Lean.Pool.LeanGameObjectPool;
using UIButton = UnityEngine.UI.Button;

public class UIManager : SingletonManager<UIManager>
{
    public bool m_IsButtonDowning = false;
    public UIButton twiceButton;
    public UIButton twiceButtonDown;
    public List<UIButton> buttons = new List<UIButton>();
    public Text isPower;

    public Image title;
    public UIButton titleStart;
    public UIButton titleQuit;

    public UIButton gameExitButton;
    public UIButton gameExit;
    public UIButton gameExitButtonclose;
    public GameObject ExitPanel;


    public bool isGameStarted = false; // ���� ���� ���� Ȯ�ο� �÷���

    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 0f;
        title.gameObject.SetActive(true);
        titleStart.onClick.AddListener(() => StartCoroutine(StartGameAfterDelay(0.5f)));  // ���� ��ư Ŭ�� �� ���� ����
        titleQuit.onClick.AddListener(Application.Quit);  // ���� ��ư Ŭ�� �� ���� ����
        ExitPanel.SetActive(false);
    }


    private void Update()
    {
        if (!isGameStarted) return;
        if (GameManager.Instance.currentState != GameState.Playing)
            return; // ���� ���°� 'Playing'�� �ƴ� ��� ������Ʈ�� ����

        isPower.text = $"���ݷ� : {GameManager.Instance.player.GetCurrentPower()}";
        UpdateUpgradeButtonColors();
        if (m_IsButtonDowning)
        {
            twiceButton.gameObject.SetActive(false);
            twiceButtonDown.gameObject.SetActive(true);
            Time.timeScale = 2.0f;
        }
        else
        {
            twiceButton.gameObject.SetActive(true);
            twiceButtonDown.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }
    public void UpgradeButton(int index)
    {
        int cost = (DataManager.Instance.upgradeLevels[index] + 1) * 10;
        bool isAffordable = DataManager.Instance.totalGold >= cost;

        if (!isAffordable)
        {
            return;
        }
        if (index == 3 && DataManager.Instance.upgradeLevels[index] == 100)
        {
            return;
        }
        // ��� ���� �� ���׷��̵� ����
        DataManager.Instance.totalGold -= cost;
        DataManager.Instance.Upgrade(index);
        UpdateUpgradeButtonColors();
    }
    private void UpdateUpgradeButtonColors()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int cost = (DataManager.Instance.upgradeLevels[i] + 1) * 10;
            bool isAffordable = DataManager.Instance.totalGold >= cost;
            UpdateButtonColor(buttons[i], isAffordable);
        }
    }
    private void UpdateButtonColor(UIButton button, bool isAffordable)
    {
        ColorBlock colorBlock = button.colors;

        if (isAffordable)
        {
            colorBlock.normalColor = Color.white; // ���� ���� ����
            colorBlock.highlightedColor = Color.white;
        }
        else
        {
            colorBlock.normalColor = Color.gray; // ��ο� �������� ����
            colorBlock.highlightedColor = Color.gray;
        }

        button.colors = colorBlock;
    }
    public void PointerDown()
    {
        m_IsButtonDowning = !m_IsButtonDowning; // ���� ��ȯ
    }

    public void GameExit()
    {
        ExitPanel.SetActive(true);
        gameExit.onClick.AddListener(Application.Quit);
        gameExitButtonclose.onClick.AddListener(() => { ExitPanel.SetActive(false); });
    }

    private IEnumerator StartGameAfterDelay(float delay)
    {
        title.enabled = false;     // Ÿ��Ʋ �̹��� ����
        titleStart.gameObject.SetActive(false);  // ���� ��ư ����
        titleQuit.gameObject.SetActive(false);   // ���� ��ư ����
        twiceButton.gameObject.SetActive(true);
        twiceButtonDown.gameObject.SetActive(false);

        yield return new WaitForSecondsRealtime(delay);  // ���� (�ǽð� ���)

        isGameStarted = true; // ���� ���� �÷��� ����
        Time.timeScale = 1f;  // ���� ���� (�ð� �帧 �簳)
    }

}
