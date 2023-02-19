using System.Xml.Serialization;

namespace Interface_da_aplicação
{
    public partial class Form1 : Form
    {
        private string caminho = @"D:\Pedro freela\Curso C#\Projetos pessoais\Comunicação socket\Items\items.xml";
        private List<string> items;
        
        private void Deserializa(ref List<string> lista)
        {
            if (File.Exists(caminho))
            {
                XmlSerializer leitor = new XmlSerializer(typeof(List<string>));
                StreamReader sr = new StreamReader(caminho);
                List<string> listaTemp = (List<string>)leitor.Deserialize(sr);
                sr.Close();
                lista = listaTemp;
            }
            else
                lista = new List<string>();
        }

        private void AtualizaTabela()
        {
            items.Clear();
            Deserializa(ref items);
            dt.Rows.Clear();
            foreach(string item in items)
            {
                dt.Rows.Add(item);
            }
        }

        public Form1()
        {
            InitializeComponent();
            Deserializa(ref items);
            AtualizaTabela();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AtualizaTabela();
        }
    }
}