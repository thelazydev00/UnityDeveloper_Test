using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerPreview playerPreview;

    private void Start ( )
    {
	   InitializePlayer ( );
    }

    public void InitializePlayer ( )
    {
	   playerMovement.InitializePlayerMovement ( );
	   playerPreview.InitializePlayerPreview ( );
    }
}
