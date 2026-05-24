using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header ( "Serialized References" )]
    [SerializeField] private float moveForce = 15f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityMagnitude = 10f;
    [SerializeField] private float playerRotationSpeed = 45f;
    [SerializeField] private Animator myAnimator;

    private Rigidbody myRb;

    private Vector2 input;

    private Vector3 moveDirection;
    private Vector3 gravityDirection;

    private Vector3 frameForward;
    private Vector3 frameUp;
    private Vector3 frameRight;

    private bool isSwitchingGravity = false;
    private bool isGrounded = false;
    private bool isCharacterFalling = false;

    private float maxVelCache;

    private void Start ( )
    {
	   InitializePlayerMovement ( );
    }

    public void InitializePlayerMovement ( )
    {
	   myRb = GetComponent<Rigidbody> ( );

	   GravityManipulator.GravityDirectionChanged += OnGravityDirectionChanged;
	   PlayerInputManager.MovementAction += SetMove;
	   PlayerInputManager.JumpAction += Jump;

	   frameForward = transform.forward;
	   frameUp = transform.up;
	   frameRight = transform.right;

	   maxVelCache = myRb.maxLinearVelocity;
    }

    private void Update ( )
    {
	   isGrounded = IsGrounded ( out float distanceToGround );

	   moveDirection = transform.forward * input.y + transform.right * input.x;

	   moveDirection = Vector3.ProjectOnPlane ( moveDirection, -gravityDirection );
	   moveDirection = Vector3.ClampMagnitude ( moveDirection, 1f );

	   RotateTowardsMovement ( );

	   if ( isGrounded )
	   {
		  // Stop Falling
		  if ( isCharacterFalling )
		  {
			 myAnimator.SetBool ( "isGrounded", isGrounded );
			 isCharacterFalling = false;
			 myRb.maxLinearVelocity = 5f;
		  }

		  // Stop anticipating Death
		  myAnimator.SetFloat ( "movement", Mathf.Clamp ( myRb.linearVelocity.magnitude / 5f, 0f, 5f ) );
	   }
	   else
	   {
		  // Start Falling
		  if ( !isCharacterFalling )
		  {
			 myAnimator.SetBool ( "isGrounded", isGrounded );
			 isCharacterFalling = true;
			 myRb.maxLinearVelocity = maxVelCache;
		  }

		  if ( distanceToGround > 25f )
		  {
			 // Start anticipating death
		  }
	   }
    }

    private void FixedUpdate ( )
    {
	   if ( !isSwitchingGravity )
	   {
		  myRb.AddForce ( gravityDirection * gravityMagnitude, ForceMode.Force );
		  myRb.AddForce ( moveDirection * moveForce, ForceMode.Force );
	   }
    }

    private void RotateTowardsMovement ( )
    {
	   Vector3 faceDir = Vector3.ProjectOnPlane ( moveDirection, -gravityDirection );

	   if ( faceDir.sqrMagnitude < 0.001f )
		  return;

	   Quaternion targetRotation =
		  Quaternion.LookRotation ( faceDir.normalized, -gravityDirection );

	   myRb.MoveRotation (
		  Quaternion.RotateTowards ( myRb.rotation, targetRotation, 360f * playerRotationSpeed * Time.deltaTime )
	   );
    }

    private void OnGravityDirectionChanged ( Vector3 forward, Vector3 up )
    {
	   gravityDirection = -up;

	   frameForward = forward;
	   frameUp = up;
	   frameRight = Vector3.Cross ( frameForward, frameUp ).normalized;

	   if ( !isSwitchingGravity )
	   {
		  StartCoroutine ( SwitchGravity ( ) );
	   }
    }

    IEnumerator SwitchGravity ( )
    {
	   //Quaternion targetRotation = Quaternion.LookRotation ( frameForward, frameUp );
	   Quaternion targetRotation = Quaternion.LookRotation ( frameForward, frameUp );

	   isSwitchingGravity = true;

	   myRb.linearVelocity = Vector3.zero;
	   myRb.angularVelocity = Vector3.zero;

	   float rotateSpeed = 360f;

	   while ( Quaternion.Angle ( myRb.rotation, targetRotation ) > 0.1f )
	   {
		  Quaternion newRotation = Quaternion.RotateTowards ( myRb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime );

		  myRb.MoveRotation ( newRotation );

		  yield return new WaitForFixedUpdate ( );
	   }

	   myRb.MoveRotation ( targetRotation );

	   isSwitchingGravity = false;
    }

    public void SetMove ( Vector2 _move )
    {
	   //moveDirection.x = _move.x;
	   //moveDirection.z = _move.y;

	   input = _move;
    }

    public void Jump ( )
    {
	   if ( isGrounded )
	   {
		  myRb.maxLinearVelocity = maxVelCache;
		  myRb.AddForce ( -gravityDirection.normalized * jumpForce, ForceMode.Impulse );
	   }
    }

    private bool IsGrounded ( out float distanceToGround )
    {
	   Ray ray = new ( transform.position + transform.up * 0.2f, -transform.up );
	   distanceToGround = 25f;

	   if ( Physics.SphereCast ( ray, 0.1f, out RaycastHit hit, distanceToGround ) )
	   {
		  distanceToGround = hit.distance;
	   }

	   return distanceToGround <= 0.2f;
    }

    private void OnDestroy ( )
    {
	   GravityManipulator.GravityDirectionChanged -= OnGravityDirectionChanged;
    }
}
