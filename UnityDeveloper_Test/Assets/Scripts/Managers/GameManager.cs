using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header ( "Serialized References" )]
    [SerializeField] private InputActionReference exit; // To Do Implement a Pause/Exit Menu
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Player player;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GravityManipulator gravityManipulator;
    [SerializeField] private CollectablesManager collectablesManager;
    [SerializeField] private float roundTimeMax = 120f;

    // Flag for when the game is running
    private bool isGameRunning = false;

    // Float value used to evaluate collection progress
    private float collectionPercent = 0f;

    private void Awake ( )
    {
	   UIManager.StartPressed += StartCountdown;
    }

    private void OnEnable ( )
    {
	   exit.action.Enable ( );
    }

    // This Start function ideally provides a single entry point for the flow of logic
    private void Start ( )
    {
	   player.InitializePlayer ( );
	   player.PlayerDied += ( ) => isGameRunning = false;
	   //gravityManipulator.InitializeGravityManipulator ( );
	   collectablesManager.Initialize ( this );
	   collectionPercent = 0f;
	   uiManager.Initialize ( collectablesManager.Collectibles.Count );
    }

    // Method to start a 3 second countdown before the round starts
    private void StartCountdown ( )
    {
	   gravityManipulator.InitializeGravityManipulator ( );
	   StartCoroutine ( CountDown ( ) );
	   UIManager.StartPressed -= StartCountdown;
    }

    // Actual method that begins the game after countdown
    private void StartGame ( )
    {
	   StartCoroutine ( Timer ( ) );
    }

    IEnumerator CountDown ( )
    {
	   float count = 3f;

	   uiManager.InitializeCounter ( );
	   uiManager.ShowCounter ( );

	   while ( count > 0f )
	   {
		  count -= Time.deltaTime;
		  string text = Mathf.CeilToInt ( count ).ToString ( );
		  uiManager.UpdateCounter ( text );
		  yield return null;
	   }

	   yield return null;

	   uiManager.UpdateCounter ( "Go!!!" );

	   yield return new WaitForSeconds ( 1f );

	   uiManager.HideCounter ( );

	   StartGame ( );
    }

    // Timer to evaluate one of the GameOver conditions. If the timer reaches 0 before the player collects all the 
    //collectibles the game ends with the player defeated
    IEnumerator Timer ( )
    {
	   isGameRunning = true;
	   
	   player.AllowPlayerMovement ( isGameRunning );
	   gravityManipulator.AllowGravityChange ( isGameRunning );

	   uiManager.InitializeTimer ( );
	   uiManager.ShowTimer ( );

	   uiManager.InitializeScores ( );
	   uiManager.ShowScores ( );

	   float timer = roundTimeMax;

	   while ( timer > 0 && isGameRunning )
	   {
		  timer -= Time.deltaTime;

		  int minutes = ( int ) ( timer / 60f );
		  int seconds = ( int ) ( timer % 60f );

		  string text = $"{minutes:00}:{seconds:00}";
		  uiManager.UpdateTimer ( text );

		  yield return null;
	   }

	   isGameRunning = false;

	   yield return new WaitForSeconds ( 0.5f );

	   player.AllowPlayerMovement ( isGameRunning );
	   gravityManipulator.AllowGravityChange( isGameRunning );

	   yield return new WaitForSeconds ( 0.5f );

	   uiManager.HideScores ( );

	   UIManager.RestartPressed += OnRestart;
	   UIManager.ExitPressed += OnExitGame;

	   uiManager.GameOver ( timer, collectablesManager.NumCollected );
    }

    // This method updates the player scores when a colectable is picked up
    public void UpdateScore ( float currentCount, float totalCount )
    {
	   uiManager.UpdateScore ( );
	   collectionPercent = currentCount / totalCount;

	   if ( collectionPercent >= 1f )
	   {
		  isGameRunning = false;
	   }
    }

    // After GameOver, the player may press a button to restart the game. This method handles it.
    private void OnRestart ( )
    {
	   Scene s = SceneManager.GetActiveScene ( );

	   SceneManager.LoadSceneAsync ( s.buildIndex, LoadSceneMode.Single );
    }

    // After GameOver, the player may press a button to exit the game. This method handles it.
    private void OnExitGame ( )
    {
	   Application.Quit ( );
    }

    private void OnDisable ( )
    {
	   exit.action.Disable ( );
    }

    // Incase we want to do something right after app is closed.
    //private void OnApplicationQuit ( )
    //{

    //}

    private void OnDestroy ( )
    {
	   UIManager.RestartPressed -= OnRestart;
	   UIManager.ExitPressed -= OnExitGame;
	   player.PlayerDied -= ( ) => isGameRunning = false;
    }
}
