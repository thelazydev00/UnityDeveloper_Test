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

    public static System.Action<Vector2> MovementAction;
    public static System.Action JumpAction;

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

	   movement.action.started += MovementAction_started;
	   movement.action.performed += MovementAction_performed;
	   movement.action.canceled += MovementAction_canceled;
    }

    public void EnableJump ( )
    {
	   jump.action.Enable ( );

	   jump.action.started += JumpAction_started;
	   jump.action.performed += JumpAction_performed;
	   jump.action.canceled += JumpAction_canceled;
    }
    #endregion

    private void JumpAction_started ( InputAction.CallbackContext obj )
    {

    }

    private void JumpAction_performed ( InputAction.CallbackContext obj )
    {
	   if ( obj.ReadValue<float> ( ) > 0f )
	   {
		  JumpAction?.Invoke ( );
	   }
    }

    private void JumpAction_canceled ( InputAction.CallbackContext obj )
    {

    }

    private void MovementAction_started ( InputAction.CallbackContext obj )
    {
	   MovementAction?.Invoke ( obj.ReadValue<Vector2> ( ) );
    }

    private void MovementAction_performed ( InputAction.CallbackContext obj )
    {
	   MovementAction?.Invoke ( obj.ReadValue<Vector2> ( ) );
    }

    private void MovementAction_canceled ( InputAction.CallbackContext obj )
    {
	   MovementAction?.Invoke ( obj.ReadValue<Vector2> ( ) );
    }

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
