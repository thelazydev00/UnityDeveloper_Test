using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerPreview;

    private Vector3 displayDirection;

    private Vector3 forward;
    private Vector3 right;

    private void Start ( )
    {
	   playerPreview.SetActive ( false );

	   transform.localRotation = Quaternion.identity;
    }

    private void OnGravityChangePreview ( Vector2 _v )
    {
	   playerPreview.SetActive ( true );

	   if ( _v.x > 0 )
	   {
		  displayDirection = -right;
	   }
	   else if ( _v.x < 0 )
	   {
		  displayDirection = right;
	   }
	   else if ( _v.y > 0 )
	   {
		  displayDirection = forward;
	   }
	   else if ( _v.y < 0 )
	   {
		  displayDirection = -forward;
	   }
	   else
	   {
		  playerPreview.SetActive ( false );
		  playerPreview.transform.localRotation = Quaternion.identity;
		  return;
	   }
    }
}
