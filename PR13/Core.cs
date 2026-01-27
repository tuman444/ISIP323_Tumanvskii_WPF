using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR13
{
    public static class Core
    {
        private static ShopDBEntities _context;
        public static ShopDBEntities Context
        {
            get
            {
                if (_context == null)
                    _context = new ShopDBEntities();
                return _context;
            }
        }

        public static List<Products> Cart = new List<Products>();
    }
}
