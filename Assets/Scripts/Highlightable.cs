using UnityEngine;
using UnityEngine.Events;

public class Highlightable : MonoBehaviour
{
    int m_HighlightCount;
    public GameObject Highlight;

    public UnityAction<Hand> GrabCallback;
    public UnityAction DropCallback;

    public void Grab( Hand hand )
    {
        StopHighlight();
        GrabCallback?.Invoke( hand );
    }

    public void Drop()
    {
        StartHighlight();
        DropCallback?.Invoke();
    }

    private void Awake()
    {
        Highlight.SetActive( false );
    }

    public void StartHighlight()
    {
        m_HighlightCount++;
        if( Highlight != null && m_HighlightCount > 0 )
        {
            Highlight.SetActive( true );
        }
    }
    public void StopHighlight()
    {
        m_HighlightCount--;
        if( Highlight != null && m_HighlightCount <= 0 )
        {
            Highlight.SetActive( false );
        }
    }
}
