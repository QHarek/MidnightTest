using UnityEngine;

public class UIShowHide : MonoBehaviour
{
    [SerializeField] private AnimationClip _clipHide;
    [SerializeField] private AnimationClip _clipShow;
    [SerializeField] private Transform _arrowTransform;

    private Animator _animator;
    private string _state;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _state = "Show";
    }

    public void OnButtonClick()
    {
        switch (_state)
        {
            case "Show":
                {
                    if (_arrowTransform)
                    {
                        _arrowTransform.Rotate(0, 180, 0);
                    }
                    _animator.Play(_clipShow.name);
                    _state = "Hide";
                    break;
                }
            case "Hide":
                {
                    if (_arrowTransform)
                    {
                        _arrowTransform.Rotate(0, 180, 0);
                    }
                    _animator.Play(_clipHide.name);
                    _state = "Show";
                    break;
                }
        }
    }
}
