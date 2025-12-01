using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class GridManager : MonoBehaviour
{
  public static GridManager Instance;

  [Header("Grid Size")]
  public int width = 6;
  public int height = 5;

  [Header("Pieces")]
  public Block blockPrefab;
  public Transform gridRoot;

  [Header("Block Sprites (ordered by BlockType enum)")]
  public Sprite[] blockSprites;

  private Block[,] grid; // storing grid on a 2d array for ease of use
  private bool isRefilling = false; // Prevent interaction during refill

  void Awake()
  {
    Instance = this;
  }

  // Start is called before the first frame update
  void Start()
  {
    GenerateGrid();
  }

  //TO_DO: since the proposed grid is small, using instantiate and destroy dont afect performance and speed much, implement a pool for larger grids and better performance
  public void ResetGrid()
  {
    if (grid != null)
    {
      foreach (var b in grid)
      {
        if (b != null)
        {
          Destroy(b.gameObject);
        }
      }
    }

    GenerateGrid();
  }

  void GenerateGrid()
  {
    grid = new Block[width, height]; // init grid

    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        SpawnBlock(x, y); // spawn blocks at random
      }
    }
  }

  void SpawnBlock(int x, int y)
  {
    int correctedX = x * 128;
    int correctedY = y * 112;
    Block b = Instantiate(blockPrefab, gridRoot);

    //BlockType type = (BlockType)Random.Range(0, System.Enum.GetValues(typeof(BlockType)).Length);
    BlockType type = (BlockType)Random.Range(0, blockSprites.Length);

    b.Init(x, y, type);

    // Map (0,0) top-left --> Unity Y reversed
    b.transform.localPosition = new Vector3(correctedX, -correctedY, 0);

    grid[x, y] = b;
  }

  // ============================================================================
  // BLOCK TAP HANDLER
  // ============================================================================
  public void HandleBlockTap(Block b)
  {
    // Prevent interaction during refill
    if (isRefilling)
    {
      return;
    }

    if (!GameManager.Instance.CanPlay())
    {
      return;
    }

    var cluster = FindCluster(b.x, b.y, b.type);

    // Do not allow single-block removals (Single blocks are ignored because removing <2 pieces is unsatisfying and results in accidental waste of moves)
    if (cluster.Count < 2)
    {
      return;
    }

    // Award score = size of cluster (linear scoring)
    GameManager.Instance.AddScore(cluster.Count);

    // Spend a move only on valid removal
    GameManager.Instance.SpendMove();

    // Remove collected blocks
    foreach (Block c in cluster)
    {
      Destroy(grid[c.x, c.y].gameObject);
      grid[c.x, c.y] = null;
    }

    StartCoroutine(RefillRoutine());
  }

  // ============================================================================
  // FLOOD FILL (4-directional"orthogonal") only
  // ============================================================================  
  List<Block> FindCluster(int startX, int startY, BlockType type)
  {
    List<Block> result = new List<Block>();
    bool[,] visited = new bool[width, height];

    void DFS(int x, int y) // Depth-First Search flood fill, Nested local function: making method signature shorter and clean approach for a recursive helper
    {
      if (x < 0 || x >= width || y < 0 || y >= height) // check if block is out of bounds in the grid
      {
        return;
      }

      if (visited[x, y]) // check if the block was already processed
      {
        return;
      }

      var b = grid[x, y];

      if (b == null || b.type != type) // check if the block is valid and is the same type/color as the tapped/clicked block
      {
        return;
      }

      visited[x, y] = true; // mark this block as processed
      result.Add(b); // add it to the cluster of blocks

      // 4-directional only (no diagonals)
      // Diagonal adjacency is ignored because it creates larger clusters too easily and breaks expected match-puzzle behavior
      DFS(x + 1, y);
      DFS(x - 1, y);
      DFS(x, y + 1);
      DFS(x, y - 1);
    }

    DFS(startX, startY);
    return result;
  }

  // ============================================================================
  // REFILL + GRAVITY (instant, no animation)
  // ============================================================================
  IEnumerator RefillRoutine()
  {
    isRefilling = true;

    // Wait 1 second BEFORE refilling (game design requirement PDF instructions)
    yield return new WaitForSeconds(1f);

    for (int x = 0; x < width; x++)
    {
      int writeY = height - 1; // Start from top (grid uses top-down logic)

      // Move down all non-null blocks
      for (int y = height - 1; y >= 0; y--)
      {
        if (grid[x, y] != null)
        {
          if (writeY != y)
          {
            grid[x, writeY] = grid[x, y];
            grid[x, y] = null;

            grid[x, writeY].x = x;
            grid[x, writeY].y = writeY;

            int correctedX = x * 128;
            int correctedY = writeY * 112;

            grid[x, writeY].transform.localPosition =
                new Vector3(correctedX, -correctedY, 0);
          }

          writeY--;
        }
      }

      // Spawn new blocks for remaining empty spaces
      for (int y = writeY; y >= 0; y--)
      {
        SpawnBlock(x, y);
      }
    }

    isRefilling = false;
  }




}
