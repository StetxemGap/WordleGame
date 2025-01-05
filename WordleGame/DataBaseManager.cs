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
        private readonly string _dbPath = "C:\\Users\\sisis\\source\\repos\\WordleGame\\WordleGame\\words.db";
        public DataBaseManager() { }

        public bool CheckWord(string word, int wordLength)
        {
            string tableName;
            switch (wordLength)
            {
                case 4:
                    tableName = "letters4";
                    break;

                case 5:
                    tableName = "letters5";
                    break;

                case 6:
                    tableName = "letters6";
                    break;

                default:
                    throw new ArgumentException("Длина слова не может быть меньше 4 и больше 6 букв");
            }

            using var connection = new SqliteConnection($"Data Source = {_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $@"
                        SELECT COUNT(*)
                        FROM {tableName}
                        WHERE word = @word;";
            command.Parameters.AddWithValue("@word", word.ToLower());

            long count = (long)command.ExecuteScalar();
            return (count > 0);
        }
    }
}
