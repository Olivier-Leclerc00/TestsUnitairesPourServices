using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsUnitairesPourServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestsUnitairesPourServices.Data;
using TestsUnitairesPourServices.Models;
using System.Net;

namespace TestsUnitairesPourServices.Services.Tests
{
    [TestClass()]
    public class CatsServiceTests
    {
        DbContextOptions<ApplicationDBContext> options;

        public const int FIRST_HOUSE = 1;
        public const int SECOND_HOUSE = 2;
        public const int FIRST_CAT = 1;
        public const int SECOND_CAT = 2;

        public CatsServiceTests()
        {
            // TODO On initialise les options de la BD, on utilise une InMemoryDatabase
            options = new DbContextOptionsBuilder<ApplicationDBContext>()
                // TODO il faut installer la dépendance Microsoft.EntityFrameworkCore.InMemory
                .UseInMemoryDatabase(databaseName: "CardsService")
                .UseLazyLoadingProxies(true) // Active le lazy loading
                .Options;
        }
        [TestInitialize]
        public void Init()
        {
            using ApplicationDBContext db = new ApplicationDBContext(options);

            House[] houses = new House[]
            {
                new House
                {
                    Id = 1,
                    Address = "nowhere",
                    OwnerName = "George",
                    Cats = []
                },
                new House
                {
                    Id = 2,
                    Address = "nowhere",
                    OwnerName = "James",
                    Cats = []
                }
            };


            Cat[] cats = new Cat[]
            {
                new Cat
                {
                    Id = 1,
                    Name = "Brad",
                    Age = 6,
                    HouseId = FIRST_HOUSE
                },
                new Cat
                {
                    Id = 2,
                    Name = "Alex",
                    Age = 8
                }
            };

            db.AddRange(houses);
            db.AddRange(cats);
            db.SaveChanges();
        }
        [TestCleanup]
        public void Dispose()
        {
            //TODO on efface les données de tests pour remettre la BD dans son état initial
            using ApplicationDBContext db = new ApplicationDBContext(options);
            db.RemoveRange(db.Cat);
            db.RemoveRange(db.House);
            db.SaveChanges();
        }

        [TestMethod()]
        public void MoveValidCat()
        {
            using ApplicationDBContext db = new ApplicationDBContext(options);
            CatsService _catService = new CatsService(db);

            House fisrtHouse = db.House.FirstOrDefault(h => h.Id == FIRST_HOUSE);
            House secondHouse = db.House.FirstOrDefault(h => h.Id == SECOND_HOUSE);

            var resultat = _catService.Move(FIRST_CAT, fisrtHouse, secondHouse);

            Assert.AreNotEqual(null, resultat);
        }

        [TestMethod()]
        public void MoveInvalidCat()
        {
            using ApplicationDBContext db = new ApplicationDBContext(options);
            CatsService _catService = new CatsService(db);

            House fisrtHouse = db.House.FirstOrDefault(h => h.Id == FIRST_HOUSE);
            House secondHouse = db.House.FirstOrDefault(h => h.Id == SECOND_HOUSE);

            var resultat = _catService.Move(3, fisrtHouse, secondHouse);

            Assert.Equals(null, resultat);
        }

        [TestMethod()]
        public void MoveInvalidFirstHouse()
        {
            using ApplicationDBContext db = new ApplicationDBContext(options);
            CatsService _catService = new CatsService(db);

            House fisrtHouse = db.House.FirstOrDefault(h => h.Id == FIRST_HOUSE);
            House secondHouse = db.House.FirstOrDefault(h => h.Id == SECOND_HOUSE);

            var resultat = _catService.Move(3, fisrtHouse, secondHouse);

            Assert.Equals(null, resultat);
        }

        [TestMethod()]
        public void MoveInvalidSecondHouse()
        {
            using ApplicationDBContext db = new ApplicationDBContext(options);
            CatsService _catService = new CatsService(db);

            House fisrtHouse = db.House.FirstOrDefault(h => h.Id == FIRST_HOUSE);
            House secondHouse = db.House.FirstOrDefault(h => h.Id == 3);

            var resultat = _catService.Move(3, fisrtHouse, secondHouse);

            Assert.Equals(null, resultat);
        }
    }
}