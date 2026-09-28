using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    [SerializeField] Transform groundPlane;
    [SerializeField] GameTile tilePrefab;
    Queue<GameTile> searchFrontier = new Queue<GameTile>();
    GameTileContentFactory contentFactory;

    public int SpawnPointCount => spawnPoints.Count;

    Vector2Int boardSize;
    GameTile[] tiles;

    public void Initialize(Vector2Int newSize, GameTileContentFactory factory)
    {
        contentFactory = factory;
        boardSize = newSize;
        groundPlane.localScale = new Vector3(newSize.x, newSize.y, 1f);

        Vector2 center = new Vector2(
            (newSize.x - 1) * 0.5f, (newSize.y - 1) * 0.5f
        );

        tiles = new GameTile[newSize.x * newSize.y];
        for (int i = 0, z = 0; z < newSize.y; z++)
        {
            for (int x = 0; x < newSize.x; x++, i++)
            {
                GameTile tile = tiles[i] = Instantiate(tilePrefab);
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(
                    x - center.x, 0f, z - center.y
                );

                if (x > 0)
                {
                    GameTile.LinkEastWest(tile, tiles[i - 1]);
                }
                if (z > 0)
                {
                    GameTile.LinkNorthSouth(tile, tiles[i - newSize.x]);
                }

                tile.Content = contentFactory.Get(GameTileContentType.Empty);
            }
        }
        Clear();
    }

    bool FindPaths()
    {
        foreach (GameTile tile in tiles)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.BecomeDestination();
                searchFrontier.Enqueue(tile);
            }
            else
            {
                tile.ClearPath();
            }
        }

        if (searchFrontier.Count == 0)
        {
            return false;
        }

        while (searchFrontier.Count > 0)
        {
            GameTile tile = searchFrontier.Dequeue();
            if (tile != null)
            {
                searchFrontier.Enqueue(tile.GrowPathNorth());
                searchFrontier.Enqueue(tile.GrowPathEast());
                searchFrontier.Enqueue(tile.GrowPathSouth());
                searchFrontier.Enqueue(tile.GrowPathWest());
            }
        }

        foreach (GameTile tile in tiles)
        {
            if (!tile.HasPath)
            {
                return false;
            }
        }

        if (showPaths)
        {
            foreach (GameTile tile in tiles)
            {
                tile.ShowPath();
            }
        }
        return true;
    }

    public GameTile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1))
        {
            int x = (int)(hit.point.x + boardSize.x * 0.5f);
            int z = (int)(hit.point.z + boardSize.y * 0.5f);
            if (x >= 0 && x < boardSize.x && z >= 0 && z < boardSize.y)
            {
                return tiles[x + z * boardSize.x];
            }
        }
        return null;
    }
    public void ToggleDestination(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Destination)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Destination);
                FindPaths();
            }
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Destination);
            FindPaths();
        }
    }
    public void ToggleWall(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Wall);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
    }

    bool showPaths;

    public bool ShowPaths
    {
        get => showPaths;
        set
        {
            showPaths = value;
            if (showPaths)
            {
                foreach (GameTile tile in tiles)
                {
                    tile.ShowPath();
                }
            }
            else
            {
                foreach (GameTile tile in tiles)
                {
                    tile.HidePath();
                }
            }
        }
    }

    [SerializeField] Texture2D gridTexture;

    bool showGrid;

    public bool ShowGrid
    {
        get => showGrid;
        set
        {
            showGrid = value;
            Material groundMaterial = groundPlane.GetComponent<MeshRenderer>().material;
            if (showGrid)
            {
                groundMaterial.mainTexture = gridTexture;
                groundMaterial.SetTextureScale("_BaseMap", new Vector2(boardSize.x, boardSize.y));
            }
            else
            {
                groundMaterial.mainTexture = null;
            }
        }
    }

    List<GameTile> spawnPoints = new List<GameTile>();
    public void ToggleSpawnPoint(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.SpawnPoint)
        {
            if (spawnPoints.Count > 1)
            {
                spawnPoints.Remove(tile);
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
            }
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.SpawnPoint);
            spawnPoints.Add(tile);
        }
    }

    public GameTile GetSpawnPoint(int index)
    {
        return spawnPoints[index];
    }



    public void ToggleTower(GameTile tile, TowerType towerType)
    {
        if (tile.Content.Type == GameTileContentType.Tower)
        {
            updatingContent.Remove(tile.Content);
            if (((Tower)tile.Content).TowerType == towerType)
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
            else
            {
                tile.Content = contentFactory.Get(towerType);
                updatingContent.Add(tile.Content);
            }
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(towerType);
            if (FindPaths())
            {
                updatingContent.Add(tile.Content);
            }
            else
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
        else if (tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = contentFactory.Get(towerType);
            updatingContent.Add(tile.Content);
        }
    }

    List<GameTileContent> updatingContent = new List<GameTileContent>();

    public void GameUpdate()
    {
        for (int i = 0; i < updatingContent.Count; i++)
        {
            updatingContent[i].GameUpdate();
        }
    }

    public void Clear()
    {
        foreach (GameTile tile in tiles)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
        }
        spawnPoints.Clear();
        updatingContent.Clear();
        ToggleDestination(tiles[tiles.Length / 2]);
        ToggleSpawnPoint(tiles[0]);
        ShowPaths = showPaths;
    }

}