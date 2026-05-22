using UnityEngine;
using UnityEngine.InputSystem;

public class GravityManipulator : MonoBehaviour
{
    [SerializeField] private InputActionReference gravityChange;
    [SerializeField] private InputActionReference gravitySet;

    private Vector3 cached;
    private static Vector3 gravityDirection = Vector3.down;
    public static System.Action<Vector3, Vector3> GravityDirectionChanged;

    [SerializeField] private Vector3 up = Vector3.up;
    [SerializeField] private Vector3 forward = Vector3.forward;
    [SerializeField] private Vector3 right = Vector3.right;

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

    private void Start ( )
    {
	   UpdateAxes ( );
    }

    private void UpdateAxes ( )
    {
	   up = -gravityDirection;

	   forward = Vector3.ProjectOnPlane ( forward, up ).normalized;

	   if ( forward.sqrMagnitude < 0.01f )
	   {
		  forward = Vector3.Cross ( up, Vector3.right );

		  if ( forward.sqrMagnitude < 0.01f )
			 forward = Vector3.Cross ( up, Vector3.forward );

		  forward.Normalize ( );
	   }

	   right = Vector3.Cross ( forward, up ).normalized;
	   forward = Vector3.Cross ( up, right ).normalized;

	   GravityDirectionChanged?.Invoke ( forward, up );
    }

    private void GravityChangeAction_started ( InputAction.CallbackContext obj )
    {

    }

    private void GravityChangeAction_performed ( InputAction.CallbackContext obj )
    {
	   cached = obj.ReadValue<Vector2> ( );
    }

    private void GravityChangeAction_canceled ( InputAction.CallbackContext obj )
    {

    }

    private void GravitySetAction_started ( InputAction.CallbackContext obj )
    {

    }

    private void GravitySetAction_performed ( InputAction.CallbackContext obj )
    {
	   OnGravityChanged ( cached );
    }

    private void GravitySetAction_canceled ( InputAction.CallbackContext obj )
    {

    }

    private void OnGravityChanged ( Vector2 _v )
    {
	   if ( _v.x > 0 )
	   {
		  gravityDirection = -right;
	   }
	   else if ( _v.x < 0 )
	   {
		  gravityDirection = right;
	   }
	   else if ( _v.y > 0 )
	   {
		  gravityDirection = forward;
	   }
	   else if ( _v.y < 0 )
	   {
		  gravityDirection = -forward;
	   }
	   else
	   {
		  return;
	   }

	   UpdateAxes ( );
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

    private void OnDrawGizmos ( )
    {
	   Gizmos.DrawSphere ( transform.position, 0.1f );

	   Gizmos.color = Color.green;
	   Gizmos.DrawLine ( transform.position, transform.position + up );

	   Gizmos.color = Color.red;
	   Gizmos.DrawLine ( transform.position, transform.position + right );

	   Gizmos.color = Color.blue;
	   Gizmos.DrawLine ( transform.position, transform.position + forward );
    }
}
