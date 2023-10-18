using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class ToggleButtonSprite : MonoBehaviour
{

    private Image _image;
    private Sprite _spriteDefault;
    [SerializeField]
    private Sprite _spriteToChange;

    [SerializeField]
    private bool _isMusic;

    private void Start()
    {
        _image = GetComponent<Image>();
        _spriteDefault = _image.sprite;
        if (_spriteToChange)
        {
            ToggleSprite();
        }
        else
        {
            throw new System.NullReferenceException(nameof(_spriteToChange));

        }
    }

    public void ToggleSprite()
    {
        if (_isMusic)
        {
            if (UIButtonsHolder.MusicEnabled)
            {
                _image.sprite = _spriteDefault;
            }
            else
            {
                _image.sprite = _spriteToChange;
            }
        }
        else
        {
            if (UIButtonsHolder.GameVolumeEnabled)
            {
                _image.sprite = _spriteDefault;
            }
            else
            {
                _image.sprite = _spriteToChange;
            }
        }
    }
}
