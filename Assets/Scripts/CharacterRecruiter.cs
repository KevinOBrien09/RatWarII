using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
public class CharacterRecruiter: Singleton<CharacterRecruiter>
{
    public int state;

    public UnityEvent shut;
    public CanvasGroup cg;
    public Transform door,interiorCamPos,exteriorCamPos,recuritPos;
    public CharacterProfileMenu profileMenu;
    public SoundData doorOpen,doorShut;
    

    void Start(){
        List<Character> chr = new List<Character>();
        for (int i = 0; i < 5; i++)
        {
            chr.Add(CharacterBuilder.inst.GenerateCharacter());
        }
        profileMenu.RecieveCharacters(chr);
    }

    public void Open()
    {
        state = 1;
        StartCoroutine(q());
        IEnumerator q()
        {
            //AudioManager.inst.GetSoundEffect().Play(doorOpen); 
            door.DOLocalRotate(new Vector3(0,-90,0),.25f).OnComplete(()=>{
                   WorldHubCamera.inst.Move(interiorCamPos,(()=>{


                    WorldHubCamera.inst.fuckOff = false;
                    WorldCity.inst.ChangeState(WorldCity.CityState.RECRUIT);
                    WorldCity.inst.close = shut;

                   }));
            });
            yield return new WaitForSeconds(.3f);
            
           // cg.alpha = 0;
            // recruiter.SetActive(true);
            // cg.DOFade(1,.2f);
           
            yield return new WaitForSeconds(.2f);
        }
    }

    public void OpenRecruitMenu(){
          EventSystem.current.SetSelectedGameObject(null);
        WorldHubCamera.inst.fuckOff = true;
        WorldHubCamera.inst.Move(recuritPos,(()=>
        {WorldHubCamera.inst.fuckOff = false;}));
            
        state = 2;
    }


    public void BackOut()
    {
        if(state == 1){
            state = 0;
            EventSystem.current.SetSelectedGameObject(null);
            WorldHubCamera.inst.fuckOff = true;
            
            WorldHubCamera.inst.Move(exteriorCamPos,(()=>
            {
                 
                door.DOLocalRotate(new Vector3(0,0,0),.25f).OnComplete(() =>
                {
                    WorldCity.inst.RetunToHover();
                 //AudioManager.inst.GetSoundEffect().Play(doorShut);
                }
                );
            }));
        }
        else if(state == 2){
            EventSystem.current.SetSelectedGameObject(null);
            WorldHubCamera.inst.fuckOff = true;
            WorldHubCamera.inst.Move(interiorCamPos,(()=>
            { WorldHubCamera.inst.fuckOff = false;
            state = 1;
               
            }));
        }
      
       
    }

    public void Close()
    {
        // cg.DOFade(0,.2f).OnComplete(()=>
        // {
         
        // });
       
    }
}