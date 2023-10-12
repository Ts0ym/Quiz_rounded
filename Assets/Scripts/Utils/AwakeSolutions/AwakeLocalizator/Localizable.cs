using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AwakeSolutions
{
    public class Localizable : MonoBehaviour
    {
        private void OnEnable()
        {
            Localize(Localizator.language);
        }

        public virtual void Localize(Localizator.Language language)
        {
        }
    }
}