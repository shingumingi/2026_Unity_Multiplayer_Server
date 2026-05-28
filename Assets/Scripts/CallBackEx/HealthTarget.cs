using UnityEngine;
using Fusion;

public class HealthTarget : NetworkBehaviour
{
    [Networked] public int HP { get; set; }         // 체력 선언

    public override void Spawned()
    {
        // Fusion 에서는 네트워크 오브젝트마다 HasStateAuthority(상태 권한)을 가진 주체가 딱 하나 존재
        //-> 보통은 Host가 HasStateAuthority == true , Client는 HasStateAuthority == false
        if (Object.HasStateAuthority)               // 오브젝트의 상태를 최종적으로 결정할 권한을 내가 가지고 있는가?
        {
            HP = 5;
        }
    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)
            return;

        HP -= damage;
        Debug.Log($"{name} HP : {HP}");

        if(HP <= 0)
        {
            HP = 5;
            transform.position = Vector3.zero;
            Debug.Log($"{name} 리스폰");
        }
    }
}
