using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleGame
{
    public class GuessResult
    {
        public string Letter1 { get; set; }
        public IBrush Background1 { get; set; }

        public string Letter2 { get; set; }
        public IBrush Background2 { get; set; }

        public string Letter3 { get; set; }
        public IBrush Background3 { get; set; }

        public string Letter4 { get; set; }
        public IBrush Background4 { get; set; }

        public string Letter5 { get; set; }
        public IBrush Background5 { get; set; }

        public (string Letter, IBrush Background)[] Letters => new[]
        {
            (Letter1, Background1),
            (Letter2, Background2),
            (Letter3, Background3),
            (Letter4, Background4),
            (Letter5, Background5)
        };

        public void SetLetter(int index, string letter, IBrush background)
        {
            switch (index)
            {
                case 0:
                    Letter1 = letter;
                    Background1 = background;
                    break;
                case 1:
                    Letter2 = letter;
                    Background2 = background;
                    break;
                case 2:
                    Letter3 = letter;
                    Background3 = background;
                    break;
                case 3:
                    Letter4 = letter;
                    Background4 = background;
                    break;
                case 4:
                    Letter5 = letter;
                    Background5 = background;
                    break;
            }
        }
    }
}
