using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ChatInputField : InputField
{
    public SubmitEvent onSubmit_ = new SubmitEvent();

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        if (onSubmit_ != null)
            onSubmit_.Invoke(text);
    }
}
