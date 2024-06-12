using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class WaterFlowMeterType : BindableBase
    {
        private string name;
        private string pathToImage;

        public WaterFlowMeterType(string name, string path)
        {
            this.Name = name;
            this.PathToImage = path;
        }

        public string Name 
        {
            get { return name; }
            set 
            { 
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string PathToImage
        {
            get { return pathToImage; }
            set
            {
                pathToImage = value;
                OnPropertyChanged("PathToImage");
            }
        }
    }
}
