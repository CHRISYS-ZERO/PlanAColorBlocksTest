using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;

  [Header("Game State")]
  public int score = 0;
  public int moves = 5;

  [Header("UI")]
  [SerializeField] private TMP_Text scoreText;
  [SerializeField] private TMP_Text movesText;
  [SerializeField] private GameObject gameOverPanel;

  public bool IsGameOver => moves <= 0;

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    UpdateUI();
  }

  /// <summary>
  /// Called only when a VALID cluster is removed.
  /// Never spend moves on invalid or single-block taps.
  /// </summary>
  public void SpendMove()
  {
    moves--;
    UpdateUI();

    if (moves <= 0)
      GameOver();
  }

  public void AddScore(int amount)
  {
    score += amount;
    UpdateUI();
  }

  public void GameOver()
  {
    // Disable all gameplay interaction.
    gameOverPanel.SetActive(true);
  }

  public bool CanPlay()
  {
    return !IsGameOver;
  }

  public void Replay()
  {
    score = 0;
    moves = 5;

    gameOverPanel.SetActive(false);

    UpdateUI();

    // Regenerate the grid.
    GridManager.Instance.ResetGrid();
  }

  public void UpdateUI()
  {
    scoreText.text = score.ToString();
    movesText.text = moves.ToString();
  }
}
