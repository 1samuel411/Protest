using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class StoreProperties : MonoBehaviour {

	public static StoreProperties INSTANCE;

	void Awake() {
		INSTANCE = this;
	}
	
	public enum Environment {
		SANDBOX, PRODUCTION
	}
	
	public Environment payPalEndpoint;

	public string payPalEmailAddressOfSeller;

	public string currencyCode;

	public string gameTitle;

	[HideInInspector]
	public string playerID;


	public enum StoreTheme {
		BASIC,
		AQUA_PAPER,
		DARK_STONE,
		DIAMOND,
		BUBBLES,
		MARBLE,
		METAL,
		MOSS,
		PINSTRIPE,
		WEATHERED,
		WOOD
	}
	
	public StoreTheme storeTheme = StoreTheme.BASIC;
	
	[HideInInspector]
	public GameObject[] storeScreens;

	[HideInInspector]
	public Text[] textBoxes;

	[HideInInspector]
	public Text playerIDText;

	// Use this for initialization
	void Start () {

		//if basic is selected then don't change background
		if (storeTheme != StoreTheme.BASIC) {
	
			for (int i=0; i<storeScreens.Length; i++) {
				GameObject nextStoreScreen = storeScreens [i];
				nextStoreScreen.GetComponent<Image> ().sprite = Resources.Load <Sprite> ("StoreThemes/" + storeTheme.ToString ());
				nextStoreScreen.GetComponent<Image> ().color = Color.white;
			}
		}

		setTextColours ();

		string gameCode = "";
		if (gameTitle.Length > 3) {
			gameCode = gameTitle.Substring(0,3);
		} else {
			gameCode = gameTitle;
		}

		//if playerID doesn't exist then create it
		if (!PlayerPrefs.HasKey ("PlayerID")) {
			PlayerPrefs.SetString ("PlayerID", gameCode + DateTime.Now.ToString ("yyyyMMddHHmmssffff"));
		}

		playerID = PlayerPrefs.GetString ("PlayerID");
		playerIDText.text = playerID;

	}

	public string parseRawHTTPresponseString(string HTTPresponse) {
		
		return HTTPresponse.Split(new string[]{"***"}, System.StringSplitOptions.None)[1];;
		
	}


	public Dictionary<string,string> createHeader(WWWForm form) {
		
		Dictionary<string,string> header = new Dictionary<string,string>();
		
		header = form.headers;

		return header;
		
	}

	private void setTextColours() {

		Color textColorToUse = Color.black;

		switch (storeTheme) {

		case StoreTheme.METAL : 
		case StoreTheme.DARK_STONE :
		case StoreTheme.BASIC :
		case StoreTheme.WEATHERED : {

				textColorToUse = Color.white;

			} break;

		}

		foreach (Text t in textBoxes) {
			t.color = textColorToUse;
		}

	}
	
}
