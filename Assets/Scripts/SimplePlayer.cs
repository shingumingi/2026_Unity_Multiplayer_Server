using Fusion;
using UnityEngine;

public class SimplePlayer : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;

    public override void FixedUpdateNetwork()
    {
        if (GetInput<FusionBootstrap.NetworkInputData>(out var inputData))
        {
            Vector3 move = new Vector3(inputData.move.x, 0f, inputData.move.y);

            if (move.sqrMagnitude > 1f)
                move.Normalize();

            transform.position += move * moveSpeed * Runner.DeltaTime;

            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotateSpeed * Runner.DeltaTime
                );
            }
        }
    }
}
