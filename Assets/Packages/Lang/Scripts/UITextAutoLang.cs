using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        /// Classe Permettant de mettre un texte d'un dictionnaire sur un <see cref="Text"/> d'UI
        /// </summary>
        [RequireComponent(typeof(Text))]
        public class UITextAutoLang : TextAutoLang
        {
            #region Fields

            //****************************************************************************************************************
            //**                                                  Protected                                                 **
            //****************************************************************************************************************

            /// <summary>
            /// Mesh contenant le texte
            /// </summary>
            protected Text text;

            #endregion Fields

            #region Methods

            #region Unity

            //****************************************************************************************************************
            //**                                             For Unity Methods                                              **
            //****************************************************************************************************************

            /// <summary>
            /// récupère le <see cref="Text"/>, s'inscrit à l'event "changement de langue" et lance <seealso cref="SetText"/>
            /// </summary>
            protected override void Awake()
            {
                text = GetComponent<Text>();
                base.Awake();
            }

            #endregion Unity

            #region Virtual/Override

            //****************************************************************************************************************
            //**                                                  Protected                                                 **
            //****************************************************************************************************************

            /// <summary>
            /// Fonction qui met le bon texte et la bonne police dans le <see cref="Text"/>
            /// </summary>
            protected override void SetText()
            {
                text.text = Lang.Get(dictionary, key);
                text.font = Lang.GetFont(font);
            }

            #endregion Virtual/Override

            #endregion Methods
        }
    }
}