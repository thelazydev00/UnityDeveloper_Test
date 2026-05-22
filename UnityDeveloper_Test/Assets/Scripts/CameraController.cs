using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _followTarget;
    [SerializeField] private float maxDistance;
    [SerializeField] private float targetDistance;
    [SerializeField] private float minDistance;

    [SerializeField] private Vector3 frameForward;
    [SerializeField] private Vector3 frameUp;

    private void Start ( )
    {
	   GravityManipulator.GravityDirectionChanged += OnGravityDirectionChanged;

	   frameForward = _followTarget.forward;
	   frameUp = _followTarget.up;
    }

    private void LateUpdate ( )
    {
	   Quaternion targetRotation = Quaternion.LookRotation ( frameForward, frameUp );
	   transform.rotation = Quaternion.Slerp ( transform.rotation, targetRotation, 15f * Time.deltaTime );

	   Vector3 moveTo = _followTarget.position - transform.position;
	   transform.position += Vector3.MoveTowards ( Vector3.zero, moveTo, moveTo.magnitude * Time.deltaTime );
	   RepositionCamera ( );
    }

    private void RepositionCamera ( )
    {
	   Ray ray = new ( transform.position, -transform.forward );
	   float currentDistance = targetDistance;

	   targetDistance = maxDistance;

	   if ( Physics.SphereCast ( ray, 0.1f, out RaycastHit hit, maxDistance ) )
	   {
		  targetDistance = hit.distance < minDistance ? minDistance : hit.distance;
	   }

	   _camera.transform.localPosition = Vector3.back * targetDistance;
    }

    private void OnGravityDirectionChanged ( Vector3 forward, Vector3 up )
    {
	   frameForward = forward;
	   frameUp = up;
    }

    private void OnDestroy ( )
    {
	   GravityManipulator.GravityDirectionChanged -= OnGravityDirectionChanged;
    }
}
