using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Set a Connection service for the Mongo URI Connection string
builder.Services.AddSingleton<IMongoClient, MongoClient>(c => {
    var conn = c.GetRequiredService<IConfiguration>()["Mongoconnection"];
    return new MongoClient(conn);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/shipwrecks", (IMongoClient client) =>
{
    var database = client.GetDatabase("sample_geospatial");
    var collection = database.GetCollection<ShipWreck>("shipwrecks");

    return collection.Find(s => s.FeatureType.Equals("Wrecks - Visible")).ToList();
})
.WithName("GetShipwrecks");

app.Run();

[BsonIgnoreExtraElements]                                   //This will help the driver ignore all the extra elements we dont want from the data
internal class ShipWreck
{
    [BsonId]                                                //This will map the document Id to the class Id
    public ObjectId Id { get; set; }    
    [BsonElement("feature_type")]                           //The driver will map properties that match the same name regarless of
    public string? FeatureType { get; set; }                //it being lowercase/uppercase. However, if the property name is different from the 
    public string? Chart { get; set; }                      // data name, you can explicitly specify using Bson Attributes
    [BsonElement("latdec")]                                         
    public double latitude { get; set; }
    [BsonElement("londec")]
    public double Longitude { get; set; }


}