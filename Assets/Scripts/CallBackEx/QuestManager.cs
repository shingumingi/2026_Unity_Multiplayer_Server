using UnityEngine;

public class QuestManager : MonoBehaviour , IQuestCallbacks
{
    [SerializeField] private Monster monster;
    private int killCount = 0;

    void Start()
    {
        monster.callbacks = this;
    }

    public void OnMonsterKilled(string monsterName)
    {
        killCount++;
        Debug.Log($"{monsterName} 처치 수 : {killCount}");

        if (killCount > 0)
        {
            Debug.Log(" 퀘스트 완료 ");
        }
    }

}
