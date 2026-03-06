using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR15
{
    public class Core
    {
        private static PCdbEntities _db;

        public static PCdbEntities DB
        {
            get
            {
                if (_db == null)
                    _db = new PCdbEntities();
                return _db;
            }
        }

    }
}
