using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputActionReference exit;

    private void OnEnable ( )
    {
	   exit.action.Enable ( );
    }

    private void OnDisable ( )
    {
	   exit.action.Disable ( );
    }
}
