using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


public class StoreActions : MonoBehaviour {

	[DllImport("__Internal")]
	private static extern void openWindow(string url);
	
	public static StoreActions INSTANCE;

	void Awake() {
		INSTANCE = this;
	}

	public GameObject MainStoreScreen;
	public GameObject PurchaseItemScreen;
	public GameObject PurchasedItemsScreen;

	public Image PurchaseItemImageField;
	public Text PurchaseItemNameField;
	public Text PurchaseItemCostField;
	public Text PurchaseItemCurrCodeField;
	public Text PurchaseItemDescField;

	public Text PurchaseStatusField;
	public Text PurchaseActionText;
	public Text PurchasePromptText;
	public Button PurchaseActionButton;
	public Image PurchaseCompleteNotification;

	public GameObject PurchaseItemFields;
	public GameObject PurchaseScreenDividers;

	public Text StoreTitleField;
	
	private string generatedPayID = "";
	private bool isPaidFlag = false;
	

	public enum PurchaseStatus {
		NO_ITEM_SELECTED,
		CREATING_PURCHASE, 
		CHECKOUT_READY,
		WAITING,
		COMPLETE
	};

	//private PurchaseStatus currentPurchaseStatus = PurchaseStatus.NO_ITEM_SELECTED;

	// Use this for initialization
	void Start () {
		OpenStore ();
		changePurchaseStatus (PurchaseStatus.NO_ITEM_SELECTED);

	}

	public void OpenStore() {

		MainStoreScreen.SetActive (true);
		StoreTitleField.text = StoreProperties.INSTANCE.gameTitle + " Store";

	}

	public void CloseStore() {

		gameObject.SetActive (false);

	}

	public void PurchaseScreenAction() {


		string checkoutURL = "";
		
		if (StoreProperties.INSTANCE.payPalEndpoint == StoreProperties.Environment.SANDBOX) {
			checkoutURL = "https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_ap-payment&paykey=";
		} else {
			checkoutURL = "https://www.paypal.com/cgi-bin/webscr?cmd=_ap-payment&paykey=";
		}
		
		checkoutURL += generatedPayID;

		//Open Checkout page in browser
		if (Application.platform.Equals(RuntimePlatform.WebGLPlayer)) {
			openWindow(checkoutURL);
		} else if (Application.isWebPlayer) {
			Application.ExternalEval("window.open('"+checkoutURL+"','_blank')");
		}
		else {
			Application.OpenURL(checkoutURL);
		}
		
		changePurchaseStatus (PurchaseStatus.WAITING);

		PollServerForPurchaseStatusChange ();

		
	}

	public void ViewPurchasedItems() {
		MainStoreScreen.SetActive (false);
		PurchasedItemsScreen.SetActive (true);

		InventoryActions.INSTANCE.refreshItems ();

	}

	public void OpenPurchaseItemScreen(StoreItemContent itemToPurchase) {

		MenuNavigation.INSTANCE.selectPurchaseIcon ();
		CancelInvoke("PollInvoker");
		CancelInvoke("CheckForPaidPurchaseStatus");
		CancelInvoke("AnimateWaitingText");

		generatedPayID = "";
		isPaidFlag = false;

		PurchaseItemImageField.sprite = itemToPurchase.itemImage;
		PurchaseItemNameField.text = itemToPurchase.itemName;

		PurchaseItemCostField.text = string.Format("{0:N}", itemToPurchase.itemCost);
		PurchaseItemCostField.text = CurrencyCodeMapper.GetCurrencySymbol (StoreProperties.INSTANCE.currencyCode) + PurchaseItemCostField.text;

		PurchaseItemDescField.text = itemToPurchase.itemDesc;
		PurchaseItemCurrCodeField.text = StoreProperties.INSTANCE.currencyCode;

		changePurchaseStatus (PurchaseStatus.CREATING_PURCHASE);

		StartCoroutine (SetupPurchase());

		InvokeRepeating("CheckForGeneratedPayID", 1f, 1f);



	}

	public void PollServerForPurchaseStatusChange() {
		float pollInterval = 5f;
		InvokeRepeating ("PollInvoker", 0f, pollInterval);
		InvokeRepeating ("CheckForPaidPurchaseStatus", 2f, pollInterval);
		InvokeRepeating ("AnimateWaitingText", 1f, 1f);

	}


	
	//this is repeatedly called until a PayID is set within SetupPurchase()
	void CheckForGeneratedPayID() {

		if (generatedPayID != "") {

			CancelInvoke("CheckForGeneratedPayID");

			string checkoutURL = "";

			if (StoreProperties.INSTANCE.payPalEndpoint == StoreProperties.Environment.SANDBOX) {
				checkoutURL = "https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_ap-payment&paykey=";
			} else {
				checkoutURL = "https://www.paypal.com/cgi-bin/webscr?cmd=_ap-payment&paykey=";
			}

			checkoutURL += generatedPayID;

			changePurchaseStatus(PurchaseStatus.CHECKOUT_READY);

			Debug.Log("opened URL: " + checkoutURL);
		}

	}

