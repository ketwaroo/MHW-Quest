using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.IO;

namespace Custom_Quest_Editor
{
    public partial class MainWindow : Window
    {

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const int ATTACH_PARENT_PROCESS = -1;

        readonly static IntPtr handle = GetConsoleWindow();
        [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)] static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("kernel32.dll", SetLastError = true)] static extern bool AttachConsole(int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)] static extern bool AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true)] static extern bool FreeConsole();
        public static void HideConsoleWindow()
        {
            ShowWindow(handle, SW_HIDE); //hide the console
        }
        public static void ShowConsoleWindow()
        {
            ShowWindow(handle, SW_SHOW); //show the console
        }
        public void Hide()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length < 2) // cli operation
            {
                HideConsoleWindow();
            }
            base.Hide();
        }

        private void HandleConsole(string[] args)
        {


            //if (!AttachConsole(ATTACH_PARENT_PROCESS))
            //{
            //    AllocConsole();

            //}
            //ShowConsoleWindow();

            IDictionary<string, string> namedArgs = CliStuff.GetNamedArgsAsDictionary(args);

            if (namedArgs.ContainsKey("-help")) {

                Console.WriteLine("Monster Hunter World Quest Editor CLI Fork");
                Console.WriteLine("author: Ketwaroo D. Yaasir");
                Console.WriteLine("Warning: may fuck shit up.");
                Console.WriteLine("Usage: ");
                Console.WriteLine(" Export: `\"Custom Quest Editor.exe\" -open <path to .mib file> -export <path to json file> -save <path to other mib file>`");
                Console.WriteLine(" Export (saves to same file): `\"Custom Quest Editor.exe\" -open <path to .mib file> -export <path to json file>`");
                Console.WriteLine(" Import: `\"Custom Quest Editor.exe\" -open <path to .mib file> -import <path to json file>`");
                //Console.WriteLine("   If a different QuestID is passed in the import json data, \n" +
                //                  "   this will override the exported filename to matches QuestID. \n" +
                //                  "   The `-open` file will be use as base for the new saved file");
                Close();

            }

            if (
                namedArgs.TryGetValue("-open", out string openFile) == false
                || openFile.Length == 0
                || false == File.Exists(openFile)
                )
            {

                throw new Exception("-open value is required and should be an existing file.");
            }

            loadData(openFile);

            if (namedArgs.ContainsKey("-export") && namedArgs["-export"].Length > 0) {

                ExportData(namedArgs["-export"]);
            
            } 
            else if (namedArgs.ContainsKey("-import") && namedArgs["-import"].Length > 0) {

                ImportData(namedArgs["-import"]);

                if (namedArgs.ContainsKey("-save")) {
                    saveData(openFile, namedArgs["-save"]);
                }
                saveData(openFile);
            }

            //FreeConsole();
            Close();
            // Environment.Exit(0);

        }

    }


    public static class CliStuff
    {


        // stolen from https://stackoverflow.com/a/74812586/912087
        public static IDictionary<string, string> GetNamedArgsAsDictionary(string[] args)
        {
            var dict = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            for (int i = 0; i < args.Length; i++)
                if (args[i].StartsWith("-"))
                {
                    if (i + 1 >= args.Length || args[i + 1].StartsWith("-"))
                        dict.Add(args[i], string.Empty);
                    else
                        dict.Add(args[i], args[++i]);
                }

            return dict;
        }
    }
}
