using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Main : NetworkBehaviour
{

	public static Main Instance;
	//Список префабов врагов:
	public List<GameObject> enemy_list = new List<GameObject> ();

	public List<GameObject> spawn_point = new List<GameObject> ();

	public bool pause = false;

	//Для оптимизации не создаю а просто перемещаю и перезапускаю взрыв:
	public GameObject missile_explosion = null;
	public ParticleSystem missile_Particle_System = null;
	public GameObject big_explosion = null;
	public ParticleSystem big_Explosion_Particle_System = null;

	public Text record_score = null;
	public int record_int = 0;
	public Text score = null;
	public int score_int = 0;

	public float height_Field = 0f;
	public float width_Field = 84f;

	public bool is_host = false;
	public bool or_local = false;

	//Параметр сложнсти игры:
	public float divicuite = 0.01f;
	public float speed_coefficient = 0.3f;

	public float enemy_spawn_pause = 10f;
	float times = 0f;

	void Start ()
	{
		Instance = this;
		if (MyNetworkHUD.Instance != null) {
			or_local = true;
			//Отключаем Main у всех кроме хоста:
			if (MyNetworkHUD.Instance.host) {
				record_int = PlayerPrefs.GetInt ("score");
				Cmd_update_records (record_int);
				this.is_host = true;
			}
		}
	}

	void Update ()
	{
		//выполняем только у хоста:
		if (is_host) {
			times += Time.deltaTime;
			if (times > enemy_spawn_pause) {
				enemy_spawn_pause -= divicuite;
				speed_coefficient += divicuite;
				times = 0f;
				enemy_spawner ();
			}
		}
	}

	//Обновляем счет во всех представлениях(в следующей версии будет выполняться с помощью реактивных решений!)
	public void update_Score ()
	{	
		if (is_host) {
			Cmd_update_score ();
		}
	}

	[ClientRpc]
	public void Rpc_update_records (int records)
	{
		record_score.text = "/" + records.ToString ();
	}

	[Command]
	public void Cmd_update_records (int records)
	{
		Main.Instance.Rpc_update_records (records);
	}

	[ClientRpc]
	public void Rpc_update_score (int scores)
	{
		score_int += 1;
		score.text = score_int.ToString ();
	}

	[Command]
	public void Cmd_update_score ()
	{
		Main.Instance.Rpc_update_score (this.score_int);
	}

		
	//Создаем и запускаем противников:
	public void enemy_spawner ()
	{
		int random_enemy = (int)Random.Range (0, enemy_list.Count);
		int random_spawn_point = (int)Random.Range (0, spawn_point.Count);
		GameObject new_enemy = (GameObject)Instantiate (enemy_list [random_enemy], spawn_point [random_spawn_point].transform.position, spawn_point [random_spawn_point].transform.rotation);
		Enemy enemyes = new_enemy.GetComponent<Enemy> ();
		//Усложняем игру:
		enemyes.speed_schips += speed_coefficient;
		enemyes.pause_schoot -= divicuite;
		enemyes.main = this;
		Network_enemy new_enemy_network = new_enemy.GetComponent<Network_enemy> ();
		//чтобы снизить уровень нагрузки на сеть, от болванчиков не на сервере будет требоваться лишь синхронизация позиции и момента выстрела(пули синхронизировть не стану пусть будет немного мистики))
		//Собственно создаем такой же обьект везде:
		MyNetworkHUD.Instance.manager.spawn_enemy_schips (new_enemy);
		//Назначаем главных болванчиков:
		enemyes.is_host = true;
		new_enemy_network.is_host = true;
	}

	//У всех пауза)
	[ClientRpc]
	public void Rpc_on_pause_button ()
	{
		if (pause) {
			pause = false;
			Time.timeScale = 1.0f;
		} else {
			pause = true;
			Time.timeScale = 0.01f;
		}
	}
	//пробуем вызвать паузу для всех прямо на сервере:
	[Command]
	public void Cmd_on_pause_button ()
	{
		Main.Instance.Rpc_on_pause_button ();
	}

	public void On_Pause_Button ()
	{
		if (is_host) {
			Cmd_on_pause_button ();
		}
	}
}
