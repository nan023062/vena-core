#if DEBUG_SYSTEM
using System;
using UnityEngine;

namespace XDTGame.Core.UnitTest;

public static class ActorUnitTest
{
    private static CharacterActor _characterActor;
    
    public static void Update()
    {
        //return;
        
        if (null == _characterActor)
        {
            if (Input.GetKey(KeyCode.B) && Input.GetMouseButtonUp(0))
            {
                //_characterActor = World.Default.CreateActor(typeof(CharacterActor)) as CharacterActor;
                _characterActor = World.Default.CreateActor<CharacterActor>(typeof(CharComponent));
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.B) && Input.GetMouseButtonUp(1))
            {
                _characterActor.AddComponent<CharComponent>();
            }
        
            if (Input.GetKey(KeyCode.D) && Input.GetMouseButtonUp(1))
            {
                CharComponent charComponent = _characterActor.GetComponent<CharComponent>();
                charComponent?.Destroy();

                charComponent = new CharComponent();
            }
            
            if (Input.GetKey(KeyCode.D) && Input.GetMouseButtonUp(0))
            {
                _characterActor.Destroy();
                
                _characterActor = null;
            }
        }
    }
}
#endif