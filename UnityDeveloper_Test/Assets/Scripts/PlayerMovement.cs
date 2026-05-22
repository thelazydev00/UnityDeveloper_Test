using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveForce = 5f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float gravityMagnitude = 10f;

    private Rigidbody myRb;

    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private Vector3 gravityDirection;

    [SerializeField] private Vector3 frameForward;
    [SerializeField] private Vector3 frameUp;

    private void Start ( )
    {
	   myRb = GetComponent<Rigidbody> ( );

	   GravityManipulator.GravityDirectionChanged += OnGravityDirectionChanged;

	   frameForward = transform.forward;
	   frameUp = transform.up;
    }

    private void Update ( )
    {
	   Quaternion targetRotation = Quaternion.LookRotation ( frameForward, frameUp );

	   myRb.MoveRotation ( Quaternion.Slerp ( transform.rotation, targetRotation, 15f * Time.deltaTime ) );
    }

    private void FixedUpdate ( )
    {
	   myRb.AddForce ( gravityDirection * gravityMagnitude + moveDirection * moveForce, ForceMode.Force );
    }

    private void OnGravityDirectionChanged ( Vector3 forward, Vector3 up )
    {
	   gravityDirection = -up;

	   frameForward = forward;
	   frameUp = up;
    }

    public void SetMove ( Vector2 _move )
    {
	   moveDirection.x = _move.x;
	   moveDirection.z = _move.y;

	   moveDirection = Vector3.ClampMagnitude ( moveDirection, 1f );
	   myRb.AddForce ( moveDirection, ForceMode.Force );
    }

    public void Jump ( )
    {
	   myRb.AddForce ( -gravityDirection * jumpForce, ForceMode.Impulse );
    }

    private void OnDestroy ( )
    {
	   GravityManipulator.GravityDirectionChanged -= OnGravityDirectionChanged;
    }
}
