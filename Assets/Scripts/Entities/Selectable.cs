using UnityEngine;

// For meeples, items, and structures that are selectable
[RequireComponent(typeof(SpriteRenderer))]
public class Selectable : MonoBehaviour
{
    protected SpriteRenderer _spriteRenderer;

    private bool _isSelected = false;
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            
            if (!_spriteRenderer) _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_isSelected)
                _spriteRenderer.material.SetInt("_ShowOutline", 1);
            else
                _spriteRenderer.material.SetInt("_ShowOutline", 0);
        }
    }
}