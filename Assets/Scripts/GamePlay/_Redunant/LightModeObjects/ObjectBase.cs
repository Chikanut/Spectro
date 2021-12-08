using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ObjectBase : PoolObject, IDragHandler
{
    public Action<PointerEventData> OnDragAction;

    private bool _isDragable;
    

    public void OnDrag(PointerEventData eventData)
    {
        if(_isDragable)
            OnDragAction?.Invoke(eventData);
    }

    public override void AcceptObjectsLinks(List<PoolObject> objects)
    {
        
    }

    public override void ResetState()
    {

    }
    
}
