using UnityEngine;

[System.Serializable]
public class Tile
{
    public int x;
    public int z;
    public bool isWall;
    public bool isDestination;
    public GameObject visualObject; // the cube/plane we'll spawn for this tile

    public Tile(int x, int z)
    {
        this.x = x;
        this.z = z;
        isWall = false;
        isDestination = false;
    }
}
