using System.Collections.Generic;

namespace LeSecretary
{
    public class ShortCutModel
    {
        public IEnumerable<ShortCutItem> ShortCutList { get; set; }
    }

    public class ShortCutItem
    {
        public int id { get; set; }
        public string title { get; set; }
        public string modKey1 { get; set; }
        public string modKey2 { get; set; }
        public string modKey3 { get; set; }
        public string key { get; set; }
        public string cmd { get; set; }
        public string parameters { get; set; }
    }
}