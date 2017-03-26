using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager
{
	[SerializeField]
	[Range (1, 10)]
	private float r = 5f;
	public NetworkClient myClient;

	/// <summary>
	/// при добавлении игрока на сервер:
	/// </summary>
	/// <param name="conn"></param>
	/// <param name="playerControllerId"></param>
	public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId)
	{
		Vector3 pos = new Vector3 (0, 0, 0);

		var player = Instantiate (playerPrefab, pos, Quaternion.identity) as GameObject;

		NetworkServer.AddPlayerForConnection (conn, player, playerControllerId);
	}

	public override void OnClientError (NetworkConnection conn, int errorCode)
	{
		base.OnClientError (conn, errorCode);
		//MyNetworkHUD.Instance.main_menu.SetActive (true);
	}

	public override void OnServerError (NetworkConnection conn, int errorCode)
	{
		base.OnServerError (conn, errorCode);
		//MyNetworkHUD.Instance.main_menu.SetActive (true);
	}

	public override void OnServerDisconnect (NetworkConnection conn)
	{
		base.OnServerDisconnect (conn);
		//MyNetworkHUD.Instance.main_menu.SetActive (true);
	}

	public void spawn_enemy_schips (GameObject enemy)
	{
		NetworkServer.Spawn (enemy);
	}

	public Vector3 GetRandomPosition ()
	{
		var point = Random.insideUnitCircle * r;

		return new Vector3 (point.x, 0, point.y);
	}

	//-----------------------------------------------------------------При нажатии кнопки  start загружаем меню
	public override void OnStartHost ()
	{
		base.OnStartHost ();
		Application.LoadLevel (1);
		//Запоминаем хоста:
		MyNetworkHUD.Instance.host = true;
	}
}