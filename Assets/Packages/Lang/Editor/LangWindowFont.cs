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
        /// Classe qui gère les polices depuis l'éditeur
        /// </summary>
        public class LangWindowFont : MonoBehaviour
        {
            #region Constants

            //****************************************************************************************************************
            //**                                                   Private                                                  **
            //****************************************************************************************************************

            /// <summary>
            /// Les différents onglés de la fenêtre dans l'éditeur
            /// </summary>
            private enum Tab
            {
                CHECK,
                MANAGE,
            }

            /// <summary>
            /// code à mettre au début du fichier FontsEnum.cs
            /// </summary>
            private static string FONT_ENUM_HEADER = "/// <summary>\r\n/// Namespace de From The Bard\r\n/// </summary>\r\nnamespace FromTheBard\r\n{\r\n\t/// <summary>\r\n\t/// Namespace du gestionnaire de langue\r\n\t/// </summary>\r\n\tnamespace Lang\r\n\t{\r\n\t\t/// <summary>\r\n\t\t/// /!\\ enum auto-généré. Polices disponibles\r\n\t\t/// </summary>\r\n\t\tpublic enum FontsEnum\r\n\t\t{\r\n";

            /// <summary>
            /// code à mettre à la fin du fichier FontEnum.cs
            /// </summary>
            private static string FONT_ENUM_FOOTER = "\t\t}\r\n\t}\r\n}";

            #endregion Constants

            #region Fields

            //****************************************************************************************************************
            //**                                                   Private                                                  **
            //****************************************************************************************************************

            /// <summary>
            /// position actuelle du scroll rect
            /// </summary>
            private static Vector2 languageScrollPosition = Vector2.zero;

            /// <summary>
            /// Est-on en train de montrer les détails des polices pour une langue
            /// </summary>
            private static bool showingDetails = false;

            /// <summary>
            /// De quelle langue on est en train de montrer les détails des polices
            /// </summary>
            private static LanguagesEnum langToDetail;

            /// <summary>
            /// Est-on en train d'ajouter une nouvelle police
            /// </summary>
            private static bool addingNewFont = false;

            /// <summary>
            /// Nom de la police qu'on est en train d'ajouter
            /// </summary>
            private static string newFontName = "";

            /// <summary>
            /// Doit-on copier les polices d'une autre police
            /// </summary>
            private static bool newFontCopyFonts = false;

            /// <summary>
            /// Police à partir de laquelle on doit copier les polices
            /// </summary>
            private static FontsEnum newFontCopiedFonts;

            /// <summary>
            /// Est-on en train de supprimer une police
            /// </summary>
            private static bool deletingFont = false;

            /// <summary>
            /// Quelle police supprimer
            /// </summary>
            private static FontsEnum deletingFontValue;

            /// <summary>
            /// position actuelle du scroll rect
            /// </summary>
            private static Vector2 fontScrollPosition = Vector2.zero;

            /// <summary>
            /// Onglé actuellement sélectionné
            /// </summary>
            private static Tab actualTab = Tab.CHECK;

            /// <summary>
            /// chemin de la nouvelle police
            /// </summary>
            private static string newFontPath;

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
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(8);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5);
                        if (GUILayout.Toggle(actualTab == Tab.CHECK, LangWindowOption.EditorInEnglish ? "Check" : "Vérifier", EditorStyles.toolbarButton))
                        {
                            actualTab = Tab.CHECK;
                        }
                        if (GUILayout.Toggle(actualTab == Tab.MANAGE, LangWindowOption.EditorInEnglish ? "Manage" : "Gérer", EditorStyles.toolbarButton))
                        {
                            actualTab = Tab.MANAGE;
                        }
                        GUILayout.Space(5);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5);
                        switch (actualTab)
                        {
                            case Tab.CHECK:
                                FontCheckGUI();
                                break;

                            case Tab.MANAGE:
                                FontManageGUI();
                                break;

                            default:
                                GUILayout.Label("Hey! You broke the UI!");
                                break;
                        }
                        GUILayout.Space(5);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }
                GUILayout.EndVertical();
            }

            //****************************************************************************************************************
            //**                                                   Private                                                  **
            //****************************************************************************************************************

            /// <summary>
            /// Affiche la partie vérification
            /// </summary>
            private static void FontCheckGUI()
            {
                GUILayout.BeginVertical();
                {
                    if (!showingDetails)
                    {
                        GUILayout.Space(8);
                        GUILayout.Label(LangWindowOption.EditorInEnglish ? "Font global state:" : "État global des polices :");
                        GUILayout.Space(8);

                        //Scrollview de toutes les langues
                        languageScrollPosition = GUILayout.BeginScrollView(languageScrollPosition);
                        {
                            GUILayout.BeginVertical();
                            {
                                GUILayout.Label("", GUI.skin.horizontalSlider);
                                foreach (LanguagesEnum language in Enum.GetValues(typeof(LanguagesEnum)))
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.Label(language.ToString());
                                        GUILayout.FlexibleSpace();
                                        GUILayout.Space(5);
                                        int missings = 0;
                                        foreach (FontsEnum font in Enum.GetValues(typeof(FontsEnum)))
                                        {
                                            if (System.IO.Directory.Exists("Assets/Resources/Lang/Fonts/" + language.ToString()))
                                            {
                                                if (!System.IO.File.Exists("Assets/Resources/Lang/Fonts/" + language.ToString() + "/" + font.ToString() + ".ttf")
                                                && !System.IO.File.Exists("Assets/Resources/Lang/Fonts/" + language.ToString() + "/" + font.ToString() + ".dfont")
                                                && !System.IO.File.Exists("Assets/Resources/Lang/Fonts/" + language.ToString() + "/" + font.ToString() + ".otf"))
                                                {
                                                    missings++;
                                                }
                                            }
                                            else
                                            {
                                                missings++;
                                            }
                                        }
                                        if (missings <= 0)
                                        {
                                            Color oldGUIContentColor = GUI.contentColor;
                                            GUI.contentColor = Color.green;
                                            GUILayout.Label("OK");
                                            GUI.contentColor = oldGUIContentColor;
                                        }
                                        else
                                        {
                                            Color oldGUIContentColor = GUI.contentColor;
                                            GUI.contentColor = Color.red;
                                            GUILayout.Label(LangWindowOption.EditorInEnglish ? ("Missing " + missings + " font" + (missings > 1 ? "s" : "")) : (missings + " police" + (missings > 1 ? "s" : "") + " manquante" + (missings > 1 ? "s" : "")));
                                            GUI.contentColor = oldGUIContentColor;
                                        }
                                        GUILayout.Space(5);
                                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Details" : "Détails"))
                                        {
                                            showingDetails = true;
                                            langToDetail = language;
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
                        GUILayout.FlexibleSpace();
                    }
                    else
                    {
                        GUILayout.Space(8);
                        GUILayout.Label(LangWindowOption.EditorInEnglish ? "The fonts of " + langToDetail.ToString() + ":" : "Polices en " + langToDetail.ToString() + " :");
                        GUILayout.Space(8);

                        //Scrollview de toutes les langues
                        languageScrollPosition = GUILayout.BeginScrollView(languageScrollPosition);
                        {
                            GUILayout.BeginVertical();
                            {
                                GUILayout.Label("", GUI.skin.horizontalSlider);
                                foreach (FontsEnum font in Enum.GetValues(typeof(FontsEnum)))
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.Label(font.ToString());
                                        GUILayout.FlexibleSpace();
                                        GUILayout.Space(5);
                                        bool ok;
                                        if (System.IO.Directory.Exists("Assets/Resources/Lang/Fonts/" + langToDetail.ToString()))
                                        {
                                            ok = !System.IO.File.Exists("Assets/Resources/Lang/Fonts/" + langToDetail.ToString() + "/" + font.ToString() + ".ttf")
                                                && !System.IO.File.Exists("Assets/Resources/Lang/Fonts/" + langToDetail.ToString() + "/" + font.ToString() + ".dfont")
                                                && !System.IO.File.Exists("Assets/Resources/Lang/Fonts/" + langToDetail.ToString() + "/" + font.ToString() + ".otf");
                                        }
                                        else
                                        {
                                            ok = true;
                                        }

                                        if (!ok)
                                        {
                                            Color oldGUIContentColor = GUI.contentColor;
                                            GUI.contentColor = Color.green;
                                            GUILayout.Label("OK");
                                            GUI.contentColor = oldGUIContentColor;
                                        }
                                        else
                                        {
                                            Color oldGUIContentColor = GUI.contentColor;
                                            GUI.contentColor = Color.red;
                                            GUILayout.Label(LangWindowOption.EditorInEnglish ? "Missing" : "Manquante");
                                            GUI.contentColor = oldGUIContentColor;
                                        }
                                        GUILayout.Space(10);
                                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Modify" : "Modifier"))
                                        {
                                            newFontPath = EditorUtility.OpenFilePanel(LangWindowOption.EditorInEnglish ? "Modify font" : "Modifier la police", "", "ttf,otf,dfont");
                                            if (newFontPath.Length != 0)
                                            {
                                                string tmp = System.IO.Path.GetExtension(newFontPath);
                                                if (tmp == ".ttf")
                                                {
                                                    System.IO.File.Copy(newFontPath, "Assets/Resources/Lang/Fonts/" + langToDetail.ToString() + "/" + font.ToString() + ".ttf");
                                                }
                                                else if (tmp == ".otf")
                                                {
                                                    System.IO.File.Copy(newFontPath, "Assets/Resources/Lang/Fonts/" + langToDetail.ToString() + "/" + font.ToString() + ".otf");
                                                }
                                                else if (tmp == ".dfont")
                                                {
                                                    System.IO.File.Copy(newFontPath, "Assets/Resources/Lang/Fonts/" + langToDetail.ToString() + "/" + font.ToString() + ".dfont");
                                                }
                                                else
                                                {
                                                    EditorUtility.DisplayDialog(LangWindowOption.EditorInEnglish ? "Extension Error" : "Erreur d'extension", LangWindowOption.EditorInEnglish ? "Only .ttf, .otf and .dfont fonts are unity compatible." : "Seules les polices de type .ttf, .otf et .dfont sont compatibles avec unity.", "OK");
                                                }
                                            }
                                        }
                                        GUILayout.Space(5);
                                    }
                                    GUILayout.EndHorizontal();
                                    GUILayout.Label("", GUI.skin.horizontalSlider);
                                }

                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.FlexibleSpace();
                                    if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Back" : "Retour"))
                                    {
                                        showingDetails = false;
                                    }
                                }
                                GUILayout.EndHorizontal();
                                GUILayout.Space(8);
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndScrollView();
                        GUILayout.Space(8);
                        GUILayout.FlexibleSpace();
                    }
                }
                GUILayout.EndVertical();
            }

            /// <summary>
            /// Affiche la partie vérification
            /// </summary>
            private static void FontManageGUI()
            {
                if (addingNewFont)
                {
                    LanguageAddNewFont();
                }
                else
                {
                    LanguageListFont();
                }
            }

            /// <summary>
            /// affiche l'interface d'ajour de police
            /// </summary>
            public static void LanguageAddNewFont()
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label(LangWindowOption.EditorInEnglish ? "Adding new font:" : "Ajout de police :");

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
                    foreach (FontsEnum val in Enum.GetValues(typeof(FontsEnum)))
                    {
                        if (val.ToString().Equals(newFontName))
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
                    newFontName = EditorGUILayout.TextArea(newFontName);
                    if (nameError)
                    {
                        GUI.backgroundColor = oldGuiBackgroundColor;
                    }
                    GUILayout.Space(5);

                    //Copier les polices?
                    newFontCopyFonts = GUILayout.Toggle(newFontCopyFonts, newFontCopyFonts ? LangWindowOption.EditorInEnglish ? " Copy from:" : " Copier :" : LangWindowOption.EditorInEnglish ? " Copy another font" : " Copier une autre police");
                    if (newFontCopyFonts)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("", GUI.skin.horizontalSliderThumb);
                            GUILayout.Space(5);
                            newFontCopiedFonts = (FontsEnum)EditorGUILayout.EnumPopup(newFontCopiedFonts);
                            GUILayout.FlexibleSpace();
                            GUILayout.Space(5);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Space(5);

                    //Boutons ajouter et annuler
                    GUILayout.BeginHorizontal();
                    {
                        if (nameError || newFontName == "")
                        {
                            GUI.enabled = false;
                        }
                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Add" : "Ajouter"))
                        {
                            if (newFontCopyFonts)
                            {
                                CopyFont(newFontName, newFontCopiedFonts);
                            }
                            AddFont(newFontName);
                            addingNewFont = false;
                        }
                        if (nameError || newFontName == "")
                        {
                            GUI.enabled = true;
                        }
                        GUILayout.Space(10);
                        oldGuiBackgroundColor = GUI.backgroundColor;
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Cancel" : "Annuler"))
                        {
                            addingNewFont = false;
                        }
                        GUI.backgroundColor = oldGuiBackgroundColor;
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }

            /// <summary>
            /// Affiche la liste des polices (avec possibilité de suppression)
            /// </summary>
            public static void LanguageListFont()
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label(LangWindowOption.EditorInEnglish ? "Fonts list:" : "Liste de toutes les polices :");
                    GUILayout.Space(8);

                    //Scrollview de toutes les langues
                    fontScrollPosition = GUILayout.BeginScrollView(fontScrollPosition);
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label("", GUI.skin.horizontalSlider);
                            foreach (FontsEnum font in Enum.GetValues(typeof(FontsEnum)))
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    //UI de suppression de langue
                                    if (deletingFont && deletingFontValue == font)
                                    {
                                        GUILayout.Label(LangWindowOption.EditorInEnglish ? "Are you sure you want to delete " + font.ToString() + "?" : "Êtes-vous sûr de vouloir supprimer " + font.ToString() + " ?");
                                        GUILayout.Space(10);
                                        Color oldUIColor = GUI.backgroundColor;
                                        GUI.backgroundColor = Color.green;
                                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Yes" : "Oui"))
                                        {
                                            RemoveFont(font.ToString());
                                            deletingFont = false;
                                        }
                                        GUI.backgroundColor = oldUIColor;
                                        GUILayout.Space(10);
                                        GUI.backgroundColor = Color.red;
                                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "No" : "Non"))
                                        {
                                            deletingFont = false;
                                        }
                                        GUI.backgroundColor = oldUIColor;
                                        GUILayout.FlexibleSpace();
                                    }
                                    //UI normale
                                    else
                                    {
                                        GUILayout.Label(font.ToString());
                                        GUILayout.FlexibleSpace();
                                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Delete" : "Supprimer"))
                                        {
                                            deletingFont = true;
                                            deletingFontValue = font;
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
                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Add new font" : "Ajouter une nouvelle police"))
                        {
                            newFontName = "";
                            newFontCopyFonts = false;
                            addingNewFont = true;
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(8);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndVertical();
            }

            /// <summary>
            /// Copie les polices d'une autre police
            /// </summary>
            /// <param name="name">nom de la police à créer</param>
            /// <param name="copiedFont">police à copier</param>
            private static void CopyFont(string name, FontsEnum copiedFont)
            {
                foreach (LanguagesEnum lang in Enum.GetValues(typeof(LanguagesEnum)))
                {
                    if (System.IO.Directory.Exists("Assets/Resources/Lang/Fonts/" + lang.ToString()))
                    {
                        if (System.IO.File.Exists("Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + copiedFont.ToString() + ".ttf"))
                        {
                            System.IO.File.Copy("Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + copiedFont.ToString() + ".ttf", "Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + name + ".ttf");
                        }
                        if (System.IO.File.Exists("Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + copiedFont.ToString() + ".dfont"))
                        {
                            System.IO.File.Copy("Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + copiedFont.ToString() + ".dfont", "Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + name + ".dfont");
                        }
                        if (System.IO.File.Exists("Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + copiedFont.ToString() + ".otf"))
                        {
                            System.IO.File.Copy("Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + copiedFont.ToString() + ".otf", "Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + name + ".otf");
                        }
                    }
                }
                AssetDatabase.Refresh();
            }

            /// <summary>
            /// Ajoute une police
            /// </summary>
            /// <param name="name">Nom de la police à ajouter</param>
            private static void AddFont(string name)
            {
                List<string> names = new List<string>();
                foreach (FontsEnum val in Enum.GetValues(typeof(FontsEnum)))
                {
                    names.Add(val.ToString());
                }
                if (!names.Contains(name))
                {
                    names.Add(name);
                    SaveFontList(names);
                }
            }

            /// <summary>
            /// Supprime une police
            /// </summary>
            /// <param name="name">Nom de la police à supprimer</param>
            private static void RemoveFont(string name)
            {
                List<string> names = new List<string>();
                foreach (FontsEnum val in Enum.GetValues(typeof(FontsEnum)))
                {
                    names.Add(val.ToString());
                }
                if (names.Contains(name))
                {
                    names.Remove(name);
                    SaveFontList(names);
                }

                foreach (LanguagesEnum lang in Enum.GetValues(typeof(LanguagesEnum)))
                {
                    if (System.IO.Directory.Exists("Assets/Resources/Lang/Fonts/" + lang.ToString()))
                    {
                        if (System.IO.File.Exists("Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + name + ".ttf"))
                        {
                            System.IO.File.Delete("Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + name + ".ttf");
                        }
                        if (System.IO.File.Exists("Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + name + ".dfont"))
                        {
                            System.IO.File.Delete("Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + name + ".dfont");
                        }
                        if (System.IO.File.Exists("Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + name + ".otf"))
                        {
                            System.IO.File.Delete("Assets/Resources/Lang/Fonts/" + lang.ToString() + "/" + name + ".otf");
                        }
                    }
                }
                AssetDatabase.Refresh();
            }

            /// <summary>
            /// Sauvegarde la liste des <paramref name="names"/> dans l'énum <see cref="FontsEnum"/>
            /// </summary>
            /// <param name="names">liste des elements à mettre dans l'enum <see cref="FontsEnum"/></param>
            private static void SaveFontList(List<string> names)
            {
                names.Sort();
                string code = FONT_ENUM_HEADER;
                foreach (string name in names)
                {
                    code += "\t\t\t" + name + ",\r\n";
                }
                code += FONT_ENUM_FOOTER;
                System.IO.File.WriteAllText("Assets/Packages/Lang/Scripts/Structs/FontsEnum.cs", code);
                AssetDatabase.Refresh();
            }

            #endregion Others

            #endregion Methods
        }
    }
}