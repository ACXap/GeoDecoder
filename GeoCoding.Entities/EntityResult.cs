﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;

namespace GeoCoding.Entities
{
    public class EntityResult<T>
    {
        public bool Successfully { get; set; }
        public Exception Error { get; set; }
        public T Entity { get; set; }
        public IEnumerable<T> Entities { get; set; }
    }
}