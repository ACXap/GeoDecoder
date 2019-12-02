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