using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform _followTarget;
    [SerializeField] private float maxDistance;
    [SerializeField] private float targetDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private LayerMask collisionLayers;

    private bool cameraFollowEnabled;
    private Vector3 cameraOffset;

    private void Start ( )
    {
	   InitializeCamera ( );
	   cameraOffset = _followTarget.position - transform.position;
    }

    public void InitializeCamera ( )
    {
	   cameraFollowEnabled = true;
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
	   Quaternion targetRotation = Quaternion.LookRotation ( _followTarget.forward, _followTarget.up );
	   transform.rotation = Quaternion.Slerp ( transform.rotation, targetRotation, 15f * Time.deltaTime );

	   //Vector3 moveVector = _followTarget.position - transform.position;
	   //transform.position += Vector3.MoveTowards ( Vector3.zero, moveVector, 1.5f * moveVector.magnitude * Time.deltaTime );

	   transform.position = _followTarget.position - cameraOffset;

	   RepositionCamera ( );
    }

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
}
