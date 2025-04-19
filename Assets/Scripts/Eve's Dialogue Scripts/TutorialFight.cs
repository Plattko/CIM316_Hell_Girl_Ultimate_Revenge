using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFight : MonoBehaviour
{
    [SerializeField] private GameObject tutorialEnemy;
    SpawnTheDevilsAdvocate spawnTDA;

    private bool enemyDefeated;

    private void Start()
    {
        spawnTDA = FindAnyObjectByType<SpawnTheDevilsAdvocate>();
        tutorialEnemy.SetActive(false);
        enemyDefeated = false;
    }

    public void SpawnEnemy()
    {
        spawnTDA.RemoveTDA();
        StartCoroutine(SpawnEnemyAfterDelay());
        StartCoroutine(CheckForEnemyAfterDelay());
    }
    public void CheckForEnemy()
    {
        if (tutorialEnemy == null)
        {
            enemyDefeated = true;
            spawnTDA.SpawnTDA();
        }
    }

    public IEnumerator SpawnEnemyAfterDelay()
    {
        yield return new WaitForSeconds(3);
        tutorialEnemy.SetActive(true);
    }

    public IEnumerator CheckForEnemyAfterDelay()
    {
        yield return new WaitForSeconds(3);
        while (!enemyDefeated)
        {
            CheckForEnemy();
            yield return new WaitForSeconds(1);
        }
        CheckForEnemy();

    }

}
