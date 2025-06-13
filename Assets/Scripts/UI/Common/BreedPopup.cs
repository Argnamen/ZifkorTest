using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class BreedPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _popupPanel;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private RectTransform _contentRect;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Button _closeButton;

    [Header("Settings")]
    [SerializeField] private float _minHeight = 300f;
    [SerializeField] private float _maxHeight = 800f;
    [SerializeField] private float _padding = 50f;
    [SerializeField] private float _animationDuration = 0.3f;

    private CanvasGroup _canvasGroup;
    private Vector2 _originalSize;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _originalSize = _popupPanel.sizeDelta;
        _closeButton.onClick.AddListener(Hide);

        Hide();
    }

    public void Show(string title, string description)
    {
        _titleText.text = title;
        _descriptionText.text = description;

        // Включаем перед расчетами
        gameObject.SetActive(true);

        // Обновляем layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(_descriptionText.rectTransform);

        // Рассчитываем новую высоту
        float preferredHeight = _descriptionText.preferredHeight + _padding;
        float newHeight = Mathf.Clamp(preferredHeight, _minHeight, _maxHeight);

        // Анимируем изменение размера
        _popupPanel.DOSizeDelta(new Vector2(_originalSize.x, newHeight), _animationDuration)
            .SetEase(Ease.OutBack);

        // Показываем попап
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, _animationDuration);

        // Прокрутка в начало
        _scrollRect.normalizedPosition = Vector2.one;
    }

    public void Hide()
    {
        _canvasGroup.DOFade(0, _animationDuration).OnComplete(() =>
        {
            gameObject.SetActive(false);
            _popupPanel.sizeDelta = _originalSize; // Сброс размера
        });
    }
}