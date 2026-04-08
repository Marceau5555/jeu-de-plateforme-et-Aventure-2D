using UnityEngine;
using UnityEngine.UI;

public class SpeedPotionUI : MonoBehaviour
{
    public Image potionImage;
    public Text potionText;
    private float potionTimer;

    public static SpeedPotionUI instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de SpeedPotionUI dans la scène");
            return;
        }
        instance = this;

        // Caché par défaut
        potionImage.gameObject.SetActive(false);
    }

    public void ShowTimer(float duration)
    {
        potionTimer = duration;
        potionImage.gameObject.SetActive(true);
    }

    void Update()
    {
        if (potionTimer > 0)
        {
            potionTimer -= Time.deltaTime;
            potionText.text = Mathf.CeilToInt(potionTimer).ToString();
        }
        else if (potionImage.gameObject.activeSelf)
        {
            potionImage.gameObject.SetActive(false);
        }
    }
}