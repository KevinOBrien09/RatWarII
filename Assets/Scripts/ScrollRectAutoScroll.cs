using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Rewired; 
//comment the next line out if you aren't using Rewired


[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAutoScroll : MonoBehaviour
{
    public  bool scrolling;
   public float scrollSpeed = .2f;
   
    public bool quickScroll;
    public List<Selectable> m_Selectables = new List<Selectable>();
    public ScrollRect m_ScrollRect;
    public Vector2 m_NextScrollPosition = Vector2.up;
 

    public virtual void Awake()
    {
    
        m_ScrollRect = GetComponent<ScrollRect>();
    }
    
    void OnEnable()
    {
        
        m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        
    }

    void OnDisable(){
        scrolling = false;
    }

    void Start()
    {
        if (m_ScrollRect)
        {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
        ScrollToSelected();
    }

    public void UpdateList()
    {
        m_Selectables.Clear();m_Selectables = new List<Selectable>();
        if (m_ScrollRect)
        {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
    }

    void Update()
    {
        InputScroll();
     
    //    Vector2 normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, m_NextScrollPosition, scrollSpeed * Time.deltaTime);
    //     m_NextScrollPosition = m_ScrollRect.normalizedPosition;
    }

    void InputScroll()
    {
        if (m_Selectables.Count > 0)
        {
            if (InputManager.inst. player.GetAxis("UI-Vertical") != 0.0f ) 
            { 
                scrolling = true;
                ScrollToSelected();
            }
            else{
                scrolling = false;
            }
        }
        else{ 
         Debug.LogWarning("No Selectables");
        }
    }

    public virtual void ScrollToSelected()
    {
        int selectedIndex = -1;
        Selectable selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

        if (selectedElement != null)
        {
            selectedIndex = m_Selectables.IndexOf(selectedElement);
        }
        if (selectedIndex > -1)
        {
            m_NextScrollPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
            m_ScrollRect.DONormalizedPos(m_NextScrollPosition,scrollSpeed);
        }
    }

    public void ShowCertainSelectable(Selectable s)
    {
        int selectedIndex = -1;
        Selectable selectedElement = s;

        if (selectedElement != null)
        {
            selectedIndex = m_Selectables.IndexOf(selectedElement);
        }

        if (selectedIndex > -1)
        {
            m_NextScrollPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
            m_ScrollRect.DONormalizedPos(m_NextScrollPosition,0);
        }
    }
}