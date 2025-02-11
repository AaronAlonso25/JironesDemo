using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using TMPro;
using Yarn.Unity;
using Rewired;

namespace NeonBlood
{
    public class ShopCanvas : MMSingleton<ShopCanvas>
    {
        [Header("Main Panels")]
        public GameObject ShopPanel;

        [Header("Shop Panel")]
        public GameObject ShopObjectsPanel_Content;
        public GameObject ShopObject_Prefab;

        public Image ShopObjectImage;
        public TMP_Text ShopObjectName;
        public TMP_Text ShopObjectShort;
        public TMP_Text ShopObjectDescription;
        public TMP_Text ShopObjectPrice;
        public List<Image> HeaderItemsImages;

        [Header("Buttons")]
        public Color HeaderColorHighlighted;

        [Header("Player Stats")]
        public float PlayerMoney = 1000;

        [Header("UI Elements")]
        public TMP_Text PlayerMoneyText; 

        //First Buttons
        private GameObject shopFirstButton;

        private ShopEvent se;
        private int indexShopPanel = 0;
        private List<ItemRPGInfo> shopItems = new List<ItemRPGInfo>();  

        
        public ShopEventDialogue shopEventDialogue;

        protected override void Awake()
        {
            base.Awake();
            
        }

        
            
       

        void Start()
        {
            PlayerMoney = 0;
            UpdateMoneyUI(); 
        }

        void Update()
        {
            if (Input.GetButtonDown("Circle_UI") && this.ShopPanel.activeSelf)
                this.ExitShopPanel();

            //Mostrar ventana de Compra-Venta
            if (Input.GetButtonDown("L1_UI") && this.ShopPanel.activeSelf)
            {
                this.indexShopPanel = 0;
                this.ConfigInventoryPanel();
            }
            else if (Input.GetButtonDown("R1_UI") && this.ShopPanel.activeSelf)
            {
                this.indexShopPanel = 1;
                this.ConfigInventoryPanel();
            }
            if (shopEventDialogue != null && PlayerMoney != shopEventDialogue.Monedas)
    {
        PlayerMoney = shopEventDialogue.Monedas;
        UpdateMoneyUI();
    }

            

            
        }

        public IEnumerator ShowShopPanel(ShopEvent se)
        {
            //Comprobar que el panel de Mapa no esta activo
            if (MapCanvas.Instance.ContainerPanel.activeSelf ||
                PauseCanvas.Instance.MainPanel.activeSelf)
                yield break;             

            yield return new WaitForEndOfFrame();

            MenuCanvas.Instance.Pause(true);
            this.ShopPanel.SetActive(true);
            PauseCanvas.Instance.CanPause = false;

            this.indexShopPanel = 0;
            this.se = se;            

            this.ConfigInventoryPanel();
        }

        public void ExitShopPanel()
        {
            MenuCanvas.Instance.Pause(false);
            this.ShopPanel.SetActive(false);
            PauseCanvas.Instance.CanPause = true;

            this.se.ReactiveEvent();
            this.se = null;

            EventSystem.current.SetSelectedGameObject(null);
        }

        public void ConfigInventoryPanel()
        {
            //Destruyo las habilidades previas
            foreach (Transform child in this.ShopObjectsPanel_Content.transform)
                Destroy(child.gameObject);

            this.shopItems = (this.indexShopPanel == 0) ?
                this.se.ShopItems : DataManager.Instance.CharactersItems;

            //Header
            foreach (Image im in this.HeaderItemsImages)
                im.color = Color.white;
            this.HeaderItemsImages[this.indexShopPanel].color = this.HeaderColorHighlighted;

            //Creo los objetos nuevos
            for (int i = 0; i < this.shopItems.Count; i++)
            {
                int indexItem = i;

                GameObject button = Instantiate(this.ShopObject_Prefab, this.ShopObjectsPanel_Content.transform);                
                if(this.indexShopPanel == 0)
                    button.GetComponent<Button>().onClick.AddListener(() => this.BuyObject(indexItem));
                else
                    button.GetComponent<Button>().onClick.AddListener(() => this.SellObject(indexItem));

                button.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("Inventory/" + this.shopItems[indexItem].ItemName);
                button.GetComponentInChildren<TMP_Text>().text = "x" + this.shopItems[indexItem].ItemAmount;

                this.AddItemEventTriggerListener(button.GetComponent<EventTrigger>(), EventTriggerType.Select, indexItem);

                if (indexItem == 0)
                    this.shopFirstButton = button;
            }

            EventSystem.current.SetSelectedGameObject(this.shopFirstButton);
            this.ShowObjectInfoPanel(0);
        }

