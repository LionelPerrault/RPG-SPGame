﻿using Dungeon.Data;
using Dungeon.Map;
using Dungeon.Network;
using Dungeon.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dungeon.Entities
{
    public class Entity : NetObject
    {
        public MapObject MapObject { get; set; }

        public string Assembly { get; set; }
    }

    public class DataEntity<TEntity,TPersist> : Entity
        where TEntity : DataEntity<TEntity, TPersist>
        where TPersist : Persist
    {
        protected virtual void Init(TPersist dataClass)
        {
        }

        public static TEntity Load(string id)
        {
            var entity = typeof(TEntity).New<TEntity>();
            var dataClass = Database.Entity<TPersist>(x => x.IdentifyName == id, id).FirstOrDefault();
            if (dataClass != default)
            {
                entity.Init(dataClass);
            }

            return entity;
        }

        public static TEntity Load(Expression<Func<TPersist, bool>> filterOne, object cacheObject=default)
        {
            var entity = typeof(TEntity).New<TEntity>();
            var dataClass = Database.Entity(filterOne, cacheObject).FirstOrDefault();
            if (dataClass != default)
            {
                entity.Init(dataClass);
            }

            return entity;
        }
    }
}