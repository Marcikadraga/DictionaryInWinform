using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DictionaryInWinform
{
    [DataContract]
    public class MyDictionary
    {
        public SqlConnection Con { get; set; }
        public string SqlQuery { get; set; }
        public SqlCommand Command { get; set; }
        public string FilePath { get; set; }
        public SqlDataReader Reader { get; set; }
        public MyDictionary Serializer { get; set; }

        [DataMember]
        public Dictionary<string, string> MyDict { get; set; }

        public MyDictionary()
        {
            Con = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            SqlQuery = @"Select WordPairs.Eng,WordPairs.Hun From MyDictionary.dbo.WordPairs ;";
            Command = new SqlCommand(SqlQuery, Con);
        }
        public void CreateFolder()
        {
            string root = @"C:\MyDictionary";
            // If directory does not exist, create it. 
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
        }
        public void SerializeFile(TextBox textbox)
        {
            CreateFolder();
            
            Con.Open();
            Reader= Command.ExecuteReader();
            Serializer = new MyDictionary { MyDict = new Dictionary<string, string>(), FilePath = $@"{textbox.Text}" };
            while (Reader.Read())
            {
                Serializer.MyDict.Add(Reader.GetString(0), Reader.GetString(1));
            }
            if (textbox.Text.Contains(".json"))
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(Serializer.GetType());
                FileStream stream = File.Create(textbox.Text);
                jsonSerializer.WriteObject(stream, Serializer);
                stream.Close();
            }
            if (textbox.Text.Contains(".xml"))
            {
                DataContractSerializer xmlSerializer = new DataContractSerializer(Serializer.GetType());
                FileStream stream = File.Create($@"{textbox.Text}");
                xmlSerializer.WriteObject(stream, Serializer);
                stream.Close();
            }
            Con.Close();
        }
        public void ExportAsTxt(TextBox textBoxPath)
        {
            CreateFolder();
            using (var command = new SqlCommand(SqlQuery, Con))
            {
                Con.Open();
                using (var reader = command.ExecuteReader())
                {
                    string filePath = $@"{textBoxPath.Text}";
                    if (filePath == @"C:\MyDictionary\")
                    {
                        filePath = @"C:\MyDictionary\Dictionary.txt";
                    }
                    string wordpairs = "";
                    while (reader.Read())
                    {
                        if (reader.GetString(0)!="")
                        {
                            wordpairs += reader.GetString(0) + " - " + reader.GetString(1) + "\n";
                        }
                    }
                    File.WriteAllText(filePath, wordpairs);
                }
                Con.Close();
            }
        }
    }
}
