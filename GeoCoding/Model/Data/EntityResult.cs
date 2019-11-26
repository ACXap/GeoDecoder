using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCoding.Model.Data
{
    public class EntityResult<T>
    {
        public bool Successfully { get; set; }
        public Exception Error { get; set; }
        public T Object { get; set; }
        public IEnumerable<T> Objects { get; set; }
    }
}