	void CheckForPaidPurchaseStatus() {
		
		if (isPaidFlag) {
			CancelInvoke("PollInvoker");
			CancelInvoke("CheckForPaidPurchaseStatus");
			CancelInvoke("AnimateWaitingText");

			changePurchaseStatus(PurchaseStatus.COMPLETE);
		}
		
	}

	void PollInvoker() {
		StartCoroutine(GetPurchaseStatus());
	}

	void AnimateWaitingText() {
		Debug.Log ("animating text");
		string waitingText = PurchaseStatusField.text;

		waitingText += ".";

		if (waitingText.Length > ("Waiting".Length + 5)) {
			waitingText = "Waiting";
		}

		PurchaseStatusField.text = waitingText;
	}


	IEnumerator GetPurchaseStatus() {

		WWWForm postData = new WWWForm();
		
		postData.AddField("PayPalPaymentID", generatedPayID);
		
		string url = "http://unityingameitemmanager.com/GetPaymentStatus.php";
		
		WWW www = new WWW(url, postData.data, StoreProperties.INSTANCE.createHeader(postData));
		
		yield return www;
		
		//if ok response
		if (www.error == null) {
			Debug.Log("WWW Ok! Full Text: " + www.text);

			string resultString = StoreProperties.INSTANCE.parseRawHTTPresponseString(www.text);

			if (resultString.ToUpper() == "Y") {
				isPaidFlag = true;
			}
			
			// check for errors
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}    
	}


	IEnumerator SetupPurchase() {

		WWWForm postData = new WWWForm();

		postData.AddField("SellerEmail", StoreProperties.INSTANCE.payPalEmailAddressOfSeller);
		postData.AddField("ItemName", PurchaseItemNameField.text);
		postData.AddField("ItemCost", PurchaseItemCostField.text.Substring(1));
		postData.AddField("ItemCurrencyCode", StoreProperties.INSTANCE.currencyCode);
		postData.AddField("PlayerID", StoreProperties.INSTANCE.playerID);
		postData.AddField("GameTitle", StoreProperties.INSTANCE.gameTitle);

		string sandboxFlag = (StoreProperties.INSTANCE.payPalEndpoint == StoreProperties.Environment.SANDBOX) ? "Y" : "N";
		postData.AddField("SandboxFlag", sandboxFlag);
		
		string setupPurchaseURL = "http://unityingameitemmanager.com/SetupPurchase.php";
		WWW www = new WWW(setupPurchaseURL, postData.data, StoreProperties.INSTANCE.createHeader(postData));		
		
		yield return www;
		
		//if ok response
		if (www.error == null) {
			Debug.Log("WWW Ok! Full Text: " + www.text);
			string resultString = StoreProperties.INSTANCE.parseRawHTTPresponseString(www.text);
			
			generatedPayID = resultString;
			
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}    
	}

	public void changePurchaseStatus (PurchaseStatus newStatus) {

		switch (newStatus) {

			case PurchaseStatus.NO_ITEM_SELECTED : {
				PurchaseItemFields.SetActive(false);
				PurchaseActionButton.gameObject.SetActive(false);
				PurchaseStatusField.text = "No Item Selected";
				PurchasePromptText.text = "No item is currently selected.  Please return to the Main Store screen and click the 'Buy' button for an item.";
				PurchaseCompleteNotification.enabled = false;
			} break;

			case PurchaseStatus.CREATING_PURCHASE : {
				PurchaseItemFields.SetActive(true);
				PurchaseActionButton.gameObject.SetActive(false);
				PurchaseStatusField.text = "Creating Purchase...";
				PurchasePromptText.text = "Please wait while the purchase is being set up for the following item:";
			} break;

			case PurchaseStatus.CHECKOUT_READY : {				
				PurchaseItemFields.SetActive(true);
				PurchaseActionButton.gameObject.SetActive(true);
				PurchaseStatusField.text = "Checkout Ready";
				PurchasePromptText.text = "Please click the 'Checkout' button to complete the purchase for this item with PayPal:";				
			} break;

			case PurchaseStatus.WAITING : {				
				PurchaseItemFields.SetActive(true);
				PurchaseActionButton.gameObject.SetActive(false);
				PurchaseStatusField.text = "Waiting";
				PurchasePromptText.text = "A Paypal tab has been opened in your web browser to complete the purchase for the following item:";				
			} break;


			case PurchaseStatus.COMPLETE : {				
				PurchaseItemFields.SetActive(true);
				PurchaseActionButton.gameObject.SetActive(false);
				PurchaseStatusField.text = "Purchase Completed!";
				PurchasePromptText.text = "Purchase complete! Your item can now be used from the inventory screen.";
				PurchaseCompleteNotification.enabled = true;				
			} break;

		}

	}

}