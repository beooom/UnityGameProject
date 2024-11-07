using System.Collections;
using UnityEngine;

public class VoidSkill : SkillBase
{
    public GameObject voidPrefab;

    protected override IEnumerator ExecuteSkill(Enemy target)
    {
        Vector3 spawnPosition = player.transform.position;
        Instantiate(voidPrefab, spawnPosition, Quaternion.identity);

        yield return null; // ��ų ��� �� �ٷ� ����
    }
}