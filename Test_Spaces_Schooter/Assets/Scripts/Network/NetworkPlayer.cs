using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour
{
	[SerializeField]
	private Player myPlayerClient = null;

	public Rigidbody this_schip_rigidbody = null;
	private Vector3 next_position = Vector3.zero;

	//Синхронизирую позицию:
	[SyncVar]
	private float schipPositionX = 0;
	public float PositionX = 1f;
	[SyncVar]
	private float schipPositionY = 0;
	public float PositionY = 1f;
	[SyncVar]
	private float schipPositionZ = 0;
	public float PositionZ = 1f;

	[SyncVar]
	public float live = 3f;
	//текущее состояние техники

	public schipState state;

	public enum schipState
	{
		work,
		//рабочее состояние
		destroy,
		//уничтожен
		schoot,
		//поворот базы
		rotateleft,
		rotateright,
	}

	//Ссылка на пулю(пуля уничтожается в момент когда она попадает в другой танк):
	public GameObject bullets = null;

	// Если локальный игрок
	public override void OnStartLocalPlayer ()
	{
		//только у нащего юнита помечаем что он главный
		myPlayerClient.islocalPlayer = true;
		//Устанавливаем корректные начальные показатели положения
		schipPositionX = myPlayerClient.gameObject.GetComponent<Rigidbody> ().transform.position.x;
		schipPositionY = myPlayerClient.gameObject.GetComponent<Rigidbody> ().transform.position.y;
		schipPositionZ = myPlayerClient.gameObject.GetComponent<Rigidbody> ().transform.position.z;
		//обозначаем наш танк уникальным именем
		this.gameObject.name = "Schip_Local_Player";
	}

	void Update ()
	{
		//у обьекта на сервере будем отключать листенер
		if (!isLocalPlayer) {
			if (myPlayerClient.controll) {
				myPlayerClient.controll = false;
			}
		} else {
			if (state == schipState.schoot) {
				CmdSchoot (this.gameObject);
				state = schipState.work;
			}
			//выстрел на пробел
			if (Input.GetKeyDown (KeyCode.Space)) {
				state = schipState.schoot;
			}
		}
		if (state != schipState.destroy) {
			//итак если локальный игрок
			if (isLocalPlayer) {
				//Следим за координатами
				if (isPositionChanged ()) {
					CmdUpdatePosition (myPlayerClient.myRigidbody.position.x, myPlayerClient.myRigidbody.position.y, myPlayerClient.myRigidbody.position.z);
					PositionX = myPlayerClient.myRigidbody.position.x;
					PositionY = myPlayerClient.myRigidbody.position.y;
					PositionZ = myPlayerClient.myRigidbody.position.z;
				}
			} else {
				//обновляем координаты положения:
				if (isPositionUpdate ()) {
					setNewPosition (schipPositionX, schipPositionY, schipPositionZ);
				}
			}
		}
	}

	//метод выполняет выстрел
	public void onButtonSchoot ()
	{
		state = NetworkPlayer.schipState.schoot;
	}

	//Обновляем у представлений координаты
	private bool isPositionChanged ()
	{
		bool result = false;
		if (PositionX != myPlayerClient.myRigidbody.position.x) {
			result = true;
		}
		if (PositionY != myPlayerClient.myRigidbody.position.y) {
			result = true;
		}
		if (PositionZ != myPlayerClient.myRigidbody.position.z) {
			result = true;
		}
		return result;
	}

	private bool isPositionUpdate ()
	{
		bool result = false;
		if (schipPositionX != myPlayerClient.myRigidbody.position.x) {
			result = true;
		}
		if (schipPositionY != myPlayerClient.myRigidbody.position.y) {
			result = true;
		}
		if (schipPositionZ != myPlayerClient.myRigidbody.position.z) {
			result = true;
		}
		return result;
	}

	private void setNewPosition (float newX, float newY, float newZ)
	{
		next_position.x = newX;
		next_position.y = newY;
		next_position.z = newZ;
		this_schip_rigidbody.position = next_position;
	}

	//Команда обновляет позицию на сервере
	[Command]
	private void CmdUpdatePosition (float newX, float newY, float newZ)
	{
		schipPositionX = newX;
		schipPositionY = newY;
		schipPositionZ = newZ;
	}

	//пробуем вызвать выстрел прямо на сервере:
	[Command]
	public void CmdSchoot (GameObject player)
	{
		if (state != schipState.destroy) {
			//выстрел будет произведен в ту сторону куда сейчас указывает башня игрока
			NetworkPlayer PlayerNetw = player.GetComponent<NetworkPlayer> ();
			PlayerNetw.RpcSchoot ();
		}
	}

	[ClientRpc]
	public void RpcSchoot ()
	{
		//выстреливают все сущности этого игрока()
		//обозначаем пулую оригинал
		myPlayerClient.schoot ();
	}

	//просим сервер выполнить на всех танках удаление их пули
	[Command]
	public void CmdDestroyBullets (GameObject obj)
	{
		obj.GetComponent<NetworkPlayer> ().RpcDestroyBullets ();
	}

	//сам запрос на выполнение удаления
	[ClientRpc]
	public void RpcDestroyBullets ()
	{
		Destroy (bullets);
	}

	//Не забываем сохранить текущий рекорд:
	[Command]
	public void CmdSaveScore (int max_score)
	{
		
	}

	[ClientRpc]
	public void RpcSaveScore (int value_score)
	{
	}
}
