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

    [Header ( "Input Refinements" )]
    [Range ( 0.01f, 1f )] public float mouseSensitivity = 0.1f;
    public float MouseSensitivity => _mouseSensitivity < 0 ? mouseSensitivity : _mouseSensitivity;
    
    public bool InvertMouseX => inversionVector.x < 0;

    public bool InvertMouseY => inversionVector.y < 0;

    private static Vector2 inversionVector = Vector2.one;

    public PlayerInputRefinements PlayerInputRefinements => new ( MouseSensitivity, mouseSensitivityMin, InvertMouseX, InvertMouseY );
    #endregion

    private float mouseSensitivityMin = 0.01f;
    private static float _mouseSensitivity = -1f;

    public static System.Action<Vector2> MovementAction;
    public static System.Action JumpAction;
    public static System.Action<Vector2> MouseLookAction;

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

	   mouseLook.action.started += MouseLookAction_started;
	   mouseLook.action.performed += MouseLookAction_performed;
	   mouseLook.action.canceled += MouseLookAction_canceled;
    }

    public void SetInvertMouseX( bool invertMouseX )
    {
	   inversionVector.x = invertMouseX ? -1f : 1f;
    }

    public void SetInvertMouseY ( bool invertMouseY )
    {
	   inversionVector.y = invertMouseY ? -1f : 1f;
    }

    public void SetMouseSensitivity ( float mouseSensitivity )
    {
	   this.mouseSensitivity = mouseSensitivity;
	   _mouseSensitivity = mouseSensitivity;
    }

    private void MouseLookAction_started ( InputAction.CallbackContext obj )
    {
	   
    }

    private void MouseLookAction_performed ( InputAction.CallbackContext obj )
    {
	   Vector2 mouseLook = obj.ReadValue<Vector2> ( );

	   mouseLook.x *= inversionVector.x;
	   mouseLook.y *= inversionVector.y;

	   MouseLookAction?.Invoke ( mouseLook * MouseSensitivity );
    }

    private void MouseLookAction_canceled ( InputAction.CallbackContext obj )
    {
	   Vector2 mouseLook = obj.ReadValue<Vector2> ( );

	   mouseLook.x *= inversionVector.x;
	   mouseLook.y *= inversionVector.y;

	   MouseLookAction?.Invoke ( mouseLook * MouseSensitivity );
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

public struct PlayerInputRefinements
{
    private float mouseSensitivity;
    public float MouseSensitivity => mouseSensitivity;

    private float mouseSensitivityMin;
    public float MouseSensitivityMin => mouseSensitivityMin;

    private bool invertMouseX;
    public bool InvertMouseX => invertMouseX;

    private bool invertMouseY;
    public bool InvertMouseY => invertMouseY;

    public PlayerInputRefinements ( float mouseSensitivity, float mouseSensitivityMin, bool invertMouseX, bool invertMouseY )
    {
	   this.mouseSensitivity = mouseSensitivity;
	   this.mouseSensitivityMin = mouseSensitivityMin;
	   this.invertMouseX = invertMouseX;
	   this.invertMouseY = invertMouseY;
    }
}
