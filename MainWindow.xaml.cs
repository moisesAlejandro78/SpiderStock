using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.IO.Ports;//libreria que usamos... 
using System.Windows.Threading;

namespace SistemaDeSeguridadIntegradoAvanzado
{
    public partial class MainWindow : Window
    {
        SerialPort puerto;

        public MainWindow()
        {
            InitializeComponent();

            puerto = new SerialPort("COM6", 9600);
            puerto.NewLine = "\n";
            puerto.DataReceived += Puerto_DataReceived;
            DispatcherTimer reloj = new DispatcherTimer();
            reloj.Interval = TimeSpan.FromSeconds(1);
            reloj.Tick += (s, e) => ActualizarHora();
            reloj.Start();

            try
            {
                puerto.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el puerto: " + ex.Message);
            }
        }

        private void Puerto_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = puerto.ReadLine().Trim();

                Dispatcher.Invoke(() =>
                {
                    ProcesarRespuesta(data);
                });
            }
            catch
            {
                // Ignorar errores de lectura para evitar bloquear la app
            }
        }

        private void Enviar(string comando)
        {
            if (puerto.IsOpen)
            {
                try { puerto.WriteLine(comando); }
                catch { }
            }
        }

        private void ProcesarRespuesta(string data)
        {
            if (data.Contains("PUERTA ABIERTA"))
            {
                puerta.Text = "Abierta";
                estadoPuerta.Text = "Abierta";
                inf1.Text = "Puerta abierta";
                inf4.Text = "Sistema Iniciado";
            }

            else if (data.Contains("PUERTA CERRADA"))
            {
                puerta.Text = "Cerrada";
                estadoPuerta.Text = "Cerrada";
                inf1.Text = "Puerta cerrada";

                inf5.Text = "---"; // Reinicio al cerrar la puerta
            }


            else if (data.Contains("PUERTA BLOQUEADA"))
            {
                inf2.Text = "Bloqueada";
            }

            else if (data.Contains("PUERTA DESBLOQUEADA"))
            {
                inf2.Text = "Desbloqueada";
            }

            else if (data.StartsWith("DISTANCIA:"))
            {
                string valor = data.Replace("DISTANCIA:", "");
                sensor.Text = valor + " cm";
                inf3.Text = "Sensor -> " + valor + " cm";
            }

            else if (data.StartsWith("ESTADO:"))
            {
                inf4.Text = data;
            }
            else if (data.Contains("MOVIMIENTO DETECTADO"))
            {
                inf5.Text = "Movimiento detectado";
            }

        }

        private void btnAbrir_Click(object sender, RoutedEventArgs e) => Enviar("ABRIR");
        private void btnCerrar_Click(object sender, RoutedEventArgs e) => Enviar("CERRAR");
        private void btnBloquear_Click(object sender, RoutedEventArgs e) => Enviar("BLOQUEAR");
        private void btnDesbloquear_Click(object sender, RoutedEventArgs e) => Enviar("DESBLOQUEAR");
        private void ActualizarHora()
        {
            string hora = DateTime.Now.ToString("[HH:mm:ss]:");

            hora1.Text = hora;
            hora2.Text = hora;
            hora3.Text = hora;
            hora4.Text = hora;
            hora5.Text = hora;
        }

    }
}

