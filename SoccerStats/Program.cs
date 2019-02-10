﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace SoccerStats
{
    class Program 
    {
        static void Main(string[] args)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);
            var fileName = Path.Combine(directory.FullName, "SoccerGameResults.csv");
            var fileContents = ReadSoccerResults(fileName);

            var fileName2 = Path.Combine(directory.FullName, "players.json");

            var players = DeSerlizePlayers(fileName2);
           
            foreach (var player in Player.getTOpTenPlayers(players))
            {
                List<NewsResult> newsReuslts = getNewsForPlayer(player.FirstName + " " + player.LastName);
                List<Player> topTen = Player.getTOpTenPlayers(players);

                //List<NewsResult> newsReuslts = new List<NewsResult>();
                // newsReuslts = getNewsForPlayer("dbz"); 
                //newsReuslts = getNewsForPlayer("dbz");
                foreach (var results in newsReuslts)
                {
                    Console.Write(String.Format("Date:{0:f}, Headline:{1}, Summary: {2} \r\n", results.datePublished, results.HeadLine, results.Summary));
                   Console.ReadKey(); // Using the enter key will move those in the list 
                }
               // Console.WriteLine("Name:" + topTen[0].FirstName + " LastName: " + topTen[0].LastName + " Score:" + topTen[0].PointsPerGame);
            }

            //Console.WriteLine(directory.FullName);
            fileName = Path.Combine(directory.FullName, "soccarStats.json");
            SeralizePLayerToFile(Player.getTOpTenPlayers(players), fileName);
         //   Console.WriteLine(getNewsForPlayer("Diego Valeri"));
        }

        public static string ReadFile(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd();
            }
        }

        public static List<GameResult> ReadSoccerResults(string fileName)
        {
                                 
            var soccerResults = new List<GameResult>();
            using (var reader = new StreamReader(fileName))
            {
                string line = "";
                reader.ReadLine();
                while((line = reader.ReadLine()) != null)
                {
                    var gameResult = new GameResult();
                    string[] values = line.Split(',');

                    DateTime gameDate;
                    if (DateTime.TryParse(values[0], out gameDate))
                    {
                        gameResult.GameDate = gameDate;
                    }
                    gameResult.TeamName = values[1];
                    HomeOrAway homeOrAway;
                    if (Enum.TryParse(values[2], out homeOrAway))
                    {
                        gameResult.HomeOrAway = homeOrAway;
                    }

                    int parseInt;
                    if (int.TryParse(values[3], out parseInt)) {
                        gameResult.Goals = parseInt;
                    }
                    if (int.TryParse(values[4], out parseInt))
                    {
                        gameResult.GoalAttempts = parseInt;
                    }
                    if (int.TryParse(values[5], out parseInt))
                    {
                        gameResult.ShotsonGoal = parseInt;
                    }
                    if (int.TryParse(values[6], out parseInt))
                    {
                        gameResult.ShotsOffGoal = parseInt;
                    }

                    double possesionPrecent;
                        if (double.TryParse(values[7], out possesionPrecent)) {
                        gameResult.PossessionPercent = possesionPrecent;
                    }


                    soccerResults.Add(gameResult);
                }
            }
            return soccerResults;
        }

        public static List<Player> DeSerlizePlayers( string fileName) {
           //var path=  Directory.GetCurrentDirectory();
           // //var fileName2 = Path.Combine(path, "filename.txt");
            //var writer = new StreamWriter(fileName2);
            //writer.WriteLine("hello");
            //writer.Close();
            var players = new List<Player>();
            var serializer = new JsonSerializer();
            using (var reader = new StreamReader(fileName))
            using (var jsonReader = new JsonTextReader(reader))
            {
                try
                {
                    players = serializer.Deserialize<List<Player>>(jsonReader);
                }
                catch (Exception e) {
                    Console.WriteLine("File not found"+e.StackTrace);
                }
                }

            return players;

        }

        public static void SeralizePLayerToFile(List<Player> players, string filename ) {
          
            var seralizer = new JsonSerializer();
            using (var writer = new StreamWriter(filename))
            using (var jsonWriter = new JsonTextWriter(writer)) { 

                seralizer.Serialize(jsonWriter,players);
            }

        }

        public static string GetGoogleHomePage()
        {
            var webClient = new WebClient();
            byte [] googleHome = webClient.DownloadData("https://www.google.com");
            // Neeed a stream reader 
            using (var stream = new MemoryStream(googleHome))
            using (var reader = new StreamReader(stream)) {

                return reader.ReadToEnd();

            }

        }

        public static List<NewsResult> getNewsForPlayer(string playerName)
        {
            // Access key from bing 7 congnitive services 
            const string accessKey = "2f1a6d364acb4a0db7e835c8cd0777d7";
            const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/news/search";
            string searchTerm = playerName;
            List<NewSearch> Searches;

            var uriQuery = uriBase + "?q=" + Uri.EscapeDataString(searchTerm);


            //webClient.Headers.Add("Ocp-Apim-Subscription-Key", accessKey);
            //byte[] serachResults = webClient.DownloadData(uriQuery);
            // Neeed a stream reader 
            //using (var stream = new MemoryStream(serachResults))
            //using (var reader = new StreamReader(stream))
            //{

            //    return reader.ReadToEnd();

            //}
            // or 
            //NewSearch s = new NewSearch();
            //s.value.RemoveAll(e=> e.name != "freddy");

            //  Searches.RemoveAll(e => e._type.Equals(false));
            // From webiste ezample 
            var results = new List<NewsResult>();
            var webClient = new WebClient();
            webClient.Headers.Add("Ocp-Apim-Subscription-Key", accessKey);
            var seralizer = new JsonSerializer();


          //  WebRequest request = WebRequest.Create(uriQuery);
            //request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
            //HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            //string json = new StreamReader(response.GetResponseStream()).ReadToEnd();


            byte[] searchResults = webClient.DownloadData(uriQuery);


            using (var memorStream = new MemoryStream(searchResults))
            using (var reader = new StreamReader(memorStream))
            using (var jsontextReader = new JsonTextReader(reader))
            {

                // saying for every NewsSearch we want the news results put it into results 
                results = seralizer.Deserialize<NewSearch>(jsontextReader).NewsResults;
            }
            

            return results;
            

             

        }

    }
}
