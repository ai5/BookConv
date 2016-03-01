using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace BookConv
{
    public sealed class Settings
    {

        public string inputFIle;
        public string outputFile;
        public BookFormat bookFormat = BookFormat.Apery;

        private static Settings settings = new Settings();

        private Settings()
        {

        }

        public static string InputFile
        {
            get { return settings.inputFIle; }
            set { settings.inputFIle = value; }
        }

        public static string OutputFile
        {
            get { return settings.outputFile; }
            set { settings.outputFile = value; }
        }

        public static BookFormat BookFormat
        {
            get { return settings.bookFormat; }
            set { settings.bookFormat = value; }
        }

        public static string GetSettingsFolder()
        {
            string dir;

            dir = Path.GetDirectoryName(Application.ExecutablePath);

            return dir;
        }

        /// <summary>
        /// 保存
        /// </summary>
        public static void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            string dir = GetSettingsFolder();

            try
            {
                // カレントディレクトリに"settings.xml"というファイルで書き出す
                using (FileStream fs = new FileStream(dir + Path.DirectorySeparatorChar + "settings.xml", FileMode.Create))
                {
                    serializer.Serialize(fs, settings);
                }
            }
            catch (Exception )
            {
            }
        }

        /// <summary>
        /// 読み込み
        /// </summary>
        public static void Load()
        {
            string dir = GetSettingsFolder();

            // XmlSerializerオブジェクトを作成
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            try
            {
                // カレントディレクトリに"settings.xml"というファイルで書き出す
                using (FileStream fs = new FileStream(dir + Path.DirectorySeparatorChar + "settings.xml", FileMode.Open))
                {
                    settings = (Settings)serializer.Deserialize(fs);
                }
            }
            catch (Exception )
            {
            }
        }
    }
}
