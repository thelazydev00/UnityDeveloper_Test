using UnityEngine;

public class Collectable : MonoBehaviour
{
    CollectablesManager manager;

    public void Initialize ( CollectablesManager collectiblesManager )
    {
	   manager = collectiblesManager;
    }

    // The collider is set to only trigger with the player's layer
    // On successful trigger, this object is collected
    private void OnTriggerEnter ( Collider other )
    {
	   manager.OnCollect ( );

	   gameObject.SetActive ( false );
    }
}
