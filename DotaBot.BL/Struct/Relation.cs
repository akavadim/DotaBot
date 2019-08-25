using System;
using System.Collections.Generic;
using System.Text;

namespace DotaBot.BL.Struct
{
    /// <summary>
    /// Отношения игрока с другим игроком
    /// </summary>
    [Serializable]
    public class Relation
    {
        /// <summary>
        /// Количество побед
        /// </summary>
        public int Wins { get; set; }
        /// <summary>
        /// Количество поражений
        /// </summary>
        public int Losses { get; set; }
        /// <summary>
        /// Игрок с/против которого были игры
        /// </summary>
        public Gamer Gamer { get; private set; }
        /// <summary>
        /// Винрейт
        /// </summary>
        public double Winrate { get => (double)Wins / (Wins + Losses); }

        /// <summary>
        /// Создать экземпляр по игроку
        /// </summary>
        /// <param name="Gamer">Игрок</param>
        public Relation(Gamer Gamer) => this.Gamer = Gamer;
    }
}
