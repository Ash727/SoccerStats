 using System;
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
                SentimentResponse sentimentResponse = getSentimentREsponse(newsReuslts);
                List<Player> topTen = Player.getTOpTenPlayers(players);


                // for each sentiment repsons
                foreach (var sentiment in sentimentResponse.sentiemnts) {
                    foreach (var newsResult in newsReuslts) {
                        if (newsResult.HeadLine == sentiment.id) {
                      
                          newsResult.SentimentScore = sentiment.score;
                        }
                    }
                }

                // Prests the news results for each of the news for each player
              foreach (var results in newsReuslts)
                {
                    Console.Write(String.Format("Sentiment Score:{0:P} Date:{1:f}, Headline:{2}, Summary: {3} \r\n",results.SentimentScore,results.datePublished, results.HeadLine, results.Summary));
                    Console.WriteLine(DateTime.Now.ToString("dddd, dd MMMM yyyy tt")   
                        );
                    Console.WriteLine();
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
          //  List<NewSearch> Searches;

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
            //string searchResult = Encoding.UTF8.GetString(searchResults);
            //JsonConvert.DeserializeObject<NewsResult>(searchResult);


            using (var memorStream = new MemoryStream(searchResults))
            using (var reader = new StreamReader(memorStream))
            using (var jsontextReader = new JsonTextReader(reader))
            {

                // saying for every NewsSearch we want the news results put it into results 
                results = seralizer.Deserialize<NewSearch>(jsontextReader).NewsResults;
            }
            
            return results;
        }

        public static SentimentResponse getSentimentREsponse(List<NewsResult> newsResults)
        {
            var sentimentResponse = new SentimentResponse();
            var sentimentRequest = new SentimentRequest();
            // Needed or run time exception 
            sentimentRequest.documents = new List<Document>();
            

            foreach (var result in newsResults) {
                sentimentRequest
                      .documents
                      .Add(new Document {
                          Id = result.HeadLine,
                          Text = result.Summary

                      });
            }

            const string accessKey = "6dfee2787cac4a9ab87d1ba313b7599b";
            const string uriBase = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";
            var results = new List<NewsResult>();
            var webClient = new WebClient();
            webClient.Headers.Add("Ocp-Apim-Subscription-Key", accessKey);
            webClient.Headers.Add(HttpRequestHeader.Accept, "applicaiton/json");
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");

            //Seralizes the object to json for transfer 
            string requestJson = JsonConvert.SerializeObject(sentimentRequest);
            // Converts the the string to byte array
            byte[] requestJson_bytearray = Encoding.UTF8.GetBytes(requestJson);

            // Send upload that to the text anylitics server through http reuqes weblient 
            byte [] response = webClient.UploadData(uriBase,requestJson_bytearray);
            // Convert JSON response back to string 
            string setniments = Encoding.UTF8.GetString(response);

            // Convert the string back to our response object
            sentimentResponse= JsonConvert.DeserializeObject<SentimentResponse>(setniments);

            
            
            return sentimentResponse;
        }

    }
}
