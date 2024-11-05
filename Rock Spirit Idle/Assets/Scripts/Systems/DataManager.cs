using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : SingletonManager<DataManager>
{
    public Text Gold; // 골드 패널

    // 각 강화 요소에 대한 텍스트 UI와 레벨을 리스트로 관리
    public List<Text> upgradeTexts = new List<Text>(); // 강화 텍스트 리스트
    public List<Text> upgradeLevelTexts = new List<Text>(); // 강화레벨 텍스트 리스트
    public List<Text> upgradeCountTexts = new List<Text>(); // 강화수치 텍스트 리스트
    public List<int> upgradeLevels = new List<int>(); // 각 강화 요소의 레벨 리스트

    public int totalGold = 0;

    private Player player;

    private void Start()
    {
        //실험용 세이브 데이터 지우기
        PlayerPrefs.DeleteAll();
        //각 강화 요소의 초기 레벨 설정
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
            // 강화 레벨 증가
            upgradeLevels[upgradeIndex]++;
            ApplyUpgrade(upgradeIndex);
            SaveData(); // 데이터 저장
        }
    }
    private void UpdateUI()
    {
        // 골드 표시
        Gold.text = $"{totalGold}";

        // 각 강화 텍스트를 업데이트
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


    // 데이터 저장 메서드
    public void SaveSkillData(SkillUnlockSystem.SkillData skillData)
    {
        // 스킬 타입을 키로 사용하여 해금 상태 저장
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

        PlayerPrefs.Save(); // 데이터 저장
    }

    // 데이터 불러오기 메서드
    public bool LoadSkillData(SkillUnlockSystem.SkillData skillData)
    {
        // 스킬이 해금되었는지 확인하여 불러옴, 기본값은 0 (잠금 상태)
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

        // 불러온 강화수치로 플레이어의 스탯을 업데이트
        for (int i = 0; i < upgradeLevels.Count; i++)
        {
            ApplyUpgrade(i);
        }
    }

    private void OnApplicationQuit()
    {
        SaveData(); // 게임 종료 시 데이터 저장
    }
}
