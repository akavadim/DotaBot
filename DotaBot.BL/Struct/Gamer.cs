using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotaBot.BL.Struct
{
    [Serializable]
    public class Gamer
    {
        /// <summary>
        /// Ник игрока
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Кооличество побед
        /// </summary>
        public int Wins { get; set; }
        /// <summary>
        /// Количество поражений
        /// </summary>
        public int Losses { get; set; }
        /// <summary>
        /// Ссылка на профиль игрока
        /// </summary>
        public string URL { get; private set; }
        /// <summary>
        /// Винрейт
        /// </summary>
        public double Winrate { get => (double)Wins / (Wins + Losses); }
        /// <summary>
        /// Отношения с напарниками
        /// </summary>
        public List<Relation> Bodies { get; set; }
        /// <summary>
        /// Отношения с врагами
        /// </summary>
        public List<Relation> Enemies { get; set; }

        /// <summary>
        /// Конструктор создающий пустой класс
        /// </summary>
        public Gamer()
        {
            Bodies = new List<Relation>();
            Enemies = new List<Relation>();
        }
        /// <summary>
        /// Создать экземпляр с именем и ссылкой на профиль
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="URL"></param>
        public Gamer(string Name, string URL)
        {
            this.Name = Name;
            this.URL = URL;
            Bodies = new List<Relation>();
            Enemies = new List<Relation>();
        }

        /// <summary>
        /// Найти отношение с напарником
        /// </summary>
        /// <param name="Bodi">Игрок - напарник</param>
        /// <returns></returns>
        public Relation FindBodi(Gamer Bodi) => Bodies.Find(r => r.Gamer == Bodi);
        /// <summary>
        /// Найти отношение с противником
        /// </summary>
        /// <param name="Enemy">Игрок - противник</param>
        /// <returns></returns>
        public Relation FindEnemy(Gamer Enemy) => Enemies.Find(r => r.Gamer == Enemy);
        /// <summary>
        /// Добавить победу напарнику
        /// </summary>
        /// <param name="Bodi">Игрок - напарник</param>
        public void AddBodiWin(Gamer Bodi)
        {
            Relation relation = FindBodi(Bodi);
            if (relation == null)
            {
                relation = new Relation(Bodi);
                Bodies.Add(relation);
            }
            relation.Wins++;
        }
        /// <summary>
        /// Добавить поражение с напарником
        /// </summary>
        /// <param name="Bodi">Игрок - напарник</param>
        public void AddBodyLose(Gamer Bodi)
        {
            Relation relation = FindBodi(Bodi);
            if (relation == null)
            {
                relation = new Relation(Bodi);
                Bodies.Add(relation);
            }
            relation.Losses++;
        }
        /// <summary>
        /// Добавить победу против противника
        /// </summary>
        /// <param name="Bodi">Игрок - противник</param>
        public void AddEnemyWin(Gamer Enemy)
        {
            Relation relation = FindEnemy(Enemy);
            if (relation == null)
            {
                relation = new Relation(Enemy);
                Enemies.Add(relation);
            }
            relation.Wins++;
        }
        /// <summary>
        /// Добавить поражение против противника
        /// </summary>
        /// <param name="Bodi">Игрок - противник</param>
        public void AddEnemyLose(Gamer Enemy)
        {
            Relation relation = FindEnemy(Enemy);
            if (relation == null)
            {
                relation = new Relation(Enemy);
                Enemies.Add(relation);
            }
            relation.Losses++;
        }
    }
}
