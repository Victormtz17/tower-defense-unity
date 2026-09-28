using UnityEngine;

public class GameTile : MonoBehaviour
{

    [SerializeField] Transform arrowTransform;

    GameTile north, east, south, west;
    GameTile nextOnPath;
    int distance;
    public GameTile NextTileOnPath => nextOnPath;
    public Vector3 ExitPoint { get; private set; }
    public Direction PathDirection { get; private set; }

    public static void LinkEastWest(GameTile eastTile, GameTile westTile)
    {
        Debug.Assert(westTile.east == null && eastTile.west == null, "Neighbor already set!");
        westTile.east = eastTile;
        eastTile.west = westTile;
    }

    public static void LinkNorthSouth(GameTile northTile, GameTile southTile)
    {
        Debug.Assert(southTile.north == null && northTile.south == null, "Neighbor already set!");
        southTile.north = northTile;
        northTile.south = southTile;
    }

    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
        ExitPoint= transform.localPosition;
    }

    public bool HasPath => distance != int.MaxValue;

    GameTile GrowPathTo(GameTile neighbor, Direction direction)
    {
        if (!HasPath || neighbor == null || neighbor.HasPath)
        {
            return null;
        }
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector();
        neighbor.PathDirection = direction;
        return neighbor.Content.BlocksPath ? null : neighbor;
    }

    public GameTile GrowPathNorth() => GrowPathTo(north, Direction.South);
    public GameTile GrowPathEast() => GrowPathTo(east, Direction.West);
    public GameTile GrowPathSouth() => GrowPathTo(south, Direction.North);
    public GameTile GrowPathWest() => GrowPathTo(west, Direction.East);

    static Quaternion
    northRotation = Quaternion.Euler(90f, 0f, 0f),
    eastRotation = Quaternion.Euler(90f, 90f, 0f),
    southRotation = Quaternion.Euler(90f, 180f, 0f),
    westRotation = Quaternion.Euler(90f, 270f, 0f);

    public void ShowPath()
    {
        if (distance == 0)
        {
            arrowTransform.gameObject.SetActive(false);
            return;
        }
        arrowTransform.gameObject.SetActive(true);
        arrowTransform.localRotation =
            nextOnPath == north ? northRotation :
            nextOnPath == east ? eastRotation :
            nextOnPath == south ? southRotation :
            westRotation;
    }

    GameTileContent content;

    public GameTileContent Content
    {
        get => content;
        set
        {
            if (content != null)
            {
                Destroy(content.gameObject);
            }
            content = value;
            content.transform.SetParent(transform, false);
        }
    }

    public void HidePath()
    {
        arrowTransform.gameObject.SetActive(false);
    }

  
}