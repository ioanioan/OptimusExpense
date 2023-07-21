using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OptimusCertificate
{
    public partial class Form1 : Form
    {
        private string port = "";
        private string adresaAnaf = "";
        public Form1()
        {
            InitializeComponent(); ;
            port = ConfigurationManager.AppSettings["port"];
            adresaAnaf = ConfigurationManager.AppSettings["adresaAnaf"];
        }

        private void Form1_Load(object sender, EventArgs e)
        {

             var bytes=GeStareEFactura("1");

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(1000);
                    //   GeStareEFactura("1");
                    this.Invoke(new MethodInvoker(() => this.Hide()));

                    CreateServer();
                }
                catch (Exception ex)
                {
                    this.Invoke(new MethodInvoker(() => MessageBox.Show(FlattenException(ex))));
                }
            });
        }

        private void CreateServer()
        {
            var tcp = new TcpListener(IPAddress.Any,int.Parse( port));
            tcp.Start();
            File.WriteAllText("START" + "_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".txt", "START");
            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    File.WriteAllText("ACCEPT" + "_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".txt", "ACCEPT");
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        NewMessage(tcpClient);
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }



        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }



        private void NewMessage(TcpClient tcpClient)
        {
            try
            {
                NetworkStream stream = tcpClient.GetStream();
                string incomming;
                // int streamEnd = Convert.ToInt32(stream.Length);
                string request = "";
                StringBuilder sb = new StringBuilder();

                
                    while (!stream.DataAvailable)
                    {
                        Thread.Sleep(20);
                    }

                    if (stream.DataAvailable && stream.CanRead)
                    {
                        Byte[] data = new Byte[1024];
                        List<byte> allData = new List<byte>();

                        do
                        {
                            int numBytesRead = stream.Read(data, 0, data.Length);

                            if (numBytesRead == data.Length)
                            {
                                allData.AddRange(data);
                            }
                            else if (numBytesRead > 0)
                            {
                                allData.AddRange(data.Take(numBytesRead));
                            }
                        } while (stream.DataAvailable);
                    request=Encoding.Default.GetString(allData.ToArray());
                    }
               
                incomming = request;
                File.WriteAllText("SEND"  + "_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".txt", incomming);
                var index = incomming.IndexOf("|");
                var type = incomming.Substring(0, index);
                var message = incomming.Substring(index + 1, incomming.Length - index - 1);
                byte[] result=new byte[] {};
                switch (type)
                {
                    case "1":
                        result = SendUploadEFactura(message);
                        break;

                    case "2":
                        result = GeStareEFactura(message);
                        break;
                    case "3":
                        result = GeStareDetaliuEFactura(message);
                        break;
                }


                File.WriteAllBytes("rezultat" + type + "_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".txt", result);

               
                stream.Write(result, 0, result.Length);
                tcpClient.Close();
            }
            catch(Exception ex)
            {
                File.WriteAllText("erorr" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".txt", FlattenException( ex));
            }
        }

        private byte[] SendUploadEFactura(String message)
        {
            var result = "";
            try
            {

                File.WriteAllText("SENFACTURA" + "_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + ".txt", message);
                DataSet ds = new DataSet();
                try
                {
                     
                    ds.ReadXml(new StringReader(message), XmlReadMode.InferSchema);
                }
                catch (Exception ex)
                {
                    return System.Text.Encoding.ASCII.GetBytes(  "Fisier incomplet");
                }
                var cif=ds.Tables["PartyLegalEntity"].Rows[0]["CompanyId"].ToString().Replace("RO", "");
                var file = System.Text.Encoding.ASCII.GetBytes(message);
                var url = adresaAnaf + "upload?standard=UBL&cif="+cif;
                MyClient client = new MyClient();
                try
                {
                    byte[] data = client.UploadData(url, file);
                    String resultr = Encoding.Default.GetString((data.ToArray()));
                    result = resultr;
                }
                catch (Exception ex)
                {
                    result = FlattenException(ex) + " " + url + " " + client.Certificates;
                }
            }
            catch (Exception ex)
            {
                result = "Eroare" + FlattenException(ex);
            }

            return   System.Text.Encoding.ASCII.GetBytes(result);
        }


        private String DownloadData(String metoda)
        {

            try
            {
                var result = DownloadDataBytes(metoda);
                return Encoding.Default.GetString((result.ToArray()));
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        private byte[] DownloadDataBytes(String metoda)
        {
            byte[] result = null;
            try
            {

              
                var url = adresaAnaf + metoda;
                MyClient client = new MyClient();
                try
                {
                    result = client.DownloadData(url);
                     
                }
                catch (Exception ex)
                {
                    throw new Exception(  ex.Message + " " + client.Certificates);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Eroare" + FlattenException(ex));
            }

            return result;
        }

        private byte[] GeStareEFactura(String id)
        {
            
            return System.Text.Encoding.ASCII.GetBytes(  DownloadData("stareMesaj?id_incarcare=" + id)); 
        }

        private byte[] GeStareDetaliuEFactura(String id)
        {

            return DownloadDataBytes("descarcare?id=" + id);
        }


        public static string FlattenException(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }

            return stringBuilder.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
          //  MessageBox.Show(SendUploadEFactura(File.ReadAllText(textBox1.Text)));
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
          File.WriteAllBytes("xxx"+DateTime.Now.ToString("ddMMMyyy_HHmmss")+".zip", DownloadDataBytes("descarcare?id=" +   textBox1.Text));
        }

        private void button4_Click(object sender, EventArgs e)
        {
           var incomming = File.ReadAllText("SEND_30_07_2022_00_01_44.txt");
            var index = incomming.IndexOf("|");
            var type = incomming.Substring(0, index);
            var message = incomming.Substring(index + 1, incomming.Length - index - 1);
            MessageBox.Show(message);
        }
    }


    public class MyClient : WebClient
    {
        public String Certificates { get; set; }
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
          ((HttpWebRequest)  request).UserAgent = "Dotnet";
            //DigiSign

            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            store.Open(OpenFlags.ReadOnly);
 
            var cert = store.Certificates.Cast<X509Certificate2>().ToList().Where(p => p.Issuer.Contains("DigiSign")).First();
    
            bool result = cert.Verify();
            Certificates += Environment.NewLine + " Cert:" + cert.Issuer + " " + cert.Subject + " " + cert.GetExpirationDateString() + " PRIVATE:" + cert.HasPrivateKey + ",Verify:" + result;

            (request as HttpWebRequest).ClientCertificates.Add(cert);

            return request;
        }
    }



}
