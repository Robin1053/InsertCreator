using HgSoftware.InsertCreator.Model;
using HgSoftware.InsertCreator.ViewModel;
using System;
using System.IO;
using System.Windows;

namespace HgSoftware.InsertCreator
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Logfield
        /// </summary>
        private static readonly log4net.ILog _log = LogHelper.GetLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            _log.Info("Startup App");

            string path = $"{Environment.GetEnvironmentVariable("userprofile")}/InsertCreator";
            string positionPath = $"{Environment.GetEnvironmentVariable("userprofile")}/InsertCreator/Position.Json";
            string corporateDesignPositionPath = $"{Environment.GetEnvironmentVariable("userprofile")}/InsertCreator/PositionCorporateDesign2026.Json";
            string bibleTextPositionPath = $"{Environment.GetEnvironmentVariable("userprofile")}/InsertCreator/BibleTextPosition.Json";

            PositionJsonReaderWriter positionDatajsonReaderWriter = new PositionJsonReaderWriter();

            PositionData positionData = new PositionData();
            CorporateDesignPositionData corporateDesignPositionData = new CorporateDesignPositionData();
            BiblewordPositionData biblewordPositionData = new BiblewordPositionData();

            _log.Info("Check InsertCreator Directory");
            if (!Directory.Exists(path))
            {
                _log.Info("Create InsertCreator Directory");
                Directory.CreateDirectory(path);
            }

            _log.Info("Check Position Data file");
            if (!File.Exists(positionPath))
            {
                _log.Info("Create Position Data file");
                var file = File.Create(positionPath, 1024);
                file.Close();
                positionDatajsonReaderWriter.WritePositionData(positionData, positionPath);
            }
            else
            {
                positionDatajsonReaderWriter.LoadPositionData(ref positionData, positionPath);
            }

            _log.Info("Check Corporate Design Position Data file");
            if (!File.Exists(corporateDesignPositionPath))
            {
                _log.Info("Create Corporate Design Position Data file");
                var file = File.Create(corporateDesignPositionPath, 1024);
                file.Close();
                positionDatajsonReaderWriter.WritePositionData(corporateDesignPositionData, corporateDesignPositionPath);
            }
            else
            {
                positionDatajsonReaderWriter.LoadPositionData(ref corporateDesignPositionData, corporateDesignPositionPath);
            }

            _log.Info("Check Bible Position Data file");
            if (!File.Exists(bibleTextPositionPath))
            {
                _log.Info("Create Bible Position Data file");
                var file = File.Create(bibleTextPositionPath, 1024);
                file.Close();
                positionDatajsonReaderWriter.WritePositionData(biblewordPositionData, bibleTextPositionPath);
            }
            else
            {
                positionDatajsonReaderWriter.LoadPositionData(ref biblewordPositionData, bibleTextPositionPath);
            }

            FadeInWriter fadeInWriter = new FadeInWriter(positionData, corporateDesignPositionData, biblewordPositionData);

            _log.Info("Create Ministry.json");
            FileCreate("Ministry.json", path);
            FileCreate("Insert.json", path);

            _log.Info("Load Images");
            fadeInWriter.ResetFade();

            var vm = new WindowViewModel(positionData, corporateDesignPositionData, biblewordPositionData);

            _log.Info("Open Window");
            var window = new MainWindow(vm);
            window.Show();
        }

        private void FileCreate(string filename, string path)
        {
            var fullpath = $"{path}/{filename}";

            if (!File.Exists(fullpath))
            {
                _log.Info($"Create {filename}");
                var file = File.Create(fullpath, 1024);
                file.Close();
            }
        }
    }
}