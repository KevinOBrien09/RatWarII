using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class BushInteractZoomer : Zoomer
{
    public Transform unitHolder,bushHolder;
    public Animator animator;
    public SoundData rustle;

    public void PlayRustle(){
        AudioManager.inst.GetSoundEffect().Play(rustle);
    }
    public  void InteractZoom(ItemContainer bush,OverworldUnit overworldUnit)
    {
        Vector3 ogBushRot = bush.transform.rotation.eulerAngles;
        Vector3 ogBushPos = bush.transform.position;
        Vector3 ogUnitPos = overworldUnit.transform.position;
        Vector3 ogUnitRot = overworldUnit.transform.rotation.eulerAngles;
        InteractionManager.inst.enabled =false;
        InteractionManager.inst.currentInteractable.OutlineToggle(false,false);
        PartyController.inst.run = false;
        overworldUnit.animator.enabled = false;
        overworldUnit.transform.rotation =  Quaternion.Euler(0,0,0);
        overworldUnit.agent.enabled = false;
        OverworldCamera.inst.enabled = false;
        overworldUnit.transform.SetParent(unitHolder);
        overworldUnit.transform.DOLocalMove(Vector3.zero,.2f);
        overworldUnit.LookLeft();
        bush.transform.SetParent(bushHolder);
        bush.transform.localRotation = Quaternion.Euler(0,0,0);
        bush.transform.DOLocalMove(Vector3.zero,.2f).OnComplete(()=>{
            animator.Play("Pick");

            StartCoroutine(q());
            IEnumerator q(){
                yield return new WaitForSeconds(1f); //length of pick animation
                Debug.Log("Anim End");
                var itemData = bush.RetrieveItems();
                List<Item> items = itemData.Item2;
                foreach (var item in items)
                { InventoryManager.inst.inventory.AddItem(item);}
             
                InspectionResult.inst.LoadItems(itemData.Item2, ()=>{Reset();});
                //Reset();
            }
        });

        void Reset()
        {
            bush.transform.SetParent(null);
            overworldUnit.transform.SetParent(null);
     
            bush.transform.rotation = Quaternion.Euler(ogBushRot.x,ogBushRot.y,ogBushRot.z);
            overworldUnit.transform.rotation =  Quaternion.Euler(0,0,0);
            
            bush.transform.DOLocalMove(ogBushPos,.3f);
            overworldUnit.transform.DOMove(ogUnitPos,.3f).OnComplete(()=>
            {
                PartyController.inst.run = true;
                overworldUnit.agent.enabled = true;
                OverworldCamera.inst.enabled = true;
                overworldUnit.animator.enabled = true;
                InteractionManager.inst.enabled = true;
                Destroy(gameObject);
            });
        }

    }
}