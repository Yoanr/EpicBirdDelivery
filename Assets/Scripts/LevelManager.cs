using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int difficulty_start;
    public int difficulty_timer;
    public int cooldown_obs;
    public float cooldown_reduction;
    public GameObject cloud;
    public GameObject heart;
    public int heart_frequency;
    public int nbr_cloud_max;
    public int cloud_speed;
    public int cloud_speed_increase;
    private int cloud_speed_current;

    private float cooldown_reduction_current;
    private List<GameObject> clouds = new List<GameObject>();
    private float timer;
    private float timer_diff;

    // Use this for initialization
    private void Start()
    {
        timer = Time.time;
        timer_diff = Time.time;
        cooldown_reduction_current = difficulty_start;
        cloud_speed_current = cloud_speed;
        //
    }

    public void Initialize()
    {
        for (int i = 0; i < clouds.Count; i++)
        {
            GameObject.Destroy(clouds[i]);
        }
        clouds = new List<GameObject>();
        timer = Time.time;
        timer_diff = Time.time;
        cooldown_reduction_current = difficulty_start;
        cloud_speed_current = cloud_speed;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.time - timer >= cooldown_obs - cooldown_reduction_current)
        {
            InstanciateCloud();

            timer = Time.time;
        }
        if (Time.time - timer_diff >= difficulty_timer)
        {
            cooldown_reduction_current *= cooldown_reduction;
            cloud_speed_current = cloud_speed_current + cloud_speed_increase;

            timer_diff = Time.time;
        }
        for (int i = 0; i < clouds.Count; i++)
        {
            MoveCloud(i);
        }
    }

    private void InstanciateCloud()
    {
        int indice_unused = -1;
        for (int i = 0; i < clouds.Count; i++)
        {
            if (clouds[i] != null && !clouds[i].activeSelf)
            {
                indice_unused = i;
                break;
            }
        }
        if (indice_unused > -1)
        {
            float position_obs_tmp = Random.Range(-9.0f, 9.0f);
            float scale_alea = Random.Range(100, 200);
            clouds[indice_unused].transform.position = new Vector3(position_obs_tmp, 10, 0);
            //clouds[indice_unused].transform.localScale = new Vector3(scale_alea, scale_alea, 1);
            clouds[indice_unused].SetActive(true);
        }
        else
        {
            float position_obs_tmp = Random.Range(-9.0f, 9.0f);
            float scale_alea = Random.Range(100, 200);
            GameObject clone = null;
            if (Random.Range(0, 100) >= heart_frequency)
            {
                clone = GameObject.Instantiate(heart);
                clone.transform.parent = gameObject.transform;
                clone.transform.position = new Vector3(position_obs_tmp, 10, 0);
                clone.transform.localScale = new Vector3(60, 60, 1);
            }
            else
            {
                clone = GameObject.Instantiate(cloud);
                clone.transform.parent = gameObject.transform;
                clone.transform.position = new Vector3(position_obs_tmp, 10, 0);
                clone.transform.localScale = new Vector3(scale_alea, scale_alea, 1);
            }

            clouds.Add(clone);
        }
    }

    private void MoveCloud(int i)
    {
        if (clouds[i] != null)
        {
            clouds[i].transform.position += Time.deltaTime * Vector3.down * cloud_speed_current;
            checkMoveCloud(i);
        }
    }

    private void checkMoveCloud(int i)
    {
        if (clouds[i].transform.position.y <= -30)
        {
            clouds[i].SetActive(false);
        }
    }
}