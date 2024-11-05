using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : SingletonManager<DataManager>
{
    public Text Gold; // ��� �г�

    // �� ��ȭ ��ҿ� ���� �ؽ�Ʈ UI�� ������ ����Ʈ�� ����
    public List<Text> upgradeTexts = new List<Text>(); // ��ȭ �ؽ�Ʈ ����Ʈ
    public List<Text> upgradeLevelTexts = new List<Text>(); // ��ȭ���� �ؽ�Ʈ ����Ʈ
    public List<Text> upgradeCountTexts = new List<Text>(); // ��ȭ��ġ �ؽ�Ʈ ����Ʈ
    public List<int> upgradeLevels = new List<int>(); // �� ��ȭ ����� ���� ����Ʈ

    public int totalGold = 0;

    private Player player;

    private void Start()
    {
        //����� ���̺� ������ �����
        PlayerPrefs.DeleteAll();
        //�� ��ȭ ����� �ʱ� ���� ����
        upgradeLevels = new List<int> { 1, 1, 0, 0, 0, 0, 0 };
        player = GameManager.Instance.player;
        LoadData();
    }
    private void Update()
    {
        UpdateUI();
    }

    public void Upgrade(int upgradeIndex)
    {
        if (upgradeIndex >= 0 && upgradeIndex < upgradeLevels.Count)
        {
            // ��ȭ ���� ����
            upgradeLevels[upgradeIndex]++;
            ApplyUpgrade(upgradeIndex);
            SaveData(); // ������ ����
        }
    }
    private void UpdateUI()
    {
        // ��� ǥ��
        Gold.text = $"{totalGold}";

        // �� ��ȭ �ؽ�Ʈ�� ������Ʈ
        for (int i = 0; i < upgradeLevels.Count; i++)
        {
            upgradeTexts[i].text = $"G {(upgradeLevels[i] + 1) * 10}";
            upgradeLevelTexts[i].text = $"Lv {upgradeLevels[i]}";
        }
        upgradeCountTexts[0].text = $"{player.power}";
        upgradeCountTexts[1].text = $"{player.maxHp}";
        upgradeCountTexts[2].text = $"{player.restoreHp}";
        upgradeCountTexts[3].text = $"{player.criticalRate}%";
        upgradeCountTexts[4].text = $"{player.criticalHit * 100f}%";
        upgradeCountTexts[5].text = $"{player.attackSpeedIncrease}";
        upgradeCountTexts[6].text = $"{player.doubleShot}%";
    }
    private void ApplyUpgrade(int upgradeIndex)
    {
        switch (upgradeIndex)
        {
            case 0: player.power = upgradeLevels[upgradeIndex] * 1f; break;
            case 1: player.maxHp = upgradeLevels[upgradeIndex] * 5f; player.hp += 5f; break;
            case 2: player.restoreHp = upgradeLevels[upgradeIndex] * 0.6f; break;
            case 3: player.criticalRate = upgradeLevels[upgradeIndex] * 1f; break;
            case 4: player.criticalHit = upgradeLevels[upgradeIndex] * 0.1f; break;
            case 5: player.attackSpeedIncrease = upgradeLevels[upgradeIndex] * 0.1f; break;
            case 6: player.doubleShot = upgradeLevels[upgradeIndex] * 1f; break;
        }
    }


    // ������ ���� �޼���
    public void SaveSkillData(SkillUnlockSystem.SkillData skillData)
    {
        // ��ų Ÿ���� Ű�� ����Ͽ� �ر� ���� ����
        PlayerPrefs.SetInt($"SkillUnlocked_{skillData.skillType}", skillData.isUnlocked ? 1 : 0);
        PlayerPrefs.Save();
    }
    private void SaveData()
    {
        PlayerPrefs.SetInt("TotalGold", totalGold);
        PlayerPrefs.SetFloat("CurHp", player.hp);

        for (int i = 0; i < upgradeLevels.Count; i++)
        {
            PlayerPrefs.SetInt($"UpgradeLevel_{i}", upgradeLevels[i]);
        }

        PlayerPrefs.Save(); // ������ ����
    }

    // ������ �ҷ����� �޼���
    public bool LoadSkillData(SkillUnlockSystem.SkillData skillData)
    {
        // ��ų�� �رݵǾ����� Ȯ���Ͽ� �ҷ���, �⺻���� 0 (��� ����)
        return PlayerPrefs.GetInt($"SkillUnlocked_{skillData.skillType}", 0) == 1;
    }
    private void LoadData()
    {
        totalGold = PlayerPrefs.GetInt("TotalGold", 0);
        player.hp = PlayerPrefs.GetFloat("CurHp", 0);

        for (int i = 0; i < upgradeLevels.Count; i++)
        {
            upgradeLevels[i] = PlayerPrefs.GetInt($"UpgradeLevel_{i}", upgradeLevels[i]);
        }

        // �ҷ��� ��ȭ��ġ�� �÷��̾��� ������ ������Ʈ
        for (int i = 0; i < upgradeLevels.Count; i++)
        {
            ApplyUpgrade(i);
        }
    }

    private void OnApplicationQuit()
    {
        SaveData(); // ���� ���� �� ������ ����
    }
}
