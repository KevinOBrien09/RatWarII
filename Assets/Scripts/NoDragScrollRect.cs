
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;using UnityEngine.EventSystems;
using UnityEngine.Events;
public class NoDragScrollRect : ScrollRect {
  public override void OnBeginDrag(PointerEventData eventData) { }
  public override void OnDrag(PointerEventData eventData) { }
  public override void OnEndDrag(PointerEventData eventData) { }
}