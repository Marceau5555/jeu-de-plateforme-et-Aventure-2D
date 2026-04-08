using UnityEngine;
using UnityEngine.UI;

public class DashBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    private float targetValue;

    public static DashBar instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de DashBar");
            return;
        }
        instance = this;

        slider.maxValue = 3f;
        slider.value = 3f;
        fill.color = gradient.Evaluate(1f);
    }

    void Update()
    {
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * 5f);
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void UpdateDashBar(int charges, float rechargeProgress)
    {
        targetValue = charges;
    }

}