using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Namespace de From The Bard
/// </summary>
namespace FromTheBard
{
    /// <summary>
    /// Namespace du gestionnaire de langue
    /// </summary>
    namespace Lang
    {
        public delegate void Callback();

        public class Lang : Singleton<Lang>
        {
            #region Constants

            //****************************************************************************************************************
            //**                                                   Public                                                   **
            //****************************************************************************************************************

            public static string PLAYER_PREFS_LANG = "PLAYER_PREFS_LANG";

            private static List<Delegate> CALLBACKS = new List<Delegate>();

            //****************************************************************************************************************
            //**                                                  Protected                                                 **
            //****************************************************************************************************************

            //****************************************************************************************************************
            //**                                                   Private                                                  **
            //****************************************************************************************************************

            #endregion Constants

            #region Fields

            //****************************************************************************************************************
            //**                                                 Serialised                                                 **
            //****************************************************************************************************************

            //****************************************************************************************************************
            //**                                          Public (HideInInspector)                                          **
            //****************************************************************************************************************

            //****************************************************************************************************************
            //**                                                  Protected                                                 **
            //****************************************************************************************************************

            /// <summary>
            /// Dictionnaire contenant tous les textes du jeu
            /// </summary>
            protected Dictionary<DictionariesEnum, Dictionary<string, string>> texts;

            /// <summary>
            /// dictionnaire des polices
            /// </summary>
            protected Dictionary<FontsEnum, Font> fonts;

            //****************************************************************************************************************
            //**                                                   Private                                                  **
            //****************************************************************************************************************

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
            //**                                             For Unity Methods                                              **
            //****************************************************************************************************************

