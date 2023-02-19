using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Servidor
{
    internal class Program
    {
        private static string url = "http://*:1237/";
        private static List<string> items = new List<string>();
        private static void ServidorSocket(int porta)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, porta);
            listener.Start();
            Console.WriteLine(">> ESPERANDO CONEXÃO SOCKET.");

            TcpClient cliente = listener.AcceptTcpClient();
            Console.WriteLine(">> CLIENTE SOCKET CONECTADO.");

            NetworkStream ns = cliente.GetStream();

            byte[] bytesRecebidos = new byte[256];
            int bytesLidos = ns.Read(bytesRecebidos,0, bytesRecebidos.Length);

            items.Add(Encoding.ASCII.GetString(bytesRecebidos,0, bytesLidos));
            Serializador.Serializa(items);

            ns.Close();
            cliente.Close();
            listener.Stop();
        }
        private static void ServidorWeb()
        {
            byte[] dados = new byte[0];
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine(">> WEB INICIADO.");

            // Corpo server
            bool continuar = true;
            while (continuar)
            {

                // Conexão
                HttpListenerContext conexao = listener.GetContext();
                Console.WriteLine(">> CONEXÃO FEITA NO SERVIDOR WEB.");
                Thread socket = new Thread(() => ServidorSocket(1236));
                

                HttpListenerRequest req = conexao.Request;
                HttpListenerResponse res = conexao.Response;

                if (req.HttpMethod == "GET")
                {
                    if (req.Url.AbsolutePath == "/home")
                    {
                        dados = Encoding.UTF8.GetBytes("<!DOCTYPE html>" + "<html>" + "<head>" + "<title>SERVER WEB</title>" + "<meta charset=\"utf-8\"/>" + "</head>" + "<body>" + "<form action=\"enviar_item\" method=\"post\">" + "<input type=\"text\" name=\"item\">" + "<input type=\"submit\">" + "</form>" + "</body>" + "</html>");
                    }
                    else
                    {
                        dados = Encoding.UTF8.GetBytes("<!DOCTYPE html>" + "<html>" + "<head>" + "<title>SERVER WEB</title>" + "<meta charset=\"utf-8\"/>" + "</head>" + "<body>" + "<h1>404</h1>" + "</body>" + "</html>");
                    }
                }
                else if (req.HttpMethod == "POST")
                {
                    if(req.Url.AbsolutePath == "/enviar_item")
                    {
                        string dadosPost = (new StreamReader(req.InputStream, req.ContentEncoding)).ReadToEnd();
                        NameValueCollection nvc = HttpUtility.ParseQueryString(dadosPost);

                        socket.Start();

                        TcpClient sCliente = new TcpClient("192.168.1.5",1236);
                        NetworkStream ns = sCliente.GetStream();
                        byte[] envioItem = Encoding.ASCII.GetBytes(nvc["item"]);
                        ns.Write(envioItem, 0, envioItem.Length);

                        

                        sCliente.Close();
                        ns.Close();
                    }
                }

                res.ContentType = "text/html";
                res.ContentEncoding = Encoding.UTF8;
                res.ContentLength64 = dados.LongLength;
                res.OutputStream.WriteAsync(dados, 0, dados.Length);
                res.Close();
            }
            listener.Close();
        }
        static void Main(string[] args)
        {
            Thread web = new Thread(() => ServidorWeb());
            web.Start();
        }
    }
}
