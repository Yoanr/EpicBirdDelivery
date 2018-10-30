using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperFXManager : MonoBehaviour
{
    #region Constants

    //****************************************************************************************************************
    //**                                                   Public                                                   **
    //****************************************************************************************************************

    //****************************************************************************************************************
    //**                                                  Protected                                                 **
    //****************************************************************************************************************

    //****************************************************************************************************************
    //**                                                   Private                                                  **
    //****************************************************************************************************************

    private enum FX
    {
        TWINKLETEXT,
        TWINKLEIMAGE,
        JUMPINGIMAGE,
        MOREAPPLES,
        COLORFADE,
    }

    #endregion Constants

    #region Fields

    //****************************************************************************************************************
    //**                                                 Serialised                                                 **
    //****************************************************************************************************************

    [SerializeField]
    private FX fx;

    [SerializeField]
    private float period;

    [SerializeField]
    private float magnitude;

    [SerializeField]
    [Range(0, 1)]
    private float opacity;

    [SerializeField]
    private float speed;

    [SerializeField]
    [Range(0, 1)]
    private float minAlpha = 0;

    [SerializeField]
    [Range(0, 1)]
    private float maxAlpha = 1;

    [SerializeField]
    private Color alternateColor = Color.yellow;

    //****************************************************************************************************************
    //**                                          Public (HideInInspector)                                          **
    //****************************************************************************************************************

    //****************************************************************************************************************
    //**                                                  Protected                                                 **
    //****************************************************************************************************************

    //****************************************************************************************************************
    //**                                                   Private                                                  **
    //****************************************************************************************************************

    private Text txt;

    private Image img;

    private SpriteRenderer spr;

    private Vector3 initPos;

    private GameObject cloneChild;

    public Vector3 InitPos
    {
        set
        {
            initPos = value;
        }
    }

    #endregion Fields

    #region Properties

    //****************************************************************************************************************
    //**                                                   Public                                                   **
    //****************************************************************************************************************

    //****************************************************************************************************************
    //**                                                  Protected                                                 **
    //****************************************************************************************************************

    //****************************************************************************************************************
    //**                                                   Private                                                  **
    //****************************************************************************************************************

    #endregion Properties

    #region Methods

    #region Unity

    //****************************************************************************************************************
    //**                                                Unity Methods                                               **
    //****************************************************************************************************************

    private void Awake()
    {
        txt = GetComponent<Text>();
        img = GetComponent<Image>();
        spr = GetComponent<SpriteRenderer>();
        initPos = transform.localPosition;
    }

    private void Update()
    {
        switch (fx)
        {
            case FX.TWINKLETEXT:
                if (txt != null) txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, ((int)Time.time * 10) % period == 0 ? 0 : 1);//minAlpha + (maxAlpha - minAlpha) * Mathf.Abs(Mathf.Cos(Time.time * 2 * Mathf.PI / period)));
                break;

            case FX.TWINKLEIMAGE:
                if (img != null) img.color = new Color(img.color.r, img.color.g, img.color.b, minAlpha + (maxAlpha - minAlpha) * Mathf.Abs(Mathf.Cos(Time.time * 2 * Mathf.PI / period)));
                break;

            case FX.JUMPINGIMAGE:
                if (img != null) img.transform.localPosition = initPos + Mathf.Cos(Time.time * 2 * Mathf.PI / period) * magnitude * Vector3.up;
                break;

            case FX.MOREAPPLES:
                if (img != null)
                {
                    if (cloneChild == null)
                    {
                        cloneChild = new GameObject();
                        cloneChild.transform.parent = transform;
                        cloneChild.transform.position = transform.position + magnitude * Vector3.down;
                        cloneChild.transform.localScale = new Vector3(1, 1, 1);
                        RectTransform rect = cloneChild.AddComponent<RectTransform>();
                        if (GetComponent<RectTransform>() != null) rect.sizeDelta = GetComponent<RectTransform>().sizeDelta;
                        Image childImg = cloneChild.AddComponent<Image>();
                        childImg.sprite = img.sprite;
                        childImg.color = new Color(1, 1, 1, opacity);
                    }
                    if (cloneChild.transform.position.y + Time.deltaTime * speed > transform.position.y)
                    {
                        cloneChild.transform.position = transform.position + magnitude * Vector3.down;
                        cloneChild.GetComponent<Image>().color = new Color(1, 1, 1, opacity);
                        gameObject.SetActive(false);
                        return;
                    }
                    cloneChild.transform.position += Time.deltaTime * speed * Vector3.up;
                    float t = 1 - Mathf.Abs((cloneChild.transform.position.y - transform.position.y) / magnitude);
                    cloneChild.GetComponent<Image>().color = new Color(1, 1, 1, Mathf.Min(1, 1 - (minAlpha + (maxAlpha - minAlpha) * t)));
                }
                break;

            case FX.COLORFADE:
                if (spr != null)
                    spr.color = Color.Lerp(Color.white, alternateColor, 0.5f * (1 + Mathf.Cos(Time.time * 2 * Mathf.PI / period)));
                if (img != null)
                    img.color = Color.Lerp(Color.white, alternateColor, 0.5f * (1 + Mathf.Cos(Time.time * 2 * Mathf.PI / period)));
                break;

            default:
                break;
        }
    }

    #endregion Unity

    #region Virtual/Override

    //****************************************************************************************************************
    //**                                                   Public                                                   **
    //****************************************************************************************************************

    //****************************************************************************************************************
    //**                                                  Protected                                                 **
    //****************************************************************************************************************

    //****************************************************************************************************************
    //**                                                   Private                                                  **
    //****************************************************************************************************************

    #endregion Virtual/Override

    #region Others

    //****************************************************************************************************************
    //**                                                   Public                                                   **
    //****************************************************************************************************************

    //****************************************************************************************************************
    //**                                                  Protected                                                 **
    //****************************************************************************************************************

    //****************************************************************************************************************
    //**                                                   Private                                                  **
    //****************************************************************************************************************

    #endregion Others

    #region Coroutines

    //****************************************************************************************************************
    //**                                                   Public                                                   **
    //****************************************************************************************************************

    //****************************************************************************************************************
    //**                                                  Protected                                                 **
    //****************************************************************************************************************

    //****************************************************************************************************************
    //**                                                   Private                                                  **
    //****************************************************************************************************************

    #endregion Coroutines

    #endregion Methods
}