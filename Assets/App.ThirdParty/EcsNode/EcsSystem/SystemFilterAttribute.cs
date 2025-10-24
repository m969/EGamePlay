using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SystemFilterAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SystemPlatformFilterAttribute : SystemFilterAttribute
    {
        public int Platform { get; private set; }

        public SystemPlatformFilterAttribute(int platform)
        {
            this.Platform = platform;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SystemGameFilterAttribute : SystemFilterAttribute
    {
        public int GameType { get; private set; }

        public SystemGameFilterAttribute(int gameType)
        {
            this.GameType = gameType;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SystemEcsFilterAttribute : SystemFilterAttribute
    {
        public int EcsType { get; private set; }

        public SystemEcsFilterAttribute(int ecsType)
        {
            this.EcsType = ecsType;
        }
    }
}
