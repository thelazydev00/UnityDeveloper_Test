using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputActionReference exit;

    [SerializeField] private float roundTimeMax = 120f;

    private bool isGameRunning = false;

    private void OnEnable ( )
    {
	   exit.action.Enable ( );
    }

    private void Start ( )
    {

    }

    private void StartGame ( )
    {

    }

    IEnumerator Timer ( )
    {
	   isGameRunning = true;

	   float timer = roundTimeMax;

	   while ( timer > 0 && isGameRunning )
	   {
		  timer -= Time.deltaTime;

		  yield return null;
	   }

	   if ( !isGameRunning )
	   {
		  // Player probably died or collected all items
	   }
	   else
	   {
		  // Player ran out of time
		  isGameRunning = false;
	   }
    }

    private void OnDisable ( )
    {
	   exit.action.Disable ( );
    }
}