            /// <summary>
            /// Initialisation du dictionnaire de langues. Charge lang.fr.text ou lang.en.text en fonction de la valeur de Application.systemLanguage
            /// </summary>
            public void Awake()
            {
                if (PlayerPrefs.HasKey(PLAYER_PREFS_LANG))
                {
                    SetLang((LanguagesEnum)PlayerPrefs.GetInt(PLAYER_PREFS_LANG));
                }
                else
                {
                    Dictionary<string, int> tmp = new Dictionary<string, int>();
                    int tmp2 = 0;
                    foreach (LanguagesEnum val in Enum.GetValues(typeof(LanguagesEnum)))
                    {
                        tmp.Add(val.ToString(), tmp2);
                        tmp2++;
                    }
                    if (tmp.ContainsKey(Application.systemLanguage.ToString()))
                    {
                        SetLang((LanguagesEnum)tmp[Application.systemLanguage.ToString()]);
                    }
                    else
                    {
                        SetLang(LanguagesEnum.English);
                    }
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

            /// <summary>
            /// Renvoi la police associée à <paramref name="fontName"/> (ou null si elle n'existe pas)
            /// </summary>
            /// <param name="fontName">nom de la police à retourner</param>
            /// <returns>La police associée à <paramref name="fontName"/> (ou null si elle n'existe pas)</returns>
            public static Font GetFont(FontsEnum fontName)
            {
                return Instance.InstanceGetFont(fontName);
            }

            /// <summary>
            /// Renvoi Instance.GetText(key)
            /// </summary>
            /// <param name="dictionary">Dictionnaire dans lequel chercher</param>
            /// <param name="key">Variable à passer à Instance.GetText</param>
            /// <returns>Instance.GetText(key)</returns>
            public static string Get(DictionariesEnum dictionary, string key)
            {
                return Instance.GetText(dictionary, key);
            }

            /// <summary>
            /// Change la langue
            /// </summary>
            /// <param name="lang">langue choisie</param>
            /// <returns>True si la langue à bien changée</returns>
            public static bool SetLang(LanguagesEnum lang)
            {
                return Instance.InstanceSetLang(lang);
            }

            /// <summary>
            /// Ajoute le callback à la liste des callbacks à invoquer lors du changement de langue
            /// </summary>
            /// <param name="method">callback à ajouter</param>
            public static void AddEvent(Callback method)
            {
                CALLBACKS.Add(method);
            }

            /// <summary>
            /// Retire le callback de la liste des invocations lors du changement de langue
            /// </summary>
            /// <param name="method">callback à retirer</param>
            public static void RemoveEvent(Callback method)
            {
                if (CALLBACKS.Contains(method))
                {
                    CALLBACKS.Remove(method);
                }
            }

            /// <summary>
            /// Retire tous les callbacks de la liste des invocations au moment du changement de langue
            /// </summary>
            public static void ClearAllEvents()
            {
                CALLBACKS.Clear();
            }

            #endregion Others

            //****************************************************************************************************************
            //**                                                  Protected                                                 **
            //****************************************************************************************************************

            //****************************************************************************************************************
            //**                                                   Private                                                  **
            //****************************************************************************************************************

            /// <summary>
            /// Renvoi la police associée à <paramref name="fontName"/> (ou null si elle n'existe pas)
            /// </summary>
            /// <param name="fontName">nom de la police à retourner</param>
            /// <returns>La police associée à <paramref name="fontName"/> (ou null si elle n'existe pas)</returns>
            private Font InstanceGetFont(FontsEnum fontName)
            {
                if (fonts.ContainsKey(fontName))
                {
                    return fonts[fontName];
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Récupère la chaîne ayant pour clé key
            /// </summary>
            /// <param name="dictionary">Dictionnaire dans lequel chercher</param>
            /// <param name="key">Clé du texte a récupérer</param>
            /// <returns>Renvoi le texte qui à pour clé key. Renvoi la clé si elle ne correspond à aucun texte</returns>
            private string GetText(DictionariesEnum dictionary, string key)
            {
                if (texts.ContainsKey(dictionary))
                {
                    if (texts[dictionary].ContainsKey(key))
                    {
                        // Rajout de la gestion des clés de lang
                        return GestionCle(texts[dictionary][key], dictionary, key);
                    }
                    else
                    {
                        return dictionary.ToString() + "_" + key;
                    }
                }
                else
                {
                    return "No dictionary : " + dictionary;
                }
            }

            /// <summary>
            /// Change la langue
            /// </summary>
            /// <param name="lang">langue choisie</param>
            /// <returns>True si la langue à bien changée</returns>
            private bool InstanceSetLang(LanguagesEnum lang)
            {
                Dictionary<DictionariesEnum, Dictionary<string, string>> tmpTexts = new Dictionary<DictionariesEnum, Dictionary<string, string>>();
                Dictionary<string, string> tmp;
                foreach (DictionariesEnum val in Enum.GetValues(typeof(DictionariesEnum)))
                {
                    tmp = LoadTextDictionary(val, lang);
                    if (tmp == null)
                    {
                        Debug.LogError("Lang : error while charging text dictionary \"" + val + "\" in \"" + lang + "\"! Not changing anything in dictionaries!");
                        return false;
                    }
                    else
                    {
                        tmpTexts.Add(val, tmp);
                    }
                }

                Dictionary<FontsEnum, Font> tmpFonts = new Dictionary<FontsEnum, Font>();
                Font tmpFont;
                foreach (FontsEnum val in Enum.GetValues(typeof(FontsEnum)))
                {
                    tmpFont = LoadFont(val, lang);
                    if (tmpFont == null)
                    {
                        Debug.LogError("Lang : error while charging font \"" + val + "\" in \"" + lang + "\"! Not changing anything in dictionaries");
                        return false;
                    }
                    else
                    {
                        tmpFonts.Add(val, tmpFont);
                    }
                }

                texts = tmpTexts;
                fonts = tmpFonts;

                foreach (Delegate d in CALLBACKS.ToArray())
                {
                    Callback c = (Callback)d;
                    if (c != null)
                        c();
                }

                return true;
            }

            /// <summary>
            /// Renvoi le dictionnaire associé au dictionnaire <paramref name="dictionary"/> pour la langue <paramref name="lang"/>.
            /// </summary>
            /// <param name="dictionary">dictionnaire qu'on veut charger</param>
            /// <param name="lang">langue qu'on veut charger</param>
            /// <returns></returns>
            private Dictionary<string, string> LoadTextDictionary(DictionariesEnum dictionary, LanguagesEnum lang)
            {
                TextAsset txtAsset = Resources.Load<TextAsset>("Lang/Texts/" + dictionary.ToString());

                if (txtAsset == null)
                {
                    Debug.LogError("Lang : no file at: Lang/Texts/" + dictionary.ToString());
                    return null;
                }
                string txtString = txtAsset.text;

                string[] lines = txtString.Split(System.Environment.NewLine.ToCharArray());

                if (lines.Length <= 0)
                {
                    Debug.LogError("Lang : not enough lines in Lang/Texts/" + dictionary.ToString());
                    return null;
                }

                string[] languages = lines[0].Split('|');
                int languageIndex = -1;
                for (int i = 0; i < languages.Length; i++)
                {
                    if (languages[i].Equals(lang.ToString()))
                    {
                        languageIndex = i;
                        break;
                    }
                }
                if (languageIndex < 0)
                {
                    Debug.LogError("Lang : " + lang.ToString() + " not found in Lang/Texts/" + dictionary.ToString());
                    return null;
                }

                Dictionary<string, string> ret = new Dictionary<string, string>();
                string[] line;
                char[] tmpCharArray;
                for (int i = 1; i < lines.Length; i++)
                {
                    line = lines[i].Split('|');
                    if (line.Length <= 0 || lines[i] == "")
                    {
                    }
                    else if (line.Length == 1)
                    {
                        Debug.LogError("LangPackage: dictionary " + dictionary + "  line " + i + " found only the key, skipping this line");
                    }
                    else if (!line[0].Equals(""))
                    {
                        tmpCharArray = line[languageIndex].ToCharArray();
                        if (tmpCharArray[0] == '"' && tmpCharArray[tmpCharArray.Length - 1] == '"')
                        {
                            line[languageIndex] = line[languageIndex].Substring(1, line[languageIndex].Length - 2);
                        }
                        line[languageIndex] = line[languageIndex].Replace("\"\"", "\"");
                        line[languageIndex] = line[languageIndex].Replace("\\n", "\n");
                        line[languageIndex] = line[languageIndex].Replace("{$pipe}", "|");
                        ret.Add(line[0], line[languageIndex]);
                    }
                }
                return ret;
            }

            private Font LoadFont(FontsEnum font, LanguagesEnum lang)
            {
                return Resources.Load<Font>("Lang/Fonts/" + lang.ToString() + "/" + font.ToString());
            }

            /// <summary>
            /// Parse le texte pour remplacer les clés de lang par leur valeur
            /// </summary>
            /// <param name="texte"></param>
            /// <returns></returns>
            private string GestionCle(string texte, DictionariesEnum dictionary, string key)
            {
                char c;
                string res = "";
                string tmp, tmpRes = "";

                for (int i = 0; i < texte.Length; i++)
                {
                    c = texte[i];

                    if (c == '{')
                    {
                        tmp = "";
                        while (i + 1 < texte.Length && texte[i + 1] != ':' && texte[i + 1] != '}')
                        {
                            i++;
                            tmp += texte[i];
                        }
                        if (i + 1 >= texte.Length)
                        {
                            Debug.LogError("Erreur de clé dans le lang : " + dictionary.ToString() + ", " + key);
                            break;
                        }

                        i++;
                        if (i + 1 < texte.Length)
                        {
                            if (texte[i + 1] == 'b')
                                i = i + 4;
                            else if (texte[i + 1] == 'g')
                            {
                                i = i + 5;
                            }
                            else
                            {
                                Debug.LogError("Erreur de clé (girl ou boy) dans le lang : " + dictionary.ToString() + ", " + key);
                                break;
                            }
                        }
                        else
                        {
                            Debug.LogError("Erreur de clé dans le lang : " + dictionary.ToString() + ", " + key);
                            break;
                        }

                        if (tmpRes == "")
                        {
                            Debug.LogError("Erreur de clé incohérente dans le lang : " + dictionary.ToString() + ", " + key);
                            break;
                        }

                        res += tmpRes;
                        continue;
                    }

                    res += c;
                }

                return res;
            }

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
    }
}