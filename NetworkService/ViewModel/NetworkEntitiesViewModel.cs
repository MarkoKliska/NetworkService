using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        private int newId;
        private string newName;
        private WaterFlowMeterType newType;
        private WaterFlowMeter selectedWaterFlowMeter;
        private int newWaterLevel;
        private string typeString;
        private string filterTypeString;
        private string filterIdString;
        private bool isLessChecked;
        private bool isGreaterChecked;
        private bool isEqualChecked;
        private string idMsg;
        private string nameMsg;

        public static ObservableCollection<WaterFlowMeter> Entities { get; set; }
        public static ObservableCollection<WaterFlowMeter> OriginalEntities { get; set; }
        public List<string> WaterFlowMeterTypes { get; set; }

        public MyICommand AddCommand { get; set; }
        public MyICommand DeleteCommand { get; set; }
        public MyICommand FilterCommand { get; set; }
        public MyICommand RemoveFilterCommand { get; set; }
        public MyICommand<object> TextBoxFocusedCommand { get; set; }
        public MyICommand<object> TextBoxLostFocusCommand {  get; set; }
        public MyICommand<string> CharPressedCommand {  get; set; }
        public MyICommand<string> IntPressedCommand { get; set; }
        public MyICommand<TextBox> CheckIdCommand {  get; set; }
        public MyICommand BackspaceCommand {  get; set; }

        public NetworkEntitiesViewModel()
        {
            if (MainWindowViewModel.WaterFlowMeters == null)
            {
                MainWindowViewModel.WaterFlowMeters = new ObservableCollection<WaterFlowMeter>();
            }
            Entities = MainWindowViewModel.WaterFlowMeters;
            OriginalEntities = new ObservableCollection<WaterFlowMeter>(Entities);
            WaterFlowMeterTypes = new List<string> { "Zapreminski", "Turbinski", "Elektronski" };

            AddCommand = new MyICommand(OnAdd);
            DeleteCommand = new MyICommand(OnDelete, CanDelete);

            FilterCommand = new MyICommand(OnFilter);
            RemoveFilterCommand = new MyICommand(OnRemoveFilter);

            

            //keyboard
            IntPressedCommand = new MyICommand<string>(OnIntPressed);
            CharPressedCommand = new MyICommand<string>(OnCharPressed);

            TextBoxFocusedCommand = new MyICommand<object>(OnTextBoxFocused);
            TextBoxLostFocusCommand = new MyICommand<object>(OnTextBoxLostFocus);

            CheckIdCommand = new MyICommand<TextBox>(OnCheckId);

            BackspaceCommand = new MyICommand(OnBackspacePressed);

            KeyboardVisibility = Visibility.Hidden;
            IsKeyboardEnabled = false;

            FilterVisibility = Visibility.Visible;
            IsFilterEnabled = true;

            AddIdVisibility = Visibility.Hidden;
            AddNameVisibility = Visibility.Hidden;
        }


        private void OnBackspacePressed()
        {
            if(SelectedTextBox.Text.Length > 0)
            {
                SelectedTextBox.Text = SelectedTextBox.Text.Remove(SelectedTextBox.Text.Length - 1, 1);
            }
        }

        private void OnIntPressed(string s)
        {
            if(SelectedTextBox != null)
            {
                SelectedTextBox.Text += s;
            }
        }


        private void OnCharPressed(string s)
        {
            if ((SelectedTextBox != null) && !SelectedTextBox.Name.Equals("ID_TB"))
            {
                SelectedTextBox.Text += s;
            } 
        }
        //keyboard
        private Visibility keyboardVisibility;
        private Visibility filterVisibility;
        private TextBox selectedTextBox;
        private bool isKeyboardEnabled;
        private bool isFilterEnabled;

        public Visibility KeyboardVisibility
        {
            get { return keyboardVisibility; }
            set
            {
                SetProperty(ref keyboardVisibility, value);
            }
        }


        public bool IsKeyboardEnabled
        {
            get { return isKeyboardEnabled; }
            set
            {
                SetProperty(ref isKeyboardEnabled, value);
            }
        }


        public Visibility FilterVisibility
        {
            get { return filterVisibility; }
            set
            {
                SetProperty(ref filterVisibility, value);
            }
        }


        public bool IsFilterEnabled
        {
            get { return isFilterEnabled; }
            set
            {
                SetProperty(ref isFilterEnabled, value);
            }
        }


        public TextBox SelectedTextBox
        {
            get { return selectedTextBox; }
            set
            {
                SetProperty(ref selectedTextBox, value);
                //OnPropertyChanged(nameof(SelectedTextBox));
            }
        }

        private void OnTextBoxFocused(object o)
        {
            if (o is System.Windows.Controls.TextBox t)
            {
                SelectedTextBox = t;
                SelectedTextBox.Focus(); //
                KeyboardVisibility = Visibility.Visible;
                IsKeyboardEnabled = true;
                FilterVisibility = Visibility.Hidden;
                IsFilterEnabled = false;
            }
        }
        private void OnTextBoxLostFocus(object o)
        {
            if (o is TextBox)
            {
                KeyboardVisibility = Visibility.Hidden;
                IsKeyboardEnabled = false;
                FilterVisibility = Visibility.Visible;
                IsFilterEnabled = true;
            }
        }

        private Visibility addIdVisibility;
        private Visibility addNameVisibility;

        private void OnCheckId(TextBox t)
        {
            if (t.Name.Equals("ID_TB"))
            {
                if(Regex.IsMatch(t.Text, @"^\d+$"))
                {
                    return;
                }
                else
                {
                    if (!string.IsNullOrEmpty(t.Text) && !t.Text.Equals("-1"))
                    {
                        if(t.Text.Length > 0)
                        {
                            t.Text = t.Text.Remove(t.Text.Length - 1);
                            t.CaretIndex = t.Text.Length - 1;
                        }

                    }
                }
            }
        }
        //keyboard end

        public bool IsLessChecked
        {
            get { return isLessChecked; }
            set 
            { 
                isLessChecked = value; 
                OnPropertyChanged(nameof(IsLessChecked));
            }
        }

        public bool IsGreaterChecked
        {
            get { return isGreaterChecked; }
            set
            {
                isGreaterChecked = value;
                OnPropertyChanged(nameof(IsGreaterChecked));
            }
        }

        public bool IsEqualChecked
        {
            get { return isEqualChecked; }
            set
            {
                isEqualChecked = value;
                OnPropertyChanged(nameof(IsEqualChecked));
            }
        }


        public string FilterTypeString
        {
            get { return filterTypeString; }
            set
            {
                filterTypeString = value;
                OnPropertyChanged(nameof(FilterTypeString));
            }
        }

        public string FilterIdString
        {
            get { return filterIdString; }
            set
            {
                filterIdString = value;
                OnPropertyChanged(nameof(FilterIdString));
            }
        }

        public string TypeString
        {
            get { return typeString; }
            set 
            { 
                typeString = value;
                OnPropertyChanged(nameof(TypeString));
            }
        }

        public int NewId
        {
            get { return newId; }
            set 
            { 
                newId = value; 
                OnPropertyChanged(nameof(NewId));
            }
        }

        public string NewName
        {
            get { return newName; }
            set 
            { 
                newName = value;
                OnPropertyChanged(nameof(NewName));
            }
        }

        public WaterFlowMeterType NewType
        {
            get { return newType; }
            set 
            { 
                newType = value;
                OnPropertyChanged(nameof(NewType));
            }
        }

        public WaterFlowMeter SelectedWaterFlowMeter
        {
            get { return selectedWaterFlowMeter; }
            set 
            { 
                selectedWaterFlowMeter = value;
                OnPropertyChanged(nameof(SelectedWaterFlowMeter));
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public int NewWaterLevel
        {
            get { return newWaterLevel; }
            set { newWaterLevel = value; }
        }

        public string IdMsg
        {
            get { return idMsg; }
            set
            {
                idMsg = value;
                OnPropertyChanged(nameof(IdMsg));
            }
        }

        public string NameMSg
        {
            get { return nameMsg; }
            set
            {
                nameMsg = value;
                OnPropertyChanged(nameof(nameMsg));
            }
        }

        public Visibility AddIdVisibility
        {
            get { return addIdVisibility; }
            set
            {
                addIdVisibility = value;
                OnPropertyChanged(nameof(AddIdVisibility));
            }
        }

        public Visibility AddNameVisibility
        {
            get { return addNameVisibility; }
            set
            {
                addNameVisibility = value;
                OnPropertyChanged(nameof(AddNameVisibility));
            }
        }

        public void OnAdd()
        {
            string path;
            //string tip = TypeString;
            if (TypeString == "Zapreminski")
            {
                path = "pack://application:,,,/NetworkService;component/Pictures/zapreminski.jpg";
            }
            else if (TypeString == "Turbinski")
            {
                path = "pack://application:,,,/NetworkService;component/Pictures/turbinski.jpg";
            }
            else if (TypeString == "Elektronski")
            {
                path = "pack://application:,,,/NetworkService;component/Pictures/elektronski.png";
            }
            else
            {
                path = "";
            }



            WaterFlowMeterType nt = new WaterFlowMeterType(TypeString, path);
            if (Verify(NewId, NewName, nt))
            {
                WaterFlowMeter newWaterFlowMeter = new WaterFlowMeter(NewId, NewName, nt);
                MainWindowViewModel.WaterFlowMeters.Add(newWaterFlowMeter);
                OriginalEntities.Add(newWaterFlowMeter);

                OnTextBoxLostFocus(SelectedTextBox);
                if (MainWindowViewModel.WaterFlowMeters.Contains(newWaterFlowMeter))
                {
                    MessageBox.Show("Successfully added a new entity!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Adding a new entity failed!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                NewId = 0;
                NewName = "";
            }

        }

        private bool Verify(int id, string name, WaterFlowMeterType t)
        {

            if (VerifyId(id) && VerifyName(name) && VerifyType(t))
            {
                AddNameVisibility = Visibility.Hidden;
                AddIdVisibility = Visibility.Hidden;
                return true;
            }
            return false;
        }

        private bool VerifyId(int id)
        {
            if (id == 0)
            {
                IdMsg = "You must enter an ID!";
                AddIdVisibility = Visibility.Visible;
                return false;
            }
            else if (CheckReplica(id))
            {
                IdMsg = $"ID {id} is taken!";
                AddIdVisibility= Visibility.Visible;
            }
            else
            {
                AddIdVisibility = Visibility.Hidden;
            }
            return true;
        }

        private bool CheckReplica(int idTest)
        {
            foreach(WaterFlowMeter wfm in Entities)
            {
                if (wfm.Id == idTest)
                    return true;
            }
            return false;
        }

        private bool VerifyName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NameMSg = "Name cannot be empty!";
                AddNameVisibility = Visibility.Visible;
                return false;
            }
            else if (Int32.TryParse(name, out int temp))
            {
                NameMSg = "Name cannot be a number!";
                AddNameVisibility = Visibility.Visible;
                return false;
            } 
            else
            {
                AddNameVisibility = Visibility.Hidden;
            }
            return true;
        }

        private bool VerifyType(WaterFlowMeterType t)
        {
            if(t.Name == "Zapreminski")
            {
                return true;
            }
            else if(t.Name == "Turbinski")
            {
                return true;
            }
            else if(t.Name == "Elektronski")
            {
                return true;
            }
            return false;
        }

        public void OnDelete()
        {
            if (SelectedWaterFlowMeter != null)
            {
                string nameDe = SelectedWaterFlowMeter.Name;
                var res = MessageBox.Show($"Are you sure you want to delete {SelectedWaterFlowMeter.Name}?", "Question!", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    MainWindowViewModel.WaterFlowMeters.Remove(SelectedWaterFlowMeter);
                    OriginalEntities.Remove(SelectedWaterFlowMeter);
                    MessageBox.Show($"Successfully deleted {nameDe}!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No entity selected to delete!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanDelete()
        {
            return SelectedWaterFlowMeter != null;
        }

        public void OnFilter()
        {
            ObservableCollection<WaterFlowMeter> filteredList = new ObservableCollection<WaterFlowMeter>(OriginalEntities);
            if (FilterTypeString != null)
            {
                filteredList = new ObservableCollection<WaterFlowMeter>(filteredList.Where(e => e.Type.Name == FilterTypeString));
            }
            if (int.TryParse(FilterIdString, out int filterId))
            {
                if (IsLessChecked == true)
                {
                    filteredList = new ObservableCollection<WaterFlowMeter>(
                        filteredList.Where(e => e.Id < filterId));
                }
                else if (IsGreaterChecked == true)
                {
                    filteredList = new ObservableCollection<WaterFlowMeter>(
                        filteredList.Where(e => e.Id > filterId));
                }
                else if (IsEqualChecked == true)
                {
                    filteredList = new ObservableCollection<WaterFlowMeter>(
                        filteredList.Where(e => e.Id == filterId));
                }
            }
            Entities.Clear();
            foreach (var item in filteredList)
            {
                Entities.Add(item);
            }
        }

        public void OnRemoveFilter()
        {
            Entities.Clear();
            foreach (var item in OriginalEntities)
            {
                Entities.Add(item);
            }
            OnPropertyChanged(nameof(Entities));
        }
    }
}
