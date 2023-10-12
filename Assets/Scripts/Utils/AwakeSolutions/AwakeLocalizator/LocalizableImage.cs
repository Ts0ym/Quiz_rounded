using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AwakeSolutions
{
    public class LocalizableImage : Localizable
    {
        public Sprite spriteRu;
        public Sprite spriteEn;

        public Texture2D textureRu;
        public Texture2D textureEn;

        public override void Localize(Localizator.Language language)
        {
            Image image = GetComponent<Image>();
            RawImage rawImage = GetComponent<RawImage>();
            Renderer _renderer = GetComponent<Renderer>();

            if (image != null)
                image.sprite = language == Localizator.Language.RU ? spriteRu : spriteEn;

            if (_renderer != null)
                _renderer.material.SetTexture("_MainTex", language == Localizator.Language.RU ? textureRu : textureEn);

            if (rawImage != null)
                rawImage.texture = language == Localizator.Language.RU ? textureRu : textureEn;
        }
    }
}