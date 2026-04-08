using UnityEngine;

public class CurrentSceneManager : MonoBehaviour
{

    public int coinsPickedUpInThisSceneCount;

    public int levelToUnlock;
    public static CurrentSceneManager instance;

    public Vector3 respawnPoint;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de DontDestroy dans la scène");
            return;
        }
        instance = this;

        respawnPoint = GameObject.FindGameObjectWithTag("Player").transform.position;
    }
   

}
