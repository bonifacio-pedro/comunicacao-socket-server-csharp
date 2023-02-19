using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Servidor
{
    public class Serializador
    {
        public static void Serializa(List<string> lista)
        {
            string caminho = @"D:\Pedro freela\Curso C#\Projetos pessoais\Comunicação socket\Items\items.xml";
            XmlSerializer escritor = new XmlSerializer(typeof(List<string>));

            if (File.Exists(caminho))
            {
                FileStream fs = File.Open(caminho, FileMode.Open);
                escritor.Serialize(fs, lista);
                fs.Close();
            }
            else
            {
                FileStream fs = File.Create(caminho);
                escritor.Serialize(fs, lista);
                fs.Close();
            }
        }
    }
}
