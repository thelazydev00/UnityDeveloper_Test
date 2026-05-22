using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    #region SerializedReferences
    [Header ( "MonoBehaviour References" )]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CameraController cameraController;

    [Header ( "Input Action References" )]
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference mouseLook;
    #endregion

    private void OnEnable ( )
    {
	   EnableMovement ( );
	   EnableJump ( );
    }

    #region EnableFunctions
    public void EnableMovement ( )
    {
	   movement.action.Enable ( );
	   mouseLook.action.Enable ( );
    }

    public void EnableJump ( )
    {
	   jump.action.Enable ( );
    }
    #endregion

    private void OnDisable ( )
    {
	   DisableMovement ( );
	   DisableJump ( );
    }

    #region DisableFunctions
    public void DisableMovement ( )
    {
	   movement.action.Disable ( );
	   mouseLook.action.Disable ( );
    }

    public void DisableJump ( )
    {
	   jump.action.Disable ( );
    }
    #endregion
}
