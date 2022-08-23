using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MorphClocks {
  
  [Serializable]
  public class AppSettings {
    
    private static readonly Lazy<AppSettings> instanceField = new Lazy<AppSettings>(Load);

    public static AppSettings Instance => instanceField.Value;

    private AppSettings() {
      FontName = "Segoe print";
      FontSize = 48;
      TextColor = Color.Transparent;
      BackColor = Color.Transparent;
      LineColor = Color.FromArgb(-7237121);
    }

    [XmlAttribute] 
    public string FontName;
    
    [XmlAttribute] 
    public float FontSize;
    
    [XmlIgnore] 
    public Color TextColor;

    [XmlAttribute]
    public int TextColorValue {
      get => TextColor.ToArgb();
      set => TextColor = Color.FromArgb(value);
    }

    [XmlIgnore] public Color BackColor;

    [XmlAttribute]
    public int BackColorValue {
      get => BackColor.ToArgb();
      set => BackColor = Color.FromArgb(value);
    }

    [XmlIgnore] public Color LineColor;

    [XmlAttribute]
    public int LineColorValue {
      get => LineColor.ToArgb();
      set => LineColor = Color.FromArgb(value);
    }

    [XmlAttribute] public bool MixPoint;
    [XmlAttribute] public bool Move3D;
    [XmlAttribute] public bool DrawCircle;
    [XmlAttribute] public bool LockOnExit;
    [XmlAttribute] public bool PrimaryDisplayOnly;

    #region Load/Save

    private static string GetSettingsFilePath() {
      return Path.ChangeExtension(Application.ExecutablePath, ".xml");
    }

    public void Save() {
      var fileName = GetSettingsFilePath();
      GC.Collect(GC.MaxGeneration);
      Extensions.SerializeToFile(fileName, this);
    }

    public static AppSettings Load() {
      var fileName = GetSettingsFilePath();
      try {
        if (File.Exists(fileName)) {
          var result = Extensions.DeserializeFile<AppSettings>(fileName);
          if (result != null)
            return result;
          Console.WriteLine("Error loading settings file");
        }
        else
          Console.WriteLine("Settings file not found in '{0}'", fileName);
      }
      catch (Exception ex) {
        Console.WriteLine("Error parsing settings file: {0}", ex.Message);
      }

      Console.WriteLine("Default settings applied");
      return new AppSettings();
    }

    #endregion Load/Save
  }
}