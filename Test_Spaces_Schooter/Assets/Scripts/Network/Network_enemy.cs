using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_enemy : NetworkBehaviour
{
	[SerializeField]
	private Enemy myPlayerClient = null;

	//Список точек которые необходимо проследовать:
	public List<Vector3> patch = new List<Vector3> ();

	public bool is_host = false;

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

	void Start ()
	{
		myPlayerClient = this.gameObject.GetComponent<Enemy> ();
		myPlayerClient.my_network = this;
	}

	void Update ()
	{
		//у обьекта на сервере будем отключать листенер
		if (!isLocalPlayer) {
		} else {
			if (state == schipState.schoot) {
				CmdSchoot (this.gameObject);
				state = schipState.work;
			}
		}
	}
		
	//метод выполняет выстрел
	public void onButtonSchoot ()
	{
		state = Network_enemy.schipState.schoot;
	}

	//запрос к серверу
	[Command]
	public void CmdDestroySchip (GameObject obj)
	{
		obj.GetComponent<Network_enemy> ().RpcDestroySchip ();
	}

	//выполнение команды на всех тенях
	[ClientRpc]
	public void RpcDestroySchip ()
	{
		myPlayerClient.deatch ();
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
		myPlayerClient.nextPosition = new Vector3 (newX, newY, newZ);
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
			Network_enemy PlayerNetw = player.GetComponent<Network_enemy> ();
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

	//пробую вызвать команду вызывающую сообщение прямо на сервере
	[Command]
	public void CmdDamaged (float damage)
	{
		//myPlayerClient.live -= damage;    
	}

	[ClientRpc]
	public void RpcTalkToDamaged (GameObject ong)
	{
	}

	//просим сервер выполнить на всех танках удаление их пули
	[Command]
	public void CmdDestroyBullets (GameObject obj)
	{
		obj.GetComponent<Network_enemy> ().RpcDestroyBullets ();
	}

	//сам запрос на выполнение удаления
	[ClientRpc]
	public void RpcDestroyBullets ()
	{
		Destroy (bullets);
	}

}
