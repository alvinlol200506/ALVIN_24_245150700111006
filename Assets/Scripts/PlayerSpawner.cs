using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // Prefab pemain
    public GameObject finishPrefab; // Prefab finish
    public Camera mainCamera; // Kamera utama
    public int[,] mazeGrid; // Representasi grid labirin: 0 = jalan, 1 = tembok
    public Vector2 cellSize = new Vector2(2f, 2f); // Ukuran setiap cell di grid
    public Vector2 gridOffset = new Vector2(-5f, -5f); // Offset untuk memindahkan grid ke koordinat dunia
    public float minimumDistanceFromPlayer = 4f; // Jarak minimal antara player dan finish

    private Vector3 playerSpawnPosition;
    private GameObject playerInstance; // Instance player yang di-spawn

    public void StartPlayer()
    {
        // Contoh grid labirin (0 = jalan, 1 = tembok)
        mazeGrid = new int[,]
        {
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
        };

        // Spawn player secara random
        playerSpawnPosition = GetRandomSpawnPosition();
        playerInstance = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);

        // Spawn finish secara random, tapi tidak terlalu dekat dengan player
        Vector3 finishPosition = GetRandomFinishPosition(playerSpawnPosition);
        Instantiate(finishPrefab, finishPosition, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Cari semua posisi valid dalam grid
        System.Collections.Generic.List<Vector2Int> validPositions = new System.Collections.Generic.List<Vector2Int>();
        for (int x = 0; x < mazeGrid.GetLength(0); x++)
        {
            for (int y = 0; y < mazeGrid.GetLength(1); y++)
            {
                if (mazeGrid[x, y] == 0) // 0 = Jalan
                {
                    validPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        // Pilih posisi random dari daftar posisi valid
        Vector2Int randomPosition = validPositions[Random.Range(0, validPositions.Count)];

        // Hitung posisi di dunia (tengah tile) dengan offset
        float worldX = randomPosition.x * cellSize.x + (cellSize.x / 2f) + gridOffset.x;
        float worldZ = randomPosition.y * cellSize.y + (cellSize.y / 2f) + gridOffset.y;

        // Set Y menjadi 2 agar player tidak berada di bawah tanah
        return new Vector3(worldX, 0.5f, worldZ);
    }

    private Vector3 GetRandomFinishPosition(Vector3 playerPosition)
    {
        System.Collections.Generic.List<Vector2Int> validPositions = new System.Collections.Generic.List<Vector2Int>();
        for (int x = 0; x < mazeGrid.GetLength(0); x++)
        {
            for (int y = 0; y < mazeGrid.GetLength(1); y++)
            {
                if (mazeGrid[x, y] == 0)
                {
                    validPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        Vector3 finishPosition;
        do
        {
            Vector2Int randomPosition = validPositions[Random.Range(0, validPositions.Count)];

            float worldX = randomPosition.x * cellSize.x + (cellSize.x / 2f) + gridOffset.x;
            float worldZ = randomPosition.y * cellSize.y + (cellSize.y / 2f) + gridOffset.y;

            finishPosition = new Vector3(worldX, 0f, worldZ); // Set Y = 2
        } while (Vector3.Distance(finishPosition, playerPosition) < minimumDistanceFromPlayer);

        return finishPosition;
    }

}
