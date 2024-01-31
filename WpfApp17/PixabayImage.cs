using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WpfApp17.Model;

namespace WpfApp17
{
    public class PixabayImage
    {
        public string WebformatURL { get; set; }
        public string PreviewURL { get; set; }
        public string LargeImageURL { get; set; }
        
        public string Title { get; set; }
        public string User { get; set; }
        public string User_Id { get; set; }
        
        public int Views { get; set; }
        public int Downloads { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
        public bool IsFavorite { get; set; }
        public int ImageId { get; set; }
        public int Id { get; set; }
        public BitmapImage ImageSource { get; set; }
    }
}
