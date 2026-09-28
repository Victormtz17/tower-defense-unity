using UnityEngine;

[SelectionBase]
public class GameTileContent : MonoBehaviour
{
    [SerializeField] GameTileContentType type;

    public GameTileContentType Type => type;

    GameTileContentFactory originFactory;

    public virtual void GameUpdate() { }

    public GameTileContentFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory!");
            originFactory = value;
        }
    }

    public void Recycle()
    {
        originFactory.Reclaim(this);
    }

    public bool BlocksPath =>
    Type == GameTileContentType.Wall || Type == GameTileContentType.Tower;

}
