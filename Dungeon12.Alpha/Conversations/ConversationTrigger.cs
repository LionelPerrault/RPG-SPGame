﻿using Dungeon12.Drawing.SceneObjects.Map;
using Dungeon12.Map;
using Dungeon.View.Interfaces;

namespace Dungeon12.Conversations
{
    public abstract class ConversationTrigger : IConversationTrigger
    {
        public virtual bool Storable => false;

        protected Replica Replica { get; private set; }

        public IDrawText Trigger(PlayerSceneObject arg1, GameMap arg2, string[] arg3, Replica arg4)
        {
            Replica = arg4;
            return Trigger(arg1, arg2, arg3);
        }

        protected abstract IDrawText Trigger(PlayerSceneObject arg1, GameMap arg2, string[] arg3);
    }
}
