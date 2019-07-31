#r "Newtonsoft.Json"

# load "serialize.csx"
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Text;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;

public static async Task < HttpResponseMessage > Run(HttpRequest req, ILogger log) {
 log.LogInformation("C# HTTP trigger function processed a request.");

 string name = req.Query["name"];

 string json = " ";

 try {
  var str = Environment.GetEnvironmentVariable("SQLDB_CONNECTION");
  using(SqlConnection conn = new SqlConnection(str)) {

   using(SqlCommand cmd = new SqlCommand())

   {

    System.Data.SqlClient.SqlDataReader dataReader;

    cmd.CommandText = "SELECT SUBSTRING(player_a, 1, 5) as player_a, SUBSTRING(player_b, 1, 5) as player_b, score FROM dbo.high_scores ORDER BY score DESC";

    cmd.CommandType = CommandType.Text;

    cmd.Connection = conn;

    conn.Open();

    dataReader = cmd.ExecuteReader();

    var r = Serialize(dataReader);

    json = JsonConvert.SerializeObject(r, Formatting.Indented);

   }

  }

 } catch (SqlException sqlex)

 {

  log.LogInformation(sqlex.Message);

  log.LogInformation(sqlex.ToString());

  return new HttpResponseMessage(HttpStatusCode.BadRequest) {
   	Content = new StringContent(JsonConvert.SerializeObject($"The following SqlException happened: {sqlex.Message}"), Encoding.UTF8, "application/json")
  };

 } catch (Exception ex)

 {

  log.LogInformation(ex.Message);

  log.LogInformation(ex.ToString());

  return new HttpResponseMessage(HttpStatusCode.BadRequest){
   Content = new StringContent(JsonConvert.SerializeObject($"The following SqlException happened: {ex.Message}"), Encoding.UTF8, "application/json")
  };

 }

 return new HttpResponseMessage(HttpStatusCode.OK)
 {
  Content = new StringContent(json, Encoding.UTF8, "application/json")
 };
}
