using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoveEngine : MonoBehaviour
{
    /// <summary>
    /// direction dans laquelle le sprite bouge
    /// </summary>
    [SerializeField]
    private Vector2 direction;

    /// <summary>
    /// Pour savoir si on change la direction en sortant de l'écran
    /// </summary>
    [SerializeField]
    private bool randomOnSreenExit;

    /// <summary>
    /// valeurs minimium et maximum pour le random en X
    /// </summary>
    [SerializeField]
    private Vector2 minMaxX;

    /// <summary>
    /// valeurs minimium et maximum pour le random en Y
    /// </summary>
    [SerializeField]
    private Vector2 minMaxY;

    /// <summary>
    /// l'objet doit-il disparaître à la fin de son trajet
    /// </summary>
    [HideInInspector] public bool disapearAtEnd = false;

    // Update is called once per frame
    private void Update()
    {
        transform.position += new Vector3(direction.x, direction.y, 0) * Time.deltaTime;
        Vector3 viewPortPosition = Camera.main.WorldToViewportPoint(transform.position);
        if ((viewPortPosition.x < -0.5 && direction.x < 0) || (viewPortPosition.x > 1.5 && direction.x > 0) || (viewPortPosition.y < -0.5 && direction.y < 0) || (viewPortPosition.y > 1.5 && direction.y > 0))
        {
            if (disapearAtEnd)
            {
                gameObject.SetActive(false);
            }
            if (randomOnSreenExit)
            {
                direction = new Vector2(Random.Range(minMaxX.x, minMaxX.y), Random.Range(minMaxY.x, minMaxY.y));
            }
            Vector3 tmp = new Vector3(direction.x, direction.y, 0);
            tmp = new Vector3(0.5f, 0.5f, 0) - 1.5f * tmp.normalized;
            tmp.z = -Camera.main.transform.position.z;

            transform.position = Camera.main.ViewportToWorldPoint(tmp);
        }
    }
}