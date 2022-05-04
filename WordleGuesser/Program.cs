class Program
{
    static void Main()
    {
        var dir = Directory.GetCurrentDirectory();
        dir = dir.Substring(0, dir.IndexOf(@"\bin")) + @"\PossibleWords.txt";
        var text = File.ReadAllText(dir);
        var textArray = text.Trim().Split("\n");
        IEnumerable<string> subset = textArray;
        string input, suggestion = "crane";
        Console.WriteLine("Type 'exit' to close any time");
        do
        {
            Console.WriteLine($"\nEnter {suggestion} and type the response, using 'b' to indicate black, 'y' to indicate yellow, and 'g' to indicate green: ");
            input = Console.ReadLine().Trim();

            //Validity Check
            while (input.Any(c => c != 'g' && c != 'b' && c != 'y') || input.Length != 5)
            {
                if (input.Equals("exit") || input.Equals("ggggg"))
                    Environment.Exit(0);
                Console.WriteLine("Enter a valid response: ");
                input = Console.ReadLine().Trim().ToLower();
            }

            //Uses the confirmed correct slots to narrow down possibilities for the answer
            if (input.Contains('g'))
                subset = NarrowSubset(subset, input, suggestion, 'g');
            if (input.Contains('y'))
                subset = NarrowSubset(subset, input, suggestion, 'y');
            if (input.Contains('b'))
                subset = NarrowSubset(subset, input, suggestion, 'b');

            double topLikelyhood = 0;
            string highestScorer = "";
            foreach (var item in subset)
            {
                var score = ScoreLikelyhood(item, suggestion);
                if (score > topLikelyhood)
                {
                    topLikelyhood = score;
                    highestScorer = item;
                }
            }

            suggestion = highestScorer;

            Console.WriteLine("\nPossibilities: ");
            foreach (var s in subset)
                Console.WriteLine(s);
            if (string.IsNullOrWhiteSpace(suggestion))
            {
                Console.WriteLine("No suggestions generated. Terminating process.");
                Environment.Exit(0);
            }


            Console.WriteLine($"\nSuggestion generated: {suggestion}");


        } while (true);
    }

    static IEnumerable<String> NarrowSubset(IEnumerable<String> subset, string input, string suggestion, char identifier)
    {
        var shortenedInput = input;
        var count = input.Count(c => c == identifier);
        for (int i = 0; i < count; i++)
        {
            var index = shortenedInput.IndexOf(identifier);

            switch (identifier)
            {
                case 'g':
                    subset = subset.Where(s => s[index].Equals(suggestion[index]));
                    break;
                case 'y':
                    subset = subset.Where(s => s.Contains(suggestion[index]) && s.IndexOf(suggestion[index]) != index);
                    break;
                case 'b':
                    if (!((suggestion.Substring(0, index) + suggestion.Substring(index + 1)).Contains(suggestion[index])))
                        subset = subset.Where(s => !s.Contains(suggestion[index]));
                    break;
            }
            var newShortenedInput = "";
            for (int j = 0; j <= index; j++)
            {
                newShortenedInput += " ";
            }

            newShortenedInput += shortenedInput.Substring(index + 1);
            shortenedInput = newShortenedInput;
        }

        return subset;
    }

    static double ScoreLikelyhood(string str, string suggestion)
    {
        var score = 0.0;
        var toBeScored = str.Where(c => !suggestion.Contains(c));
        foreach (var c in toBeScored)
        {
            score += GetScore(c);
        }
        return score;
    }

    static double GetScore(char c) //Based loosely on Samuel Morse's chart of letter frequencies in proportion to q
    {
        switch (c)
        {
            case 'i':
                return 38.45;
            case 'o':
                return 36.51;
            case 't':
                return 35.43;
            case 's':
                return 29.23;
            case 'l':
                return 27.98;
            case 'u':
                return 18.51;
            case 'd':
                return 17.25;
            case 'p':
                return 16.14;
            case 'm':
                return 15.36;
            case 'h':
                return 15.31;
            case 'g':
                return 12.59;
            case 'b':
                return 10.56;
            case 'f':
                return 9.24;
            case 'y':
                return 9.06;
            case 'w':
                return 6.57;
            case 'k':
                return 5.61;
            case 'v':
                return 5.13;
            case 'x':
                return 1.48;
            case 'z':
                return 1.39;
            case 'j':
                return 1.0;
            case 'q':
                return 1;
            default:
                return 8.0;
        }
    }
}