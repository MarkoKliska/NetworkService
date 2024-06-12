using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class WaterFlowMeter : ValidationBase
    {
        private int id;
        private string name;
        private WaterFlowMeterType type;
        private int waterValue;

        public WaterFlowMeter(int id, string name, WaterFlowMeterType typeW, int waterValue = -1)
        {
            this.Id = id;
            this.Name = name;
            this.Type = typeW;
            this.WaterValue = waterValue;
        }

        public WaterFlowMeter()
        {

        }
        public int Id
        {
            get { return id; }
            set 
            { 
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public WaterFlowMeterType Type
        {
            get { return type; }
            set {  type = value; }
        }

        public int WaterValue
        {
            get { return waterValue; }
            set 
            { 
                waterValue = value; 
                OnPropertyChanged(nameof(WaterValue));
            }
        }

        protected override void ValidateSelf()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Type.Name + " : " + Name + "_" + Id;
        }

        public bool IsValueValidForType()
        {
            bool valid = true;

            if(WaterValue < 670 || WaterValue > 735)
            {
                valid = false;
            }

            return valid;
        }
    }
}
