using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ObjectBase : PoolObject, IDragHandler
{
    public Action<PointerEventData> OnDragAction;

    public void OnDrag(PointerEventData eventData)
    {
        OnDragAction?.Invoke(eventData);
    }

    public override void AcceptObjectsLinks(List<PoolObject> objects)
    {
        
    }

    public override void ResetState()
    {

    }
}
