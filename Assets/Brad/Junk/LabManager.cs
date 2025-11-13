using UnityEngine;

public class LabManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerStart;
    [SerializeField] private Transform enemyStart;

    private void Start()
    {
        GameObject playerObj = Instantiate(playerPrefab);
        GameObject enemyObj = Instantiate(enemyPrefab);
        playerObj.transform.position = playerStart.position;
        enemyObj.transform.position = enemyStart.position;
    }


}
