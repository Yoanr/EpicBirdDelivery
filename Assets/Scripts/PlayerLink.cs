using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLink : MonoBehaviour
{
    [SerializeField]
    private GameObject p1;

    [SerializeField]
    private GameObject p2;

    [SerializeField]
    private Color impossibleTranmissionColor;

    [SerializeField]
    private SpriteRenderer bigLinkRenderer;

    public LayerMask nocturnCollisionMask;

    private SpriteRenderer renderer;

    // Use this for initialization
    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position = 0.5f * (p1.transform.position + p2.transform.position);
        transform.localRotation = new Quaternion(0, 0, 0, 1);
        transform.localScale = new Vector3(Vector3.Distance(p1.transform.position, p2.transform.position) * 9, transform.localScale.y, transform.localScale.z); //pourquoi 9, je sais pas !
        transform.Rotate(Vector3.forward, Vector2.SignedAngle(Vector2.right, p2.transform.position - p1.transform.position));

        RaycastHit2D hit = Physics2D.Raycast(p1.transform.position, p2.transform.position - p1.transform.position, Vector3.Distance(p1.transform.position, p2.transform.position) * 9, nocturnCollisionMask);
        if (hit)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !GameManager.hasLost)
            {
                GameManager.hp--;
                SoundManager.PlayBruitage("nocturnAouch");
                bigLinkRenderer.color = Color.red;
                if (!bigLinkRenderer.gameObject.activeSelf) StartCoroutine(LINKCoroutine());
            }
            renderer.color = impossibleTranmissionColor;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && !GameManager.hasLost)
            {
                SoundManager.PlayBruitage("sendLetter");
                bigLinkRenderer.color = Color.cyan;
                if (!bigLinkRenderer.gameObject.activeSelf) StartCoroutine(LINKCoroutine());
            }
            renderer.color = Color.white;
        }
    }

    private IEnumerator LINKCoroutine()
    {
        bigLinkRenderer.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        bigLinkRenderer.gameObject.SetActive(false);
    }
}