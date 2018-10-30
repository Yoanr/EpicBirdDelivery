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
        /// Classe abstraite permettant de créer des classes qui remplissent un texte avec un texte d'un dictionnaire
        /// </summary>
        public abstract class TextAutoLang : MonoBehaviour
        {
            #region Fields

            //****************************************************************************************************************
            //**                                                 Serialised                                                 **
            //****************************************************************************************************************

            /// <summary>
            /// Dictionnaire dans lequel on va chercher la clé
            /// </summary>
            [SerializeField, Tooltip("Dictionnaire dans lequel on va chercher la clé")]
            protected DictionariesEnum dictionary;

            /// <summary>
            /// Clé du texte à afficher
            /// </summary>
            [SerializeField, Tooltip("Clé du texte à afficher")]
            protected string key;

            /// <summary>
            /// Police à utiliser pour ce texte
            /// </summary>
            [SerializeField, Tooltip("Police à utiliser pour ce texte")]
            protected FontsEnum font;

            #endregion Fields

            #region Methods

            #region Unity

            //****************************************************************************************************************
            //**                                             For Unity Methods                                              **
            //****************************************************************************************************************

            /// <summary>
            /// S'inscrit à l'event "changement de langue" et lance <see cref="SetText"/>
            /// </summary>
            protected virtual void Awake()
            {
                Lang.AddEvent(SetText);
                SetText();
            }

            /// <summary>
            /// Se désinscrit de l'event "changement de langue"
            /// </summary>
            protected virtual void OnDestroy()
            {
                Lang.RemoveEvent(SetText);
            }

            #endregion Unity

            #region Abstract

            //****************************************************************************************************************
            //**                                                  Protected                                                 **
            //****************************************************************************************************************

            /// <summary>
            /// Fonction qui met le bon texte et la bonne police au bon endroit
            /// </summary>
            protected abstract void SetText();

            #endregion Abstract

            #endregion Methods
        }
    }
}