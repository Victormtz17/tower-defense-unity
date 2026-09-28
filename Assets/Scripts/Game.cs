using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] Vector2Int boardSize = new Vector2Int(11, 11);
    [SerializeField] GameBoard board;
    [SerializeField] GameTileContentFactory contentFactory;
    [SerializeField] WarFactory warFactory;
    [SerializeField] GameScenario scenario;
    [SerializeField, Range(0, 100)] int startingPlayerHealth = 10;
    [SerializeField, Range(1f, 10f)] float playSpeed = 1f;
    const float pausedTimeScale = 0f;


    int playerHealth;

    GameScenario.State activeScenario;
    GameBehaviorCollection enemies = new GameBehaviorCollection();
    GameBehaviorCollection nonEnemies = new GameBehaviorCollection();

    static Game instance;

 
    TowerType selectedTowerType;

    Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

   

    void OnEnable()
    {
        instance = this;
    }

    public static Shell SpawnShell()
    {
        Shell shell = instance.warFactory.Shell;
        instance.nonEnemies.Add(shell);
        return shell;
    }

    void Awake()
    {
        playerHealth = startingPlayerHealth;
        board.Initialize(boardSize, contentFactory);
        board.ShowGrid = true;
        activeScenario = scenario.Begin();
    }

    void OnValidate()
    {
        boardSize.x = Mathf.Max(boardSize.x, 2);
        boardSize.y = Mathf.Max(boardSize.y, 2);
    }
    void Update()
    {
       
            if (Input.GetMouseButtonDown(0))
            {
                HandleTouch();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                HandleAlternativeTouch();
            }

        if (Input.GetKeyDown(KeyCode.V))
        {
            board.ShowPaths = !board.ShowPaths;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            board.ShowGrid = !board.ShowGrid;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedTowerType = TowerType.Laser;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedTowerType = TowerType.Mortar;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = Time.timeScale > pausedTimeScale ? pausedTimeScale : playSpeed;
        }
        else if (Time.timeScale > pausedTimeScale)
        {
            Time.timeScale = playSpeed;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            BeginNewGame();
        }
        if (playerHealth <= 0 && startingPlayerHealth > 0)
        {
            Debug.Log("Defeat! Health was: " + playerHealth);
            BeginNewGame();
        }

        if (!activeScenario.Progress() && enemies.IsEmpty && playerHealth > 0)
        {
            Debug.Log("Victory! Health remaining: " + playerHealth);
            BeginNewGame();
            activeScenario.Progress();
        }




        enemies.GameUpdate();
        Physics.SyncTransforms();
        board.GameUpdate();
        nonEnemies.GameUpdate();

    }

    void HandleTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                board.ToggleTower(tile, selectedTowerType);
            }
            else
            {
                board.ToggleWall(tile);
            }
        }
    }

    void HandleAlternativeTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                board.ToggleDestination(tile);
            }
            else
            {
                board.ToggleSpawnPoint(tile);
            }
        }
    }

    public static void SpawnEnemy(EnemyFactory factory, EnemyType type)
    {
        GameTile spawnPoint = instance.board.GetSpawnPoint(Random.Range(0, instance.board.SpawnPointCount));
        Enemy enemy = factory.Get(type);
        enemy.SpawnOn(spawnPoint);
        instance.enemies.Add(enemy);
    }
    public static Explosion SpawnExplosion()
    {
        Explosion explosion = instance.warFactory.Explosion;
        instance.nonEnemies.Add(explosion);
        return explosion;
    }

    void BeginNewGame()
    {
        playerHealth = startingPlayerHealth;
        enemies.Clear();
        nonEnemies.Clear();
        board.Clear();
        activeScenario = scenario.Begin();
    }
    public static void EnemyReachedDestination()
    {
        instance.playerHealth -= 1;
    }
}