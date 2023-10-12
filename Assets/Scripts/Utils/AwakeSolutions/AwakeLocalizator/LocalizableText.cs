using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AwakeSolutions
{
    public class LocalizableText : Localizable
    {
        public string stringRu;
        public string stringEn;

        public override void Localize(Localizator.Language language)
        {
            Text text = GetComponent<Text>();
            TMPro.TextMeshProUGUI tmpText = GetComponent<TMPro.TextMeshProUGUI>();

            if (text != null)
                text.text = nl2br(language == Localizator.Language.RU ? stringRu : stringEn);

            if (tmpText != null)
                tmpText.text = language == Localizator.Language.RU ? stringRu : stringEn;
        }

        string nl2br(string input)
        {
            return input.Replace("\\n", "\n");
        }
    }
}