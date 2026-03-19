using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class FusionBootstrap : MonoBehaviour , INetworkRunnerCallbacks
{
    [Header("Session")]
    [SerializeField] private string sessionName = "Room_01";

    [Header("Player")]
    [SerializeField] private NetworkPrefabRef playerPrefab;                             //네트워크에 등록된 프리팹
    [SerializeField] private Transform[] spawnPoints;                                   //스폰 위치 설정 

    private Dictionary<PlayerRef, NetworkObject> playerObjects = new();

    private NetworkRunner runner;

    public struct NetworkInputData : INetworkInput
    {
        public Vector2 move;
    }


    public void StartHost() => _ = StartGame(GameMode.Host);
    public void StartClinet() => _ = StartGame(GameMode.Client);

    private Vector3 GetSpawnPosition(PlayerRef player)
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int index = player.RawEncoded % spawnPoints.Length;
            return spawnPoints[index].position;
        }

        return new Vector3(player.RawEncoded * 2, 1, 0);                //RawEncoded (바이트(byte)) 형태로 변환 (직렬화) 중간단계
    }

    private async Task StartGame(GameMode mode)
    {
        if (runner != null) return;

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        runner.AddCallbacks(this);

        var SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            SessionName = sessionName,
            SceneManager = SceneManager 
        });

        if (result.Ok)
            Debug.Log($"[Fusion] StartGame OK - {mode} / {sessionName}");
        else
            Debug.LogError($"[Fusion] StartGame FAILED - {result.ShutdownReason}");
    }

  

    // --------------------- 콜백 (필수/미사용은 빈 구현) -------------------

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        Debug.Log($"플레이어 입장 : {player}");

        if (runner.IsPlayer == false)
            return;

        Vector3 spawnPos = GetSpawnPosition(player);

        var obj = runner.Spawn(
            playerPrefab,
            spawnPos,
            Quaternion.identity,
            player
        );

        playerObjects[player] = obj;

    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
    {
        if (!runner.IsServer) return;

        if (playerObjects.TryGetValue(player, out var obj))
        {
            runner.Despawn(obj);
            playerObjects.Remove(player);
        }

        Debug.Log($"플레이어 제거됨 : {player}");
    
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData();

        data.move = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player , NetworkInput input) { }

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner , NetDisconnectReason reason)    
    {
        Debug.Log($"[Fusion] Disconnected : {reason}");
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        Debug.Log($"[Fusion] Shutdown : {reason}");
        this.runner = null;
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request , byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    //------------------------------------------------------------------------------------------------------------

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
