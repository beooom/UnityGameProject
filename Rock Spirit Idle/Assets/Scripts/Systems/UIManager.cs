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

    public bool isGameStarted = false; // 게임 시작 여부 확인용 플래그

    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 0f;
        title.enabled = true;
        titleStart.onClick.AddListener(() => StartCoroutine(StartGameAfterDelay(1f)));  // 시작 버튼 클릭 시 게임 시작
        titleQuit.onClick.AddListener(Application.Quit);  // 종료 버튼 클릭 시 게임 종료
    }


    private void Update()
    {
        if (!isGameStarted) return;

        ColorBlock colorBlock = twiceButton.colors;
        isPower.text = $"공격력 : {GameManager.Instance.player.GetCurrentPower()}";
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
        m_IsButtonDowning = !m_IsButtonDowning; // 상태 전환
    }
    private IEnumerator StartGameAfterDelay(float delay)
    {
        title.enabled = false;     // 타이틀 이미지 숨김
        titleStart.gameObject.SetActive(false);  // 시작 버튼 숨김
        titleQuit.gameObject.SetActive(false);   // 종료 버튼 숨김

        yield return new WaitForSecondsRealtime(delay);  // 지연 (실시간 대기)

        isGameStarted = true; // 게임 시작 플래그 설정
        Time.timeScale = 1f;  // 게임 시작 (시간 흐름 재개)
    }

}
