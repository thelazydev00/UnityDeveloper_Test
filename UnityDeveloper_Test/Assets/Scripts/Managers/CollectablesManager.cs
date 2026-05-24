using System.Collections.Generic;
using UnityEngine;

public class CollectablesManager : MonoBehaviour
{
    [SerializeField] private List<Collectable> collectibles = new ( );
    public List<Collectable> Collectibles => collectibles;

    private GameManager gameManager;

    private int totalCount;
    public int TotalCount => totalCount;

    private int numCollected;
    public int NumCollected => numCollected;

    // Create a list of all collectables
    public void Initialize ( GameManager gameManager )
    {
	   this.gameManager = gameManager;

	   Transform child;

	   for ( int i = 0 ; i < transform.childCount ; i++ )
	   {
		  child = transform.GetChild ( i );

		  if ( child.TryGetComponent ( out Collectable c ) )
		  {
			 c.Initialize ( this );
			 collectibles.Add ( c );
		  }
	   }

	   totalCount = collectibles.Count;
	   numCollected = 0;
    }

    public void OnCollect ( ) => gameManager.UpdateScore ( ++numCollected, totalCount );
}
