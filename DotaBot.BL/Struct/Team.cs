using System;
using System.Collections.Generic;
using System.Text;

namespace DotaBot.BL.Struct
{
    [Serializable]
    public class Team
    {
        /// <summary>
        /// Название команды
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Игроки в команде
        /// </summary>
        public List<Gamer> Gamers { get; set; }

        /// <summary>
        /// Создает команду с Название и игроками
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="gamers">Игроки</param>
        public Team(string name, Gamer[] gamers)
        {
            this.Name = name;
            Gamers = new List<Gamer>(gamers);
        }

        /// <summary>
        /// Обновление команд по результатам матча, добавление союзников, врагов, побед и поражений
        /// </summary>
        /// <param name="Left">Левая команда</param>
        /// <param name="Right">Правая команда</param>
        /// <param name="ScoresLeft">Счет левой команды</param>
        /// <param name="ScoresRight">Счет правой команды</param>
        public static void UpdateTeamByResultMatch(Team Left, Team Right, int ScoresLeft, int ScoresRight)
        {
            if (ScoresLeft > ScoresRight)
            {
                Left.AddWin();
                Right.AddLose();
            }
            else if (ScoresRight > ScoresLeft)
            {
                Left.AddLose();
                Right.AddWin();
            }
            foreach (Gamer leftGamer in Left.Gamers)
            {
                foreach (Gamer bodi in Left.Gamers)
                {
                    if (leftGamer == bodi)
                        continue;
                    if (ScoresLeft > ScoresRight)
                        leftGamer.AddBodiWin(bodi);
                    else if (ScoresRight > ScoresLeft)
                        leftGamer.AddBodyLose(bodi);
                }
                foreach (Gamer enemy in Right.Gamers)
                {
                    if (ScoresLeft > ScoresRight)
                        leftGamer.AddEnemyWin(enemy);
                    else if (ScoresRight > ScoresLeft)
                        leftGamer.AddEnemyLose(enemy);
                }
            }
            foreach (Gamer rightGamer in Right.Gamers)
            {
                foreach (Gamer bodi in Right.Gamers)
                {
                    if (rightGamer == bodi)
                        continue;
                    if (ScoresRight > ScoresLeft)
                        rightGamer.AddBodiWin(bodi);
                    else if (ScoresLeft > ScoresRight)
                        rightGamer.AddBodyLose(bodi);
                }
                foreach (Gamer enemy in Left.Gamers)
                {
                    if (ScoresRight > ScoresLeft)
                        rightGamer.AddEnemyWin(enemy);
                    else if (ScoresLeft > ScoresRight)
                        rightGamer.AddEnemyLose(enemy);
                }
            }
        }

        /// <summary>
        /// Добавляет всем игрокам победы
        /// </summary>
        /// <param name="Wins">Количество побед</param>
        public void AddWin(int Wins=1)
        {
            foreach (Gamer gamer in Gamers)
                gamer.Wins += Wins;
        }
        /// <summary>
        /// Добавляет всем игрокм поражения
        /// </summary>
        /// <param name="Losses">Количество поражений</param>
        public void AddLose(int Losses = 1)
        {
            foreach (Gamer gamer in Gamers)
                gamer.Losses += Losses;
        }
    }
}
