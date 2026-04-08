using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int coinsCount;
    public Text coinsCountText;

    public List<Item> content = new List<Item>();
    private int contentCurrentIndex = 0;
    public Image itemUIImage;
    public Sprite emptyItemImage;
    public Text itemNameUI;
    public PlayerEffects playerEffects;

    public static Inventory instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de Inventory dans la scène");
            return;
        }
        instance = this;
    }

    private void Start() 
    {
        UpdateInventoryUI();
    }
    public void ConsumeItem()
    {
        if(content.Count == 0)
        {
            return;
        }

        Item currentItem = content[contentCurrentIndex];
        Playerhealth.instance.HealPlayer(currentItem.hpGiven);
        playerEffects.AddSpeed(currentItem.speedGiven, currentItem.speedDuration);
        content.Remove(currentItem);
        GetNextItem();
        UpdateInventoryUI();
    }

    public void GetNextItem()
    {
        if(content.Count == 0)
        {
            return;
        }

        contentCurrentIndex++;
        if(contentCurrentIndex > content.Count - 1)
        {
            contentCurrentIndex = 0;   
        }
        UpdateInventoryUI();
    }

    public void GetPreviousItem()
    {
        if(content.Count == 0)
        {
            return;
        }

        contentCurrentIndex--;
        if(contentCurrentIndex < 0)
        {
            contentCurrentIndex = content.Count - 1;   
        }
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        if(content.Count > 0)
        {
            itemUIImage.sprite = content[contentCurrentIndex].image;
            itemNameUI.text = content[contentCurrentIndex].name;
        }
        else
        {
            itemUIImage.sprite = emptyItemImage;
            itemNameUI.text = "AUCUN  ITEM";
        }
    }


    public void AddCoins(int count)
    {
        coinsCount += count;
        UpdateTextUI();
    }

    public void RemoveCoins(int count)
    {
        coinsCount -= count;
        if(coinsCount < 0)
        {
            coinsCount = 0;
        }
        UpdateTextUI();
    }

    public void UpdateTextUI()
    {
        coinsCountText.text = coinsCount.ToString();
    }
}
