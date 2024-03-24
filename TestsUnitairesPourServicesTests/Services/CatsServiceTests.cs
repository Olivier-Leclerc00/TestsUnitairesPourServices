using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsUnitairesPourServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestsUnitairesPourServices.Data;
using TestsUnitairesPourServices.Exceptions;
using TestsUnitairesPourServices.Models;

namespace TestsUnitairesPourServices.Services.Tests
{
    [TestClass()]
    public class CatsServiceTests
    {
        DbContextOptions<ApplicationDBContext> options;

        public CatsServiceTests()
        {
            options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "CatsService")
                .Options;
        }

        [TestInitialize]
        public void Init()
        {
            // TODO avoir la durée de vie d'un context la plus petite possible
            using ApplicationDBContext db = new ApplicationDBContext(options);

            db.Cat.Add(new Cat()
            {
                Id = 1,
                Name = "Lonely",
                Age = 12
            });

            House maisonPropre = new House()
            {
                Id = 1,
                Address = "Tite maison propre et orange",
                OwnerName = "Ludwig"
            };

            House maisonSale = new House()
            {
                Id = 2,
                Address = "Grosse maison sale",
                OwnerName = "Bob"
            };

            db.House.Add(maisonPropre);
            db.House.Add(maisonSale);

            Cat chatPasPropre = new Cat()
            {
                Id = 2,
                Name = "ToutSale",
                Age = 3,
                House = maisonSale
            };
            db.Cat.Add(chatPasPropre);
            db.SaveChanges();
        }

        [TestCleanup]
        public void Dispose()
        {
            using ApplicationDBContext db = new ApplicationDBContext(options);
            db.Cat.RemoveRange(db.Cat);
            db.House.RemoveRange(db.House);
            db.SaveChanges();
        }

        [TestMethod()]
        public void MoveTest()
        {
            using ApplicationDBContext db = new ApplicationDBContext(options);
            var catsService = new CatsService(db);
            var maisonPropre = db.House.Find(1)!;
            var maisonSale = db.House.Find(2)!;

            // Tout est bon, le chat va être dans une maison propre
            var chatMaintenantPropre = catsService.Move(2, maisonSale, maisonPropre);
            Assert.IsNotNull(chatMaintenantPropre);
        }

        [TestMethod()]
        public void MoveTestCatNotFound()
        {
            using ApplicationDBContext db = new ApplicationDBContext(options);
            var catsService = new CatsService(db);
            var maisonPropre = db.House.Find(1)!;
            var maisonSale = db.House.Find(2)!;

            //Retourne null si le chat ne peut pas être trouvé (aucun chat avec Id: 42)
            var cat = catsService.Move(42, maisonSale, maisonPropre);
            Assert.IsNull(cat);
        }

        [TestMethod()]
        public void MoveTestNoHouse()
        {
            using ApplicationDBContext db = new ApplicationDBContext(options);
            var catsService = new CatsService(db);
            var maisonPropre = db.House.Find(1)!;
            var maisonSale = db.House.Find(2)!;

            //Le chat avec l'Id 1 n'a pas de maison
            Exception e = Assert.ThrowsException<WildCatException>(() => catsService.Move(1, maisonSale, maisonPropre));
            Assert.AreEqual("On n'apprivoise pas les chats sauvages", e.Message);
        }

        [TestMethod()]
        public void MoveTestWrongHouse()
        {
            using ApplicationDBContext db = new ApplicationDBContext(options);
            var catsService = new CatsService(db);
            var maisonPropre = db.House.Find(1)!;
            var maisonSale = db.House.Find(2)!;

            // Les maisons sont inversées
            Exception e = Assert.ThrowsException<DontStealMyCatException>(() => catsService.Move(2, maisonPropre, maisonSale));
            Assert.AreEqual("Touche pas à mon chat!", e.Message);
        }
    }
}