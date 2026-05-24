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

    // Private member variables
    private Rigidbody m_Rigidbody;

    private Vector2 input;

    private Vector3 moveDirection;
    private Vector3 gravityDirection;

    private Vector3 frameForward;
    private Vector3 frameUp;
    private Vector3 frameRight;

    private float maxVelCache;

    // Booleans to handle or flag states
    private bool isSwitchingGravity = false;
    private bool isGrounded = false;
    private bool isCharacterFalling = false;
    private bool predictingDeath = false;

    public bool AllowPlayerMovement { get; set; } = false;

    public System.Action<bool> PredictDeath;

    // Initializes Player Movement
    public void InitializePlayerMovement ( )
    {
	   m_Rigidbody = GetComponent<Rigidbody> ( );

	   GravityManipulator.GravityDirectionChanged += OnGravityDirectionChanged;
	   PlayerInputManager.MovementAction += SetMove;
	   PlayerInputManager.JumpAction += Jump;

	   frameForward = transform.forward;
	   frameUp = transform.up;
	   frameRight = transform.right;

	   maxVelCache = m_Rigidbody.maxLinearVelocity;
    }

    // If AllowPlayerMovement is false, we bail out as there's nothing else to do
    private void Update ( )
    {
	   if ( !AllowPlayerMovement )
	   {
		  return;
	   }

	   // Ground Check for jumps and death prediction
	   isGrounded = IsGrounded ( out float distanceToGround );

	   // Updating moveDirection for smooth movement
	   moveDirection = transform.forward * input.y + transform.right * input.x;

	   // Projection allows the Vector to be transformed according to the modified gravity
	   moveDirection = Vector3.ProjectOnPlane ( moveDirection, -gravityDirection );

	   // Magnitud Clamping works better incase we expand the input system to include analog sticks
	   moveDirection = Vector3.ClampMagnitude ( moveDirection, 1f );

	   // Rotates towards the direction of movement while respecting the new gravity direction
	   RotateTowardsMovement ( );

	   if ( isGrounded )
	   {
		  // Stop Falling and flag state
		  if ( isCharacterFalling )
		  {
			 myAnimator.SetBool ( "isGrounded", isGrounded );
			 isCharacterFalling = false;
			 m_Rigidbody.maxLinearVelocity = 5f;
		  }

		  myAnimator.SetFloat ( "movement", Mathf.Clamp ( m_Rigidbody.linearVelocity.magnitude / 5f, 0f, 5f ) );

		  // Stop predicting death
		  //Debug.Log ( "Stopped Predicting Death" );
		  if ( predictingDeath )
		  {
			 predictingDeath = false;
			 PredictDeath?.Invoke ( predictingDeath );
		  }
	   }
	   else
	   {
		  // Start Falling and flag state
		  if ( !isCharacterFalling )
		  {
			 myAnimator.SetBool ( "isGrounded", isGrounded );
			 isCharacterFalling = true;
			 m_Rigidbody.maxLinearVelocity = maxVelCache;
		  }

		  if ( distanceToGround > 20f )
		  {
			 // Start predicting death
			 //Debug.Log ( $"Predicting Death" );
			 if ( !predictingDeath )
			 {
				predictingDeath = true;
				PredictDeath?.Invoke ( predictingDeath );
			 }
		  }
		  else
		  {
			 // Stop predicting death
			 //Debug.Log ( "Stopped Predicting Death" );
			 if ( predictingDeath )
			 {
				predictingDeath = false;
				PredictDeath?.Invoke ( predictingDeath );
			 }
		  }
	   }
    }

    // Bail out if AllowPlayerMovement is false
    private void FixedUpdate ( )
    {
	   if ( !AllowPlayerMovement )
	   {
		  return;
	   }

	   // Do not add new forces until gravity direction is completely changed
	   if ( !isSwitchingGravity )
	   {
		  m_Rigidbody.AddForce ( gravityDirection * gravityMagnitude, ForceMode.Force );
		  m_Rigidbody.AddForce ( moveDirection * moveForce, ForceMode.Force );
	   }
    }

    // Rotate towards intended move Direction while respecting the new gravity direction
    private void RotateTowardsMovement ( )
    {
	   Vector3 faceDir = Vector3.ProjectOnPlane ( moveDirection, -gravityDirection );

	   if ( faceDir.sqrMagnitude < 0.001f )
		  return;

	   Quaternion targetRotation =
		  Quaternion.LookRotation ( faceDir.normalized, -gravityDirection );

	   m_Rigidbody.MoveRotation (
		  Quaternion.RotateTowards ( m_Rigidbody.rotation, targetRotation, 360f * playerRotationSpeed * Time.deltaTime )
	   );
    }

    // Handles changed gravity
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

    // handles orientation update after gravity is switched
    IEnumerator SwitchGravity ( )
    {
	   //Quaternion targetRotation = Quaternion.LookRotation ( frameForward, frameUp );
	   Quaternion targetRotation = Quaternion.LookRotation ( frameForward, frameUp );

	   isSwitchingGravity = true;

	   m_Rigidbody.linearVelocity = Vector3.zero;
	   m_Rigidbody.angularVelocity = Vector3.zero;

	   float rotateSpeed = 360f;

	   while ( Quaternion.Angle ( m_Rigidbody.rotation, targetRotation ) > 0.1f )
	   {
		  Quaternion newRotation = Quaternion.RotateTowards ( m_Rigidbody.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime );

		  m_Rigidbody.MoveRotation ( newRotation );

		  yield return new WaitForFixedUpdate ( );
	   }

	   m_Rigidbody.MoveRotation ( targetRotation );

	   isSwitchingGravity = false;
    }

    // Get control data for movement
    public void SetMove ( Vector2 _move )
    {
	   //moveDirection.x = _move.x;
	   //moveDirection.z = _move.y;

	   input = _move;
    }

    // Jump is only valid when player is grounded
    public void Jump ( )
    {
	   if ( isGrounded )
	   {
		  m_Rigidbody.maxLinearVelocity = maxVelCache;
		  m_Rigidbody.AddForce ( -gravityDirection.normalized * jumpForce, ForceMode.Impulse );
	   }
    }

    // Method to check for ground contact and distance check
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
