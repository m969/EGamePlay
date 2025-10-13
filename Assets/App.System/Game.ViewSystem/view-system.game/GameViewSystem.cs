using ECS;
using EGamePlay;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class GameViewSystem : AEntitySystem<Game>,
IAwake<Game>,
IInit<Game>,
IUpdate<Game>
    {
        public void Awake(Game entity)
        {
            
        }

        public void Init(Game entity)
        {

        }

        public void Update(Game entity)
        {
            if (entity.GetComponent<SpellPreviewComponent>() is { } spellPreviewComponent)
            {
                SpellPreviewSystem.Update(entity, spellPreviewComponent);
            }
            if (entity.GetComponent<PlayerInputComponent>() is { } inputComponent)
            {
                PlayerInputSystem.Update(entity, inputComponent);
            }
        }
    } 
}
