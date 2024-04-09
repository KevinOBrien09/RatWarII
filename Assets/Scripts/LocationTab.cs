using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LocationTab : Button
{
    public TextMeshProUGUI locName,cardinalDir;
    public RectTransform arrowRT;
    public Image locImage;
    LocationInfo i;
    public void Init(LocationInfo info,Vector2 currentID)
    {
        i = info;
        locName.text = info.locationName;
        if(info.locationPic != null){
            locImage.sprite = info.locationPic;
        }
       
        Location l = LocationManager.inst.GetRelativeDirection(currentID,info.stage.GetID());
        cardinalDir.text = l.ToString();
        switch(l)
        {
            case Location.NORTH:
            arrowRT.rotation = Quaternion.Euler(0,0,0);
            break;
            case Location.SOUTH:
            arrowRT.rotation = Quaternion.Euler(0,0,180);
            break;
            case Location.EAST:
            arrowRT.rotation = Quaternion.Euler(0,0,0-90);

            break;
            case Location.WEST:
            arrowRT.rotation = Quaternion.Euler(0,0,90);
            break;
        }

    }

    public void Click(){
        EventSystem.current.SetSelectedGameObject(null);
        PartyManager.inst.currentParty = LocationDetailHandler.inst.clickedInParty.ID;
       
        if( LocationManager.inst.BeginTravel(i))
        {
            if(LocationManager.inst.INSTANT_TRAVEL)
            {LocationManager.inst.Transfer();}
            else
            {
                LocationDetailHandler.inst.Hide();
                BlackFade.inst.toggleRaycast(true);
                MusicManager.inst.FadeAndChange(null,1);
                AudioManager.inst.GetSoundEffect().Play(i.travelSting);
                HubCharacterDisplay.inst.fandfLogo.gameObject.SetActive(true);
                HubCharacterDisplay.inst.fandfLogo.DOFade(1,2f);
                BlackFade.inst.FadeInEvent(()=>{
                GameManager.inst.  StartCoroutine(q());
                IEnumerator q()
                {
                    yield return new WaitForSeconds(.5f);
                    SceneManager.LoadScene("Arena");
                }
                });
            }
        }
        else
        {Debug.LogAssertion("COULD NOT TRAVEL!");}
    }

}