using System.Collections;
using UnityEngine;

public class PlayerPreview : MonoBehaviour
{
    [SerializeField] private GameObject playerPreviewVisual;

    private Vector3 frameForward;
    private Vector3 frameUp;

    private bool isRotating = false;

    public void InitializePlayerPreview ( )
    {
	   playerPreviewVisual.SetActive ( false );

	   playerPreviewVisual.transform.localRotation = Quaternion.identity;

	   GravityManipulator.PreviewGravityDirectionChange += OnGravityChangePreviewed;
	   GravityManipulator.GravityDirectionChanged += OnGravityChanged;
    }

    private bool OnGravityChangePreviewed ( Vector3 forward, Vector3 up )
    {
	   if ( !isRotating )
	   {
		  if ( !playerPreviewVisual.activeSelf )
		  {
			 playerPreviewVisual.transform.localRotation = Quaternion.identity;
			 playerPreviewVisual.SetActive ( true );
		  }
		  else if ( Vector3.Dot ( frameForward, forward ) == 1f && Vector3.Dot ( frameUp, up ) == 1f )
		  {
			 playerPreviewVisual.SetActive ( false );
			 return false;
		  }

		  frameForward = forward;
		  frameUp = up;

		  StartCoroutine ( RotatePreview ( ) );
		  return true;
	   }
	   return false;
    }

    IEnumerator RotatePreview ( )
    {
	   Quaternion targetRotation = Quaternion.LookRotation ( frameForward, frameUp );
	   float rotateSpeed = 360f;
	   isRotating = true;

	   while ( Quaternion.Angle ( playerPreviewVisual.transform.rotation, targetRotation ) > 0.1f )
	   {
		  Quaternion newRotation = Quaternion.RotateTowards ( playerPreviewVisual.transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime );

		  playerPreviewVisual.transform.rotation = newRotation;

		  yield return null;
	   }

	   playerPreviewVisual.transform.rotation = targetRotation;
	   isRotating = false;
    }

    private void OnGravityChanged ( Vector3 forward, Vector3 up )
    {
	   if ( playerPreviewVisual.activeSelf )
	   {
		  playerPreviewVisual.SetActive ( false );
	   }
    }

    private void OnDestroy ( )
    {
	   GravityManipulator.PreviewGravityDirectionChange -= OnGravityChangePreviewed;
	   GravityManipulator.GravityDirectionChanged -= OnGravityChanged;
    }
}
