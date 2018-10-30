using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        /// <summary>
        /// Classe qui gère les langues depuis l'éditeur
        /// </summary>
        public class LangWindowLanguages : MonoBehaviour
        {
            #region Constants

            //****************************************************************************************************************
            //**                                                   Public                                                   **
            //****************************************************************************************************************

            /// <summary>
            /// code à mettre au début du fichier LanguagesEnum.cs
            /// </summary>
            private static string LANGUAGE_ENUM_HEADER = "/// <summary>\r\n/// Namespace de From The Bard\r\n/// </summary>\r\nnamespace FromTheBard\r\n{\r\n\t/// <summary>\r\n\t/// Namespace du gestionnaire de langue\r\n\t/// </summary>\r\n\tnamespace Lang\r\n\t{\r\n\t\t/// <summary>\r\n\t\t/// /!\\ enum auto-généré. Langues disponibles\r\n\t\t/// </summary>\r\n\t\tpublic enum LanguagesEnum\r\n\t\t{\r\n";

            /// <summary>
            /// code à mettre à la fin du fichier LanguagesEnum.cs
            /// </summary>
            private static string LANGUAGE_ENUM_FOOTER = "\t\t}\r\n\t}\r\n}";

            #endregion Constants

            #region Fields

            //****************************************************************************************************************
            //**                                                   Private                                                  **
            //****************************************************************************************************************

            /// <summary>
            /// Est-on en train d'ajouter une nouvelle langue
            /// </summary>
            private static bool addingNewLanguage = false;

            /// <summary>
            /// Nom de la langue qu'on est en train d'ajouter
            /// </summary>
            private static string newLanguageName = "";

            /// <summary>
            /// Doit-on copier les polices d'une autre langue
            /// </summary>
            private static bool newLanguageCopyFonts = false;

            /// <summary>
            /// langue à partir de laquelle on doit copier les polices
            /// </summary>
            private static LanguagesEnum newLanguageCopiedFonts;

            /// <summary>
            /// Doit-on copier les textes d'une autre langue
            /// </summary>
            private static bool newLanguageCopyTexts = false;

            /// <summary>
            /// langue à partir de laquelle on doit copier les textes
            /// </summary>
            private static LanguagesEnum newLanguageCopiedTexts;

            /// <summary>
            /// Est-on en train de supprimer une langue
            /// </summary>
            private static bool deletingLanguage = false;

            /// <summary>
            /// Quelle langue supprimer
            /// </summary>
            private static LanguagesEnum deletingLanguageValue;

            /// <summary>
            /// position actuelle du scroll rect
            /// </summary>
            private static Vector2 scrollPosition = Vector2.zero;

            #endregion Fields

            #region Methods

            #region Others

            //****************************************************************************************************************
            //**                                                   Public                                                   **
            //****************************************************************************************************************

            /// <summary>
            /// choses à afficher sur l'éditeur
            /// </summary>
            public static void LanguagesGUI()
            {
                if (addingNewLanguage)
                {
                    LanguageAddNewLanguage();
                }
                else
                {
                    LanguageListGUI();
                }
            }

            /// <summary>
            /// affiche l'interface d'ajour de langue
            /// </summary>
            public static void LanguageAddNewLanguage()
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label(LangWindowOption.EditorInEnglish ? "Adding new language:" : "Ajout de langue :");

                    //Infos pour la création
                    GUILayout.Space(5);
                    //nom de la langue
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5);
                        GUILayout.Label("", GUI.skin.horizontalSliderThumb);
                        GUILayout.Space(5);
                        GUILayout.Label(LangWindowOption.EditorInEnglish ? "Name:" : "Nom :");
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    bool nameError = false;
                    foreach (LanguagesEnum val in Enum.GetValues(typeof(LanguagesEnum)))
                    {
                        if (val.ToString().Equals(newLanguageName))
                        {
                            nameError = true;
                            break;
                        }
                    }
                    Color oldGuiBackgroundColor = GUI.backgroundColor;
                    if (nameError)
                    {
                        GUI.backgroundColor = Color.red;
                    }
                    newLanguageName = EditorGUILayout.TextArea(newLanguageName);
                    if (nameError)
                    {
                        GUI.backgroundColor = oldGuiBackgroundColor;
                    }
                    GUILayout.Space(5);

                    //Copier les polices?
                    newLanguageCopyFonts = GUILayout.Toggle(newLanguageCopyFonts, newLanguageCopyFonts ? LangWindowOption.EditorInEnglish ? " Copy fonts from:" : " Copier les polices de :" : LangWindowOption.EditorInEnglish ? " Copy fonts from another language" : " Copier les polices d'une autre langue");
                    if (newLanguageCopyFonts)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("", GUI.skin.horizontalSliderThumb);
                            GUILayout.Space(5);
                            newLanguageCopiedFonts = (LanguagesEnum)EditorGUILayout.EnumPopup(newLanguageCopiedFonts);
                            GUILayout.FlexibleSpace();
                            GUILayout.Space(5);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Space(5);

                    //Copier les textes?
                    newLanguageCopyTexts = GUILayout.Toggle(newLanguageCopyTexts, newLanguageCopyTexts ? LangWindowOption.EditorInEnglish ? " Copy texts from:" : " Copier les textes de :" : LangWindowOption.EditorInEnglish ? " Copy texts from another language" : " Copier les textes d'une autre langue");
                    if (newLanguageCopyTexts)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("", GUI.skin.horizontalSliderThumb);
                            GUILayout.Space(5);
                            newLanguageCopiedTexts = (LanguagesEnum)EditorGUILayout.EnumPopup(newLanguageCopiedTexts);
                            GUILayout.FlexibleSpace();
                            GUILayout.Space(5);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Space(5);

                    //Boutons ajouter et annuler
                    GUILayout.BeginHorizontal();
                    {
                        if (nameError)
                        {
                            GUI.enabled = false;
                        }
                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Add" : "Ajouter"))
                        {
                            if (newLanguageCopyFonts)
                            {
                                if (!newLanguageCopyTexts)
                                {
                                    AddLanguageCopyFont(newLanguageName, newLanguageCopiedFonts);
                                }
                                else
                                {
                                    AddLanguage(newLanguageName, newLanguageCopiedFonts, newLanguageCopiedTexts);
                                }
                            }
                            else
                            {
                                if (!newLanguageCopyTexts)
                                {
                                    AddLanguage(newLanguageName);
                                }
                                else
                                {
                                    AddLanguageCopyText(newLanguageName, newLanguageCopiedTexts);
                                }
                            }
                            addingNewLanguage = false;
                        }
                        if (nameError)
                        {
                            GUI.enabled = true;
                        }
                        GUILayout.Space(10);
                        oldGuiBackgroundColor = GUI.backgroundColor;
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Cancel" : "Annuler"))
                        {
                            addingNewLanguage = false;
                        }
                        GUI.backgroundColor = oldGuiBackgroundColor;
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }

            /// <summary>
            /// Affiche la liste des langages (avec possibilité de suppression)
            /// </summary>
            public static void LanguageListGUI()
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label(LangWindowOption.EditorInEnglish ? "Languages list:" : "Liste de tous les langages :");
                    GUILayout.Space(8);

                    //Scrollview de toutes les langues
                    scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label("", GUI.skin.horizontalSlider);
                            foreach (LanguagesEnum language in Enum.GetValues(typeof(LanguagesEnum)))
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    //UI de suppression de langue
                                    if (deletingLanguage && deletingLanguageValue == language)
                                    {
                                        GUILayout.Label(LangWindowOption.EditorInEnglish ? "Are you sure you want to delete " + language.ToString() + "?" : "Êtes-vous sûr de vouloir supprimer " + language.ToString() + " ?");
                                        GUILayout.Space(10);
                                        Color oldUIColor = GUI.backgroundColor;
                                        GUI.backgroundColor = Color.green;
                                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Yes" : "Oui"))
                                        {
                                            RemoveLanguage(language);
                                            deletingLanguage = false;
                                        }
                                        GUI.backgroundColor = oldUIColor;
                                        GUILayout.Space(10);
                                        GUI.backgroundColor = Color.red;
                                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "No" : "Non"))
                                        {
                                            deletingLanguage = false;
                                        }
                                        GUI.backgroundColor = oldUIColor;
                                        GUILayout.FlexibleSpace();
                                    }
                                    //UI normale
                                    else
                                    {
                                        GUILayout.Label(language.ToString());
                                        GUILayout.FlexibleSpace();
                                        if (language != LanguagesEnum.English && GUILayout.Button(LangWindowOption.EditorInEnglish ? "Delete" : "Supprimer"))
                                        {
                                            deletingLanguage = true;
                                            deletingLanguageValue = language;
                                        }
                                    }
                                }
                                GUILayout.EndHorizontal();
                                GUILayout.Label("", GUI.skin.horizontalSlider);
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndScrollView();
                    GUILayout.Space(8);

                    //Bouton d'ajout de langue
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Add new Language" : "Ajouter une nouvelle langue"))
                        {
                            newLanguageName = "";
                            newLanguageCopyFonts = false;
                            newLanguageCopyTexts = false;
                            addingNewLanguage = true;
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(8);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndVertical();
            }

            /// <summary>
            /// Copie récursivement le dossier
            /// </summary>
            /// <param name="sourceFolder">dossier source</param>
            /// <param name="destFolder">destination</param>
            public static void CopyFolder(string sourceFolder, string destFolder)
            {
                if (!System.IO.Directory.Exists(destFolder))
                    System.IO.Directory.CreateDirectory(destFolder);

                string[] files = System.IO.Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = System.IO.Path.GetFileName(file);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    System.IO.File.Copy(file, dest);
                }
                string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = System.IO.Path.GetFileName(folder);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);
                }
            }

            //****************************************************************************************************************
            //**                                                   Private                                                  **
            //****************************************************************************************************************

            /// <summary>
            /// Ajoute une langue
            /// </summary>
            /// <param name="name">Nom de la langue à ajouter</param>
            private static void AddLanguage(string name)
            {
                List<string> names = new List<string>();
                foreach (LanguagesEnum val in Enum.GetValues(typeof(LanguagesEnum)))
                {
                    names.Add(val.ToString());
                }
                if (!names.Contains(name))
                {
                    names.Add(name);
                    SaveLanguageList(names);
                }

                if (System.IO.Directory.Exists("Assets/Resources/Lang/Fonts/" + name))
                {
                    System.IO.Directory.Delete("Assets/Resources/Lang/Fonts/" + name, true);
                }
                System.IO.Directory.CreateDirectory("Assets/Resources/Lang/Fonts/" + name);

                AddLanguageToText(name);
            }

            /// <summary>
            /// Ajoute une langue
            /// </summary>
            /// <param name="name">Nom de la langue à ajouter</param>
            private static void AddLanguageCopyFont(string name, LanguagesEnum font)
            {
                List<string> names = new List<string>();
                foreach (LanguagesEnum val in Enum.GetValues(typeof(LanguagesEnum)))
                {
                    names.Add(val.ToString());
                }
                if (!names.Contains(name))
                {
                    names.Add(name);
                    SaveLanguageList(names);
                }

                if (System.IO.Directory.Exists("Assets/Resources/Lang/Fonts/" + name))
                {
                    System.IO.Directory.Delete("Assets/Resources/Lang/Fonts/" + name, true);
                }
                if (System.IO.Directory.Exists("Assets/Resources/Lang/Fonts/" + font.ToString()))
                {
                    CopyFolder("Assets/Resources/Lang/Fonts/" + font.ToString(), "Assets/Resources/Lang/Fonts/" + name);
                }
                else
                {
                    System.IO.Directory.CreateDirectory("Assets/Resources/Lang/Fonts/" + name);
                }

                AddLanguageToText(name);
            }

            /// <summary>
            /// Ajoute une langue
            /// </summary>
            /// <param name="name">Nom de la langue à ajouter</param>
            /// <param name="langToCopy">Langue à partir de laquelle copier les textes</param>
            private static void AddLanguageCopyText(string name, LanguagesEnum langToCopy)
            {
                List<string> names = new List<string>();
                foreach (LanguagesEnum val in Enum.GetValues(typeof(LanguagesEnum)))
                {
                    names.Add(val.ToString());
                }
                if (!names.Contains(name))
                {
                    names.Add(name);
                    SaveLanguageList(names);
                }

                if (System.IO.Directory.Exists("Assets/Resources/Lang/Fonts/" + name))
                {
                    System.IO.Directory.Delete("Assets/Resources/Lang/Fonts/" + name, true);
                }
                System.IO.Directory.CreateDirectory("Assets/Resources/Lang/Fonts/" + name);

                AddLanguageToText(name, langToCopy);
            }

            /// <summary>
            /// Ajoute une langue
            /// </summary>
            /// <param name="name">Nom de la langue à ajouter</param>
            /// <param name="langToCopy">Langue à partir de laquelle copier les textes</param>
            private static void AddLanguage(string name, LanguagesEnum font, LanguagesEnum langToCopy)
            {
                List<string> names = new List<string>();
                foreach (LanguagesEnum val in Enum.GetValues(typeof(LanguagesEnum)))
                {
                    names.Add(val.ToString());
                }
                if (!names.Contains(name))
                {
                    names.Add(name);
                    SaveLanguageList(names);
                }

                if (System.IO.Directory.Exists("Assets/Resources/Lang/Fonts/" + name))
                {
                    System.IO.Directory.Delete("Assets/Resources/Lang/Fonts/" + name, true);
                }
                if (System.IO.Directory.Exists("Assets/Resources/Lang/Fonts/" + font.ToString()))
                {
                    CopyFolder("Assets/Resources/Lang/Fonts/" + font.ToString(), "Assets/Resources/Lang/Fonts/" + name);
                }
                else
                {
                    System.IO.Directory.CreateDirectory("Assets/Resources/Lang/Fonts/" + name);
                }

                AddLanguageToText(name);
                AddLanguageToText(name, langToCopy);
            }

            /// <summary>
            /// Sauvegarde la liste des <paramref name="names"/> dans l'énum <see cref="LanguagesEnum"/>
            /// </summary>
            /// <param name="names">liste des elements à mettre dans l'enum <see cref="LanguagesEnum"/></param>
            private static void SaveLanguageList(List<string> names)
            {
                names.Sort();
                string code = LANGUAGE_ENUM_HEADER;
                foreach (string name in names)
                {
                    code += "\t\t\t" + name + ",\r\n";
                }
                code += LANGUAGE_ENUM_FOOTER;
                System.IO.File.WriteAllText("Assets/Packages/Lang/Scripts/Structs/LanguagesEnum.cs", code);
                AssetDatabase.Refresh();
            }

            /// <summary>
            /// Ajoute une langue a tous les dictionnaires
            /// </summary>
            /// <param name="name">Nom de la langue à ajouter</param>
            private static void AddLanguageToText(string name)
            {
                foreach (DictionariesEnum dictionary in Enum.GetValues(typeof(DictionariesEnum)))
                {
                    TextAsset txtAsset = Resources.Load<TextAsset>("Lang/Texts/" + dictionary.ToString());

                    if (txtAsset == null)
                    {
                        continue;
                    }
                    string txtString = txtAsset.text;

                    string[] lines = txtString.Split(System.Environment.NewLine.ToCharArray());

                    if (lines.Length <= 0)
                    {
                        continue;
                    }

                    string[] languages = lines[0].Split('|');
                    Dictionary<string, int> languagesIndexes = new Dictionary<string, int>();
                    List<string> languagesList = new List<string>();
                    for (int i = 1; i < languages.Length; i++)
                    {
                        languagesList.Add(languages[i]);
                        languagesIndexes.Add(languages[i], i);
                    }
                    if (languagesList.Contains(name))
                    {
                        continue;
                    }

                    languagesList.Add(name);
                    languagesList.Sort();

                    Dictionary<string, string[]> tmpDico = new Dictionary<string, string[]>();
                    string[] line;
                    string[] newline;
                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (lines[i] == "")
                        {
                            continue;
                        }
                        line = lines[i].Split('|');
                        newline = new string[languagesList.Count];
                        for (int j = 0; j < newline.Length; j++)
                        {
                            if (languagesIndexes.ContainsKey(languagesList[j]))
                            {
                                newline[j] = line[languagesIndexes[languagesList[j]]];
                            }
                            else
                            {
                                newline[j] = dictionary.ToString() + "_" + line[0];
                            }
                        }
                        tmpDico.Add(line[0], newline);
                    }

                    string dictionaryText = "";
                    for (int i = 0; i < languagesList.Count; i++)
                    {
                        dictionaryText += "|" + languagesList[i];
                    }
                    dictionaryText += System.Environment.NewLine;
                    foreach (KeyValuePair<string, string[]> pair in tmpDico)
                    {
                        dictionaryText += pair.Key;
                        for (int i = 0; i < pair.Value.Length; i++)
                        {
                            dictionaryText += "|" + pair.Value[i];
                        }
                        dictionaryText += System.Environment.NewLine;
                    }

                    System.IO.File.WriteAllText("Assets/Resources/Lang/Texts/" + dictionary.ToString() + ".txt", dictionaryText);
                }
                AssetDatabase.Refresh();
            }

            /// <summary>
            /// Ajoute une langue a tous les dictionnaires
            /// </summary>
            /// <param name="name">Nom de la langue à ajouter</param>
            /// <param name="langToCopy">Langue à partir de laquelle copier</param>
            private static void AddLanguageToText(string name, LanguagesEnum langToCopy)
            {
                foreach (DictionariesEnum dictionary in Enum.GetValues(typeof(DictionariesEnum)))
                {
                    TextAsset txtAsset = Resources.Load<TextAsset>("Lang/Texts/" + dictionary.ToString());

                    if (txtAsset == null)
                    {
                        continue;
                    }
                    string txtString = txtAsset.text;

                    string[] lines = txtString.Split(System.Environment.NewLine.ToCharArray());

                    if (lines.Length <= 0)
                    {
                        continue;
                    }

                    string[] languages = lines[0].Split('|');
                    Dictionary<string, int> languagesIndexes = new Dictionary<string, int>();
                    List<string> languagesList = new List<string>();
                    for (int i = 1; i < languages.Length; i++)
                    {
                        languagesList.Add(languages[i]);
                        languagesIndexes.Add(languages[i], i);
                    }
                    if (languagesList.Contains(name))
                    {
                        continue;
                    }

                    languagesList.Add(name);
                    languagesList.Sort();

                    Dictionary<string, string[]> tmpDico = new Dictionary<string, string[]>();
                    string[] line;
                    string[] newline;
                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (lines[i] == "")
                        {
                            continue;
                        }
                        line = lines[i].Split('|');
                        newline = new string[languagesList.Count];
                        for (int j = 0; j < newline.Length; j++)
                        {
                            if (languagesIndexes.ContainsKey(languagesList[j]))
                            {
                                newline[j] = line[languagesIndexes[languagesList[j]]];
                            }
                            else
                            {
                                if (languagesIndexes.ContainsKey(langToCopy.ToString()))
                                {
                                    newline[j] = line[languagesIndexes[langToCopy.ToString()]];
                                }
                                else
                                {
                                    newline[j] = dictionary.ToString() + "_" + line[0];
                                }
                            }
                        }
                        tmpDico.Add(line[0], newline);
                    }

                    string dictionaryText = "";
                    for (int i = 0; i < languagesList.Count; i++)
                    {
                        dictionaryText += "|" + languagesList[i];
                    }
                    dictionaryText += System.Environment.NewLine;
                    foreach (KeyValuePair<string, string[]> pair in tmpDico)
                    {
                        dictionaryText += pair.Key;
                        for (int i = 0; i < pair.Value.Length; i++)
                        {
                            dictionaryText += "|" + pair.Value[i];
                        }
                        dictionaryText += System.Environment.NewLine;
                    }

                    System.IO.File.WriteAllText("Assets/Resources/Lang/Texts/" + dictionary.ToString() + ".txt", dictionaryText);
                }
                AssetDatabase.Refresh();
            }

            /// <summary>
            /// Retire <paramref name="lang"/> des dictionnaires
            /// </summary>
            /// <param name="lang">Langue à retirer</param>
            private static void RemoveLanguage(LanguagesEnum lang)
            {
                if (System.IO.Directory.Exists("Assets/Resources/Lang/Fonts/" + lang.ToString()))
                {
                    System.IO.Directory.Delete("Assets/Resources/Lang/Fonts/" + lang.ToString(), true);
                }

                List<string> names = new List<string>();
                foreach (LanguagesEnum val in Enum.GetValues(typeof(LanguagesEnum)))
                {
                    names.Add(val.ToString());
                }
                if (names.Contains(lang.ToString()))
                {
                    names.Remove(lang.ToString());
                    SaveLanguageList(names);
                }

                foreach (DictionariesEnum dictionary in Enum.GetValues(typeof(DictionariesEnum)))
                {
                    TextAsset txtAsset = Resources.Load<TextAsset>("Lang/Texts/" + dictionary.ToString());

                    if (txtAsset == null)
                    {
                        continue;
                    }
                    string txtString = txtAsset.text;

                    string[] lines = txtString.Split(System.Environment.NewLine.ToCharArray());

                    if (lines.Length <= 0)
                    {
                        continue;
                    }

                    string[] languages = lines[0].Split('|');
                    Dictionary<string, int> languagesIndexes = new Dictionary<string, int>();
                    List<string> languagesList = new List<string>();
                    for (int i = 1; i < languages.Length; i++)
                    {
                        languagesList.Add(languages[i]);
                        languagesIndexes.Add(languages[i], i);
                    }
                    if (!languagesList.Contains(lang.ToString()))
                    {
                        continue;
                    }

                    languagesList.Remove(lang.ToString());
                    languagesIndexes.Remove(lang.ToString());
                    languagesList.Sort();

                    Dictionary<string, string[]> tmpDico = new Dictionary<string, string[]>();
                    string[] line;
                    string[] newline;
                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (lines[i] == "")
                        {
                            continue;
                        }
                        line = lines[i].Split('|');
                        newline = new string[languagesList.Count];
                        for (int j = 0; j < newline.Length; j++)
                        {
                            if (languagesIndexes.ContainsKey(languagesList[j]))
                            {
                                newline[j] = line[languagesIndexes[languagesList[j]]];
                            }
                        }
                        tmpDico.Add(line[0], newline);
                    }

                    string dictionaryText = "";
                    for (int i = 0; i < languagesList.Count; i++)
                    {
                        dictionaryText += "|" + languagesList[i];
                    }
                    dictionaryText += System.Environment.NewLine;
                    foreach (KeyValuePair<string, string[]> pair in tmpDico)
                    {
                        dictionaryText += pair.Key;
                        for (int i = 0; i < pair.Value.Length; i++)
                        {
                            dictionaryText += "|" + pair.Value[i];
                        }
                        dictionaryText += System.Environment.NewLine;
                    }

                    System.IO.File.WriteAllText("Assets/Resources/Lang/Texts/" + dictionary.ToString() + ".txt", dictionaryText);
                }
                AssetDatabase.Refresh();
            }

            #endregion Others

            #endregion Methods
        }
    }
}