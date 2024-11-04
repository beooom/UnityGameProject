using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : SingletonManager<DataManager>
{
    public Text Gold; // ��� �г�

    // �� ��ȭ ��ҿ� ���� �ؽ�Ʈ UI�� ������ ����Ʈ�� ����
    public List<Text> upgradeTexts = new List<Text>(); // ��ȭ �ؽ�Ʈ ����Ʈ
    public List<int> upgradeLevels = new List<int>(); // �� ��ȭ ����� ���� ����Ʈ

    public int totalGold = 0;

    private Player player;

    private void Start()
    {
        // �� ��ȭ ����� �ʱ� ���� ����
        upgradeLevels = new List<int> { 0, 0, 0, 0, 0, 0, 0 };
        player = GameManager.Instance.player;
    }
    private void Update()
    {
        // ��� ǥ��
        Gold.text = $"{totalGold}";

        // �� ��ȭ �ؽ�Ʈ�� ������Ʈ
        for (int i = 0; i < upgradeTexts.Count; i++)
        {
            upgradeTexts[i].text = $"G {(upgradeLevels[i] + 1) * 10}";
        }
    }

    public void Upgrade(int upgradeIndex)
    {
        if (upgradeIndex >= 0 && upgradeIndex < upgradeLevels.Count)
        {
            // ��ȭ ���� ����
            upgradeLevels[upgradeIndex]++;

            // ���׷��̵� �ε����� ���� �÷��̾� ���� ������Ʈ
            switch (upgradeIndex)
            {
                case 0: // ���ݷ� ��ȭ
                    player.power += 1f;
                    break;
                case 1: // ü�� ��ȭ
                    player.maxHp += 5f;
                    player.hp = player.maxHp; // ü�� ȸ��
                    break;
                case 2: // ü�� ȸ�� ��ȭ
                    player.restoreHp += 0.6f;
                    break;
                case 3: // ġ��Ÿ Ȯ�� ��ȭ
                    player.criticalRate += 1f;
                    break;
                case 4: // ġ��Ÿ ������ ��ȭ
                    player.criticalHit += 1f;
                    break;
                case 5: // ���� �ӵ� ��ȭ
                    player.attackSpeed = player.attackSpeed * 0.933f; // ���� �ӵ��� �ٿ� ������ ��
                    break;
                case 6: // ��Ÿ Ȯ�� ��ȭ
                    player.doubleShot += 1f;
                    break;
                default:
                    Debug.LogWarning("Invalid upgrade index");
                    break;
            }
        }
    }
}
