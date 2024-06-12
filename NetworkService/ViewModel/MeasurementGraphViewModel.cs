using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {
        public List<string> ComboBoxEntities { get; set; }
        public ObservableCollection<Marker> Markers { get; set; }
        private string selectedEntity;
        public string SelectedEntity
        {
            get { return selectedEntity; }
            set 
            { 
                selectedEntity = value;
                OnPropertyChanged(nameof(SelectedEntity));
                SetMarkers(LoadMarkers(selectedEntity));
            }
        }

        public void UpdateGraph()
        {
            SetMarkers(LoadMarkers(selectedEntity));
        }

        public MeasurementGraphViewModel() 
        {
            ComboBoxEntities = new List<string>();
            foreach(WaterFlowMeter wfm in MainWindowViewModel.WaterFlowMeters)
            {
                ComboBoxEntities.Add(wfm.Name);
            }
            Markers = new ObservableCollection<Marker>();
            for(int i = 0; i <= 4; ++i)
            {
                Markers.Add(new Marker());
            }

            //SetMarkers(LoadMarkers(SelectedEntity));
        }

        public List<Marker> LoadMarkers(string selectedEntity)
        {
            List<Marker> retVal = new List<Marker>();

            if (!File.Exists("LogFile.txt"))
            {
                MessageBox.Show("LogFile.txt does not exist.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            string[] input = File.ReadAllLines("LogFile.txt");

            for(int i = input.Count() - 1; i >= 0; --i)
            {
                string temp = input[i];

                string[] elements = temp.Split(',');

                if((SelectedEntity == elements[1]) && (retVal.Count < 5))
                {
                    retVal.Add(new Marker(int.Parse(elements[2]), elements[0]));
                }
            }

            return (retVal.Count == 5) ? retVal : null;
        }

        private void InitializeMarkers()
        {
            for (int i = 0; i <= 4; ++i)
            {
                Markers[i].MarkerValue = 0;
                Markers[i].MarkerTime = "Time";
            }
        }

        public void SetMarkers(List<Marker> markersSet)
        {
            if(markersSet != null)
            {
                for (int i = 4; i >=0; --i)
                {
                    Markers[i].MarkerValue = markersSet[i].MarkerValue;
                    Markers[i].MarkerTime = markersSet[i].MarkerTime;
                }
            }
            else
            {
                InitializeMarkers();
            }
        }
    }
}
