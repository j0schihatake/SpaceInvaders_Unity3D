using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyNetworkHUD : MonoBehaviour
{
	public MyNetworkManager manager = null;

	public static MyNetworkHUD Instance = null;

	public Text record_score_text = null;

	//Компоненты ввода ip
	public GameObject ip_panel = null;
	public Text ip_text = null;
	public bool schow_ip = false;

	public bool host = false;

	//Ссылка на главное меню:
	public GameObject main_menu = null;

	void Start ()
	{
		Instance = this;
	}

	void Update ()
	{
		if (Application.loadedLevel == 0) {
			main_menu.SetActive (true);
			record_score_text.text = PlayerPrefs.GetInt ("score").ToString ();
		} else {
			main_menu.SetActive (false);
		}
	}

	public void on_of_menu ()
	{
		if (main_menu.activeSelf) {
			main_menu.SetActive (false);
		} else {
			main_menu.SetActive (true);
		}
	}

	//Метод при нажатии старт Host:
	public void on_host_button ()
	{
		manager.StartHost ();
		on_of_menu ();
	}

	public void on_client_button ()
	{
		//Сначала делаем панель видимой:
		if (!schow_ip) {
			schow_ip = true;
			ip_panel.SetActive (true);
		} else {
			//При повторном нажатии запускаем игру:
			if (!ip_text.text.Equals ("")) {
				manager.networkAddress = ip_text.text;
			} else {
				manager.networkAddress = "localhost";
			}
			manager.StartClient ();
			on_of_menu ();	
			ip_panel.SetActive (false);
		}
	}

	//При нажатии кнопки выход:
	public void on_button_exit ()
	{
		Application.Quit ();
	}

}
