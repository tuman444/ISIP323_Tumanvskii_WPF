using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR17
{
    using System;
    public class Core
    {
        private static TheCosmicLodgeEntities _db;

        public static TheCosmicLodgeEntities DB => GetContext();

        public static TheCosmicLodgeEntities GetContext()
        {
            if (_db == null)
            {
                _db = new TheCosmicLodgeEntities();
            }
            return _db;
        }

        public static Users AuthUser = null; 
    }
}
