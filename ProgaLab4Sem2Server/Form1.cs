using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProgaLab4Sem2Server
{
    public partial class Form1 : Form
    {
        Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        Socket client;
        int port = 0;
        public Form1()
        {
            InitializeComponent();
        }
        async private void Form1_Load(object sender, EventArgs e)
        {

            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 49002);
            tcpSocket.Bind(ep);
            ReceiveInformation();
        }
        async private void ReceiveInformation()
        {
            
            tcpSocket.Listen();
            client = await tcpSocket.AcceptAsync();
            var responseBytes = new byte[512];
            var builder = new StringBuilder();
            int bytes;
            DriveInfo[] drives;
            DirectoryInfo directoryInfo;
            StreamReader reader;
            do
            {
                bytes = await client.ReceiveAsync(responseBytes);
                string responsePart = Encoding.UTF8.GetString(responseBytes, 0, bytes);
                if(responsePart == "")
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
                    await client.SendAsync(Encoding.UTF8.GetBytes(names));
                }
                else if(responsePart.Contains(".txt"))
                {
                    reader = new StreamReader(responsePart);
                    await client.SendAsync(Encoding.UTF8.GetBytes(reader.ReadToEnd()));
                }
                else
                {
                    directoryInfo = new DirectoryInfo(responsePart);
                    string namesOfDirectories = "";
                    string namesOfFiles = "";
                    foreach(DirectoryInfo directory in directoryInfo.GetDirectories())
                    {
                        namesOfDirectories+= directory.Name + " ";
                    }
                    foreach(FileInfo file in directoryInfo.GetFiles())
                    {
                        namesOfFiles+= file.Name + " "; 
                    }
                    await client.SendAsync(Encoding.UTF8.GetBytes($"{namesOfDirectories+namesOfFiles}"));
                }
                textBox1.Text += "\nСервер получил "+responsePart;
            }
            while (bytes > 0);
            client.Close();
            ReceiveInformation();
        }


    }
}