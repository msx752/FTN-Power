using System.Text.RegularExpressions;

namespace Fortnite.Core.ModifiedModels
{
    public class DailyLlama
    {
        private string _title;
        private string _description = "";

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                if (_title == null)
                {
                    Regex r = new Regex("\\[VIRTUAL\\](\\d+) x (.*) for (\\d+) MtxCurrency");
                    var m = r.Match(DevName);
                    if (m.Success)
                    {
                        _title = m.Groups[2].Value;
                    }
                }
            }
        }

        public string Description
        {
            get => _description; set
            {
                _description = value;
                if (_description == null)
                {
                    _description = "";
                }
            }
        }

        public short Price { get; set; }
        public short Amount { get; set; }
        public bool GameItem { get; set; }
        public string DevName { get; set; }
    }
}