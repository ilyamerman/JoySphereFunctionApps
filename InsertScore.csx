#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Text;

public static async Task<HttpResponseMessage> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");

    string player_a = req.Query["player_a"];
    string player_b = req.Query["player_b"];
    string score = req.Query["score"];

    var str = Environment.GetEnvironmentVariable("SQLDB_CONNECTION");
    using (SqlConnection conn = new SqlConnection(str))
    {
        conn.Open();
        string query = "INSERT INTO dbo.high_scores (player_a, player_b ,score) VALUES (@player_a, @player_b, @score)";

        using(SqlCommand command = new SqlCommand(query, conn))
	    {
	        command.Parameters.AddWithValue("@player_a", player_a);
	        command.Parameters.AddWithValue("@player_b", player_b);
	        command.Parameters.AddWithValue("@score", score);

	        int result = command.ExecuteNonQuery();

	        // Check Error
	        if(result < 0)
	            Console.WriteLine("Error inserting data into Database!");
	    }
    }

    return new HttpResponseMessage(HttpStatusCode.OK){
    	Content = new StringContent("{message: Inserted " + player_a + " " + player_b + " " + score + " into the DB}", Encoding.UTF8, "application/json")	  
 	};
}
