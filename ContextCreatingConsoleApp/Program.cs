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
using GraduateEntity;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace ContextCreatingConsoleApp {
	class MainClass {
		public static void Main(string[] args) {
            GraduateDbContext dbContext = new GraduateDbContext();

            string[] arg = { "Fatih", "Şen" };

            string firstName = arg[0];
            string lastName = arg[1];
            List<Person> people = dbContext.People.Where(p => (p.FirstName.Contains(firstName) || (p.MiddleName != null && p.MiddleName.Contains(firstName))) && p.LastName.Contains(lastName)).ToList();


            Console.WriteLine("Press a Key to Exit!...");
            Console.ReadKey();
		}

        public static string hash(string str) {
            using(MD5 md5 = MD5.Create()) {
                byte[] sink = Encoding.UTF8.GetBytes(str);
                byte[] data = md5.ComputeHash(sink);
                StringBuilder strBuilder = new StringBuilder();
                data.ToList()
                    .ForEach(b => strBuilder.Append(b.ToString("X2")));
                return strBuilder.ToString();
            }
        }
	}
}
