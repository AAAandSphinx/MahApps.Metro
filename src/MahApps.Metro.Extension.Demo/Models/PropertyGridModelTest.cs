using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
#nullable disable
namespace MahApps.Metro.Extension.Demo.Models
{
    public class PropertyGridModelTest : ViewModelBase
    {
        public string Name { get; set; } = nameof(Name);
        public string Description { get; set; } = nameof(Description);
        public int Age { get; set; } = 99;
        [Category("Useable")]
        public bool IsCheck { get; set; } = false;
        public EType EQType { get; set; } = EType.OUT;
        public float Width { get; set; } = 100f;
        [Category("PictureA")]
        public ImageSource PictureACTS { get; set; }  
        [Category("DateTime")]
        public DateTime CreateTime { get; set; }
    }
    public enum EType
    {
        IN,
        OUT, BOTH
    }
}
