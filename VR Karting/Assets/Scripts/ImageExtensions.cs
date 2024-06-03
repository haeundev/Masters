using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Util.Extensions
{
    public static class ImageExtensions
    {
        public static void SetSprite(this Image image, string path, Action onComplete = default)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Path is null or empty");
                return;
            }

            var sprite = Resources.Load<Sprite>(path);
            if (sprite == null)
            {
                Debug.LogError("Sprite is null");
                return;
            }

            image.sprite = sprite;
            onComplete?.Invoke();
        }

        public static void DoScale(this Image image, Vector3 targetScale, Ease ease = Ease.OutExpo,
            float duration = 0.3f)
        {
            image.transform.localScale = Vector3.zero;
            var tweener = image.transform.DOScale(targetScale, duration).SetEase(ease);
            tweener.timeScale = 1;
            Time.timeScale = 1;
        }

        public static void DoScale(this TextMeshProUGUI text, Ease ease = Ease.OutExpo, float duration = 0.3f)
        {
            text.transform.localScale = Vector3.zero;
            var tweener = text.transform.DOScale(Vector3.one, duration).SetEase(ease);
            tweener.timeScale = 1;
            Time.timeScale = 1;
        }

        public static void DoScale(this Transform transform, Ease ease = Ease.OutExpo, float duration = 0.3f)
        {
            transform.localScale = Vector3.zero;
            var tweener = transform.DOScale(Vector3.one, duration).SetEase(ease);
            tweener.timeScale = 1;
            Time.timeScale = 1;
        }
        
        public static void DoScaleToZero(this Transform transform, Ease ease = Ease.OutExpo, float duration = 0.3f)
        {
            transform.localScale = Vector3.one;
            var tweener = transform.DOScale(Vector3.zero, duration).SetEase(ease);
            tweener.timeScale = 1;
            Time.timeScale = 1;
        }
    }
}