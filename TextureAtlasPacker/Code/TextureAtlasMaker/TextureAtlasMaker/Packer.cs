using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json;
using System.IO.Compression;

namespace TextureAtlasMaker
{
    public partial class Packer : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hwc, IntPtr hwp);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(System.String className, System.String windowName);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        private SearchOption SEARCH_OPTION = SearchOption.TopDirectoryOnly;
        private string[] FileArray = new string[] { };
        private int debugCounter = 0;
        private string _INPUT_PATH = string.Empty;
        private string _DEPENDENCIES_PATH = string.Empty;
        private string _OUTPUT_PATH = string.Empty;
        private string _OUTPUT_FILENAME = string.Empty;

        private const string PACKER_EXE = "\\Packer.exe";
        private const string STREAMING_ASSETS = "\\Packer_Data\\StreamingAssets";
        private const string STREAMING_FILE = "\\Packer_Data\\StreamingAssets\\files.txt";

        public string INPUT_PATH
        {
            get
            {
                return _INPUT_PATH;
            }
            set
            {
                _INPUT_PATH = value;
                txtInputFolder.Text = value;
            }
        }
        public string DEPENDENCIES_PATH
        {
            get
            {
                return _DEPENDENCIES_PATH;
            }
            set
            {
                _DEPENDENCIES_PATH = value;
                txtDependencies.Text = value;
            }
        }
        public string OUTPUT_PATH
        {
            get
            {
                return _OUTPUT_PATH;
            }
            set
            {
                _OUTPUT_PATH = value;
                txtOutputFolder.Text = value;
            }
        }
        public string OUTPUT_FILENAME
        {
            get
            {
                return _OUTPUT_FILENAME;
            }
            set
            {
                _OUTPUT_FILENAME = value;
            }
        }
        
        //public string[] files = new string[] { };

        

        public Packer()
        {
            InitializeComponent();
            Debugger.lblError = lblErrorLog;
            Debugger.ClearDebugger();
        }

        private int Initialize()
        {
            //clear the debugger:
            Debugger.ClearDebugger();

            if (!Directory.Exists(INPUT_PATH))
            {
                Debugger.ErrorMessage += "Input path is not valid. \n";
                debugCounter += 1;
            }
            else
            {
                FileArray = Directory.GetFiles(INPUT_PATH, "*.png", SEARCH_OPTION);
                if (FileArray.Length == 0)
                {
                    Debugger.ErrorMessage += "No files to process. \n";
                    debugCounter += 1;
                }
            }

            if (!Directory.Exists(DEPENDENCIES_PATH))
            {
                Debugger.ErrorMessage += "Dependencies path is not valid. \n";
                debugCounter += 1;
            }
            else
            {
                //check for the exe and streaming assets folder:
                if (!File.Exists(DEPENDENCIES_PATH + PACKER_EXE))
                {
                    Debugger.ErrorMessage += "Packer.exe not found \n";
                    debugCounter += 1;
                }

                if (!Directory.Exists(DEPENDENCIES_PATH + STREAMING_ASSETS))
                {
                    Debugger.ErrorMessage += "Build folder not found \n";
                    debugCounter += 1;
                }

                if (File.Exists(DEPENDENCIES_PATH + STREAMING_FILE))
                {
                    try
                    {
                        File.Delete(DEPENDENCIES_PATH + STREAMING_FILE);
                    }
                    catch
                    {
                        Debugger.ErrorMessage += "Could not delete the following File : \n";
                        Debugger.ErrorMessage += DEPENDENCIES_PATH + STREAMING_FILE + "\n";
                        Debugger.ErrorMessage += "Please delete it manually. \n";
                        debugCounter += 1;
                    }
                }
            }

            if (!Directory.Exists(OUTPUT_PATH))
            {
                Debugger.ErrorMessage += "Output path is not valid. \n";
                debugCounter += 1;
            }

            if (txtFileName.Text == string.Empty)
            {
                Debugger.ErrorMessage += "Output filename is not valid. \n";
                debugCounter += 1;
            }
            else
            {
                OUTPUT_FILENAME = txtFileName.Text;
            }

            if (debugCounter > 0)
            {
                Debugger.UpdateConsole();
                return 0;
            }
            return 0;
        }

       
        //custom functions
        private int ProcessAtlas()
        {
            StreamWriter sw = new StreamWriter(DEPENDENCIES_PATH + STREAMING_FILE);
            InformationClass info = new InformationClass(OUTPUT_FILENAME, OUTPUT_PATH + "\\" + OUTPUT_FILENAME, FileArray); ;
            string json = JsonConvert.SerializeObject(info);
            sw.Write(json); 
            sw.Close();
            sw.Dispose();

            Process p = Process.Start(DEPENDENCIES_PATH + PACKER_EXE);
            Thread.Sleep(500);
            p.WaitForInputIdle();
            SetWindowPos(FindWindow(null, "packer"), 0, 0, 0, 0, 0, 0 * 0 == 0 ? 1 : 0);

            int SW_SHOW = 0;
            ShowWindow(p.MainWindowHandle, SW_SHOW);

            //SetParent(p.MainWindowHandle, this.Handle);

            p.WaitForExit();

            ZipFile.CreateFromDirectory(OUTPUT_PATH + "\\" + OUTPUT_FILENAME, OUTPUT_PATH + "\\"  + OUTPUT_FILENAME + ".zip");

            MessageBox.Show("Atlas created successfully");
            //Console.WriteLine("DONE makn atlas");
            return 0;
        }


        

        private void btnProcess_Click(object sender, EventArgs e)
        {
            debugCounter += Initialize();
            if (debugCounter == 0)
            {
                ProcessAtlas();
            }
        }

        private void btnInputFolder_Click(object sender, EventArgs e)
        {
            if (dlgFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                //check for the path:
                INPUT_PATH = dlgFolderBrowser.SelectedPath;
            }
        }

        private void btnDependenciesFolder_Click(object sender, EventArgs e)
        {
            if (dlgFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                //check for the path:
                DEPENDENCIES_PATH = dlgFolderBrowser.SelectedPath;
            }
        }

        private void btnOutputFolder_Click(object sender, EventArgs e)
        {
            if (dlgFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                //check for the path:
                OUTPUT_PATH = dlgFolderBrowser.SelectedPath;
            }
        }

        private void rdoTopDir_CheckedChanged(object sender, EventArgs e)
        {
            SEARCH_OPTION = SearchOption.TopDirectoryOnly;
        }

        private void rdoAllDir_CheckedChanged(object sender, EventArgs e)
        {
            SEARCH_OPTION = SearchOption.AllDirectories;
        }
    }

    public class InformationClass
    {
        public string FileName;
        public string Path;
        public string[] Files;

        public InformationClass()
        {

        }

        public InformationClass(string fName, string path, string[] files)
        {
            FileName = fName;
            Path = path;
            Files = files;
        }
    }
}
