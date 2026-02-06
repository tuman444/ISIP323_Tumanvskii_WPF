using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR14
{
    public class Core
    {
        private static CinemaDBEntities _context;
        public static CinemaDBEntities Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new CinemaDBEntities();
                }
                return _context;
            }
        }

        public static Users CurrentUser { get; set; }
    }

}
