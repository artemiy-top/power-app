using Dapper;
using Microsoft.Data.Sqlite;
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
var connection = new SqliteConnection("DataSource=app.db");
var powerBanks = connection.Query<PowerBank, Model, Point, PowerBank>(
    @"SELECT * FROM power_banks
INNER JOIN models ON models.id = power_banks.model_id 
INNER JOIN points ON points.id = power_banks.point_id",
    (pb, model, point) =>
    {
        pb.Model = model;
        pb.Point = point;
        return pb;
    }
);
foreach (PowerBank pb in powerBanks) {
    Console.WriteLine($@"{pb.Id} [{pb.Value}/{pb.Model.MaxValue}, {Math.Floor((double)pb.Value * 100 / pb.Model.MaxValue)}%] {pb.Model.Name} {pb.Point.Address}");
}
