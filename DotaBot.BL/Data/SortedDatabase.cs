using System;
using System.Collections.Generic;
using System.Text;
using DotaNetUpgrade.Struct;

namespace DotaNetUpgrade.Data
{
    [Serializable]
    class SortedDatabase
    {
        /// <summary>
        /// Все матчи в безе данных
        /// </summary>
        public List<Match> Matches { get; private set; }
        /// <summary>
        /// Все команды в базе данных
        /// </summary>
        public List<Team> Teams { get; private set; }
        /// <summary>
        /// Все  игроки в базе данных
        /// </summary>
        public List<Gamer> Gamers { get; private set; }

        /// <summary>
        /// Создает пустую базу данных
        /// </summary>
        public SortedDatabase()
        {
            Matches = new List<Match>();
            Teams = new List<Team>();
            Gamers = new List<Gamer>();
        }

        /// <summary>
        ///Добавляет в базу данных не сортированный матч, то есть только полученный с помощью парсера 
        /// </summary>
        /// <param name="match">не отсортированный матч</param>
        public void AddNotSortedMatch(Match match)
        {
            Gamer[] gamersLeft = match.Left.Gamers.ToArray();
            Gamer[] gamersRight = match.Right.Gamers.ToArray();
            Team left = CreateTeamByNewGamers(match.Left.Name, gamersLeft);
            Team right = CreateTeamByNewGamers(match.Right.Name, gamersRight);
            Team.UpdateTeamByResultMatch(left, right, match.ScoresLeft, match.ScoresRight);
            Match addingMatch = new Match(left, right, match.ScoresLeft, match.ScoresRight, match.URL);
            Teams.Add(left); Teams.Add(right);
            Matches.Add(addingMatch);
        }
        /// <summary>
        /// Создает уникальную команду по новым игрокам
        /// </summary>
        /// <param name="teamName">Название команды</param>
        /// <param name="gamers">Игроки</param>
        /// <returns>Команда из базы данных</returns>
        public Team CreateTeamByNewGamers(string teamName ,Gamer[] gamers)
        {
            for (int i = 0; i < gamers.Length; i++)
            {
                Gamer gamerGet = Gamers.Find(g => g.URL == gamers[i].URL);
                if (gamerGet != null)
                    gamers[i] = gamerGet;
                else Gamers.Add(gamers[i]);
            }
            Team team = new Team(teamName, gamers);
            return team;
        }
    }
}
