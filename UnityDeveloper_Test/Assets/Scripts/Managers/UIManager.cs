using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Actions that may be used by any other system if required.
    public static System.Action StartPressed;
    public static System.Action RestartPressed;
    public static System.Action ExitPressed;

    public static System.Action<bool> MouseXChanged;
    public static System.Action<bool> MouseYChanged;
    public static System.Action<bool> GravityReferenceChanged;
    public static System.Action<float> MouseSensitivityChanged;

    [Header ( "Serialized References" )]
    [SerializeField] private OptionsUI optionsUI;
    [SerializeField] private CounterUI counterUI;
    [SerializeField] private TimerUI timerUI;
    [SerializeField] private ScoreUI scoreUI;
    [SerializeField] private GameOverUI gameOverUI;

    // Private member variables
    private int currentCount;
    private int maxScore;

    // This is referenced by a Unity Button in the Editor
    public void StartGame ( )
    {
	   StartPressed?.Invoke ( );
    }

    public void Initialize ( int maxScore, PlayerInputRefinements playerInputRefinements, bool cameraGravityReference )
    {
	   this.maxScore = maxScore;
	   optionsUI.Initialize ( playerInputRefinements, cameraGravityReference );

	   optionsUI.MouseXToggle += OnMouseXChanged;
	   optionsUI.MouseYToggle += OnMouseYChanged;
	   optionsUI.MouseSensitivityChanged += OnMouseSensitivityChanged;
	   optionsUI.GravityReferenceToggle += OnGravityReferenceChanged;

	   counterUI.Hide ( );
	   timerUI.Hide ( );
	   scoreUI.Hide ( );
	   gameOverUI.Hide ( );
    }
    #region Methods for OptionsUI
    public void OnMouseXChanged ( bool value )
    {
	   MouseXChanged?.Invoke ( value );
    }

    public void OnMouseYChanged ( bool value )
    {
	   MouseYChanged?.Invoke ( value );
    }

    public void OnMouseSensitivityChanged( float value )
    {
	   MouseSensitivityChanged?.Invoke ( value );
    }

    public void OnGravityReferenceChanged ( bool value )
    {
	   GravityReferenceChanged?.Invoke ( value );
    }
    #endregion

    // This is referenced by a Unity Button in the Editor
    public void RestartGame ( )
    {
	   RestartPressed?.Invoke ( );
    }

    // This is referenced by a Unity Button in the Editor
    public void ExitGame ( )
    {
	   ExitPressed?.Invoke ( );
    }

    #region Methods for Counter
    public void InitializeCounter ( ) => counterUI.Initialize ( "..." );
    public void ShowCounter ( ) => counterUI.Display ( );
    public void UpdateCounter ( string text ) => counterUI.SetText ( text );
    public void HideCounter ( ) => counterUI.Hide ( );
    #endregion

    #region Methods for Timer
    public void InitializeTimer ( ) => timerUI.Initialize ( "..." );
    public void ShowTimer ( ) => timerUI.Display ( );
    public void UpdateTimer ( string text ) => timerUI.SetText ( text );
    public void HideTimer ( ) => timerUI.Hide ( );
    #endregion

    #region Methods for Scores
    public void InitializeScores ( ) => scoreUI.Initialize ( maxScore );
    public void ShowScores ( ) => scoreUI.Display ( );
    public void UpdateScore ( ) => scoreUI.UpdateScore ( );
    public void HideScores ( ) => scoreUI.Hide ( );
    #endregion

    // GameOver handler
    public void GameOver ( float timeRemaining, int score )
    {
	   gameOverUI.Initialize ( );
	   gameOverUI.Display ( timeRemaining, score, maxScore );
    }

    private void OnDestroy ( )
    {
	   optionsUI.OnDestroy ( );

	   optionsUI.MouseXToggle -= OnMouseXChanged;
	   optionsUI.MouseYToggle -= OnMouseYChanged;
	   optionsUI.MouseSensitivityChanged -= OnMouseSensitivityChanged;
	   optionsUI.GravityReferenceToggle -= OnGravityReferenceChanged;
    }
}

// This class handles Counter
[System.Serializable]
public class CounterUI
{
    [SerializeField] private GameObject counterRoot;
    [SerializeField] private TextMeshProUGUI counterText;

    public void Initialize ( string initText = "0" )
    {
	   SetText ( initText );
    }

    public void Display ( )
    {
	   counterRoot.SetActive ( true );
    }

    public void SetText ( string text )
    {
	   counterText.text = text;
    }

    public void Hide ( )
    {
	   counterRoot.SetActive ( false );
    }
}

// This class handles the TimerUI.
[System.Serializable]
public class TimerUI
{
    [SerializeField] private GameObject timerRoot;
    [SerializeField] private TextMeshProUGUI timerText;

    public void Initialize ( string initText = "00:00" )
    {
	   SetText ( initText );
    }

    public void Display ( )
    {
	   timerRoot.SetActive ( true );
    }

    public void SetText ( string text )
    {
	   timerText.text = text;
    }

    public void Hide ( )
    {
	   timerRoot.SetActive ( false );
    }
}

