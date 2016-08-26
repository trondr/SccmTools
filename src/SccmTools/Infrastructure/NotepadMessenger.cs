using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NCmdLiner;

namespace SccmTools.Infrastructure
{
    /// <summary>
    /// Output Help, License and Credits to file instead of to the console and show it in the default text file editor
    /// </summary>
    public class NotepadMessenger : IMessenger
    {
        private StreamWriter Sw
        {
            get
            {
                if (_sw == null)
                {
                    File.Move(_tempFileName, _tempTextFileName);
                    _sw = new StreamWriter(_tempTextFileName);
                }
                return _sw;
            }
        }
        private StreamWriter _sw;
        private readonly string _tempTextFileName;
        public object Sync = new object();
        private readonly string _tempFileName;

        public NotepadMessenger()
        {
            _tempFileName = Path.GetTempFileName();
            _tempTextFileName = _tempFileName + ".txt";
        }

        ~NotepadMessenger()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (_sw != null)
            {
                lock (Sync)
                {
                    if (_sw != null)
                    {
                        _sw.Close();
                        _sw.Dispose();
                        _sw = null;
                    }
                }
            }
            if (File.Exists(_tempTextFileName))
            {
                File.Delete(_tempTextFileName);
            }
        }

        public void Write(string formatMessage, params object[] args)
        {
            this.Sw.Write(formatMessage.Replace("\r\n", "\n").Replace("\n", Environment.NewLine), args);
        }

        public void WriteLine(string formatMessage, params object[] args)
        {
            this.Sw.WriteLine(formatMessage.Replace("\r\n", "\n").Replace("\n", Environment.NewLine), args);
        }

        public void Show()
        {
            this.Sw.Close();
            if (Environment.UserInteractive)
            {
                Process.Start(_tempTextFileName);
                Thread.Sleep(2000);                
            }
            else
            {
                using (var sr = new StreamReader(_tempTextFileName))
                {
                    System.Console.WriteLine(sr.ReadToEnd());
                }
            }
            Cleanup();
        }
    }
}