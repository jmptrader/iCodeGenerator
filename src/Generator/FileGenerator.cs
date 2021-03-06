using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using iCodeGenerator.DatabaseStructure;

namespace iCodeGenerator.Generator
{
	/// <summary>
	/// Summary description for FileGenerator.
	/// </summary>
	public class FileGenerator
	{
		private IDictionary _CustomValue = null;

		public IDictionary CustomValue
		{
			get { return _CustomValue; }
			set { _CustomValue = value; }
		}

		public FileGenerator()
		{
		}

		public event EventHandler OnComplete;
		protected void CompleteNotifier(EventArgs e)
		{
			if (OnComplete != null)
			{
				OnComplete(this, e);
			}
		}
		
		public void Generate(Table table,string inputDir, string outputDir)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(inputDir);
			Client client = new Client();
            //string originalSD = client.StartDelimiter;
            //string originalED = client.EndingDelimiter;
			if(_CustomValue != null)
			{
				client.CustomValues = _CustomValue;	
			}			
			foreach(FileInfo fileInfo in directoryInfo.GetFiles())
			{
                //client.StartDelimiter = originalSD;
                //client.EndingDelimiter = originalED;
				StreamReader sr = File.OpenText(fileInfo.FullName);
				string fileContent = sr.ReadToEnd();
				sr.Close();
				string codeGenerated = client.Parse(table,fileContent);
				
                //client.StartDelimiter = String.Empty;
                //client.EndingDelimiter = String.Empty;
                string dirname = client.Parse(table, directoryInfo.Name);
				string filename = client.Parse(table,fileInfo.Name);
                //change back Delimiters immediately (subsequent calls to Generate() fail if you don't do so)
                //client.StartDelimiter = originalSD;
                //client.EndingDelimiter = originalED;

                String fullDir = outputDir + Path.DirectorySeparatorChar + dirname;

				try
				{
                    //StreamWriter sw = new StreamWriter(outputDir + Path.DirectorySeparatorChar + filename);

                    if (Directory.Exists(fullDir) == false)
                    {
                        Directory.CreateDirectory(fullDir);
                    }

                    StreamWriter sw = new StreamWriter(fullDir + Path.DirectorySeparatorChar + filename);
					sw.Write(codeGenerated);
					sw.Flush();
					sw.Close();
				}
				catch (Exception e)
				{
					Debug.WriteLine(e);
				}
			}
            //CompleteNotifier(new EventArgs());
		}

        public void Generate(Table[] tables, string inputDir, string baseOutputDir)
        {
            foreach (Table table in tables)
            {
                String specificOutputDir = baseOutputDir;// +Path.DirectorySeparatorChar + table.Name;
                //if (Directory.Exists(specificOutputDir) == false)
                //{
                //    Directory.CreateDirectory(specificOutputDir);
                //}

                Generate(table, inputDir, specificOutputDir);
            }
            CompleteNotifier(new EventArgs());
        }
	}
}
