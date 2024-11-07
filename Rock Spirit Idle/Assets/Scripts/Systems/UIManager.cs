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


    public bool isGameStarted = false; // 게임 시작 여부 확인용 플래그

    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 0f;
        title.gameObject.SetActive(true);
        titleStart.onClick.AddListener(() => StartCoroutine(StartGameAfterDelay(0.5f)));  // 시작 버튼 클릭 시 게임 시작
        titleQuit.onClick.AddListener(Application.Quit);  // 종료 버튼 클릭 시 게임 종료
        ExitPanel.SetActive(false);
    }


    private void Update()
    {
        if (!isGameStarted) return;
        if (GameManager.Instance.currentState != GameState.Playing)
            return; // 게임 상태가 'Playing'이 아닌 경우 업데이트를 중지

        isPower.text = $"공격력 : {GameManager.Instance.player.GetCurrentPower()}";
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
        // 골드 차감 후 업그레이드 수행
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
            colorBlock.normalColor = Color.white; // 정상 상태 색상
            colorBlock.highlightedColor = Color.white;
        }
        else
        {
            colorBlock.normalColor = Color.gray; // 어두운 색상으로 설정
            colorBlock.highlightedColor = Color.gray;
        }

        button.colors = colorBlock;
    }
    public void PointerDown()
    {
        m_IsButtonDowning = !m_IsButtonDowning; // 상태 전환
    }

    public void GameExit()
    {
        ExitPanel.SetActive(true);
        gameExit.onClick.AddListener(Application.Quit);
        gameExitButtonclose.onClick.AddListener(() => { ExitPanel.SetActive(false); });
    }

    private IEnumerator StartGameAfterDelay(float delay)
    {
        title.enabled = false;     // 타이틀 이미지 숨김
        titleStart.gameObject.SetActive(false);  // 시작 버튼 숨김
        titleQuit.gameObject.SetActive(false);   // 종료 버튼 숨김
        twiceButton.gameObject.SetActive(true);
        twiceButtonDown.gameObject.SetActive(false);

        yield return new WaitForSecondsRealtime(delay);  // 지연 (실시간 대기)

        isGameStarted = true; // 게임 시작 플래그 설정
        Time.timeScale = 1f;  // 게임 시작 (시간 흐름 재개)
    }

}
