using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    { 
        public ObservableCollection<WaterFlowMeter> EntitiesDisplay {  get; set; }
        public ObservableCollection<Canvas> CanvasCollection { get; set; }
        public ObservableCollection<WaterFlowMeter> CanvasInfo { get; set; }
        public ObservableCollection<Brush> BorderBrushCollection {  get; set; }
        
        private WaterFlowMeter selectedWaterFlowMeter;

        private WaterFlowMeter draggedItem = null;
        private bool dragging = false;
        public int draggingSourceIndex = -1;

        public MyICommand<object> DropEntityOnCanvas {  get; set; }
        public MyICommand<object> LeftMouseButtonDownCanvas { get; set; }
        public MyICommand LeftMouseButtonUp {  get; set; }
        public MyICommand<object> SelectionChanged {  get; set; }
        public MyICommand<object> OslobodiCanvas {  get; set; }
        public MyICommand<object> FreeCanvas {  get; set; }

        public NetworkDisplayViewModel()
        {
            if(EntitiesDisplay == null)
            {
                EntitiesDisplay = new ObservableCollection<WaterFlowMeter>();
            }
            foreach(WaterFlowMeter wfm in MainWindowViewModel.WaterFlowMeters)
            {
                if(!EntitiesDisplay.Contains(wfm))
                    EntitiesDisplay.Add(wfm);
            }

            CanvasCollection = new ObservableCollection<Canvas>();
            CanvasInfo = new ObservableCollection<WaterFlowMeter>();
            for (int i = 0; i < 12; i++)
            {
                CanvasCollection.Add(new Canvas()
                {
                    Background = Brushes.LightGray,
                    AllowDrop = true
                });
                //
                CanvasInfo.Add(new WaterFlowMeter());
                //
            }

            

            BorderBrushCollection = new ObservableCollection<Brush>();
            for (int i = 0; i < 12; i++)
            {
                BorderBrushCollection.Add(Brushes.DarkGray);
            }

            DropEntityOnCanvas = new MyICommand<object>(OnDrop);
            LeftMouseButtonDownCanvas = new MyICommand<object>(OnLeftMouseButtonDown);
            LeftMouseButtonUp = new MyICommand(OnLeftMOuseButtonUp);
            SelectionChanged = new MyICommand<object>(OnSelectionChanged);
            FreeCanvas = new MyICommand<object>(OnFreeCanvas);
        }

        public WaterFlowMeter SelectedWaterFlowMeter
        {
            get { return selectedWaterFlowMeter; }
            set
            {
                selectedWaterFlowMeter = value;
                OnPropertyChanged(nameof(SelectedWaterFlowMeter));
            }
        }

        private void OnDrop(object o)
        {
            if(draggedItem != null)
            {
                int index = Convert.ToInt32(o);

                if (CanvasCollection[index].Resources["taken"] == null)
                {
                    BitmapImage img = new BitmapImage();

                    try
                    {
                        img.BeginInit();
                        img.UriSource = new Uri(draggedItem.Type.PathToImage, UriKind.RelativeOrAbsolute);
                        img.EndInit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        img = null; 
                    }

                    if (img != null)
                    {
                        CanvasCollection[index].Background = new ImageBrush(img);
                    }
                    else
                    {
                        CanvasCollection[index].Background = Brushes.LightGray; 
                    }
                    CanvasCollection[index].Background = new ImageBrush(img);
                    CanvasCollection[index].Resources.Add("taken", true);
                    CanvasCollection[index].Resources.Add("data", draggedItem);
                    BorderBrushCollection[index] = (draggedItem.IsValueValidForType()) ? Brushes.Green : Brushes.Red;

                    //obrisi ako puca
                    CanvasInfo[index].WaterValue = draggedItem.WaterValue;
                    CanvasInfo[index].Id = draggedItem.Id;

                    if (EntitiesDisplay.Contains(draggedItem))
                    {
                        EntitiesDisplay.Remove(draggedItem);
                    }
                }
            }
        }

        private void OnLeftMouseButtonDown(object o)
        {
            if (!dragging)
            {
                int index = Convert.ToInt32(o);

                if (CanvasCollection[index].Resources["taken"] != null)
                {
                    dragging = true;
                    draggedItem = (WaterFlowMeter)(CanvasCollection[index].Resources["data"]);
                    draggingSourceIndex = index;
                    DragDrop.DoDragDrop(CanvasCollection[index], draggedItem, DragDropEffects.Move);
                }
            }
        }

        private void OnLeftMOuseButtonUp()
        {
            draggedItem = null;
            SelectedWaterFlowMeter = null;
            dragging = false;
            draggingSourceIndex = -1;
        }
        
        private void OnSelectionChanged(object o)
        {
            if (!dragging)
            {
                dragging = true;
                draggedItem = SelectedWaterFlowMeter;
                DragDrop.DoDragDrop((ListView)o, draggedItem, DragDropEffects.Move);
            }
        }

        private void OnFreeCanvas(object o)
        {
            int index = Convert.ToInt32(o);

            if (CanvasCollection[index].Resources["taken"] != null)
            {
                CanvasInfo[index].Id = -1;
                CanvasInfo[index].WaterValue = -1;

                EntitiesDisplay.Add((WaterFlowMeter)CanvasCollection[index].Resources["data"]);
                CanvasCollection[index].Background = Brushes.LightGray;
                CanvasCollection[index].Resources.Remove("taken");
                CanvasCollection[index].Resources.Remove("data");
                BorderBrushCollection[index] = Brushes.DarkGray;
            }
        }

        public void DeleteEntityFromCanvas(WaterFlowMeter wfm)
        {
            int canvasIndex = GetCanvasIndexForEntityId(wfm.Id);

            if (canvasIndex != -1)
            {
                CanvasCollection[canvasIndex].Background = Brushes.LightGray;
                CanvasCollection[canvasIndex].Resources.Remove("taken");
                CanvasCollection[canvasIndex].Resources.Remove("data");
                BorderBrushCollection[canvasIndex] = Brushes.DarkGray;
            }
        }

        public int GetCanvasIndexForEntityId(int entityId)
        {
            for (int i = 0; i < CanvasCollection.Count; i++)
            {
                WaterFlowMeter wfm = (CanvasCollection[i].Resources["data"]) as WaterFlowMeter;

                if ((wfm != null) && (wfm.Id == entityId))
                {
                    return i;
                }
            }
            return -1;
        }

        public void UpdateEntityOnCanvas(WaterFlowMeter wfm)
        {
            int canvasIndex = GetCanvasIndexForEntityId(wfm.Id);

            if (canvasIndex != -1)
            {
                if (wfm.IsValueValidForType())
                {
                    BorderBrushCollection[canvasIndex] = Brushes.Green;
                }
                else
                {
                    BorderBrushCollection[canvasIndex] = Brushes.Red;
                }
            }
        }
    }
}