        private void ShowObjectInfoPanel(int indexItem)
        {
            this.ShopObjectImage.sprite = Resources.Load<Sprite>("Inventory/" + this.shopItems[indexItem].ItemName);

            this.ShopObjectName.text =
                TextsManager.Instance.ObjectsInfo[this.shopItems[indexItem].ItemName].Title;
            this.ShopObjectShort.text =
                TextsManager.Instance.ObjectsInfo[this.shopItems[indexItem].ItemName].Short;
            this.ShopObjectDescription.text =
                TextsManager.Instance.ObjectsInfo[this.shopItems[indexItem].ItemName].Text;
            this.ShopObjectPrice.text =
                (this.indexShopPanel == 0) ? "" + this.shopItems[indexItem].ItemPriceBuy : "" + this.shopItems[indexItem].ItemPriceSell;
        }

        private void BuyObject(int indexItem)
{
    ItemRPGInfo itemToBuy = this.shopItems[indexItem];

    // Verificar si el jugador tiene suficiente dinero
    if (PlayerMoney < itemToBuy.ItemPriceBuy)
    {
        Debug.Log("No tienes suficiente dinero para comprar " + itemToBuy.ItemName);
        return;
    }

    // Restar dinero al jugador
    PlayerMoney -= itemToBuy.ItemPriceBuy;
    shopEventDialogue.Monedas = PlayerMoney;
    UpdateMoneyUI();

    // Agregar el objeto al inventario del jugador
    var existingItem = DataManager.Instance.CharactersItems.SingleOrDefault(x => x.ItemName == itemToBuy.ItemName);
    if (existingItem != null)
        existingItem.ItemAmount++;
    else
        DataManager.Instance.CharactersItems.Add(new ItemRPGInfo(itemToBuy, 1));

    // Restar cantidad del objeto en la tienda
    itemToBuy.ItemAmount--;
    if (itemToBuy.ItemAmount <= 0)
        this.shopItems.RemoveAt(indexItem); // Eliminar si se agotó

    // Actualizar la interfaz
    this.ConfigInventoryPanel();
}

        private void SellObject(int indexItem)
        {
            //Aumentamos cantidad del objeto de la tienda, si no hay, lo creamos
            if (this.se.ShopItems.Any(x => x.ItemName == this.shopItems[indexItem].ItemName))
                this.se.ShopItems.Single(x => x.ItemName == this.shopItems[indexItem].ItemName).ItemAmount++;
            else
                this.se.ShopItems.Add(new ItemRPGInfo(this.shopItems[indexItem], 1));

            //Restamos cantidad del objeto en el inventario y si es 0, lo borramos del inventario
            this.shopItems[indexItem].ItemAmount--;
            PlayerMoney+=this.shopItems[indexItem].ItemPriceSell;
            shopEventDialogue.Monedas = PlayerMoney;
            UpdateMoneyUI();
            if (this.shopItems[indexItem].ItemAmount <= 0)
                this.shopItems.Remove(this.shopItems[indexItem]);

            this.ConfigInventoryPanel();
        }

        private void UpdateMoneyUI()
        {
           PlayerMoneyText.text = "" + PlayerMoney;
        }

       

        #region Tools

        private void AddItemEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, int indexItem)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventType;
            entry.callback.AddListener((functionIWant) => { this.ShowObjectInfoPanel(indexItem); });
            trigger.triggers.Add(entry);
        }

        #endregion
    }
}