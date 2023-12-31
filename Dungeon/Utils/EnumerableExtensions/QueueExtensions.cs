﻿using Dungeon.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dungeon
{
    public static class QueueExtensions
    {
        public static SafeQueue<T> AsQueue<T>(this IEnumerable<T> @enum) => new SafeQueue<T>(@enum);
    }
}
