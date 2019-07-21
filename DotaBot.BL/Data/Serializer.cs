using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DotaNetUpgrade.Struct;
using DotaNetUpgrade.Data;

namespace DotaNetUpgrade.Data
{
    /// <summary>
    /// Класс для бинарной сериализации данных в файл
    /// </summary>
    class BinarySerializer
    {
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Экземпляр для работы с файлом Path
        /// </summary>
        /// <param name="Path">Путь к файлу</param>
        public BinarySerializer(string Path) => this.Path = Path;

        /// <summary>
        /// Сохраняет объект в файл Path
        /// </summary>
        /// <param name="obj">Сохраняемый объект</param>
        public void Serialize(object obj)
        {
            using (FileStream fs = new FileStream(Path, FileMode.CreateNew))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, obj);
            }
        }
        /// <summary>
        /// Десериализует объект по пути Path
        /// </summary>
        /// <returns>Объект из файла</returns>
        public object Deserialize()
        {
            object obj;
            using (FileStream fs=new FileStream(Path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                obj = formatter.Deserialize(fs);
            }
            return obj;
        }
        /// <summary>
        /// Десериализзует объект по пути Path
        /// </summary>
        /// <returns>Массив Матчей</returns>
        public Match[] DeserializeMatches() => (Match[])Deserialize();
        /// <summary>
        /// Дессериализует объект по пусти Path
        /// </summary>
        /// <returns>Сортированная база данных</returns>
        public SortedDatabase DeserializeSortedDatabase() => (SortedDatabase)Deserialize();
    }
}
