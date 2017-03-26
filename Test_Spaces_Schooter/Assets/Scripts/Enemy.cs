using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Enemy : NetworkBehaviour
{
	//Префаб снаряда:
	public GameObject bullet_prefab = null;
	public GameObject explosion_prefab = null;
	public Network_enemy my_network = null;

	//Не сразу убиваемые враги:
	public int live = 3;

	public bool is_host = false;

	public Main main = null;

	public List<GameObject> schoot_Point = null;
	//Список точек которые необходимо проследовать:
	public List<Vector3> patch = new List<Vector3> ();

	public Rigidbody myRigidbody = null;
	private bool IsMoving = false;
	public Transform targetPosition = null;
	private Vector3 lostPointVector3D = Vector3.zero;
	public Vector3 nextPosition = Vector3.zero;
	private int countPoint = 0;

	public float pause_schoot = 0f;
	public float timeCount = 0f;

	public float speed_schips = 10f;

	void Start ()
	{
		myRigidbody = this.gameObject.GetComponent<Rigidbody> ();
		targetPosition = myRigidbody.transform;
		nextPosition = targetPosition.position;
	}

	void Update ()
	{
		if (is_host) {
			//Увеличиваем время(враги будут стрелять периодически):
			timeCount += Time.deltaTime;
			if (timeCount > pause_schoot) {
				timeCount = 0;
				my_network.CmdSchoot (this.gameObject);
			}

			//Если я закину какую то позицию:
			if (targetPosition != null) {
				if (patch.Count == 0) {
					patch = get_random_point_list ();
				}
			}

			//Боту нужно следовать от точки к точке:
			if (patch.Count > 0) {
				if (!IsMoving) {
					if (patch.Count > 0) {
						countPoint = patch.Count - 1;
						nextPosition = patch [countPoint];
					}
				}
			}
			move (nextPosition);
		}
	}

	public void schoot ()
	{
		for (int i = 0; i < schoot_Point.Count; i++) {
			//Для каждой точки выстрела(например если их несколько)
			GameObject selected_schoot_point = schoot_Point [i];
			//Выполняем выстрел:
			GameObject clone = (GameObject)Instantiate (bullet_prefab, selected_schoot_point.transform.position, selected_schoot_point.transform.localRotation);
			clone.GetComponent<SFE_BulletController_Analog> ().enemy = this;
		}
	}

	//Генерируем случайный список точек:
	public List<Vector3> get_random_point_list ()
	{
		List<Vector3> point_list = new List<Vector3> ();
		int random_point_count = (int)Random.Range (2, 7);
		//Теперь создаем такое число случайных точек:
		for (int i = 0; i < random_point_count; i++) {
			float random_x = Random.Range (0, main.width_Field);
			float random_z = Random.Range (0, main.height_Field);
			Vector3 random = new Vector3 (random_x, 0, random_z);
			point_list.Add (random);
		}
		//Debug.Log (point_list.Count);
		return point_list;
	}

	//Выполнение перемещения к точке:
	void move (Vector3 nextPosition)
	{
		if (myRigidbody.transform.position != nextPosition) {
			IsMoving = true;
			myRigidbody.transform.position = Vector3.MoveTowards (myRigidbody.transform.position, nextPosition, speed_schips * Time.deltaTime);
		} else {
			if (patch.Count > 0) {
				patch.RemoveAt (patch.Count - 1);
			}
			if (targetPosition != null) {
				if (lostPointVector3D == targetPosition.position) {
					targetPosition = null;
				}
			}
			nextPosition = myRigidbody.transform.position;
			lostPointVector3D = nextPosition;
			//lostPoint = null;
			IsMoving = false;
		}
	}

	//уничтожвем противника и добавляем счет:
	public void destroy_and_add_score (int uron)
	{
		live -= uron;
		if (live <= 0) {
			if (is_host) {
				my_network.RpcDestroySchip ();
			}
		}
	}

	public void deatch ()
	{
		Main.Instance.big_explosion.transform.position = this.gameObject.transform.position;
		Main.Instance.big_Explosion_Particle_System.Play ();
		Main.Instance.update_Score ();
		Destroy (this.gameObject);
	}

	//Из условий тестового задания при столкновении тое проигрываем.
	void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.GetComponent<Player> ()) {
			collision.gameObject.GetComponent<Player> ().destroy_and_end_level ();
		}
	}
}
