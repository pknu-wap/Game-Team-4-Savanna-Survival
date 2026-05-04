using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkEntity : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase[] tileAssets;

    public Vector2Int ChunkIndex { get; private set; }

    private void Awake()
    {
        if (tilemap == null)
            tilemap = GetComponentInChildren<Tilemap>();
    }

    public void Initialize(Vector2Int chunkIndex)
    {
        ChunkIndex = chunkIndex;
        transform.position = new Vector3(chunkIndex.x * 20f, chunkIndex.y * 20f, 0f);
        RenderTiles();
    }

    public void ResetChunk()
    {
        tilemap.ClearAllTiles();
        ChunkIndex = Vector2Int.zero;
    }

    private void RenderTiles()
    {
        if (tileAssets == null || tileAssets.Length == 0) return;

        for (int x = 0; x < 20; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tileAssets[0]);
            }
        }
    }
}
