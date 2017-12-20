using System;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;

namespace MorphClocks
{
    [Serializable]
    public class AppSettings
    {
        private static readonly Lazy<AppSettings> InstanceField = new Lazy<AppSettings>(Load);
        public static AppSettings Instance
        {
            get { return InstanceField.Value; }
        }

        private AppSettings()
        {
            FontName = "Vivaldi";
            TextColor = Color.CornflowerBlue;
            LineColor = Color.White;
        }

        [XmlAttribute] public string FontName;
        [XmlIgnore] public Color TextColor;
        [XmlAttribute]
        public int TextColorValue
        {
            get { return TextColor.ToArgb(); }
            set { TextColor = Color.FromArgb(value); }
        }

        [XmlIgnore] public Color LineColor;
        [XmlAttribute] public int LineColorValue
        {
            get { return LineColor.ToArgb(); }
            set { LineColor = Color.FromArgb(value); }
        }

        [XmlAttribute] public int WorkEnd;
        [XmlAttribute] public bool MixPoint;
        [XmlAttribute] public bool Move3D;
        [XmlAttribute] public bool BackTimer;
        [XmlAttribute] public bool DrawCircle;

        #region Load/Save

        private static string GetSettingsFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.ChangeExtension(AppDomain.CurrentDomain.FriendlyName, ".xml"));
        }

        public void Save()
        {
            var fileName = GetSettingsFilePath();
            GC.Collect(GC.MaxGeneration);
            XmlHelper.SerializeToFile(fileName, this);
        }

        public static AppSettings Load()
        {
            var fileName = GetSettingsFilePath();
            try
            {
                if (File.Exists(fileName))
                {
                    var result = XmlHelper.DeserializeFile<AppSettings>(fileName);
                    if (result != null)
                        return result;
                    Console.WriteLine("Error loading settings file");
                }
                else
                    Console.WriteLine("Settings file not found in '{0}'", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing settings file: {0}", ex.Message);
            }
            Console.WriteLine("Default settings applied");
            return new AppSettings();
        }

        #endregion Load/Save
    }
}