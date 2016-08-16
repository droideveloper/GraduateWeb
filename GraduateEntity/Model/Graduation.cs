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

namespace GraduateEntity {

	public class Graduation {

		public int GraduationID { get; set; }
		public int EducationID { get; set; }
		public int UniversityID { get; set; }
		public int GraduationTypeID { get; set; }
		public int GraduateYear { get; set; }
		public int CityID { get; set; }
		public int CountryID { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }

		//FK_Graduation_University
		public virtual University University { get; set; }
        //FK_Graduation_GraduationType
        public virtual GraduationType GraduationType { get; set; }
		public virtual City City { get; set; }
		public virtual Country Country { get; set; }
	}
}

