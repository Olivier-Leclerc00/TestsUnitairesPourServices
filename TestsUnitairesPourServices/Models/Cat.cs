using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace TestsUnitairesPourServices.Models
{
	public class Cat
	{
		public Cat()
		{
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public int Age { get; set; }

		[ForeignKey("House")]
		public int HouseId { get; set; }
        public virtual House House { get; set; }
	}
}

