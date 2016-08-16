﻿//
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
using System.Runtime.Serialization;

namespace GraduateEntity {

	public class Workplace {

		public int WorkplaceID { get; set; }
		public int PersonID { get; set; }
		public string OrganizationName { get; set; }
		public int DepartmentID { get; set; }
		public int TitleID { get; set; }
		public int CityID { get; set; }
		public int CountryID { get; set; }
        public int IndustryID { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }

        public virtual Title Title { get; set; }
        public virtual Department Department { get; set; }
        public virtual Industry Industry { get; set; }
        public virtual Country Country { get; set; }
        public virtual City City { get; set; }
	}
}
