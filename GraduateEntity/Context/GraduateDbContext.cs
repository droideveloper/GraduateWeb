//
//  Copyright 2016  
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
//using MySql.Data.Entity;
using System.Data.Entity.Spatial;

namespace GraduateEntity {

	//[DbConfigurationType(typeof(MySqlEFConfiguration))]
	public class GraduateDbContext : DbContext {

		private const string STR_CONNECTION = "name=GraduateEntityContext";

		public GraduateDbContext() : base(STR_CONNECTION) { }

		public DbSet<User> Users { get; set; }
		public DbSet<Person> People { get; set; }
		public DbSet<Graduation> Graduations { get; set; }
		public DbSet<Education> Educations { get; set; }
		public DbSet<Session> Sessions { get; set; }
		public DbSet<Workplace> Workplaces { get; set; }
		public DbSet<University> Universities { get; set; }
		public DbSet<Social> Socials { get; set; }
		public DbSet<Exchange> Exchanges { get; set; }
		public DbSet<Cap> Caps { get; set; }

		//work related 
		public DbSet<Industry> Industries { get; set; }
		public DbSet<Title> Titles { get; set; }
		public DbSet<Department> Departments { get; set; }
		//education related
		public DbSet<GraduationType> GraduationTypes { get; set; }
		//city and country
		public DbSet<Country> Countries { get; set; }
		public DbSet<City> Cities { get; set; }
		//filter
		public DbSet<Filter> Filters { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //will ensure that if exchange delete as entity not try to delete its depandants on City and Country
            //equavilant of FK relation DELETE ON ....
            modelBuilder.Entity<Exchange>()
                        .HasRequired(e => e.City)
                        .WithRequiredDependent()
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Exchange>()
                        .HasRequired(e => e.Country)
                        .WithRequiredDependent()
                        .WillCascadeOnDelete(false);
        }
	}
}

