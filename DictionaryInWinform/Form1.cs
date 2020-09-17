using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace DictionaryInWinform
{
    public partial class Form1 : Form
    {
        MyDictionary m = new MyDictionary();
        WordPairs model = new WordPairs();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Clear();
            PopulateDataGridView();
            richTextBox1.AllowDrop = true;
            richTextBox1.DragDrop += RichTextBox1_DragDrop;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }
        void Clear()
        {
            txtEng.Text = txtHun.Text = "";
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
            model.Id = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtEng.Text!="")
            {
                model.Eng = txtEng.Text.Trim();
                model.Hun = txtHun.Text.Trim();

                using (DBModels db = new DBModels())
                {
                    if (model.Id == 0)//Insert
                    {
                        db.WordPairs.Add(model);
                    }
                    else//update
                    {
                        db.Entry(model).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                Clear();
                PopulateDataGridView();
                MessageBox.Show("Submitted Succesfully!");
            }
            
        }
        void PopulateDataGridView()
        {
            using (DBModels db = new DBModels())
            {
                dataGridDictionary.DataSource = db.WordPairs.ToList<WordPairs>();
            }
        }

        //double click event
        private void dataGridDictionary_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridDictionary.CurrentRow.Index != -1)
            {
                model.Id = Convert.ToInt32(dataGridDictionary.CurrentRow.Cells["Id"].Value);
                using (DBModels db = new DBModels())
                {
                    model = db.WordPairs.Where(x => x.Id == model.Id).FirstOrDefault();
                    txtEng.Text = model.Eng;
                    txtHun.Text = model.Hun;
                }
                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            using (DBModels db = new DBModels())
            {
                var entry = db.Entry(model);
                if (entry.State == EntityState.Detached)
                    db.WordPairs.Attach(model);
                db.WordPairs.Remove(model);
                db.SaveChanges();
                PopulateDataGridView();
                Clear();
                // MessageBox.Show("Deleted Succesfully!");
            }


        }
        private void RichTextBox1_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                string[] fileNames = data as string[];

                if (fileNames.Length > 0)
                    richTextBox1.LoadFile(fileNames[0], RichTextBoxStreamType.PlainText);
            }
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length != 0)
            {
                //copy words from richTextBox
                string sub = "";
                foreach (var item in richTextBox1.Text)
                {
                    sub += item;
                }

                //remove numbers and special characters
                string subWithoutSpecChars = Regex.Replace(sub, "[^a-zA-Z ]+", "");
                //append the subtitles words to array in lowercase
                string[] splitedSub = subWithoutSpecChars.Split(' ');
                //remove duplicates
                string[] resultSub = splitedSub.Distinct().ToArray();
                Array.Sort(resultSub);

                for (int i = 0; i < resultSub.Length; i++)
                {
                    if (resultSub[i]!="")
                    {
                        model.Eng = resultSub[i].ToLower();
                        model.Hun = "";

                        using (DBModels db = new DBModels())
                        {
                            //check if value already exists in database
                            bool IsDataExists = db.WordPairs.Any(c => c.Eng == model.Eng);

                            if (IsDataExists == false)
                            {
                                //insert
                                db.WordPairs.Add(model);
                            }
                            db.SaveChanges();
                        }
                        Clear();
                        PopulateDataGridView();
                    }
                }
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void btnSettings_Click(object sender, EventArgs e)
        {
            btnCancel.Visible = false;
            btnDelete.Visible = false;
            btnSave.Visible = false;
            btnCancel.Visible = false;
            dataGridDictionary.Visible = false;
            btnSaveAll.Visible = false;
            btnReset.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            txtEng.Visible = false;
            txtHun.Visible = false;
            richTextBox1.Visible = false;
            btnSettings.Visible = false;
            btnBack.Visible = true;
            lblPath.Visible = true;
            textBoxPath.Visible = true;
            label3.Visible = true;
            btnTxt.Visible = true;
            btnJson.Visible = true;
            btnXml.Visible = true;
            lblFileName.Visible = true;
            txtFileName.Visible = true;

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            btnCancel.Visible = true;
            btnDelete.Visible = true;
            btnSave.Visible = true;
            btnCancel.Visible = true;
            dataGridDictionary.Visible = true;
            btnSaveAll.Visible = true;
            btnReset.Visible = true;
            label1.Visible = true;
            label2.Visible = true;
            txtEng.Visible = true;
            txtHun.Visible = true;
            richTextBox1.Visible = true;
            btnSettings.Visible = true;
            btnBack.Visible = false;
            lblPath.Visible = false;
            textBoxPath.Visible = false;
            label3.Visible = false;
            btnTxt.Visible = false;
            btnJson.Visible = false;
            btnXml.Visible = false;
            lblFileName.Visible = false;
            txtFileName.Visible = false;
        }
        
        private void btnTxt_Click(object sender, EventArgs e)
        {
            btnExportTxt.Visible = true;
            btnExportJson.Visible = false;
            btnExportXml.Visible = false;
            textBoxPath.Text = @"C:\MyDictionary\";
            textBoxPath.Text = $@"{textBoxPath.Text}{txtFileName.Text}{btnTxt.Text}";
        }
        private void btnJson_Click(object sender, EventArgs e)
        {
            btnExportJson.Visible = true;
            btnExportTxt.Visible = false;
            btnExportXml.Visible = false;
            textBoxPath.Text = @"C:\MyDictionary\";
            textBoxPath.Text = $@"{textBoxPath.Text}{txtFileName.Text}{btnJson.Text}";
        }

        private void btnXml_Click(object sender, EventArgs e)
        {
            btnExportXml.Visible = true;
            btnExportJson.Visible = false;
            btnExportTxt.Visible = false;
            textBoxPath.Text = @"C:\MyDictionary\";
            textBoxPath.Text = $@"{textBoxPath.Text}{txtFileName.Text}{btnXml.Text}";
        }


        private void btnExportTxt_Click(object sender, EventArgs e)
        {
            m.ExportAsTxt(textBoxPath);
        }
        private void btnExportJson_Click(object sender, EventArgs e)
        {
            m.SerializeFile(textBoxPath);
        }
        private void btnExportXml_Click(object sender, EventArgs e)
        {
            m.SerializeFile(textBoxPath);
        }
    }
}
