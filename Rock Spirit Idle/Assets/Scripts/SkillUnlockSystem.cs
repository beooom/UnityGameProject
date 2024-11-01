using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum SkillType
{
    스타라이트,
    보이드,
    메테오,
    벼락,
    분노
}

public class SkillUnlockSystem : SingletonManager<SkillUnlockSystem>
{
    [System.Serializable]
    public class SkillData
    {
        public SkillType skillType;            // 스킬 타입
        public GameObject skillObject;
        public Button skillButton;             // 스킬 버튼
        public Image lockImage;                // 잠금 상태 이미지
        [TextArea]
        public string description;             // 스킬 설명
        public bool isUnlocked = false;        // 스킬 해금 상태
        public int unlockingCost;
    }

    public SkillData[] skills;                 // 모든 스킬 데이터 배열
    public GameObject descriptionPanel;        // 설명 창 패널
    public Text skillDescriptionText;          // 스킬 설명 텍스트
    public Button unlockButton;                // 해금 버튼
    public Button cancelButton;                // 해금 취소 버튼
    public Button closeButton;                 // 설명창 닫기 버튼

    private SkillData currentSkillData;        // 현재 선택된 스킬 데이터

    private void Start()
    {
        // 각 스킬 버튼에 리스너 등록
        foreach (SkillData skillData in skills)
        {
            SkillData capturedSkillData = skillData; // 클로저 문제 방지
            skillData.skillButton.onClick.AddListener(() => OnSkillButtonClicked(capturedSkillData));
            skillData.lockImage.gameObject.SetActive(!skillData.isUnlocked); // 초기 잠금 상태 설정
            if (skillData.skillObject != null)
            {
                skillData.skillObject.SetActive(skillData.isUnlocked);
            }
        }

        // 설명 창과 닫기 버튼 초기화
        descriptionPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
        unlockButton.onClick.AddListener(OnUnlockButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnSkillButtonClicked(SkillData skillData)
    {
        currentSkillData = skillData;

        // 스킬이 잠금 상태일 때 설명 창 열기
        if (!skillData.isUnlocked)
        {
            descriptionPanel.SetActive(true);
            skillDescriptionText.text = $"{skillData.skillType}\n\n {skillData.description}";
            unlockButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(false); // 해금 전에는 닫기 버튼 숨기기
        }
        else
        {
            // 해금된 스킬의 설명창 열기
            descriptionPanel.SetActive(true);
            skillDescriptionText.text = $"{skillData.skillType}\n\n {skillData.description}";
            unlockButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(true);
        }
    }

    private void OnUnlockButtonClicked()
    {
        if (DataManager.Instance.totalGold >= currentSkillData.unlockingCost && currentSkillData != null)
        {
            // 스킬 해금 처리
            DataManager.Instance.totalGold -= currentSkillData.unlockingCost;
            currentSkillData.skillObject.SetActive(true);
            currentSkillData.isUnlocked = true;
            currentSkillData.lockImage.gameObject.SetActive(false);
            unlockButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(true); // 해금 후 닫기 버튼 활성화
            skillDescriptionText.text = $"{currentSkillData.skillType} 스킬이 해금되었습니다!";
        }
    }

    private void OnCancelButtonClicked()
    {
        // 해금 취소
        descriptionPanel.SetActive(false);
    }

    private void OnCloseButtonClicked()
    {
        // 설명창 닫기
        descriptionPanel.SetActive(false);
    }
}
