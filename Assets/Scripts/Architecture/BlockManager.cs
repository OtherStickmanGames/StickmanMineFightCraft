using Architecture;
using Unity.Netcode;
using UnityEngine;

public class BlockManager : NetworkBehaviour
{
    public static BlockManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to handle block changes requested by the player
    public void RequestBlockChange(int layer, Vector2Int position, int blockID)
    {
        if (IsServer)
        {
            // If called on the server, directly update the block
            SetBlock(layer, position, blockID);
        }
        else
        {
            // If called on the client, send a request to the server
            RequestBlockChangeServerRpc(layer, position, blockID);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestBlockChangeServerRpc(int layer, Vector2Int position, int blockID)
    {
        SetBlock(layer, position, blockID);
    }

    // Method to set a block on the server and sync with clients
    public void SetBlock(int layer, Vector2Int position, int blockID)
    {
        Chunk chunk = WorldManager.Instance.GetOrCreateChunk(layer, position);
        if (chunk != null)
        {
            chunk.SetBlock(position, blockID);
            SyncBlockClientRpc(layer, position, blockID);
        }
        print(position);
    }

    [ClientRpc]
    private void SyncBlockClientRpc(int layer, Vector2Int position, int blockID)
    {
        Chunk chunk = WorldManager.Instance.GetOrCreateChunk(layer, position);
        if (chunk != null)
        {
            chunk.SetBlock(position, blockID);
        }
    }

    // Save world state (can be called on the server)
    public void SaveWorldState()
    {
        //WorldManager.Instance.SaveWorldState();
    }

    // Load world state (can be called on the server)
    public void LoadWorldState()
    {
        //WorldManager.Instance.LoadWorldState();
    }
}
