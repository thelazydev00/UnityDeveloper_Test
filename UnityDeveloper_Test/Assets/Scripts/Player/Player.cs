using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header ( "Serialized References" )]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerPreview playerPreview;

    [SerializeField] private float countdownTimeToDeath = 5f;

    public System.Action PlayerDied;

    // Booleans to handle flags and states
    private bool predictingDeath;
    public bool MaySurvive { get; set; }

    public void InitializePlayer ( )
    {
	   playerMovement.InitializePlayerMovement ( cameraController );
	   cameraController.InitializeCamera ( );
	   playerPreview.InitializePlayerPreview ( );
	   MaySurvive = false;
	   predictingDeath = false;

	   playerMovement.PredictDeath += HandleDeathPrediction;
    }

    public void AllowPlayerMovement ( bool isAllowed )
    {
	   playerMovement.AllowPlayerMovement = isAllowed;
	   cameraController.AllowCameraMovement ( isAllowed );
    }

    // Handles the prediction of death.
    private void HandleDeathPrediction ( bool _predictDeath )
    {
	   if ( !predictingDeath && _predictDeath )
	   {
		  StartCoroutine ( CountdownToDeath ( ) );
		  return;
	   }

	   if ( !_predictDeath )
	   {
		  MaySurvive = true;
		  return;
	   }
    }

    [SerializeField] float timer;

    IEnumerator CountdownToDeath ( )
    {
	   predictingDeath = true;
	   MaySurvive = false;
	   timer = countdownTimeToDeath;

	   while ( timer > 0 && !MaySurvive )
	   {
		  timer = Mathf.Clamp ( timer - Time.deltaTime, 0, countdownTimeToDeath );

		  yield return null;
	   }

	   if ( timer == 0f )
	   {
		  PlayerDied?.Invoke ( );
	   }
	   predictingDeath = false;
    }

    private void OnDestroy ( )
    {
	   playerMovement.PredictDeath -= HandleDeathPrediction;
    }
}
