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
        /// Classe qui gère l'affichage de la fenêtre dans l'éditeur
        /// </summary>
        public class LangWindow : EditorWindow
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
                TEXTS,
                FONTS,
                LANGUAGES,
                OOPTIONS
            }

            #endregion Constants

            #region Fields

            //****************************************************************************************************************
            //**                                                   Private                                                  **
            //****************************************************************************************************************

            /// <summary>
            /// Onglé actuellement sélectionné
            /// </summary>
            private Tab actualTab = Tab.TEXTS;

            #endregion Fields

            #region Toolbar Butons

            /// <summary>
            /// Bouton Show editor dans la bare d'outils
            /// </summary>
            [MenuItem("From The Bard/Lang/Show editor")]
            private static void ShowEditor()
            {
                EditorWindow.GetWindow<LangWindow>("Lang");
            }

            #endregion Toolbar Butons

            #region Methods

            #region Unity

            //****************************************************************************************************************
            //**                                             For Unity Methods                                              **
            //****************************************************************************************************************

            /// <summary>
            /// Fonction d'affichage dans la fenêtre
            /// </summary>
            private void OnGUI()
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5);
                    if (GUILayout.Toggle(actualTab == Tab.TEXTS, LangWindowOption.EditorInEnglish ? "Texts" : "Textes", EditorStyles.toolbarButton))
                    {
                        actualTab = Tab.TEXTS;
                    }

                    if (GUILayout.Toggle(actualTab == Tab.FONTS, LangWindowOption.EditorInEnglish ? "Fonts" : "Polices", EditorStyles.toolbarButton))
                    {
                        actualTab = Tab.FONTS;
                    }

                    if (GUILayout.Toggle(actualTab == Tab.LANGUAGES, LangWindowOption.EditorInEnglish ? "Languages" : "Langues", EditorStyles.toolbarButton))
                    {
                        actualTab = Tab.LANGUAGES;
                    }

                    if (GUILayout.Toggle(actualTab == Tab.OOPTIONS, "Options", EditorStyles.toolbarButton))
                    {
                        actualTab = Tab.OOPTIONS;
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
                        case Tab.TEXTS:
                            LangWindowTexts.TextsGUI();
                            break;

                        case Tab.FONTS:
                            LangWindowFont.LanguagesGUI();
                            break;

                        case Tab.LANGUAGES:
                            LangWindowLanguages.LanguagesGUI();
                            break;

                        case Tab.OOPTIONS:
                            LangWindowOption.OptionsGUI();
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

            #endregion Unity

            #endregion Methods
        }
    }
}