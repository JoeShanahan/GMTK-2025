using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gmtk2025
{
    public class LoadMenuItem : MonoBehaviour
    {
        public Action<LoadMenuItem> OnPress;
        
        [SerializeField] private Image _highlightImage;

        [SerializeField] private string _filename;

        [SerializeField] private Text _text;

        public string Filename => _filename;
        
        public void Init(string filename)
        {
            gameObject.SetActive(true);
            _filename = filename;
            _text.text = filename;
            _highlightImage.DOFade(0, 0);
        }

        public void SelectMe()
        {
            _highlightImage.DOFade(0.8f, 0.15f);
        }

        public void UnselectMe()
        {
            _highlightImage.DOFade(0.0f, 0.15f);
        }

        public void ButtonPress()
        {
            OnPress?.Invoke(this);
        }
    }
}
