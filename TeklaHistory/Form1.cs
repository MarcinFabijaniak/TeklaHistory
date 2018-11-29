using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using Tekla.Structures;
using Tekla.Structures.Model;
using Tekla.Structures.Model.History;

using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;

namespace TeklaHistory
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Run_GetModifications();
        }

        private void Run_GetModifications()
        {
            Model CurrentModel = new Model();
            //var x = Tekla.Structures.Model.History.ModelHistory.GetModifications("x").Deleted;

            if (CurrentModel.GetConnectionStatus())
            {
                MessageBox.Show(CurrentModel.GetProjectInfo().Designer);
                
                var Modifications = Tekla.Structures.Model.History.ModelHistory.TakeModifications("x");

                while (Modifications.Modified.MoveNext())
                {
                    ModelObject mo = Modifications.Modified.Current;
                    Console.WriteLine("Run_TakeModifications: Modified id: {0}", mo.Identifier.GUID);
                }               
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var stopwatch_insertion = new Stopwatch();
            stopwatch_insertion.Start();

            var model = new Model();
            var parts = model.GetAllParts(true);

            int i = 1;
            string filePath = "C:\\Users\\mfabijaniak\\Desktop\\SQLite\\1722_parts.sqlite";

            if (File.Exists(filePath))
            {
                
            }
            else
            {
                //SQLite create a database file
                SQLiteConnection.CreateFile(filePath);

                //SQLite create table with columns
                CreateTableSQLite(filePath);
            }   

            //SQLite establish connection to file
            string cmd = "Data Source=" + filePath + ";Version = 3; ";
            SQLiteConnection connection = new SQLiteConnection(cmd);
            SQLiteCommand command = connection.CreateCommand();
            connection.Open();
            SQLiteTransaction transaction = connection.BeginTransaction();

            command.CommandText = "INSERT OR IGNORE INTO parts " + "(guid, name, type) " + "VALUES (@guid, @name, @type)";

            command.Parameters.AddWithValue("@guid", "");
            command.Parameters.AddWithValue("@name", "");
            command.Parameters.AddWithValue("@type", "");

            foreach (var part in parts)
            {
                //Console.WriteLine("Part Line #: " + i + " GUID: " + part.Identifier.GUID + " Name: " + part.Name);
                InsertResultItem(part.Identifier.GUID.ToString(), part.Name, part.GetType().Name, command);
                i++;
            }
            transaction.Commit();
            command.Dispose();
            connection.Dispose();

            //time elapsed insertion
            stopwatch_insertion.Stop();
            var elapsed_time_insertion = stopwatch_insertion.Elapsed.TotalSeconds;

            var stopwatch_console = new Stopwatch();
            stopwatch_console.Start();
            ReadFromSQLite("parts", filePath);
            stopwatch_console.Stop();
            var elapsed_time_console = stopwatch_console.Elapsed.TotalSeconds;

            Console.WriteLine("Elapsed Time To Insert to SQlite in Seconds: " + elapsed_time_insertion + " Number of records inserted: " + i);
            Console.WriteLine("Elapsed Time To Console in Seconds: " + elapsed_time_console);

            double total_elapsed_time = elapsed_time_insertion + elapsed_time_console;

            Console.WriteLine("Total Elapsed Time in Seconds: " + total_elapsed_time);
        }

        public int InsertResultItem(string guid, string name, string type, SQLiteCommand command)
        {        
            command.Parameters["@guid"].Value = guid;
            command.Parameters["@name"].Value = name;
            command.Parameters["@type"].Value = type;

            return command.ExecuteNonQuery();
        }
        

        private void CreateTableSQLite(string filePath)
        {
            SQLiteConnection m_dbConnection;
            string cmd = "Data Source=" + filePath + ";Version = 3; ";

            m_dbConnection = new SQLiteConnection(cmd);
            m_dbConnection.Open();

             string sql = "CREATE TABLE parts (guid VARCHAR(50), name VARCHAR(20), type VARCHAR(20))";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        private void WriteToSQLite(string filePath)
        {
            SQLiteConnection m_dbConnection;
            string cmd = "Data Source=" + filePath + ";Version = 3; ";

            m_dbConnection = new SQLiteConnection(cmd);
            m_dbConnection.Open();

            string sql = "insert into parts (guid, namecore) values ('Me', 3000)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

        }

        private void ReadFromSQLite(string type, string filePath)
        {
            if (type == "parts")
            {
                int i = 1;
                SQLiteConnection m_dbConnection;
                string cmd = "Data Source=" + filePath + ";Version = 3; ";

                m_dbConnection = new SQLiteConnection(cmd);
                m_dbConnection.Open();

                string sql = "select * from parts";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("Line #: " + i + " GUID: " + reader["guid"] + " Name: " + reader["name"] + " Type: " + reader["type"]);
                    i++;
                }
                
            }
            else if (type == "assemblies")
            {
            }
            else if (type == "connections")
            {

            }
            else if (type == "castunits")
            {

            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var model = new Model();
            var assemblies = model.GetAllAssemblies(true);

            stopwatch.Stop();
            var elapsed_time = stopwatch.Elapsed.TotalSeconds;

            int i = 1;

            foreach (var assembly in assemblies)
            {
                Console.WriteLine("Assembly Line #: " + i + " GUID: " + assembly.Identifier.GUID + " Name: " + assembly.Name);
                i++;
            }

            Console.WriteLine("Elapsed Time in Seconds: " + elapsed_time);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var model = new Model();
            var connections = model.GetAllConnections(true);

            stopwatch.Stop();
            var elapsed_time = stopwatch.Elapsed.TotalSeconds;

            int i = 1;

            foreach (var connection in connections)
            {
                Console.WriteLine("Connection Line #: " + i + " GUID: " + connection.Identifier.GUID + " Name: " + connection.Name);
                i++;
            }

            Console.WriteLine("Elapsed Time in Seconds: " + elapsed_time);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string fileNameGlobal = "";
            comboBox1.Items.Clear();
            checkedListBox1.Items.Clear();

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileNameGlobal = openFileDialog.FileName;

                string[] desiredRange = File.ReadLines(@fileNameGlobal).Skip(45).Take(762).ToArray();
                string[] desiredRangeNoEmptyLines = desiredRange.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                string[] desiredLines = desiredRangeNoEmptyLines.Where(l => !l.Contains("//")).ToArray();

                string[,] finishedLines = new string[desiredLines.Length, 9];

                for (int i = 0; i < desiredLines.Length; i++)
                {
                    // removes redundant whitespaces
                    string removedWhitespaces = Regex.Replace(desiredLines[i], "\\s+", " ");
                    // splits the string
                    string[] splitArray = removedWhitespaces.Split(' ');

                    finishedLines[i, 0] = splitArray[0].ToString();//Name
                    comboBox1.Items.Add(finishedLines[i, 0]);
                    checkedListBox1.Items.Add(finishedLines[i, 0]);

                    finishedLines[i, 1] = splitArray[1].ToString();//Datatype
                    finishedLines[i, 2] = splitArray[2].ToString();//Justify
                    finishedLines[i, 3] = splitArray[3].ToString();//Cacheable

                    //they may be empty so check if exists
                    var exists4 = splitArray.ElementAtOrDefault(4) != null;
                    if (exists4)
                    {
                        finishedLines[i, 4] = splitArray[4].ToString();//Length
                    }

                    var exists5 = splitArray.ElementAtOrDefault(5) != null;
                    if (exists5)
                    {
                        finishedLines[i, 5] = splitArray[5].ToString();//Decimals
                    }

                    var exists6 = splitArray.ElementAtOrDefault(6) != null;
                    if (exists6)
                    {
                        finishedLines[i, 6] = splitArray[6].ToString();//Unit type
                    }

                    var exists7 = splitArray.ElementAtOrDefault(7) != null;
                    if (exists7)
                    {
                        finishedLines[i, 7] = splitArray[7].ToString();//Unit
                    }

                    var exists8 = splitArray.ElementAtOrDefault(8) != null;
                    if (exists8)
                    {
                        finishedLines[i, 8] = splitArray[8].ToString();//Precision
                    }

                    Console.WriteLine("Line #: " + i +  "  Name: " + finishedLines[i, 0] + " Data type:" + finishedLines[i, 1]
                        + " Justify: " + finishedLines[i, 2] + " Cacheable: " + finishedLines[i, 3] + " Length: " + finishedLines[i, 4] + " Decimals: " + finishedLines[i, 5] + " Unit type: " + finishedLines[i, 6] 
                        + " Unit: " + finishedLines[i, 7] + " Precision: " + finishedLines[i, 8]);
                }                                
            }            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string fileNameUserdefined = "";
            comboBox1.Items.Clear();
            checkedListBox1.Items.Clear();

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileNameUserdefined = openFileDialog.FileName;

                string[] desiredRange = File.ReadLines(@fileNameUserdefined).Skip(53).Take(425).ToArray();
                string[] desiredRangeNoEmptyLines = desiredRange.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                string[] desiredLines = desiredRangeNoEmptyLines.Where(l => !l.Contains("//")).ToArray();

                string[,] finishedLines = new string[desiredLines.Length, 9];

                for (int i = 0; i < desiredLines.Length; i++)
                {
                    // removes redundant whitespaces
                    string removedWhitespaces = Regex.Replace(desiredLines[i], "\\s+", " ");
                    // splits the string
                    string[] splitArray = removedWhitespaces.Split(' ');

                    finishedLines[i, 0] = splitArray[0].ToString();//Name
                    comboBox1.Items.Add(finishedLines[i, 0]);
                    checkedListBox1.Items.Add(finishedLines[i, 0]);

                    finishedLines[i, 1] = splitArray[1].ToString();//Datatype
                    finishedLines[i, 2] = splitArray[2].ToString();//Justify
                    finishedLines[i, 3] = splitArray[3].ToString();//Cacheable

                    //they may be empty so check if exists
                    var exists4 = splitArray.ElementAtOrDefault(4) != null;
                    if (exists4)
                    {
                        finishedLines[i, 4] = splitArray[4].ToString();//Length
                    }

                    var exists5 = splitArray.ElementAtOrDefault(5) != null;
                    if (exists5)
                    {
                        finishedLines[i, 5] = splitArray[5].ToString();//Decimals
                    }

                    var exists6 = splitArray.ElementAtOrDefault(6) != null;
                    if (exists6)
                    {
                        finishedLines[i, 6] = splitArray[6].ToString();//Unit type
                    }

                    var exists7 = splitArray.ElementAtOrDefault(7) != null;
                    if (exists7)
                    {
                        finishedLines[i, 7] = splitArray[7].ToString();//Unit
                    }

                    var exists8 = splitArray.ElementAtOrDefault(8) != null;
                    if (exists8)
                    {
                        finishedLines[i, 8] = splitArray[8].ToString();//Precision
                    }

                    Console.WriteLine("Line #: " + i + "  Name: " + finishedLines[i, 0] + " Data type:" + finishedLines[i, 1]
                        + " Justify: " + finishedLines[i, 2] + " Cacheable: " + finishedLines[i, 3] + " Length: " + finishedLines[i, 4] + " Decimals: " + finishedLines[i, 5] + " Unit type: " + finishedLines[i, 6]
                        + " Unit: " + finishedLines[i, 7] + " Precision: " + finishedLines[i, 8]);
                }
            }
        }
    }
}
