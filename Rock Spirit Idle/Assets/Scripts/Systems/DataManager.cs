using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : SingletonManager<DataManager>
{
    public Text Gold; // 골드 패널

    // 각 강화 요소에 대한 텍스트 UI와 레벨을 리스트로 관리
    public List<Text> upgradeTexts = new List<Text>(); // 강화 텍스트 리스트
    public List<int> upgradeLevels = new List<int>(); // 각 강화 요소의 레벨 리스트

    public int totalGold = 0;

    private Player player;

    private void Start()
    {
        // 각 강화 요소의 초기 레벨 설정
        upgradeLevels = new List<int> { 0, 0, 0, 0, 0, 0, 0 };
        player = GameManager.Instance.player;
    }
    private void Update()
    {
        // 골드 표시
        Gold.text = $"{totalGold}";

        // 각 강화 텍스트를 업데이트
        for (int i = 0; i < upgradeTexts.Count; i++)
        {
            upgradeTexts[i].text = $"G {(upgradeLevels[i] + 1) * 10}";
        }
    }

    public void Upgrade(int upgradeIndex)
    {
        if (upgradeIndex >= 0 && upgradeIndex < upgradeLevels.Count)
        {
            // 강화 레벨 증가
            upgradeLevels[upgradeIndex]++;

            // 업그레이드 인덱스에 따라 플레이어 스탯 업데이트
            switch (upgradeIndex)
            {
                case 0: // 공격력 강화
                    player.power += 1f;
                    break;
                case 1: // 체력 강화
                    player.maxHp += 5f;
                    player.hp = player.maxHp; // 체력 회복
                    break;
                case 2: // 체력 회복 강화
                    player.restoreHp += 0.6f;
                    break;
                case 3: // 치명타 확률 강화
                    player.criticalRate += 1f;
                    break;
                case 4: // 치명타 데미지 강화
                    player.criticalHit += 1f;
                    break;
                case 5: // 공격 속도 강화
                    player.attackSpeed = player.attackSpeed * 0.933f; // 공격 속도를 줄여 빠르게 함
                    break;
                case 6: // 연타 확률 강화
                    player.doubleShot += 1f;
                    break;
                default:
                    Debug.LogWarning("Invalid upgrade index");
                    break;
            }
        }
    }
}
