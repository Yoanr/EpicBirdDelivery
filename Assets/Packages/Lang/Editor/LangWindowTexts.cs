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
        /// Classe qui gère les textes depuis l'éditeur
        /// </summary>
        public class LangWindowTexts : Editor
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
                MODIFY,
                MANAGE,
                IMPORT_EXPORT,
            }

            /// <summary>
            /// Les différents sous-onglés d'import/export de la fenêtre dans l'éditeur
            /// </summary>
            private enum ImportExportTab
            {
                IMPORT,
                EXPORT,
            }

            /// <summary>
            /// code à mettre au début du fichier DictionariesEnum.cs
            /// </summary>
            private static string DICTIONARIES_ENUM_HEADER = "/// <summary>\r\n/// Namespace de From The Bard\r\n/// </summary>\r\nnamespace FromTheBard\r\n{\r\n\t/// <summary>\r\n\t/// Namespace du gestionnaire de langue\r\n\t/// </summary>\r\n\tnamespace Lang\r\n\t{\r\n\t\t/// <summary>\r\n\t\t/// /!\\ enum auto-généré. Dictionnaires existants\r\n\t\t/// </summary>\r\n\t\tpublic enum DictionariesEnum\r\n\t\t{\r\n";

            /// <summary>
            /// code à mettre à la fin du fichier DictionariesEnum.cs
            /// </summary>
            private static string DICTIONARIES_ENUM_FOOTER = "\t\t}\r\n\t}\r\n}";

            #endregion Constants

            #region Fields

            //****************************************************************************************************************
            //**                                                   Private                                                  **
            //****************************************************************************************************************

            #region Modify

            /// <summary>
            /// langue du dictionnaire actuellement affiché dans la partie MODIFY
            /// </summary>
            private static LanguagesEnum modifiedTextsLanguage;

            /// <summary>
            /// dictionnaire actuellement affiché dans la partie MODIFY
            /// </summary>
            private static DictionariesEnum modifiedTextsDictionary;

            /// <summary>
            /// Dictionnaire actuellement modifié
            /// </summary>
            private static Dictionary<string, string> modifiedDictionary;

            /// <summary>
            /// Texte à rechercher
            /// </summary>
            private static string searchString = "";

            /// <summary>
            /// Doit-on chercher dans les clés
            /// </summary>
            private static bool searchInKeys = true;

            /// <summary>
            /// Doit-on chercher dans les valeurs
            /// </summary>
            private static bool searchInValues = false;

            /// <summary>
            /// position du cadre de scroll pour la liste des textes
            /// </summary>
            private static Vector2 TextsScrollPosition = Vector2.zero;

            /// <summary>
            /// Est-on en train de supprimer un texte
            /// </summary>
            private static bool deletingText = false;

            /// <summary>
            /// Clé qu'on est en train de supprimer
            /// </summary>
            private static string deletedTextKey;

            /// <summary>
            /// Est-on en train de supprimer un texte
            /// </summary>
            private static bool modifyingText = false;

            /// <summary>
            /// Clé qu'on est en train de supprimer
            /// </summary>
            private static string modifiedTextKey;

            /// <summary>
            /// Valeur qu'on est en train de supprimer
            /// </summary>
            private static string modifiedTextValue;

            /// <summary>
            /// Est-on en train d'ajouter un texte
            /// </summary>
            private static bool addingText;

            /// <summary>
            /// Clé du texte à ajouter
            /// </summary>
            private static string addedTextKey = "";

            /// <summary>
            /// Valeur du texte à ajouter
            /// </summary>
            private static string addedTextValue = "";

            #endregion Modify

            #region Manage

            /// <summary>
            /// Est-on en train d'ajouter un nouveau dictionnaire
            /// </summary>
            private static bool addingNewDictinary = false;

            /// <summary>
            /// Est-on en train de supprimer un dictionnaire
            /// </summary>
            private static bool deletingDictinary = false;

            /// <summary>
            /// quel dictionnaire on est en train de supprimer
            /// </summary>
            private static DictionariesEnum deletingDictionaryValue;

            /// <summary>
            /// position de la fenêtre de scroll de l'onglet manage
            /// </summary>
            private static Vector2 manageScrollPosition = Vector2.zero;

            /// <summary>
            /// nom du dictionnaire en train d'être créé
            /// </summary>
            private static string newDictionaryName = "";

            /// <summary>
            /// Doit-on copier un autre dictionnaire
            /// </summary>
            private static bool newDictionaryCopyDictionary = false;

            /// <summary>
            /// Dictionnaire que l'on doit copier
            /// </summary>
            private static DictionariesEnum newDictionaryCopiedDictionary;

            #endregion Manage

            #region Import/Export

            /// <summary>
            /// Onglé actuellement sélectionné dans la partie import/export
            /// </summary>
            private static ImportExportTab importExportActualTab = ImportExportTab.IMPORT;

            /// <summary>
            /// Dictionnaire à exporter
            /// </summary>
            private static DictionariesEnum exportDictionnary;

            /// <summary>
            /// Liste des langagues
            /// </summary>
            private static List<LanguagesEnum> exportLanguages;

            /// <summary>
            /// reorderable liste pour afficher <see cref="exportLanguages"/>
            /// </summary>
            private static UnityEditorInternal.ReorderableList exportLanguageRL = new UnityEditorInternal.ReorderableList(exportLanguages = new List<LanguagesEnum>(), typeof(LanguagesEnum), false, false, true, true);

            /// <summary>
            /// Dictionnaire à importer
            /// </summary>
            private static DictionariesEnum importDictionnary;

            /// <summary>
            /// A l'import doit-on override les textes déjà existants
            /// </summary>
            private static bool importOverride;

            /// <summary>
            /// A l'import doit-on ajouter les clés qui n'existent pas
            /// </summary>
            private static bool importAdd;

            #endregion Import/Export

            /// <summary>
            /// Onglé actuellement sélectionné
            /// </summary>
            private static Tab actualTab = Tab.MODIFY;

            #endregion Fields

            #region Methods

            #region Others

            //****************************************************************************************************************
            //**                                                   Public                                                   **
            //****************************************************************************************************************

            /// <summary>
            /// choses à afficher sur l'éditeur
            /// </summary>
            public static void TextsGUI()
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(8);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5);
                        if (GUILayout.Toggle(actualTab == Tab.MODIFY, LangWindowOption.EditorInEnglish ? "Modify" : "Modifier", EditorStyles.toolbarButton))
                        {
                            actualTab = Tab.MODIFY;
                        }
                        if (GUILayout.Toggle(actualTab == Tab.MANAGE, LangWindowOption.EditorInEnglish ? "Manage" : "Gérer", EditorStyles.toolbarButton))
                        {
                            actualTab = Tab.MANAGE;
                        }
                        if (GUILayout.Toggle(actualTab == Tab.IMPORT_EXPORT, LangWindowOption.EditorInEnglish ? "Import/Export" : "Importer/Exporter", EditorStyles.toolbarButton))
                        {
                            actualTab = Tab.IMPORT_EXPORT;
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
                            case Tab.MODIFY:
                                TextsModifyGUI();
                                break;

                            case Tab.MANAGE:
                                TextsManageGUI();
                                break;

                            case Tab.IMPORT_EXPORT:
                                TextsImportExportGUI();
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

            #region GUI

            /// <summary>
            /// Affiche la partie modification
            /// </summary>
            private static void TextsModifyGUI()
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(8);
                    if (LangWindowOption.EditorInEnglish)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("Texts of ");
                            modifiedTextsDictionary = (DictionariesEnum)EditorGUILayout.EnumPopup(modifiedTextsDictionary);
                            GUILayout.Label(" dictionary in ");
                            modifiedTextsLanguage = (LanguagesEnum)EditorGUILayout.EnumPopup(modifiedTextsLanguage);
                            GUILayout.Label(":");
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("Textes du dictionnaire ");
                            modifiedTextsDictionary = (DictionariesEnum)EditorGUILayout.EnumPopup(modifiedTextsDictionary);
                            GUILayout.Label("en ");
                            modifiedTextsLanguage = (LanguagesEnum)EditorGUILayout.EnumPopup(modifiedTextsLanguage);
                            GUILayout.Label(" :");
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Space(8);

                    LoadInModifiedDictionary(modifiedTextsLanguage, modifiedTextsDictionary);

                    if (modifiedDictionary != null)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.BeginVertical();
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Add new Text" : "Ajouter un nouveau texte"))
                                    {
                                        addingText = true;
                                        addedTextKey = "";
                                        addedTextValue = "";
                                    }
                                    GUILayout.FlexibleSpace();
                                }
                                GUILayout.EndHorizontal();
                                if (addingText)
                                {
                                    bool correctKey = addedTextKey != "" && !modifiedDictionary.ContainsKey(addedTextKey);
                                    GUILayout.Space(5);
                                    GUILayout.BeginHorizontal();
                                    {
                                        Color oldColor = GUI.contentColor;
                                        if (!correctKey)
                                        {
                                            GUI.contentColor = Color.red;
                                        }
                                        addedTextKey = EditorGUILayout.TextField(addedTextKey, GUILayout.MinWidth(100));
                                        GUILayout.Space(2);
                                        GUI.contentColor = new Color(0.6f, 0.6f, 0.6f, 1);
                                        GUILayout.Label("→");
                                        GUILayout.Space(2);
                                        GUI.contentColor = oldColor;
                                        addedTextValue = EditorGUILayout.TextArea(addedTextValue, GUILayout.MinWidth(200));
                                        GUILayout.FlexibleSpace();
                                    }
                                    GUILayout.EndHorizontal();
                                    GUILayout.Space(5);
                                    GUILayout.BeginHorizontal();
                                    {
                                        if (!correctKey)
                                        {
                                            GUI.enabled = false;
                                        }
                                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Add" : "Ajouter"))
                                        {
                                            addingText = false;
                                            AddNewText(addedTextKey, addedTextValue, modifiedTextsDictionary, modifiedTextsLanguage);
                                        }
                                        if (!correctKey)
                                        {
                                            GUI.enabled = true;
                                        }
                                        GUILayout.Space(5);

                                        Color oldColor = GUI.backgroundColor;
                                        GUI.backgroundColor = Color.red;
                                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Cancel" : "Annuler"))
                                        {
                                            addingText = false;
                                        }
                                        GUILayout.FlexibleSpace();
                                        GUI.backgroundColor = oldColor;
                                    }
                                    GUILayout.EndHorizontal();
                                    GUILayout.Space(5);
                                }
                            }
                            GUILayout.EndVertical();
                            GUILayout.FlexibleSpace();
                            GUILayout.BeginVertical();
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label(LangWindowOption.EditorInEnglish ? "Search:" : "Chercher :");
                                    searchString = EditorGUILayout.TextField(searchString, GUILayout.MinWidth(100));
                                }
                                GUILayout.EndHorizontal();
                                if (searchString != "")
                                {
                                    searchInKeys = GUILayout.Toggle(searchInKeys, LangWindowOption.EditorInEnglish ? "Search in Keys" : "Chercher dans les clés");
                                    searchInValues = GUILayout.Toggle(searchInValues, LangWindowOption.EditorInEnglish ? "Search in Texts" : "Chercher dans les Textes");
                                }
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(8);
                        GUILayout.Label("", GUI.skin.horizontalSlider);
                        GUILayout.Space(8);

                        //Scrollview de tous les testes
                        TextsScrollPosition = GUILayout.BeginScrollView(TextsScrollPosition);
                        {
                            GUILayout.BeginVertical();
                            {
                                foreach (KeyValuePair<string, string> pair in modifiedDictionary)
                                {
                                    if (searchString == "" || (searchInKeys && pair.Key.Contains(searchString)) || (searchInValues && pair.Value.Contains(searchString)) || (deletingText && deletedTextKey == pair.Key) || (modifyingText && modifiedTextKey == pair.Key))
                                    {
                                        GUILayout.BeginHorizontal();
                                        {
                                            Color oldColor = GUI.contentColor;
                                            GUI.contentColor = new Color(1, 221f / 255f, 35f / 255f, 1);
                                            GUILayout.Label(pair.Key);
                                            GUILayout.Space(2);
                                            GUI.contentColor = new Color(0.6f, 0.6f, 0.6f, 1);
                                            GUILayout.Label("→");
                                            GUILayout.Space(2);
                                            GUI.contentColor = oldColor;
                                            if (modifyingText && modifiedTextKey == pair.Key)
                                            {
                                                modifiedTextValue = EditorGUILayout.TextArea(modifiedTextValue);
                                            }
                                            else
                                            {
                                                GUILayout.Box(pair.Value);
                                            }
                                            GUILayout.FlexibleSpace();
                                            GUILayout.Space(5);
                                            GUILayout.BeginVertical();
                                            {
                                                if (deletingText && deletedTextKey == pair.Key)
                                                {
                                                    GUILayout.Label(LangWindowOption.EditorInEnglish ? "Delete" : "Supprimer");
                                                    GUILayout.Space(5);
                                                    oldColor = GUI.backgroundColor;
                                                    GUI.backgroundColor = Color.green;
                                                    if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Yes" : "Oui"))
                                                    {
                                                        deletingText = false;
                                                        DeleteTextKey(deletedTextKey, modifiedTextsDictionary);
                                                    }
                                                    GUILayout.Space(5);
                                                    GUI.backgroundColor = Color.red;
                                                    if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "No" : "Non"))
                                                    {
                                                        deletingText = false;
                                                    }
                                                    GUI.backgroundColor = oldColor;
                                                }
                                                else if (modifyingText && modifiedTextKey == pair.Key)
                                                {
                                                    oldColor = GUI.backgroundColor;
                                                    GUI.backgroundColor = Color.green;
                                                    if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Modify" : "Modifier"))
                                                    {
                                                        modifyingText = false;
                                                        ModifyText(modifiedTextKey, modifiedTextValue, modifiedTextsDictionary, modifiedTextsLanguage);
                                                    }
                                                    GUILayout.Space(5);
                                                    GUI.backgroundColor = Color.red;
                                                    if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Cancel" : "Annuler"))
                                                    {
                                                        modifyingText = false;
                                                    }
                                                    GUI.backgroundColor = oldColor;
                                                }
                                                else
                                                {
                                                    if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Modify" : "Modifier"))
                                                    {
                                                        modifyingText = true;
                                                        modifiedTextKey = pair.Key;
                                                        modifiedTextValue = pair.Value;
                                                    }
                                                    GUILayout.Space(5);
                                                    oldColor = GUI.backgroundColor;
                                                    GUI.backgroundColor = Color.red;
                                                    if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Delete" : "Supprimer"))
                                                    {
                                                        deletingText = true;
                                                        deletedTextKey = pair.Key;
                                                    }
                                                    GUI.backgroundColor = oldColor;
                                                }
                                            }
                                            GUILayout.EndVertical();
                                            GUILayout.Space(5);
                                        }
                                        GUILayout.EndHorizontal();
                                        GUILayout.Label("", GUI.skin.horizontalSlider);
                                    }
                                }
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndScrollView();
                        GUILayout.Space(8);
                    }
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndVertical();
            }

            #region Manage

            /// <summary>
            /// Affiche la partie gestion
            /// </summary>
            private static void TextsManageGUI()
            {
                if (!addingNewDictinary)
                {
                    TextsManageRecapGUI();
                }
                else
                {
                    TextsManageAddGUI();
                }
            }

            /// <summary>
            /// Affiche la partie récap de la partie gestion
            /// </summary>
            private static void TextsManageRecapGUI()
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label(LangWindowOption.EditorInEnglish ? "Dictionaries list:" : "Liste de tous les dictionnaires :");
                    GUILayout.Space(8);

                    //Scrollview de toutes les langues
                    manageScrollPosition = GUILayout.BeginScrollView(manageScrollPosition);
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label("", GUI.skin.horizontalSlider);
                            foreach (DictionariesEnum dico in Enum.GetValues(typeof(DictionariesEnum)))
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    //UI de suppression de langue
                                    if (deletingDictinary && deletingDictionaryValue == dico)
                                    {
                                        GUILayout.Label(LangWindowOption.EditorInEnglish ? "Are you sure you want to delete " + dico.ToString() + "?" : "Êtes-vous sûr de vouloir supprimer " + dico.ToString() + " ?");
                                        GUILayout.Space(10);
                                        Color oldUIColor = GUI.backgroundColor;
                                        GUI.backgroundColor = Color.green;
                                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Yes" : "Oui"))
                                        {
                                            RemoveDictionary(dico.ToString());
                                            deletingDictinary = false;
                                        }
                                        GUI.backgroundColor = oldUIColor;
                                        GUILayout.Space(10);
                                        GUI.backgroundColor = Color.red;
                                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "No" : "Non"))
                                        {
                                            deletingDictinary = false;
                                        }
                                        GUI.backgroundColor = oldUIColor;
                                        GUILayout.FlexibleSpace();
                                    }
                                    //UI normale
                                    else
                                    {
                                        GUILayout.Label(dico.ToString());
                                        GUILayout.FlexibleSpace();
                                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Delete" : "Supprimer"))
                                        {
                                            deletingDictinary = true;
                                            deletingDictionaryValue = dico;
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
                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Add new dictionary" : "Ajouter un nouveau dictionnaire"))
                        {
                            newDictionaryName = "";
                            newDictionaryCopyDictionary = false;
                            addingNewDictinary = true;
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(8);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndVertical();
            }

            /// <summary>
            /// Affiche la partie ajout de la partie gestion
            /// </summary>
            private static void TextsManageAddGUI()
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label(LangWindowOption.EditorInEnglish ? "Adding new dictionary:" : "Ajout de dictionnaire :");

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
                    foreach (DictionariesEnum val in Enum.GetValues(typeof(DictionariesEnum)))
                    {
                        if (val.ToString().Equals(newDictionaryName))
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
                    newDictionaryName = EditorGUILayout.TextArea(newDictionaryName);
                    if (nameError)
                    {
                        GUI.backgroundColor = oldGuiBackgroundColor;
                    }
                    GUILayout.Space(5);

                    //Copier les polices?
                    newDictionaryCopyDictionary = GUILayout.Toggle(newDictionaryCopyDictionary, newDictionaryCopyDictionary ? LangWindowOption.EditorInEnglish ? " Copy from:" : " Copier :" : LangWindowOption.EditorInEnglish ? " Copy another dictionary" : " Copier un autre dictionnaire");
                    if (newDictionaryCopyDictionary)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("", GUI.skin.horizontalSliderThumb);
                            GUILayout.Space(5);
                            newDictionaryCopiedDictionary = (DictionariesEnum)EditorGUILayout.EnumPopup(newDictionaryCopiedDictionary);
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
                            if (newDictionaryCopyDictionary)
                            {
                                CopyDictionary(newDictionaryCopiedDictionary, newDictionaryName);
                            }
                            else
                            {
                                AddDictionary(newDictionaryName);
                            }
                            addingNewDictinary = false;
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
                            addingNewDictinary = false;
                        }
                        GUI.backgroundColor = oldGuiBackgroundColor;
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }

            #endregion Manage

            #region Import/Export

            /// <summary>
            /// Affiche la partie Import/export
            /// </summary>
            private static void TextsImportExportGUI()
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(8);
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(5);
                            if (GUILayout.Toggle(importExportActualTab == ImportExportTab.IMPORT, LangWindowOption.EditorInEnglish ? "Import" : "Importer", EditorStyles.toolbarButton))
                            {
                                importExportActualTab = ImportExportTab.IMPORT;
                            }
                            if (GUILayout.Toggle(importExportActualTab == ImportExportTab.EXPORT, LangWindowOption.EditorInEnglish ? "Export" : "Exporter", EditorStyles.toolbarButton))
                            {
                                importExportActualTab = ImportExportTab.EXPORT;
                            }
                            GUILayout.Space(5);
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(5);
                            switch (importExportActualTab)
                            {
                                case ImportExportTab.IMPORT:
                                    TextsImportGUI();
                                    break;

                                case ImportExportTab.EXPORT:
                                    TextsExportGUI();
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
                GUILayout.EndVertical();
            }

            /// <summary>
            /// Affiche la partie import de dictionnaire
            /// </summary>
            private static void TextsImportGUI()
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(LangWindowOption.EditorInEnglish ? "Dictionary: " : "Dictionnaire : ");
                        importDictionnary = (DictionariesEnum)EditorGUILayout.EnumPopup(importDictionnary);
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Import" : "Importer"))
                        {
                            string importDictionnaryPath = EditorUtility.OpenFilePanel(LangWindowOption.EditorInEnglish ? "Import Dictionary" : "Importer le dictionnaire", "", "txt");
                            if (importDictionnaryPath.Length != 0)
                            {
                                ImportDictionnary(importDictionnaryPath, importOverride, importAdd, importDictionnary);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        importOverride = GUILayout.Toggle(importOverride, LangWindowOption.EditorInEnglish ? "Override current textes." : "Remplacer les textes actuels");
                        if (importOverride)
                        {
                            GUILayout.Space(5);
                            Color oldColor = GUI.contentColor;
                            GUI.contentColor = Color.red;
                            GUILayout.Label(LangWindowOption.EditorInEnglish ? "/!\\ Caution : importing will override curent texts. /!\\" : "/!\\ Attention: l'import remplacera les textes actuels. /!\\");
                            GUI.contentColor = oldColor;
                            GUILayout.FlexibleSpace();
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    importAdd = GUILayout.Toggle(importAdd, LangWindowOption.EditorInEnglish ? "Add missing keys." : "Ajouter les textes manquants.");
                }
                GUILayout.EndVertical();
            }

            /// <summary>
            /// Affiche la partie export de dictionnaire
            /// </summary>
            private static void TextsExportGUI()
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(LangWindowOption.EditorInEnglish ? "Dictionary: " : "Dictionnaire : ");
                        exportDictionnary = (DictionariesEnum)EditorGUILayout.EnumPopup(exportDictionnary);
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(LangWindowOption.EditorInEnglish ? "Export" : "Exporter"))
                        {
                            if (exportLanguages.Count <= 0)
                            {
                                EditorUtility.DisplayDialog(LangWindowOption.EditorInEnglish ? "Languages error" : "Erreur de langues", LangWindowOption.EditorInEnglish ? "Select the languages in which you want to export." : "Sélectionnez les langues dans les quelles vous voulez exporter.", "OK");
                            }
                            else
                            {
                                string exportDictionnaryPath = EditorUtility.SaveFilePanel(LangWindowOption.EditorInEnglish ? "Export Dictionary" : "Exporter le dictionnaire", "", exportDictionnary.ToString(), "txt");
                                if (exportDictionnaryPath.Length != 0)
                                {
                                    ExportDictionnary(exportDictionnary, exportLanguages, exportDictionnaryPath);
                                }
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(LangWindowOption.EditorInEnglish ? "Languages: " : "Langues :");
                        GUILayout.Space(2);

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    exportLanguageRL.onAddDropdownCallback = (Rect buttonRect, UnityEditorInternal.ReorderableList l) =>
                    {
                        var menu = new GenericMenu();
                        foreach (LanguagesEnum lang in Enum.GetValues(typeof(LanguagesEnum)))
                        {
                            if (!exportLanguages.Contains(lang))
                            {
                                menu.AddItem(new GUIContent(lang.ToString()), false, ClickHandler, lang);
                            }
                        }
                        menu.ShowAsContext();
                    };
                    exportLanguageRL.DoLayoutList();
                }
                GUILayout.EndVertical();
            }

            #endregion Import/Export

            #endregion GUI

            /// <summary>
            /// Charge le dictionnaire <paramref name="dico"/> en <paramref name="lang"/> dans <see cref="modifiedDictionary"/>
            /// </summary>
            /// <param name="lang">langue à charger</param>
            /// <param name="dico">dictionnaire à charger</param>
            private static void LoadInModifiedDictionary(LanguagesEnum lang, DictionariesEnum dico)
            {
                TextAsset txtAsset = Resources.Load<TextAsset>("Lang/Texts/" + dico.ToString());

                if (txtAsset == null)
                {
                    modifiedDictionary = null;
                    return;
                }
                string txtString = txtAsset.text;

                string[] lines = txtString.Split(System.Environment.NewLine.ToCharArray());

                if (lines.Length <= 0)
                {
                    modifiedDictionary = null;
                    return;
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
                    modifiedDictionary = null;
                    return;
                }

                modifiedDictionary = new Dictionary<string, string>();
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
                    }
                    else if (!line[0].Equals(""))
                    {
                        if (line.Length <= languageIndex)
                        {
                            Debug.LogError("error in dictionnary " + dico + "in " + lang + " : key \"" + line[0] + "\"");
                            modifiedDictionary = new Dictionary<string, string>();
                            return;
                        }
                        tmpCharArray = line[languageIndex].ToCharArray();
                        if (tmpCharArray.Length >= 2 && tmpCharArray[0] == '"' && tmpCharArray[tmpCharArray.Length - 1] == '"')
                        {
                            line[languageIndex] = line[languageIndex].Substring(1, line[languageIndex].Length - 2);
                        }
                        line[languageIndex] = line[languageIndex].Replace("\"\"", "\"");
                        line[languageIndex] = line[languageIndex].Replace("\\n", "\n");
                        line[languageIndex] = line[languageIndex].Replace("{$pipe}", "|");
                        modifiedDictionary.Add(line[0], line[languageIndex]);
                    }
                }
            }

            /// <summary>
            /// Fonction qui ajoute à la liste <see cref="exportLanguages"/>
            /// </summary>
            /// <param name="target">objet à ajouter</param>
            private static void ClickHandler(object target)
            {
                if (!exportLanguages.Contains((LanguagesEnum)target))
                {
                    exportLanguages.Add((LanguagesEnum)target);
                }
                exportLanguages.Sort();
            }

            #region Modify

            /// <summary>
            /// Supprime la clé <paramref name="key"/> du dictionnaire <paramref name="dico"/>
            /// </summary>
            /// <param name="key">Clé à retirer du dictionnaire</param>
            /// <param name="dico">Dictionnaire dans le quel retirer la clé</param>
            private static void DeleteTextKey(string key, DictionariesEnum dico)
            {
                TextAsset txtAsset = Resources.Load<TextAsset>("Lang/Texts/" + dico.ToString());

                if (txtAsset == null)
                {
                    modifiedDictionary = null;
                    return;
                }
                string txtString = txtAsset.text;

                string[] lines = txtString.Split(System.Environment.NewLine.ToCharArray());

                if (lines.Length <= 0)
                {
                    modifiedDictionary = null;
                    return;
                }

                Dictionary<string, string> tmpDictionary = new Dictionary<string, string>();
                string[] line;
                for (int i = 1; i < lines.Length; i++)
                {
                    line = lines[i].Split('|');
                    if (line.Length <= 0 || lines[i] == "")
                    {
                    }
                    else if (line.Length == 1)
                    {
                    }
                    else if (!line[0].Equals(""))
                    {
                        if (line[0] != key)
                        {
                            string tmp = "";
                            for (int j = 1; j < line.Length; j++)
                            {
                                tmp += line[j] + "|";
                            }
                            tmp = tmp.Substring(0, tmp.Length - 1);
                            tmpDictionary.Add(line[0], tmp);
                        }
                    }
                }
                string text = lines[0] + System.Environment.NewLine;
                foreach (KeyValuePair<string, string> pair in tmpDictionary)
                {
                    text += pair.Key + "|" + pair.Value + System.Environment.NewLine;
                }

                System.IO.File.WriteAllText("Assets/Resources/Lang/Texts/" + dico.ToString() + ".txt", text);
                AssetDatabase.Refresh();
            }

            /// <summary>
            /// modifie la clé <paramref name="key"/> du dictionnaire <paramref name="dico"/> pour la langue <paramref name="lang"/>
            /// </summary>
            /// <param name="key">Clé de laquelle on doit modifier la valeur</param>
            /// <param name="value">Valeur à mettre pour la clé</param>
            /// <param name="dico">Dictionnaire dans le quel retirer la clé</param>
            /// <param name="lang">Langue dans laquelle modifier le texte de la clé</param>
            private static void ModifyText(string key, string value, DictionariesEnum dico, LanguagesEnum lang)
            {
                TextAsset txtAsset = Resources.Load<TextAsset>("Lang/Texts/" + dico.ToString());

                if (txtAsset == null)
                {
                    modifiedDictionary = null;
                    return;
                }
                string txtString = txtAsset.text;

                string[] lines = txtString.Split(System.Environment.NewLine.ToCharArray());

                if (lines.Length <= 0)
                {
                    modifiedDictionary = null;
                    return;
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
                    return;
                }

                Dictionary<string, string> tmpDictionary = new Dictionary<string, string>();
                string[] line;
                for (int i = 1; i < lines.Length; i++)
                {
                    line = lines[i].Split('|');
                    if (line.Length <= 0 || lines[i] == "")
                    {
                    }
                    else if (line.Length == 1)
                    {
                    }
                    else if (!line[0].Equals(""))
                    {
                        if (line[0] != key)
                        {
                            string tmp = "";
                            for (int j = 1; j < line.Length; j++)
                            {
                                tmp += line[j] + "|";
                            }
                            tmp = tmp.Substring(0, tmp.Length - 1);
                            tmpDictionary.Add(line[0], tmp);
                        }
                        else
                        {
                            line[languageIndex] = value;
                            line[languageIndex] = line[languageIndex].Replace("\"", "\"\"");
                            line[languageIndex] = line[languageIndex].Replace("\n", "\\n");
                            line[languageIndex] = line[languageIndex].Replace("|", "{$pipe}");
                            line[languageIndex] = "\"" + line[languageIndex] + "\"";
                            string tmp = "";
                            for (int j = 1; j < line.Length; j++)
                            {
                                tmp += line[j] + "|";
                            }
                            tmp = tmp.Substring(0, tmp.Length - 1);
                            tmpDictionary.Add(line[0], tmp);
                        }
                    }
                }
                string text = lines[0] + System.Environment.NewLine;
                foreach (KeyValuePair<string, string> pair in tmpDictionary)
                {
                    text += pair.Key + "|" + pair.Value + System.Environment.NewLine;
                }

                System.IO.File.WriteAllText("Assets/Resources/Lang/Texts/" + dico.ToString() + ".txt", text);
                AssetDatabase.Refresh();
            }

            /// <summary>
            /// Ajoute une clé au dictionnaire
            /// </summary>
            /// <param name="key">Clé à ajouter</param>
            /// <param name="value">Valeur à ajouter</param>
            /// <param name="dico">Dictionnaire dans lequel ajouter</param>
            /// <param name="lang">Langue dans laquelle ajouter</param>
            private static void AddNewText(string key, string value, DictionariesEnum dico, LanguagesEnum lang)
            {
                TextAsset txtAsset = Resources.Load<TextAsset>("Lang/Texts/" + dico.ToString());

                if (txtAsset == null)
                {
                    modifiedDictionary = null;
                    return;
                }
                string txtString = txtAsset.text;

                string[] lines = txtString.Split(System.Environment.NewLine.ToCharArray());

                if (lines.Length <= 0)
                {
                    modifiedDictionary = null;
                    return;
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
                    return;
                }

                Dictionary<string, string> tmpDictionary = new Dictionary<string, string>();
                string[] line;
                for (int i = 1; i < lines.Length; i++)
                {
                    line = lines[i].Split('|');
                    if (line.Length <= 0 || lines[i] == "")
                    {
                    }
                    else if (line.Length == 1)
                    {
                    }
                    else if (!line[0].Equals(""))
                    {
                        string tmp = "";
                        for (int j = 1; j < line.Length; j++)
                        {
                            tmp += line[j] + "|";
                        }
                        tmp = tmp.Substring(0, tmp.Length - 1);
                        tmpDictionary.Add(line[0], tmp);
                    }
                }
                if (!tmpDictionary.ContainsKey(key))
                {
                    string[] tmpStrArr = new string[languages.Length];
                    tmpStrArr[0] = key.Replace(" ", "_").Replace("|", "_");
                    for (int j = 1; j < tmpStrArr.Length; j++)
                    {
                        tmpStrArr[j] = dico.ToString() + "_" + key;
                    }
                    tmpStrArr[languageIndex] = value;
                    tmpStrArr[languageIndex] = tmpStrArr[languageIndex].Replace("\"", "\"\"");
                    tmpStrArr[languageIndex] = tmpStrArr[languageIndex].Replace("\n", "\\n");
                    tmpStrArr[languageIndex] = tmpStrArr[languageIndex].Replace("|", "{$pipe}");
                    tmpStrArr[languageIndex] = "\"" + tmpStrArr[languageIndex] + "\"";
                    string tmp = "";
                    for (int j = 1; j < tmpStrArr.Length; j++)
                    {
                        tmp += tmpStrArr[j] + "|";
                    }
                    tmp = tmp.Substring(0, tmp.Length - 1);
                    tmpDictionary.Add(tmpStrArr[0], tmp);
                }
                string text = lines[0] + System.Environment.NewLine;
                foreach (KeyValuePair<string, string> pair in tmpDictionary)
                {
                    text += pair.Key + "|" + pair.Value + System.Environment.NewLine;
                }

                System.IO.File.WriteAllText("Assets/Resources/Lang/Texts/" + dico.ToString() + ".txt", text);
                AssetDatabase.Refresh();
            }

            #endregion Modify

            #region Manage

            /// <summary>
            /// supprime le dictionnaire
            /// </summary>
            /// <param name="name">nom du dictionnaire à supprimer</param>
            private static void RemoveDictionary(string name)
            {
                List<string> names = new List<string>();
                foreach (DictionariesEnum val in Enum.GetValues(typeof(DictionariesEnum)))
                {
                    names.Add(val.ToString());
                }
                if (names.Contains(name))
                {
                    names.Remove(name);
                    SaveDictionariesList(names);

                    if (System.IO.File.Exists("Assets/Resources/Lang/Texts/" + name + ".txt"))
                    {
                        System.IO.File.Delete("Assets/Resources/Lang/Texts/" + name + ".txt");
                    }
                    AssetDatabase.Refresh();
                }
            }

            /// <summary>
            /// ajoute le dictionnaire sans le copier le dictionnaire
            /// </summary>
            /// <param name="name">nom du dictionnaire à ajouter</param>
            private static void AddDictionary(string name)
            {
                List<string> names = new List<string>();
                foreach (DictionariesEnum val in Enum.GetValues(typeof(DictionariesEnum)))
                {
                    names.Add(val.ToString());
                }
                if (!names.Contains(name))
                {
                    names.Add(name);
                    SaveDictionariesList(names);

                    string tmp = "";

                    foreach (LanguagesEnum lang in Enum.GetValues(typeof(LanguagesEnum)))
                    {
                        tmp += "|" + lang.ToString();
                    }

                    System.IO.File.WriteAllText("Assets/Resources/Lang/Texts/" + name + ".txt", tmp);

                    AssetDatabase.Refresh();
                }
            }

            /// <summary>
            /// copie le dictionnaire
            /// </summary>
            /// <param name="source">dictionnaire à copier</param>
            /// <param name="name">nom du dictionnaire à créer</param>
            private static void CopyDictionary(DictionariesEnum source, string name)
            {
                List<string> names = new List<string>();
                foreach (DictionariesEnum val in Enum.GetValues(typeof(DictionariesEnum)))
                {
                    names.Add(val.ToString());
                }
                if (!names.Contains(name))
                {
                    names.Add(name);
                    SaveDictionariesList(names);

                    if (System.IO.File.Exists("Assets/Resources/Lang/Texts/" + name + ".txt"))
                    {
                        System.IO.File.Delete("Assets/Resources/Lang/Texts/" + name + ".txt");
                    }
                    if (System.IO.File.Exists("Assets/Resources/Lang/Texts/" + source.ToString() + ".txt"))
                    {
                        System.IO.File.Copy("Assets/Resources/Lang/Texts/" + source.ToString() + ".txt", "Assets/Resources/Lang/Texts/" + name + ".txt");
                    }
                    AssetDatabase.Refresh();
                }
            }

            /// <summary>
            /// Sauvegarde la liste des <paramref name="names"/> dans l'énum <see cref="DictionariesEnum"/>
            /// </summary>
            /// <param name="names">liste des elements à mettre dans l'enum <see cref="DictionariesEnum"/></param>
            private static void SaveDictionariesList(List<string> names)
            {
                names.Sort();
                string code = DICTIONARIES_ENUM_HEADER;
                foreach (string name in names)
                {
                    code += "\t\t\t" + name + ",\r\n";
                }
                code += DICTIONARIES_ENUM_FOOTER;
                System.IO.File.WriteAllText("Assets/Packages/Lang/Scripts/Structs/DictionariesEnum.cs", code);
                AssetDatabase.Refresh();
            }

            #endregion Manage

            #region Import/Export

            /// <summary>
            /// Exporte le dictionnaire
            /// </summary>
            /// <param name="dictionnary">dictionnaire à exporter</param>
            /// <param name="languages">langues à extraire du dictionnaire</param>
            /// <param name="path">chemin auquel enregistrer le dictionnaire</param>
            private static void ExportDictionnary(DictionariesEnum dictionnary, List<LanguagesEnum> languages, string path)
            {
                TextAsset txtAsset = Resources.Load<TextAsset>("Lang/Texts/" + dictionnary.ToString());

                if (txtAsset == null)
                {
                    EditorUtility.DisplayDialog(LangWindowOption.EditorInEnglish ? "Export error" : "Erreur d'export", LangWindowOption.EditorInEnglish ? "File Lang/Texts/" + dictionnary.ToString() + " not found." : "Le fichier Lang/Texts/" + dictionnary.ToString() + " n'a pas été trouvé.", "OK");
                    return;
                }
                string txtString = txtAsset.text;

                string[] lines = txtString.Split(System.Environment.NewLine.ToCharArray());

                if (lines.Length <= 0)
                {
                    EditorUtility.DisplayDialog(LangWindowOption.EditorInEnglish ? "Export error" : "Erreur d'export", LangWindowOption.EditorInEnglish ? "File Lang/Texts/" + dictionnary.ToString() + " is empty." : "Le fichier Lang/Texts/" + dictionnary.ToString() + " est vide.", "OK");
                    return;
                }

                string[] dictionnaryLanguages = lines[0].Split('|');
                Dictionary<LanguagesEnum, int> languageIndex = new Dictionary<LanguagesEnum, int>();
                for (int i = 0; i < languages.Count; i++)
                {
                    for (int j = 0; j < dictionnaryLanguages.Length; j++)
                    {
                        if (languages[i].ToString() == dictionnaryLanguages[j])
                        {
                            languageIndex.Add(languages[i], j);
                            break;
                        }
                    }
                }
                if (languageIndex.Count <= 0)
                {
                    EditorUtility.DisplayDialog(LangWindowOption.EditorInEnglish ? "Export error" : "Erreur d'export", LangWindowOption.EditorInEnglish ? "None of the specified languages where found in the dictionary " + dictionnary.ToString() + "." : "Aucune des langues spécifiées n'a été trouvée dans le dictionnaire " + dictionnary.ToString() + ".", "OK");
                    return;
                }

                Dictionary<string, string> tmpDictionary = new Dictionary<string, string>();
                string[] line;
                for (int i = 1; i < lines.Length; i++)
                {
                    line = lines[i].Split('|');
                    if (line.Length <= 0 || lines[i] == "")
                    {
                    }
                    else if (line.Length == 1)
                    {
                    }
                    else if (!line[0].Equals(""))
                    {
                        string tmp = "";
                        for (int j = 0; j < languages.Count; j++)
                        {
                            if (line.Length <= languageIndex[languages[j]])
                            {
                                Debug.LogError("error in dictionnary " + dictionnary + "in " + languages[j] + " : key \"" + line[0] + "\"");
                                modifiedDictionary = new Dictionary<string, string>();
                                return;
                            }
                            tmp += line[languageIndex[languages[j]]] + "|";
                        }
                        tmp = tmp.Substring(0, tmp.Length - 1);
                        tmpDictionary.Add(line[0], tmp);
                    }
                }
                string text = "";
                for (int i = 0; i < languages.Count; i++)
                {
                    text += "|" + languages[i].ToString();
                }
                text += System.Environment.NewLine;
                foreach (KeyValuePair<string, string> pair in tmpDictionary)
                {
                    text += pair.Key + "|" + pair.Value + System.Environment.NewLine;
                }

                System.IO.File.WriteAllText(path, text);
                AssetDatabase.Refresh();
            }

            /// <summary>
            /// Importe le fichier dans le dictionnaire
            /// </summary>
            /// <param name="path">Chemin du fichier à importer</param>
            /// <param name="overrideTexts">Doit-on écraser les textes déjà présents dans le dictionnaire</param>
            /// <param name="add">Doit-on ajouter les clés non déjà présentes</param>
            /// <param name="dictionnary">dictionnaire dans lequel on import</param>
            private static void ImportDictionnary(string path, bool overrideTexts, bool add, DictionariesEnum dictionnary)
            {
                if (!System.IO.File.Exists(path))
                {
                    EditorUtility.DisplayDialog(LangWindowOption.EditorInEnglish ? "Import error" : "Erreur d'import", LangWindowOption.EditorInEnglish ? "File " + path + " not found." : "Le fichier " + path + " n'a pas été trouvé.", "OK");
                    return;
                }
                string importedTxtString = System.IO.File.ReadAllText(path);

                string[] importedLines = importedTxtString.Split(System.Environment.NewLine.ToCharArray());

                if (importedLines.Length <= 0)
                {
                    EditorUtility.DisplayDialog(LangWindowOption.EditorInEnglish ? "Import error" : "Erreur d'import", LangWindowOption.EditorInEnglish ? "File " + path + " is empty." : "Le fichier " + path + " est vide.", "OK");
                    return;
                }

                TextAsset txtAsset = Resources.Load<TextAsset>("Lang/Texts/" + dictionnary.ToString());

                if (txtAsset == null)
                {
                    EditorUtility.DisplayDialog(LangWindowOption.EditorInEnglish ? "Import error" : "Erreur d'import", LangWindowOption.EditorInEnglish ? "File Lang/Texts/" + dictionnary.ToString() + " not found." : "Le fichier Lang/Texts/" + dictionnary.ToString() + " n'a pas été trouvé.", "OK");
                    return;
                }
                string txtString = txtAsset.text;

                string[] localLines = txtString.Split(System.Environment.NewLine.ToCharArray());

                if (localLines.Length <= 0)
                {
                    EditorUtility.DisplayDialog(LangWindowOption.EditorInEnglish ? "Import error" : "Erreur d'import", LangWindowOption.EditorInEnglish ? "File Lang/Texts/" + dictionnary.ToString() + " is empty." : "Le fichier Lang/Texts/" + dictionnary.ToString() + " est vide.", "OK");
                    return;
                }

                string[] localLanguages = localLines[0].Split('|');
                string[] importedlanguages = importedLines[0].Split('|');

                int[] languageIndexConverterImportedToLocal = new int[importedlanguages.Length];
                for (int i = 1; i < importedlanguages.Length; i++)
                {
                    languageIndexConverterImportedToLocal[i] = -1;
                    for (int j = 0; j < localLanguages.Length; j++)
                    {
                        if (importedlanguages[i] == localLanguages[j])
                        {
                            languageIndexConverterImportedToLocal[i] = j - 1;
                            break;
                        }
                    }
                }

                int[] languageIndexConverterLocalToImported = new int[localLanguages.Length];
                for (int i = 1; i < localLanguages.Length; i++)
                {
                    languageIndexConverterLocalToImported[i] = -1;
                    for (int j = 0; j < importedlanguages.Length; j++)
                    {
                        if (importedlanguages[j] == localLanguages[i])
                        {
                            languageIndexConverterLocalToImported[i] = j - 1;
                            break;
                        }
                    }
                }

                Dictionary<string, string> tmpLocalDictionary = new Dictionary<string, string>();
                string[] line;
                for (int i = 1; i < localLines.Length; i++)
                {
                    line = localLines[i].Split('|');
                    if (line.Length <= 0 || localLines[i] == "")
                    {
                    }
                    else if (line.Length == 1)
                    {
                    }
                    else if (!line[0].Equals(""))
                    {
                        string tmp = "";
                        for (int j = 1; j < line.Length; j++)
                        {
                            tmp += line[j] + "|";
                        }
                        tmp = tmp.Substring(0, tmp.Length - 1);
                        tmpLocalDictionary.Add(line[0], tmp);
                    }
                }

                Dictionary<string, string> tmpImportedDictionary = new Dictionary<string, string>();
                for (int i = 1; i < importedLines.Length; i++)
                {
                    line = importedLines[i].Split('|');
                    if (line.Length <= 0 || importedLines[i] == "")
                    {
                    }
                    else if (line.Length == 1)
                    {
                    }
                    else if (!line[0].Equals(""))
                    {
                        string tmp = "";
                        for (int j = 1; j < line.Length; j++)
                        {
                            tmp += line[j] + "|";
                        }
                        tmp = tmp.Substring(0, tmp.Length - 1);
                        tmpImportedDictionary.Add(line[0], tmp);
                    }
                }

                if (overrideTexts)
                {
                    Dictionary<string, string> tmpDictionary = tmpImportedDictionary;
                    foreach (KeyValuePair<string, string> pair in tmpDictionary)
                    {
                        if (tmpLocalDictionary.ContainsKey(pair.Key))
                        {
                            string tmp = "";

                            for (int i = 1; i < localLanguages.Length; i++)
                            {
                                if (languageIndexConverterLocalToImported[i] >= 0)
                                {
                                    string[] tmp2 = pair.Value.Split('|');
                                    if (tmp2.Length <= languageIndexConverterLocalToImported[i])
                                    {
                                        EditorUtility.DisplayDialog(LangWindowOption.EditorInEnglish ? "Import error" : "erreur d'import", LangWindowOption.EditorInEnglish ? "Error with key " + pair.Key + "." : "Erreur avec la clé " + pair.Key + ".", "ok");
                                        return;
                                    }
                                    tmp += tmp2[languageIndexConverterLocalToImported[i]] + '|';
                                }
                                else
                                {
                                    string[] tmp2 = tmpLocalDictionary[pair.Key].Split('|');
                                    tmp += tmp2[i - 1] + '|';
                                }
                            }
                            tmpLocalDictionary[pair.Key] = tmp;
                        }
                    }
                }
                if (add)
                {
                    Dictionary<string, string> tmpDictionary = tmpImportedDictionary;
                    foreach (KeyValuePair<string, string> pair in tmpDictionary)
                    {
                        if (!tmpLocalDictionary.ContainsKey(pair.Key))
                        {
                            string tmp = "";

                            for (int i = 1; i < localLanguages.Length; i++)
                            {
                                if (languageIndexConverterLocalToImported[i] >= 0)
                                {
                                    string[] tmp2 = pair.Value.Split('|');
                                    if (tmp2.Length <= languageIndexConverterLocalToImported[i])
                                    {
                                        EditorUtility.DisplayDialog(LangWindowOption.EditorInEnglish ? "Import error" : "erreur d'import", LangWindowOption.EditorInEnglish ? "Error with key " + pair.Key + "." : "Erreur avec la clé " + pair.Key + ".", "ok");
                                        return;
                                    }
                                    tmp += tmp2[languageIndexConverterLocalToImported[i]] + '|';
                                }
                                else
                                {
                                    tmp += dictionnary.ToString() + "_" + pair.Key + '|';
                                }
                            }

                            tmpLocalDictionary.Add(pair.Key, tmp);
                        }
                    }
                }

                string text = "";
                for (int i = 1; i < localLanguages.Length; i++)
                {
                    text += "|" + localLanguages[i].ToString();
                }
                text += System.Environment.NewLine;
                foreach (KeyValuePair<string, string> pair in tmpLocalDictionary)
                {
                    text += pair.Key + "|" + pair.Value + System.Environment.NewLine;
                }

                System.IO.File.WriteAllText("Assets/Resources/Lang/Texts/" + dictionnary.ToString() + ".txt", text);
                AssetDatabase.Refresh();
            }

            #endregion Import/Export

            #endregion Others

            #endregion Methods
        }
    }
}