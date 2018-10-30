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
        /// <summary>
        /// Classe Permettant de mettre un texte d'un dictionnaire sur un <see cref="TextMesh"/>
        /// </summary>
        [RequireComponent(typeof(TextMesh))]
        public class TextMeshAutoLang : TextAutoLang
        {
            #region Fields

            //****************************************************************************************************************
            //**                                                  Protected                                                 **
            //****************************************************************************************************************

            /// <summary>
            /// Mesh contenant le texte
            /// </summary>
            protected TextMesh text;

            #endregion Fields

            #region Methods

            #region Unity

            //****************************************************************************************************************
            //**                                             For Unity Methods                                              **
            //****************************************************************************************************************

            /// <summary>
            /// récupère le <see cref="TextMesh"/>, s'inscrit à l'event "changement de langue" et lance <seealso cref="SetText"/>
            /// </summary>
            protected override void Awake()
            {
                text = GetComponent<TextMesh>();
                base.Awake();
            }

            #endregion Unity

            #region Virtual/Override

            //****************************************************************************************************************
            //**                                                  Protected                                                 **
            //****************************************************************************************************************

            /// <summary>
            /// Fonction qui met le bon texte et la bonne police dans le <see cref="TextMesh"/>
            /// </summary>
            protected override void SetText()
            {
                text.text = Lang.Get(dictionary, key);
                text.font = Lang.GetFont(font);
                text.GetComponent<Renderer>().sharedMaterial = text.font.material;
            }

            #endregion Virtual/Override

            #endregion Methods
        }
    }
}