using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProgaLab4Sem2Server
{
    public partial class Form1 : Form
    {
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        int port = 49002;
        TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 49002);

        public Form1()
        {
            tcpListener.Start();
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ReceiveInformation();
        }
        async private void ReceiveInformation()
        {
            using TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
            NetworkStream stream = tcpClient.GetStream();
            var responseBytes = new byte[512];
            var builder = new StringBuilder();
            int bytes;
            DriveInfo[] drives;
            DirectoryInfo directoryInfo;
            StreamReader reader;
            do
            {
                bytes = await stream.ReadAsync(responseBytes);
                string responsePart = Encoding.UTF8.GetString(responseBytes, 0, bytes);
                if (responsePart == "")
                {
                    break;
                }
                if (responsePart == "SomeSecretWord")
                {
                    drives = DriveInfo.GetDrives();
                    string names = "";
                    foreach (DriveInfo drive in drives)
                    {
                        names += drive.Name + " ";
                    }
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(names));
                }
                else if (responsePart.Contains(".txt"))
                {
                    reader = new StreamReader(responsePart);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(reader.ReadToEnd()));
                }
                else
                {
                    directoryInfo = new DirectoryInfo(responsePart);
                    string namesOfDirectories = "";
                    string namesOfFiles = "";
                    foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                    {
                        namesOfDirectories += directory.Name + ": ";
                    }
                    foreach (FileInfo file in directoryInfo.GetFiles())
                    {
                        namesOfFiles += file.Name + ": ";
                    }
                    await stream.WriteAsync(Encoding.UTF8.GetBytes($"{namesOfDirectories + namesOfFiles}"));
                }
                textBox1.Text += "\tСервер получил " + responsePart;
            }
            while (bytes > 0);
            ReceiveInformation();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            tcpListener.Stop();
        }
    }
}