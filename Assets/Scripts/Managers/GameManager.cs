using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;

  [Header("Game State")]
  public int score;
  public int moves = 5;

  [Header("UI")]
  [SerializeField] private TMP_Text scoreText;
  [SerializeField] private TMP_Text movesText;
  [SerializeField] private GameObject gameOverPanel;

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    UpdateUI();
  }

  public void MakeMove()
  {
    if (moves <= 0) return;

    moves--;
    score += 10;

    UpdateUI();

    if (moves == 0)
      GameOver();
  }

  public void GameOver()
  {
    gameOverPanel.SetActive(true);
  }

  public void Replay()
  {
    score = 0;
    moves = 5;
    gameOverPanel.SetActive(false);
    UpdateUI();
  }

  private void UpdateUI()
  {
    scoreText.text = score.ToString();
    movesText.text = moves.ToString();
  }
}
