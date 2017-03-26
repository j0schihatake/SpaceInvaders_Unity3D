using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public GameObject bullet_prefab = null;
	public Rigidbody myRigidbody = null;
	public Vector3 movedirection = Vector3.zero;
	public Vector3 nextPosition = Vector3.zero;

	public bool controll = true;

	public float speed = 20f;

	//Помечаем игрока:
	public bool islocalPlayer = false;

	public List<GameObject> schoot_Point = null;

	void Update ()
	{
		if (controll) {
			Move ();
		}
	}

	public void schoot ()
	{
		for (int i = 0; i < schoot_Point.Count; i++) {
			//Для каждой точки выстрела(например если их несколько)
			GameObject selected_schoot_point = schoot_Point [i];
			//Выполняем выстрел:
			GameObject clone = (GameObject)Instantiate (bullet_prefab, selected_schoot_point.transform.position, selected_schoot_point.transform.localRotation);
		}
	}

	//Метод перемещения (на входе список поинтов(просчитанный путь)):
	private void Move ()
	{
		movedirection = (myRigidbody.rotation * new Vector3 (Input.GetAxis ("Horizontal") * speed, 0, 0));
		myRigidbody.MovePosition (GetComponent<Rigidbody> ().position + movedirection * Time.deltaTime);
		if (Input.GetAxis ("Horizontal") == 0) {
			myRigidbody.velocity = Vector3.zero;
		}
	}

	//Взрываем игрока и заканчиваем игру
	public void destroy_and_end_level ()
	{
		//Debug.Log ("В меня попали...");
		//Итак конец игры:
		if (PlayerPrefs.GetInt ("score") < Main.Instance.score_int) {
			PlayerPrefs.SetInt ("score", Main.Instance.score_int);
		}
		MyNetworkHUD.Instance.manager.StopHost ();
		Application.LoadLevel ("MainMenu");
		MyNetworkHUD.Instance.on_of_menu ();
	}
}
