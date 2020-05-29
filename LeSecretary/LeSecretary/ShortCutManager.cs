using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace LeSecretary
{
    public class ShortCutManager
    {
        private string JsonPath
        {
            get
            {
                string totPath =Path.Combine(Application.StartupPath, "shortCuts.json");
                
                if (!File.Exists(totPath)) throw new Exception("shortCuts.json 파일이 실행 파일 디렉토리에 없습니다.");
                return totPath;
            }
        }
        
        public ShortCutModel GetShortCutData()
        {
            string jsonStr = File.ReadAllText(JsonPath);
            ShortCutModel scm = JsonConvert.DeserializeObject<ShortCutModel>(jsonStr);
            return scm;
        }
    }
}