using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class Block : MonoBehaviour
{
  public int x;
  public int y;
  public BlockType type;
 
  private Image buttonImage; // Reference to the button's Image component

  public void Init(int x, int y, BlockType type)
  {
    this.x = x;
    this.y = y;
    this.type = type;

    buttonImage = GetComponent<Image>();

    // Assign sprite using array lookup
    if (buttonImage != null)
    {
      buttonImage.sprite = GridManager.Instance.blockSprites[(int)type]; // Set the new sprite
    }
  }

  // Wrapped click handling for better mobile abstraction
  public void OnPointerClick()
  {
    Debug.Log("Block Tapped/Clicked");
    GridManager.Instance.HandleBlockTap(this);
  }
}
