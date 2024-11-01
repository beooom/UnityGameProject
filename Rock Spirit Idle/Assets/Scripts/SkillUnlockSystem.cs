using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum SkillType
{
    ��Ÿ����Ʈ,
    ���̵�,
    ���׿�,
    ����,
    �г�
}

public class SkillUnlockSystem : SingletonManager<SkillUnlockSystem>
{
    [System.Serializable]
    public class SkillData
    {
        public SkillType skillType;            // ��ų Ÿ��
        public GameObject skillObject;
        public Button skillButton;             // ��ų ��ư
        public Image lockImage;                // ��� ���� �̹���
        [TextArea]
        public string description;             // ��ų ����
        public bool isUnlocked = false;        // ��ų �ر� ����
        public int unlockingCost;
    }

    public SkillData[] skills;                 // ��� ��ų ������ �迭
    public GameObject descriptionPanel;        // ���� â �г�
    public Text skillDescriptionText;          // ��ų ���� �ؽ�Ʈ
    public Button unlockButton;                // �ر� ��ư
    public Button cancelButton;                // �ر� ��� ��ư
    public Button closeButton;                 // ����â �ݱ� ��ư

    private SkillData currentSkillData;        // ���� ���õ� ��ų ������

    private void Start()
    {
        // �� ��ų ��ư�� ������ ���
        foreach (SkillData skillData in skills)
        {
            SkillData capturedSkillData = skillData; // Ŭ���� ���� ����
            skillData.skillButton.onClick.AddListener(() => OnSkillButtonClicked(capturedSkillData));
            skillData.lockImage.gameObject.SetActive(!skillData.isUnlocked); // �ʱ� ��� ���� ����
            if (skillData.skillObject != null)
            {
                skillData.skillObject.SetActive(skillData.isUnlocked);
            }
        }

        // ���� â�� �ݱ� ��ư �ʱ�ȭ
        descriptionPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
        unlockButton.onClick.AddListener(OnUnlockButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnSkillButtonClicked(SkillData skillData)
    {
        currentSkillData = skillData;

        // ��ų�� ��� ������ �� ���� â ����
        if (!skillData.isUnlocked)
        {
            descriptionPanel.SetActive(true);
            skillDescriptionText.text = $"{skillData.skillType}\n\n {skillData.description}";
            unlockButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(false); // �ر� ������ �ݱ� ��ư �����
        }
        else
        {
            // �رݵ� ��ų�� ����â ����
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
            // ��ų �ر� ó��
            DataManager.Instance.totalGold -= currentSkillData.unlockingCost;
            currentSkillData.skillObject.SetActive(true);
            currentSkillData.isUnlocked = true;
            currentSkillData.lockImage.gameObject.SetActive(false);
            unlockButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(true); // �ر� �� �ݱ� ��ư Ȱ��ȭ
            skillDescriptionText.text = $"{currentSkillData.skillType} ��ų�� �رݵǾ����ϴ�!";
        }
    }

    private void OnCancelButtonClicked()
    {
        // �ر� ���
        descriptionPanel.SetActive(false);
    }

    private void OnCloseButtonClicked()
    {
        // ����â �ݱ�
        descriptionPanel.SetActive(false);
    }
}
