using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private int count = 15; // Inicijalna vrednost broja objekata u sistemu
                                // ######### ZAMENITI stvarnim brojem elemenata
                                //           zavisno od broja entiteta u listi

        public MainWindowViewModel()
        {
            NavCommand = new MyICommand<string>(OnNav);
            CurrentViewModel = networkEntitiesViewModel;
            WaterFlowMeters = new ObservableCollection<WaterFlowMeter>();
            createListener(); //Povezivanje sa serverskom aplikacijom
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(WaterFlowMeters.Count.ToString()); //ovde je bilo count.ToString()
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji
                            //ProcessIncomingData(incomming);
                            int index = int.Parse(incomming.Split(':')[0].Split('_')[1]);
                            int value = int.Parse(incomming.Split(':')[1]);
                            WaterFlowMeters[index].WaterValue = value;
                            WriteToFile(WaterFlowMeters[index].Name, value);
                            //networkDisplayViewModel.UpdateEntityOnCanvas(networkDisplayViewModel.EntitiesDisplay[index]);
                            //SetMarkers(LoadMarkers(selectedEntity));
                            measurementGraphViewModel.UpdateGraph();
                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }
        
        public void WriteToFile(string entity, int val)
        {
            if (firstWrite)
            {
                StreamWriter sw;
                using (sw = new StreamWriter(path.ToString()))
                {
                    sw.WriteLine($"{DateTime.Now.ToString("HH:mm")},{entity},{val}");
                }
                firstWrite = false;
            }
            else
            {               
                StreamWriter sw;
                using (sw = new StreamWriter(path.ToString(), true))
                {
                    sw.WriteLine($"{DateTime.Now.ToString("HH:mm")},{entity},{val}");
                }
            }
        }

        public MyICommand<string> NavCommand { get; private set; }
        public MyICommand<Window> CloseWindowCommand { get; private set; }
        public NetworkEntitiesViewModel networkEntitiesViewModel = new NetworkEntitiesViewModel();
        public NetworkDisplayViewModel networkDisplayViewModel = new NetworkDisplayViewModel();
        public MeasurementGraphViewModel measurementGraphViewModel = new MeasurementGraphViewModel();
        private BindableBase currentViewModel;
        private static ObservableCollection<WaterFlowMeter> waterFlowMeters;

        private Uri path = new Uri("LogFile.txt", UriKind.Relative);
        private bool firstWrite = true;

        public static ObservableCollection<WaterFlowMeter> WaterFlowMeters
        {
            get { return waterFlowMeters; }
            set
            {
                waterFlowMeters = value;
                OnStaticPropertyChanged(nameof(WaterFlowMeters));
            }
        }

        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "home":
                    CurrentViewModel = networkEntitiesViewModel;
                    break;
                case "display":
                    CurrentViewModel = networkDisplayViewModel; ;
                    break;
                case "graph":
                    CurrentViewModel = measurementGraphViewModel;
                    break;
            }
        }
    }
}
