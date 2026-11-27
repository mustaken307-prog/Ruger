using MongoDB.Driver;
using RC.Models;

namespace RC.Services
{
    public class DbS // لاحظ الاسم هنا DbS
    {
        private readonly IMongoDatabase _db;
        private readonly int _maxSubs;

        public DbS(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            _db = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _maxSubs = int.Parse(config["AppSettings:MaxSubscribers"] ?? "5");
        }

        public IMongoCollection<AppUser> Users => _db.GetCollection<AppUser>("Users");

        public async Task<AppUser?> GetUser(string email)
        {
            return await Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<AppUser?> GetUserById(string id)
        {
            return await Users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<AppUser>> GetAllSubs()
        {
            return await Users.Find(u => u.Role == "Subscriber").ToListAsync();
        }

        public async Task AddUser(AppUser u)
        {
            if (u.Role == "Subscriber")
            {
                var count = await Users.CountDocumentsAsync(x => x.Role == "Subscriber");
                if (count >= _maxSubs) throw new Exception("تم الوصول للحد الأقصى (5 مشتركين).");
            }
            await Users.InsertOneAsync(u);
        }

        public async Task DelUser(string id)
        {
            await Cars.DeleteManyAsync(c => c.OwnerId == id);
            await Users.DeleteOneAsync(u => u.Id == id);
        }

        public async Task UpdateQuota(string id, int quota)
        {
            var upd = Builders<AppUser>.Update.Set(u => u.CarQuota, quota);
            await Users.UpdateOneAsync(u => u.Id == id, upd);
        }

        public IMongoCollection<Car> Cars => _db.GetCollection<Car>("Cars");

        public async Task<List<Car>> GetAllCars(string? search = null)
        {
            var f = Builders<Car>.Filter.Empty;
            if (!string.IsNullOrWhiteSpace(search))
            {
                f = Builders<Car>.Filter.Or(
                    Builders<Car>.Filter.Regex(c => c.Brand, new MongoDB.Bson.BsonRegularExpression(search, "i")),
                    Builders<Car>.Filter.Regex(c => c.ModelName, new MongoDB.Bson.BsonRegularExpression(search, "i"))
                );
            }
            return await Cars.Find(f).SortByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<List<Car>> GetCarsByOwner(string ownerId)
        {
            return await Cars.Find(c => c.OwnerId == ownerId).SortByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<Car?> GetCar(string id)
        {
            return await Cars.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddCar(Car c)
        {
            var u = await GetUserById(c.OwnerId);
            if (u == null) throw new Exception("المستخدم غير موجود.");

            var count = await Cars.CountDocumentsAsync(x => x.OwnerId == c.OwnerId);
            if (count >= u.CarQuota) throw new Exception($"الحد الأقصى لسياراتك هو ({u.CarQuota}).");

            await Cars.InsertOneAsync(c);
        }

        public async Task EditCar(Car c)
        {
            await Cars.ReplaceOneAsync(x => x.Id == c.Id, c);
        }

        public async Task DelCar(string id)
        {
            await Cars.DeleteOneAsync(c => c.Id == id);
        }
    }
}