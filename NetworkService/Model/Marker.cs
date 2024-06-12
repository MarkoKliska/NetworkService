using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace NetworkService.Model
{
    public class Marker : BindableBase
    {
        private int markerValue;
        private string markerTime;
        private Brush markerColor;
        private Thickness markerHeight;

        public Marker()
        {

        }

        public Marker(int markerValue, string markerTime, Brush markerColor)
        {
            this.markerValue = markerValue;
            this.markerTime = markerTime;
            this.markerColor = markerColor;
        }

        public Marker(int markerValue, string markerTime)
        {
            this.markerValue = markerValue;
            this.markerTime = markerTime;
        }

        public Thickness MarkerHeight
        {
            get { return markerHeight; } 
            set 
            {
                markerHeight = value;
                OnPropertyChanged(nameof(MarkerHeight));
            }
        }

        public int MarkerValue
        {
            get { return markerValue; }
            set
            {
                markerValue = value;
                if(markerValue < 670 || markerValue > 735)
                {
                    MarkerColor = Brushes.Red;
                }
                else
                {
                    MarkerColor = Brushes.Blue;
                }
                MarkerHeight = new Thickness(0, 0, 0, MarkerValue/3);
                OnPropertyChanged(nameof(MarkerValue));
            }
        }

        public string MarkerTime
        {
            get
            {
                return markerTime;
            }
            set
            {
                markerTime = value;
                OnPropertyChanged(nameof(MarkerTime));
            }
        }

        public Brush MarkerColor
        {
            get
            {
                return markerColor;
            }
            set
            {
                markerColor = value;
                OnPropertyChanged(nameof(MarkerColor));
            }
        }
    }
}
