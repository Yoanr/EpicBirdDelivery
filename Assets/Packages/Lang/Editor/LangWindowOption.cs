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
        /// <summary>
        /// Classe qui gère les options de l'éditeur
        /// </summary>
        public class LangWindowOption : MonoBehaviour
        {
            #region Fields

            //****************************************************************************************************************
            //**                                                   Private                                                  **
            //****************************************************************************************************************

            /// <summary>
            /// L'éditeur doit-il être en anglais?
            /// </summary>
            private static bool editorInEnglish = false;

            #endregion Fields

            #region Properties

            //****************************************************************************************************************
            //**                                                   Public                                                   **
            //****************************************************************************************************************

            /// <summary>
            /// L'éditeur doit-il être en anglais?
            /// </summary>
            public static bool EditorInEnglish
            {
                get
                {
                    return editorInEnglish;
                }

                private set
                {
                    editorInEnglish = value;
                }
            }

            #endregion Properties

            #region Methods

            #region Others

            //****************************************************************************************************************
            //**                                                   Public                                                   **
            //****************************************************************************************************************

            /// <summary>
            /// choses à afficher sur l'éditeur
            /// </summary>
            public static void OptionsGUI()
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(5);
                    if (GUILayout.Button(EditorInEnglish ? " Passer l'éditeur en français" : " Turn editor in English"))
                    {
                        EditorInEnglish = !editorInEnglish;
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.Space(5);
                }
                GUILayout.EndHorizontal();
            }

            #endregion Others

            #endregion Methods
        }
    }
}