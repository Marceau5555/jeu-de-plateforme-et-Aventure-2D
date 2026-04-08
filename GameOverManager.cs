using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;
    
    public static GameOverManager instance;


private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de GameOverManager dans la scène");
            return;
        }
        instance = this;
    }


    public void OnPlayerDeath()
    {
        StartCoroutine(DelayBeforeMenuAppears());
    }

    public void RespawnButton()
    {
        Inventory.instance.RemoveCoins(CurrentSceneManager.instance.coinsPickedUpInThisSceneCount);
        //recommencer le level
        //rechargement de la scène
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        //replacer le joueur au spawn
        //réactiver PlayerMovement + rendre HP
        Playerhealth.instance.Respawn();

        gameOverUI.SetActive(false);   
    }

    public void MainMenuButton()
    {
        // retour au menu des niveaux
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitButton()
    {
        //Quitter le jeu
        Application.Quit();
    }

    public IEnumerator DelayBeforeMenuAppears()
    {
        yield return new WaitForSeconds(1.2f);
        gameOverUI.SetActive(true);
    }
}
