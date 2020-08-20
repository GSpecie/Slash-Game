using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private LayerMask overlapSpawnLayerEnemies;
    [SerializeField] private LayerMask overlapSpawnLayerPlayer;

    private Collider[] overlapSpawnColliderPlayer;
    private Collider[] overlapSpawnColliderEnemies;

    private bool isEmpty = true;

    public bool IsEmpty { get { return isEmpty; } }

    [SerializeField] private ScoreGameManager gameManager;

    [SerializeField] int enemyType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        CheckOverlap();
    }

    public void CheckOverlap()
    {
        overlapSpawnColliderPlayer = Physics.OverlapBox(transform.position, new Vector3(2.8f, 1f, 2.8f), Quaternion.identity, overlapSpawnLayerPlayer);
        overlapSpawnColliderEnemies = Physics.OverlapBox(transform.position, new Vector3(0.8f, 1f, 0.8f), Quaternion.identity, overlapSpawnLayerEnemies);

        if (overlapSpawnColliderPlayer.Length != 0 || overlapSpawnColliderEnemies.Length != 0)
        {
            //temgente  
            isEmpty = false;
            gameManager.ManageSpawnPoint(false, this, enemyType);
        }
        else
        {
            isEmpty = true;
            gameManager.ManageSpawnPoint(true, this, enemyType);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(2.8f, 1f, 2.8f) * 2);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.8f, 1f, 0.8f) * 2);
    }
}
