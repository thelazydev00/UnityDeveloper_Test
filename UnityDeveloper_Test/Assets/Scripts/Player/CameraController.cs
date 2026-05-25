using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header ( "Serialized References" )]
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform cameraYaw;
    [SerializeField] private Transform cameraPitch;
    [SerializeField] private Transform _followTarget;
    [SerializeField] private float maxDistance;
    [SerializeField] private float targetDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private LayerMask collisionLayers;

    public Transform CameraForwardReference => cameraYaw;

    private bool cameraFollowEnabled;
    public bool MovedThisFrame { get; set; }

    private Vector2 pitchYaw;
    private Vector3 cameraOffset;

    // Cache the position offset to apply it back to the main transform
    private void Awake ( )
    {
	   cameraOffset = _followTarget.position - transform.position;
    }

    public void InitializeCamera ( )
    {
	   cameraFollowEnabled = true;
    }

    public void AllowCameraMovement ( bool allowed )
    {
	   cameraFollowEnabled = allowed;

	   if ( cameraFollowEnabled )
	   {
		  PlayerInputManager.MouseLookAction += OnMouseLook;
	   }
	   else
	   {
		  PlayerInputManager.MouseLookAction -= OnMouseLook;
	   }
    }

    private void Update ( )
    {
	   if ( !cameraFollowEnabled )
	   {
		  return;
	   }
    }

    private void LateUpdate ( )
    {
	   if ( cameraFollowEnabled )
	   {
		  float pitch;
		  float yaw;

		  pitch = Mathf.Clamp ( pitchYaw.y, -60f, 60f );
		  yaw = pitchYaw.x;

		  pitchYaw.y = pitch;

		  cameraYaw.localEulerAngles = Vector3.up * yaw;
		  cameraPivot.localEulerAngles = Vector3.left * pitch;

		  //if ( MovedThisFrame )
		  //{
			 //Quaternion targetRotation = Quaternion.LookRotation ( _followTarget.forward, _followTarget.up );
			 //transform.rotation = Quaternion.Slerp ( transform.rotation, targetRotation, 15f * Time.fixedDeltaTime );
			 //MovedThisFrame = false;
		  //}

		  transform.position = _followTarget.position - cameraOffset;
	   }

	   RepositionCamera ( );
    }

    // Simple Camera Obstacle Avoidance
    private void RepositionCamera ( )
    {
	   Ray ray = new ( cameraPivot.position, -cameraPivot.forward );
	   float currentDistance = targetDistance;

	   targetDistance = maxDistance;

	   if ( Physics.SphereCast ( ray, 0.1f, out RaycastHit hit, maxDistance, collisionLayers ) )
	   {
		  targetDistance = hit.distance < minDistance ? minDistance : hit.distance;
	   }

	   _camera.transform.localPosition = Vector3.back * targetDistance;
    }

    private void OnMouseLook ( Vector2 _delta )
    {
	   pitchYaw += _delta;
    }

    private void OnDestroy ( )
    {
	   PlayerInputManager.MouseLookAction -= OnMouseLook;
    }
}
