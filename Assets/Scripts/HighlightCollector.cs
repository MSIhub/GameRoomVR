using UnityEngine;
using System.Collections.Generic;
using Fusion;

public class HighlightCollector : SimulationBehaviour, ISpawned
{
    List<Collider> m_Highlights = new List<Collider>();
    Dictionary<Collider, Highlightable> m_ColliderPairs = new Dictionary<Collider, Highlightable>();

    Collider m_CurrentCollider;
    bool m_IsLocal;

    public Highlightable CurrentHighlight { get; private set; }

    public void Spawned()
    {
        m_IsLocal = Object.InputAuthority == Runner.LocalPlayer;
    }

    private void OnTriggerEnter( Collider other )
    {
        var highlight = other.GetComponentInParent<Highlightable>();

        if( highlight != null )
        {
            m_Highlights.Add( other );
            m_ColliderPairs.Add( other, highlight );

            ChangeHighlight( other );
        }
    }
    private void OnTriggerExit( Collider other )
    {
        if( m_Highlights.Contains( other ) )
        {
            m_Highlights.Remove( other );
            m_ColliderPairs.Remove( other );

            if( other == m_CurrentCollider )
            {
                if( m_Highlights.Count > 0 )
                {
                    ChangeHighlight( m_Highlights[ m_Highlights.Count - 1 ] );
                }
                else
                {
                    ChangeHighlight( null );
                }
            }
        }
    }

    void ChangeHighlight( Collider collider )
    {
        if( CurrentHighlight != null )
        {
            if( m_IsLocal )
            {
                CurrentHighlight.StopHighlight();
            }
            CurrentHighlight = null;
        }

        m_CurrentCollider = collider;

        if( m_CurrentCollider != null )
        {
            CurrentHighlight = m_ColliderPairs[ collider ];
            if( m_IsLocal )
            {
                CurrentHighlight.StartHighlight();
            }
        }
    }

}
