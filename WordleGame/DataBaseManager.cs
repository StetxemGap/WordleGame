using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace WordleGame
{
    public class DataBaseManager
    {
        private readonly string _dbPath = "words.db";
        public DataBaseManager() { }

        public bool CheckWord(string word, int wordLength)
        {
            string tableName;
            switch (wordLength)
            {
                case 4:
                    tableName = "4letters";
                    break;

                case 5:
                    tableName = "5letters";
                    break;

                case 6:
                    tableName = "6letters";
                    break;

                default:
                    throw new ArgumentException("Длина слова не может быть меньше 4 и больше 6 букв");
            }

            using var connection = new SqliteConnection($"Data Source = words.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $@"
                        SELECT COUNT(*)
                        FROM {tableName}
                        WHERE Word = $word;";
            command.Parameters.AddWithValue("$word", word.ToUpper());

            long count = (long)command.ExecuteScalar();
            return (count > 0);
        }
    }
}
