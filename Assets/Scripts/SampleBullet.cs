using Fusion;
using UnityEngine;

public class SampleBullet : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private float hitRadius = 0.3f;

    [Networked] private TickTimer LifeTimer { get; set; }                   // 네트워크의 타이머

    [Networked] private PlayerRef Owner { get; set; }

    public void Init(PlayerRef owner)
    {
        Owner = owner;
    }

    public override void Spawned()                                          // 네트워크상 스폰이 되었을 때
    {
        if (Object.HasStateAuthority)                                       // 오브젝트의 권한이 있을 때
        {
            LifeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);      // 타이머를 셋팅 한다.
        }
    }

    public override void FixedUpdateNetwork()                               // 네트워크 업데이트에서
    {
        if (!Object.HasStateAuthority)
            return;

        transform.position += transform.forward * speed * Runner.DeltaTime; // 총알은 앞으로 간다

        if(Object.HasStateAuthority && LifeTimer.Expired(Runner))           // 타이머가 만료 될 경우
        {
            Runner.Despawn(Object);                                         // 디스폰
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius);

        foreach(var hit in hits)
        {
            SimplePlayer player = hit.GetComponentInParent<SimplePlayer>();

            if (player == null)
                continue;

            if (player.Object.InputAuthority == Owner)
                continue;

            Debug.Log($"총알이 플레이어를 맞춤: {player.Object.InputAuthority}");

            Runner.Despawn(Object);

            return;
        }
    }
}
