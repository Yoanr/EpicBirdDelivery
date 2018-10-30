using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NocturnAlleyManager : MonoBehaviour
{
    public GameObject wall;
    public GameObject hole;

    [SerializeField]
    private float speed;

    [SerializeField]
    [Range(0, 100)]
    private int wallRatio;

    public float period;

    public float minVerticalValue;

    private List<GameObject> instanciatedWalls = new List<GameObject>();

    private List<GameObject> instanciatedHoles = new List<GameObject>();

    private float nextTime = 0;

    // Use this for initialization
    private void Start()
    {
        BoxCollider2D[] initTab = GetComponentsInChildren<BoxCollider2D>();
        for (int i = 0; i < initTab.Length; i++)
        {
            instanciatedWalls.Add(initTab[i].gameObject);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.time > nextTime)
        {
            nextTime = Time.time + period;
            Instanciate();
        }
        Translation();
    }

    private void Instanciate()
    {
        GameObject obj = null;
        int recycleIndice = -1;
        int rnd = Random.Range(0, 100);
        if (wallRatio < rnd)
        {
            obj = wall;
            for (int i = 0; i < instanciatedWalls.Count; i++)
            {
                if (!instanciatedWalls[i].activeSelf)
                {
                    recycleIndice = i;
                    break;
                }
            }
        }
        else
        {
            obj = hole;
            for (int i = 0; i < instanciatedHoles.Count; i++)
            {
                if (!instanciatedHoles[i].activeSelf)
                {
                    recycleIndice = i;
                    break;
                }
            }
        }

        if (recycleIndice > -1)
        {
            if (obj == wall)
            {
                instanciatedWalls[recycleIndice].SetActive(true);
                instanciatedWalls[recycleIndice].transform.localPosition = new Vector3(0, 0, 0);
            }
            else
            {
                instanciatedHoles[recycleIndice].SetActive(true);
                instanciatedHoles[recycleIndice].transform.localPosition = new Vector3(0, 0, 0);
            }
        }
        else
        {
            GameObject clone = GameObject.Instantiate(obj);
            clone.transform.parent = gameObject.transform;
            clone.transform.localPosition = new Vector3(0, 0, 0);
            clone.transform.localScale = new Vector3(100, 100, 1);

            if (obj == wall)
                instanciatedWalls.Add(clone);
            else
                instanciatedHoles.Add(clone);
        }
    }

    private void Translation()
    {
        for (int i = 0; i < instanciatedWalls.Count; i++)
        {
            if (instanciatedWalls[i].activeSelf)
            {
                instanciatedWalls[i].transform.position += Vector3.down * Time.deltaTime * speed;
                if (instanciatedWalls[i].transform.position.y * 100 <= minVerticalValue)
                    instanciatedWalls[i].SetActive(false);
            }
        }
        for (int i = 0; i < instanciatedHoles.Count; i++)
        {
            if (instanciatedHoles[i].activeSelf)
            {
                instanciatedHoles[i].transform.position += Vector3.down * Time.deltaTime * speed;
                if (instanciatedHoles[i].transform.position.y * 100 <= minVerticalValue)
                    instanciatedHoles[i].SetActive(false);
            }
        }
    }
}