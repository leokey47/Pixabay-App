using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp17.Model;

namespace WpfApp17
{
    public class PixabayImage
    {
        public string WebformatURL { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Views { get; set; }
        public int Downloads { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
        public bool IsFavorite { get; set; }
        public int ImageId { get; set; }
        public int id { get; set; }
    }
}
