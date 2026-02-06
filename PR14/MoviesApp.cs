using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR14
{
    public partial class Movies
    {
        public string GenreString
        {
            get
            {
                if (this.Genres != null && this.Genres.Count > 0)
                {
                    return string.Join(", ", this.Genres.Select(g => g.Name));
                }
                return "Жанры не указаны";
            }
        }
    }
}
