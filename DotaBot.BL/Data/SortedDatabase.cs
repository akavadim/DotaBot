using System;
using System.Collections.Generic;
using System.Text;
using DotaBot.BL.Struct;
using System.Linq;

namespace DotaBot.BL.Data
{
    [Serializable]
    public class SortedDatabase
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
        private Team CreateTeamByNewGamers(string teamName ,Gamer[] gamers)
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

        public (double Left, double Right) GetResultMatch(string[] lefts, string[] rights)
        {
            if (lefts == null || rights == null)
                throw new ArgumentNullException("Один из аргументов был пустой");
            List<Gamer> left = new List<Gamer>();
            List<Gamer> right = new List<Gamer>();

            foreach (var gamerName in lefts)
            {
                var gamer = GetGamerByURL(gamerName);
                if (gamer == null)
                    throw new Exception("Игрок " + gamerName + " не найден в базе");
                left.Add(gamer);
            }

            foreach (var gamerName in rights)
            {
                var gamer = GetGamerByURL(gamerName);
                if (gamer == null)
                    throw new Exception("Игрок " + gamerName + " не найден в базе");
                right.Add(gamer);
            }

            return GetResultMatch(left.ToArray(), right.ToArray());
        }

        public (double Left, double Right)GetResultMatch(Gamer[] leftGamers, Gamer[] rightGamers)
        {
            foreach (var gamer in leftGamers.Concat(rightGamers))
                if (!Gamers.Contains(gamer))
                    throw new ArgumentException("Игроки должны содеражаться в базе данных");

            double scoresLeft = 0;
            double scoresRight = 0;

            foreach (Gamer gamer in leftGamers)
            {
                scoresLeft += gamer.Winrate;
                foreach (Gamer body in leftGamers)
                    if (body != gamer)
                        scoresLeft += gamer.FindBodi(body).Winrate;
                foreach (Gamer enemy in rightGamers)
                    scoresLeft += gamer.FindEnemy(enemy).Winrate;
            }
            foreach (Gamer gamer in rightGamers)
            {
                scoresRight += gamer.Winrate;
                foreach (Gamer body in rightGamers)
                    if (body != gamer)
                        scoresRight += gamer.FindBodi(body).Winrate;
                foreach (Gamer enemy in leftGamers)
                    scoresRight += gamer.FindEnemy(enemy).Winrate;
            }

            return (scoresLeft, scoresRight);
        }

        /// <summary>
        /// Возвращает игрока, если оон содердится в базе данных
        /// </summary>
        /// <param name="URL">Ссылка на игрока</param>
        /// <returns>Игрок или null</returns>
        private Gamer GetGamerByURL(string name)
        {
            foreach (var gamer in Gamers)
                if (gamer.URL == name)
                    return gamer;
            return null;
        }
    }
}
