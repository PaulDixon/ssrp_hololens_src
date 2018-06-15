// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// This class implements IInputClickHandler to handle the tap gesture.
    /// It increases the scale of the object when tapped.
    /// </summary>
    public class TapResponder : MonoBehaviour, IInputClickHandler
    {
        PersistantManager boss;// = PersistantManager.Instance;

        public void OnInputClicked(InputClickedEventData eventData)
        {
            boss = PersistantManager.Instance;
            // Increase the scale of the object just as a response.
            //gameObject.transform.localScale += 0.05f * gameObject.transform.localScale;
            try { 
                SSRP_context_element_controller contextElement = gameObject.transform.GetComponent<SSRP_context_element_controller>();
                contextElement.isClosed = !contextElement.isClosed;
                eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
            }
            catch
            {
                boss = PersistantManager.Instance;
                boss.hud.addText("Object has no SSRP_context_element_controller to Open/Close ");
                boss.hud.addText("Go look for A sensesmart logo ");
            }
        }
    }
}