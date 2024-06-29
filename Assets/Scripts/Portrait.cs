using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Portrait : MonoBehaviour
{
    public RawImage icon;
    public HealthBar healthBar;
    public ResourceBar resourceBar;
    Unit unit;
    public RectTransform rt;
    public Image redFlash;
    public HPBoxResizer hPBoxResizer;
    public void Init(Unit u)
    {
        unit = u;
        healthBar.health =  u.health;
        u.health.onRefresh.AddListener(()=>{ 
            healthBar.Refresh(); 
            hPBoxResizer.Resize(u.health.currentHealth.ToString().Length,u.skillResource.current.ToString().Length);
        });
        u.health.onHit.AddListener((()=>
        {

            PortraitManager.inst.toggleLayout(false);
            Vector2 og = rt.anchoredPosition;
            redFlash.DOFade(.5f,0).OnComplete(()=>
            {
                StartCoroutine(f());
                IEnumerator f()
                {
                    yield return new WaitForSeconds(.1f);
                    redFlash.DOFade(0,.5f);
                }
            });

            rt.DOShakeAnchorPos(.2f,10,100).OnComplete(()=>
            { rt.anchoredPosition = og; });
        }));
        icon.texture = IconGraphicHolder.inst.dict[u.character.ID];
        resourceBar.Init(u);
        u.skillResource.onChange.AddListener(()=>{hPBoxResizer.Resize(u.health.currentHealth.ToString().Length,u.skillResource.current.ToString().Length);});
        u.skillResource.onChange.Invoke();
        u.health.onRefresh.Invoke();
        healthBar.Refresh();
        resourceBar.Refresh();
    }
}