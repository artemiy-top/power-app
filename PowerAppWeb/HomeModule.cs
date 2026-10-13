using System;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

public class HomeModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) => app.MapGet("/", () =>
    {
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
        return Results.Content($@"<ul>
            {string.Join(
                "", 
                powerBanks.Select(pb => $@"<li>
                    {pb.Id} [{pb.Value}/{pb.Model.MaxValue},
                    {Math.Floor((double)pb.Value * 100 / pb.Model.MaxValue)}%]
                    {pb.Model.Name} {pb.Point.Address}
                </li>"))}
        </ul>", "text/html; charset=utf-8");
    });
}
