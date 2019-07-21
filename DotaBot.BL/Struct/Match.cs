using System;
using System.Collections.Generic;
using System.Text;

namespace DotaNetUpgrade.Struct
{
    [Serializable]
    class Match
    {
        /// <summary>
        /// Левая команда
        /// </summary>
        public Team Left { get; set; }
        /// <summary>
        /// Правая команда
        /// </summary>
        public Team Right { get; set; }
        /// <summary>
        /// Очки левой команы
        /// </summary>
        public int ScoresLeft { get; set; }
        /// <summary>
        /// Очки правой команды
        /// </summary>
        public int ScoresRight { get; set; }
        /// <summary>
        /// Ссылка на матч
        /// </summary>
        public string URL { get; set; }

        public Match(Team Left, Team Right, int ScoresLeft, int ScoresRight, string URL)
        {
            this.Left = Left;
            this.Right = Right;
            this.ScoresLeft = ScoresLeft;
            this.ScoresRight = ScoresRight;
            this.URL = URL;
        }
    }
}