// This handles the ScoreUI
[System.Serializable]
public class ScoreUI
{
    [SerializeField] private GameObject scoreRoot;
    [SerializeField] private TextMeshProUGUI scoreText;

    private int currentScore;
    private int maxScore;

    public void Initialize ( int _maxScore )
    {
	   maxScore = _maxScore;
	   SetText ( $"{currentScore}/{maxScore}" );
    }

    public void Display ( )
    {
	   scoreRoot.SetActive ( true );
    }

    public void UpdateScore ( )
    {
	   currentScore++;
	   SetText ( $"{currentScore}/{maxScore}" );
    }

    public void SetText ( string text )
    {
	   scoreText.text = text;
    }

    public void Hide ( )
    {
	   scoreRoot.SetActive ( false );
    }
}

// Handles the UI data for GameOver Screens
[System.Serializable]
public class GameOverUI
{
    [SerializeField] private GameObject gameOverRoot;
    [SerializeField] private Transform playerWin;
    [SerializeField] private Transform playerLost;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private TextMeshProUGUI textTimeRemaining;
    [SerializeField] private TextMeshProUGUI textScore;

    public void Initialize ( )
    {
	   // No Op
    }

    // Displays the appropriate GameOver Stats based on player score and remainng time
    // 1. GameOver - Player Wins if all collectables are acquired before timer ends.
    // 2. GameOver - PlayerLoss if out of time.
    // 3. GameOver - Player Loss if player doesn't land for certain time while free falling
    public void Display ( float timeRemaining, int score, int maxScore )
    {
	   int minutes = ( int ) ( timeRemaining / 60 );
	   int seconds = ( int ) ( timeRemaining % 60 );

	   textTimeRemaining.text = $"Time Remaining {minutes:00}:{seconds:00}";
	   textScore.text = $"Score {score}/{maxScore}";

	   float collectionPercent = ( score / maxScore );

	   if ( collectionPercent >= 1f )
	   {
		  message.text = $"You Won!";
		  playerWin.gameObject.SetActive ( true );
	   }
	   else if ( timeRemaining <= 0f )
	   {
		  message.text = $"Out of Time!";
		  playerLost.gameObject.SetActive ( true );
	   }
	   else
	   {
		  message.text = $"Out of Bounds!";
		  playerLost.gameObject.SetActive ( true );
	   }

	   gameOverRoot.SetActive ( true );
    }

    public void Hide ( )
    {
	   playerWin.gameObject.SetActive ( false );
	   playerLost.gameObject.SetActive ( false );
	   gameOverRoot.gameObject.SetActive ( false );
    }
}

[System.Serializable]
public class OptionsUI
{
    [SerializeField] private Toggle mouseXToggle;
    [SerializeField] private Toggle mouseYToggle;
    [SerializeField] private Toggle gravityReferenceToggle;
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private TextMeshProUGUI mouseSensitivityText;

    private float mouseSensitivityMin;

    public System.Action<bool> MouseXToggle;
    public System.Action<bool> MouseYToggle;
    public System.Action<bool> GravityReferenceToggle;
    public System.Action<float> MouseSensitivityChanged;

    public void Initialize ( PlayerInputRefinements playerInputRefinements, bool cameraGravityReference )
    {
	   mouseXToggle.isOn = playerInputRefinements.InvertMouseX;
	   mouseYToggle.isOn = playerInputRefinements.InvertMouseY;
	   gravityReferenceToggle.isOn = cameraGravityReference;

	   mouseSensitivitySlider.normalizedValue = playerInputRefinements.MouseSensitivity;

	   Debug.Log ( $"{playerInputRefinements.MouseSensitivity}, {mouseSensitivitySlider.value}" );

	   mouseSensitivityMin = playerInputRefinements.MouseSensitivityMin;

	   mouseSensitivityText.text = ( playerInputRefinements.MouseSensitivity / mouseSensitivityMin ).ToString ( "F0" );

	   mouseXToggle.onValueChanged.AddListener ( OnMouseXToggle );
	   mouseYToggle.onValueChanged.AddListener ( OnMouseYToggle );
	   gravityReferenceToggle.onValueChanged.AddListener ( OnGravityReferenceToggle );

	   mouseSensitivitySlider.onValueChanged.AddListener ( OnMouseSensitivitySliderChanged );
    }

    private void OnMouseXToggle ( bool value )
    {
	   MouseXToggle?.Invoke ( value );
    }

    private void OnMouseYToggle ( bool value )
    {
	   MouseYToggle?.Invoke ( value );
    }

    private void OnGravityReferenceToggle ( bool value )
    {
	   GravityReferenceToggle?.Invoke ( value );
    }

    private void OnMouseSensitivitySliderChanged ( float value )
    {
	   mouseSensitivityText.text = ( value ).ToString ( "F0" );

	   MouseSensitivityChanged?.Invoke ( value * mouseSensitivityMin );
    }

    public void OnDestroy ( )
    {
	   mouseXToggle.onValueChanged.RemoveAllListeners ( );
	   mouseYToggle.onValueChanged.RemoveAllListeners ( );
	   gravityReferenceToggle.onValueChanged.RemoveAllListeners ( );

	   mouseSensitivitySlider.onValueChanged.RemoveAllListeners ( );
    }
}