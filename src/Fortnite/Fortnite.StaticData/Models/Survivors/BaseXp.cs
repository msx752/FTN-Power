namespace Fortnite.Static.Models.Survivors
{
    public class BaseXp
    {
        private short _heroes;
        private short _leadSurvivors;
        private short _traps;
        private short _weapons;
        public short Defenders { get; set; }

        public short Heroes
        {
            get
            {
                return _heroes == 0 ? Survivors : _leadSurvivors;
            }

            set => _heroes = value;
        }

        public short LeadSurvivors
        {
            get
            {
                return _leadSurvivors == 0 ? Survivors : _leadSurvivors;
            }

            set => _leadSurvivors = value;
        }

        public short Survivors { get; set; }

        public short Traps
        {
            get
            {
                return _traps == 0 ? Survivors : _leadSurvivors;
            }

            set => _traps = value;
        }

        public short Weapons
        {
            get
            {
                return _weapons == 0 ? Survivors : _leadSurvivors;
            }

            set => _weapons = value;
        }
    }
}