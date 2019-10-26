﻿using Rogue.Events.Network;
using System.Collections.Generic;

namespace Rogue.Network
{
    public class NetProxy : ProxyProperty
    {
        /// <summary>
        /// 
        /// <para>
        /// [Кэшируемый]
        /// </para>
        /// </summary>
        public override T Get<T>(T v, string proxyId)
        {
            if (!___GetCache.Contains(proxyId))
            {
                Global.Events.Subscribe<NetworkReciveEvent<T>>(e =>
                {
                    this.__Set(e.Message);
                }, false, proxyId);

                ___GetCache.Add(proxyId);
            }

            return v;
        }
        private readonly HashSet<string> ___GetCache = new HashSet<string>();
        
        public override T Set<T>(T v, string proxyId)
        {
            Global.Events.Raise(new NetworkSendEvent<T>(v), proxyId);
            return v;
        }
    }
}
