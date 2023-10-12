using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwakeSolutions
{
    public class Localizator : MonoBehaviour
    {
        public enum Language { RU, EN };

        public static Language language;
        public static string currentLangCode = "ru";

        public void SwitchLanguage()
        {
            Debug.Log("[AwakeLocalizator] Language '" + currentLangCode + "' switched to '" + (language == Language.RU ? "en" : "ru") + "'!");

            language = language == Language.RU ? Language.EN : Language.RU;
            currentLangCode = language == Language.RU ? "ru" : "en";

            List<Localizable> localizables = new List<Localizable>(FindObjectsOfType<Localizable>());
            //List<AwakeButton> buttons = new List<AwakeButton>(FindObjectsOfType<AwakeButton>());

            foreach (Localizable localizable in localizables)
                localizable.Localize(language);

            //foreach (AwakeButton button in buttons)
            //    button.Localize(language);
        }
    }
}
