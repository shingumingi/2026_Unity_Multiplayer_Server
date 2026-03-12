using UnityEngine;

public class Monster : MonoBehaviour
{
    public IQuestCallbacks callbacks;
    private bool isDead = false;

    private void Update()
    {
       if (isDead) return;

       if (Input.GetKeyDown(KeyCode.K))
        {
            isDead = true;
            Debug.Log("슬라임 처치");
            callbacks?.OnMonsterKilled("슬라임");
            gameObject.SetActive(false);
        }
    }
}
