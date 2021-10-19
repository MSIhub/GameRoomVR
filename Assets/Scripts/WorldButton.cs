using UnityEngine;
using UnityEngine.Events;

public class WorldButton : MonoBehaviour
{
    public UnityEvent OnClicked;

    Highlightable m_Highlight;

    private void Awake()
    {
        m_Highlight = GetComponentInChildren<Highlightable>();
        m_Highlight.GrabCallback += OnGrab;
    }

    private void OnDestroy()
    {
        if( m_Highlight != null )
        {
            m_Highlight.GrabCallback -= OnGrab;
        }
    }

    void OnGrab( Hand hand )
    {
        OnClicked?.Invoke();
    }
}
