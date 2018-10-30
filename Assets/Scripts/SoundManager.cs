using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Classe qui gère quasiment tout les sons
/// </summary>
public class SoundManager : Singleton<SoundManager>
{
    /// <summary>
    /// lien vers la source sonore (pour la musique)
    /// </summary>
    private static AudioSource source;

    /// <summary>
    /// lien vers la source sonore (pour les bruitages)
    /// </summary>
    private static AudioSource sourceBruitage;

    /// <summary>

    /// <summary>
    /// Coroutine utilisé pour le FadeIn/FadeOut du barde
    /// </summary>
    private IEnumerator maCoroutine;

    /// <summary>
    /// Retient si la coroutine est lancée ou pas
    /// </summary>
    private bool used;

    //************************
    //********Musiques********
    //************************

    public AudioClip titleScreen;
    public AudioClip level;

    //************************
    //********Bruitages*******
    //************************
    /// <summary>
    /// Lien vers le bruitage du logo
    /// </summary>
    public AudioClip aouch;

    public AudioClip button_press;
    public AudioClip victory;
    public AudioClip lose;
    public AudioClip nocturnAouch;
    public AudioClip sendLetter;

    /// <summary>
    /// Initialisation
    /// </summary>
    public void Awake()
    {
        used = false;

        GameObject sourceGO = GameObject.Instantiate(new GameObject());
        sourceGO.AddComponent<AudioSource>();
        sourceGO.name = "Musique";

        GameObject sourceBruitageGO = GameObject.Instantiate(new GameObject());
        sourceBruitageGO.AddComponent<AudioSource>();
        sourceBruitageGO.name = "Bruitage";

        sourceGO.transform.SetParent(this.transform);
        sourceBruitageGO.transform.SetParent(this.transform);
        source = sourceGO.GetComponent<AudioSource>();
        sourceBruitage = sourceBruitageGO.GetComponent<AudioSource>();
    }

    /// <summary>
    /// Change le volume de la musique
    /// </summary>
    /// <param name="newVolume">Nouveau volume</param>
    public static void ChangeVolume(int newVolume)
    {
        Instance.InstanceChangeVolume(newVolume);
    }

    /// <summary>
    /// Comme ChangeVolume mais pour une instance
    /// </summary>
    /// <param name="newVolume">Nouveau volume</param>
    private void InstanceChangeVolume(int newVolume)
    {
        source.volume = ((float)newVolume) / 100;
    }

    /// <summary>
    /// Change le volume des bruitages
    /// </summary>
    /// <param name="newVolume">Nouveau volume</param>
    public static void ChangeBruitage(int newVolume)
    {
        Instance.InstanceChangeBruitage(newVolume);
    }

    /// <summary>
    /// Comme ChangeBruitage mais pour une instance
    /// </summary>
    /// <param name="newVolume">Nouveau volume</param>
    private void InstanceChangeBruitage(int newVolume)
    {
        sourceBruitage.volume = ((float)newVolume) / 100;
    }

    //*************************
    //********Bruitages********
    //*************************
    /// <summary>
    /// Joue un son sur la piste de bruitage
    /// </summary>
    /// <param name="name">Nom du son à jouer</param>
    public static void PlayBruitage(string name)
    {
        Instance.InstancePlayBruitage(name);
    }

    /// <summary>
    /// Comme PlayBruitage mais pour une instance
    /// </summary>
    /// <param name="name">{Logo}Nom du son à jouer</param>
    private void InstancePlayBruitage(string name)
    {
        AudioClip originalClip;
        originalClip = null;
        switch (name)
        {
            case "aouch":
                originalClip = aouch;
                break;

            case "buttonPress":
                originalClip = button_press;
                break;

            case "nocturnAouch":
                originalClip = nocturnAouch;
                break;

            case "victory":
                originalClip = victory;
                break;

            case "lose":
                originalClip = lose;
                break;

            case "sendLetter":
                originalClip = sendLetter;
                break;
        }
        sourceBruitage.PlayOneShot(originalClip);
    }

    //************************
    //********Musiques********
    //************************
    /// <summary>
    /// Fait un fade depuis l'ancienne musique vers la nouvelle
    /// </summary>
    /// <param name="name">Nom de la musique à jouer</param>
    public static void ChangeMusique(string name)
    {
        Instance.InstanceChangeMusique(name);
    }

    /// <summary>
    /// Comme ChangeMusique mais pour une instance
    /// </summary>
    /// <param name="name">Nom de la musique à jouer</param>
    private void InstanceChangeMusique(string name)
    {
        StartCoroutine(FadeMusique(name));
    }

    /// <summary>
    /// Fait un fade depuis l'ancienne musique vers la nouvelle
    /// </summary>
    /// <param name="name">Nom de la nouvelle musique</param>
    private IEnumerator FadeMusique(string name)
    {
        bool played = source.isPlaying;
        float tmp = source.volume;
        if (played)
        {
            for (int i = 4; i > -1; i--)
            {
                source.volume = tmp * i * 0.2f;
                yield return new WaitForSeconds(0.1f);
            }
            source.Stop();
        }

        PlayMusique(name);

        if (played)
        {
            for (int i = 1; i < 6; i++)
            {
                source.volume = tmp * i * 0.2f;
                yield return new WaitForSeconds(0.1f);
            }
            //source.volume = OptionFileManager.GetVolume() / 100f;
            source.volume = 0.5f;
        }
    }

    /// <summary>
    /// Lance la couroutine <see cref="StopMusiqueCoroutine"/> qui stope la musique avec un fade
    /// </summary>
    public static void StopMusique()
    {
        Instance.InstanceStopMusique();
    }

    /// <summary>
    /// Comme StopMusique mais pour une instance
    /// </summary>
    private void InstanceStopMusique()
    {
        StartCoroutine(StopMusiqueCoroutine());
    }

    /// <summary>
    /// Stope la musique avec un fade
    /// </summary>
    private IEnumerator StopMusiqueCoroutine()
    {
        float tmp = source.volume;
        for (int i = 4; i > -1; i--)
        {
            source.volume = tmp * i * 0.2f;
            yield return new WaitForSeconds(0.1f);
        }
        source.Stop();
    }

    /// <summary>
    /// Joue un son sur la piste de musique
    /// </summary>
    /// <param name="name">Nom de la musique à jouer</param>
    public static void PlayMusique(string name)
    {
        Instance.InstancePlayMusique(name);
    }

    /// <summary>
    /// Comme PlayMusique mais pour une instance
    /// </summary>
    /// <param name="name">{""}Nom de la musique à jouer</param>
    private void InstancePlayMusique(string name)
    {
        source.Stop();
        AudioClip originalClip = null;
        //TODO ajouter un swich pour chaque musique
        switch (name)
        {
            case "titleScreen":
                originalClip = titleScreen;
                break;

            case "level":
                originalClip = level;
                break;
        }
        source.clip = originalClip;
        source.loop = true;
        source.Play();
    }
}