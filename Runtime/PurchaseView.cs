using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sayollo {
    public class PurchaseView : MonoBehaviour
    {
        [Header("URLs")]
        [TextArea]
        [SerializeField]
        private string productListUrl;
        [TextArea]
        [SerializeField]
        private string buyUrl;

        [Header("Canvases")]
        [SerializeField]
        private Canvas canvasProducts;
        [SerializeField]
        private Canvas canvasError;
        [SerializeField]
        private Canvas canvasShowButton;
        [SerializeField]
        private Canvas canvasInputs;


        [SerializeField]
        private GameObject progressIndicator;


        [Tooltip("Used for dim canvas")]
        [SerializeField]
        private Image imageDim;


        [SerializeField]
        private InputWindow inputWindow;

        [SerializeField]
        private Image productImage;
        [SerializeField]
        private TextMeshProUGUI tmpTitle;
        [SerializeField]
        private TextMeshProUGUI tmpPrice;

        [SerializeField]
        private GameObject buyButton;

        //TODO: Move to separate class

        [SerializeField]
        private Button tryAgainBtn;

        private PurchaseItemModel purchaseItemModel;

        private void Awake()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
            canvasProducts.enabled = false;
            imageDim.enabled = false;
            progressIndicator.SetActive(false);
            canvasError.enabled = false;

            canvasShowButton.worldCamera = Camera.main;
            canvasProducts.worldCamera = Camera.main;
            canvasError.worldCamera = Camera.main;
            canvasInputs.worldCamera = Camera.main;
        }

        public void GetProducts()
        {

            ShowButtonData showButtonData = new ShowButtonData("Subscriptions");
            progressIndicator.SetActive(true);

            tryAgainBtn.onClick.RemoveAllListeners();
            tryAgainBtn.onClick.AddListener(() => {
                GetProducts();
            });

            ApiRequest.PostJson(productListUrl, showButtonData,
                (string success) => {

                    Debug.Log("Received: " + success);

                    purchaseItemModel = JsonUtility.FromJson<PurchaseItemModel>(success);
                    Debug.Log("PurchaseView: purchaseItemModel: title: " + purchaseItemModel.title + ", currency_sign: " + purchaseItemModel.currency_sign);


                    canvasError.enabled = false;
                    LoadProductImage(purchaseItemModel.item_image);
                },
                (string error) => {
                    Debug.Log("GetProducts: Error: " + error);
                    progressIndicator.SetActive(false);
                    canvasError.enabled = true;
                }
            );
        }

        private void LoadProductImage(string url)
        {
            ApiRequest.GetTexture(url,
                (Texture2D texture2D) => {
                    Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height)
                        , new Vector2(.5f, .5f), 10f);
                    productImage.sprite = sprite;
                    tmpTitle.text = purchaseItemModel.title + " " + purchaseItemModel.item_name;
                    tmpPrice.text = purchaseItemModel.price + " " + purchaseItemModel.currency_sign;

                    progressIndicator.SetActive(false);
                    canvasProducts.enabled = true;

                },
                (string error) => {
                    Debug.Log("Image load error: " + error);
                    progressIndicator.SetActive(false);
                    canvasError.enabled = true;
                }
            );
        }

        public void StartPurchase()
        {
            imageDim.enabled = true;
            inputWindow.Show(
                (BuyDataModel buyDataModel) => {
                    Debug.Log("PurchaseView: startPurchase: buyDataModel: " + buyDataModel.email);
                    BuyProduct(buyDataModel);
                },
                () => {
                    Debug.Log("PurchaseView: startPurchase: cancel");
                    inputWindow.Hide();
                    imageDim.enabled = false;
                }
            );
        }

        public void BuyProduct(BuyDataModel buyDataModel)
        {
            progressIndicator.SetActive(true);

            tryAgainBtn.onClick.RemoveAllListeners();
            tryAgainBtn.onClick.AddListener(() => {
                imageDim.enabled = false;
                progressIndicator.SetActive(false);
                canvasError.enabled = false;
            });

            ApiRequest.PostJson(buyUrl, buyDataModel,
                (string success) => {
                    Debug.Log("Received: " + success);
                    PurchasedResponseModel purchasedResponseModel = JsonUtility.FromJson<PurchasedResponseModel>(success);
                    tmpPrice.fontSize = 20f;
                    tmpPrice.text = purchasedResponseModel.user_message;
                    buyButton.SetActive(false);
                    imageDim.enabled = false;
                    progressIndicator.SetActive(false);
                    canvasError.enabled = false;
                },
                (string error) => {
                    Debug.Log("Error: " + error);
                    progressIndicator.SetActive(false);
                    canvasError.enabled = true;
                }
            );
        }
    }

}