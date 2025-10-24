using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ECSGame
{
    public class GameSystem : AEntitySystem<Game>,
IInit<Game>,
IUpdate<Game>
    {
        public static Game Create(Assembly systemAssembly)
        {
            var game = EcsNodeSystem.Create<Game>(EcsType.Game, systemAssembly);
            return game;
        }

        public void Init(Game game)
        {

        }

        public void Update(Game entity)
        {
            AppStatic.DeltaTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - AppStatic.NowMilliseconds;
            AppStatic.DeltaTimeSeconds = AppStatic.DeltaTimeMilliseconds / 1000f;
            AppStatic.NowMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            AppStatic.NowSeconds = AppStatic.NowMilliseconds / 1000f;
            // if (entity.Type == ((int)GameType.TrueGameDemo))
            // {
            //     GameTrueWorldSystem.Update(entity);
            // }
            // else
            // {
            //     GameWorldSystem.Update(entity);
            // }
        }

        public void FixedUpdate(Game entity)
        {
            // if (entity.Type == ((int)GameType.TrueGameDemo))
            // {
            //     GameTrueWorldSystem.FixedUpdate(entity);
            // }
            // else
            // {
            //     GameWorldSystem.FixedUpdate(entity);
            // }
        }
    } 
}
