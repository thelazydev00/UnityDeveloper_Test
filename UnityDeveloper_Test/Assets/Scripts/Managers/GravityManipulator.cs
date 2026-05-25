using UnityEngine;
using UnityEngine.InputSystem;

// The gravity manipulator script that changes Gravity according to the player input
public class GravityManipulator : MonoBehaviour
{
    [Header ( "Serialized References" )]
    [SerializeField] private Player player;
    [SerializeField] private InputActionReference gravityChange;
    [SerializeField] private InputActionReference gravitySet;

    private Vector3 gravityDirection = Vector3.down;
    private Vector3 targetGravityDirection = Vector3.down;

    public static System.Func<Vector3, Vector3, bool> PreviewGravityDirectionChange;
    public static System.Action<Vector3, Vector3> GravityDirectionChanged;

    private Vector3 targetUp = Vector3.up;
    private Vector3 targetForward = Vector3.forward;
    private Vector3 targetRight = Vector3.right;

    private Vector3 up;
    private Vector3 forward;
    private Vector3 right;

    private bool allowGravityChange = false;
    private bool allowGravitySet = true;

    private void OnEnable ( )
    {
	   EnableGravityModification ( );
    }

    public void EnableGravityModification ( )
    {
	   gravityChange.action.Enable ( );
	   gravitySet.action.Enable ( );

	   gravityChange.action.started += GravityChangeAction_started;
	   gravityChange.action.performed += GravityChangeAction_performed;
	   gravityChange.action.canceled += GravityChangeAction_canceled;

	   gravitySet.action.started += GravitySetAction_started;
	   gravitySet.action.performed += GravitySetAction_performed;
	   gravitySet.action.canceled += GravitySetAction_canceled;
    }

    public void InitializeGravityManipulator ( )
    {
	   targetUp = player.transform.up;
	   targetForward = player.transform.forward;

	   UpdateTargetAxes ( );
	   UpdatePlayerGravityAxesAfterPreview ( );
    }

    public void AllowGravityChange ( bool allowed )
    {
	   allowGravityChange = allowed;
    }

    // This updates the actual Axes of the player constrained to the Cube formed by the world/scene
    private void UpdateTargetAxes ( )
    {
	   targetGravityDirection = SnapToAxis ( targetGravityDirection );

	   targetUp = -targetGravityDirection;

	   targetForward = Vector3.ProjectOnPlane ( player.transform.forward, targetUp ).normalized;

	   if ( targetForward.sqrMagnitude < 0.01f )
	   {
		  targetForward = Vector3.Cross ( targetUp, Vector3.right );

		  if ( targetForward.sqrMagnitude < 0.01f )
			 targetForward = Vector3.Cross ( targetUp, Vector3.forward );

		  targetForward.Normalize ( );
	   }

	   targetRight = Vector3.Cross ( targetUp, targetForward ).normalized;
	   targetForward = Vector3.Cross ( targetRight, targetUp ).normalized;

	   allowGravitySet = ( bool ) PreviewGravityDirectionChange?.Invoke ( targetForward, targetUp );
    }

    // Since the target axes are already calculated, this step is short but meaningful
    private void UpdatePlayerGravityAxesAfterPreview ( )
    {
	   gravityDirection = targetGravityDirection;
	   up = -gravityDirection;
	   forward = targetForward;
	   right = targetRight;

	   GravityDirectionChanged?.Invoke ( forward, up );
    }

    // The actual method that snaps arbitrary player based axis to the 3 World axes
    private Vector3 SnapToAxis ( Vector3 v )
    {
	   v.Normalize ( );

	   float x = Mathf.Abs ( v.x );
	   float y = Mathf.Abs ( v.y );
	   float z = Mathf.Abs ( v.z );

	   if ( x >= y && x >= z )
		  return Mathf.Sign ( v.x ) * Vector3.right;

	   if ( y >= x && y >= z )
		  return Mathf.Sign ( v.y ) * Vector3.up;

	   return Mathf.Sign ( v.z ) * Vector3.forward;
    }

    private void GravityChangeAction_started ( InputAction.CallbackContext obj )
    {

    }

    private void GravityChangeAction_performed ( InputAction.CallbackContext obj )
    {
	   OnGravityChanged ( obj.ReadValue<Vector2> ( ) );
    }

    private void GravityChangeAction_canceled ( InputAction.CallbackContext obj )
    {

    }

    private void GravitySetAction_started ( InputAction.CallbackContext obj )
    {

    }

    private void GravitySetAction_performed ( InputAction.CallbackContext obj )
    {
	   if ( allowGravitySet )
	   {
		  UpdatePlayerGravityAxesAfterPreview ( );
	   }
    }

    private void GravitySetAction_canceled ( InputAction.CallbackContext obj )
    {

    }

    private void OnGravityChanged ( Vector2 _v )
    {
	   if ( _v.x > 0 )
	   {
		  targetGravityDirection = Vector3.ProjectOnPlane ( player.transform.right, -gravityDirection );
	   }
	   else if ( _v.x < 0 )
	   {
		  targetGravityDirection = -Vector3.ProjectOnPlane ( player.transform.right, -gravityDirection );
	   }
	   else if ( _v.y > 0 )
	   {
		  targetGravityDirection = Vector3.ProjectOnPlane ( player.transform.forward, -gravityDirection );
	   }
	   else if ( _v.y < 0 )
	   {
		  targetGravityDirection = -Vector3.ProjectOnPlane ( player.transform.forward, -gravityDirection );
	   }
	   else
	   {
		  return;
	   }

	   UpdateTargetAxes ( );
    }

    private void OnDisable ( )
    {
	   DisableGravityModification ( );
    }

    public void DisableGravityModification ( )
    {
	   gravityChange.action.Disable ( );
	   gravitySet.action.Disable ( );

	   gravityChange.action.started -= GravityChangeAction_started;
	   gravityChange.action.performed -= GravityChangeAction_performed;
	   gravityChange.action.canceled -= GravityChangeAction_canceled;

	   gravitySet.action.started -= GravitySetAction_started;
	   gravitySet.action.performed -= GravitySetAction_performed;
	   gravitySet.action.canceled -= GravitySetAction_canceled;
    }
}
