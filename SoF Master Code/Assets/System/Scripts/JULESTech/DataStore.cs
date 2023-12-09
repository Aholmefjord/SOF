using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace JULESTech
{
    public sealed class DataStore
    {
        /// <summary>
        /// Singleton Instance
        /// </summary>
        private static DataStore instance = null;

        public DataMap gameCommandData = new DataMap(); //Lesson plan contains every information about the whole school lessons, including the link to each lesson data file.
        //public DataMap sofLessonPlanData = new DataMap(); //Sof Lesson Plan contains only information related to SOF
        public DataMap lessonData = new DataMap(); //Contains command for lesson detail
        //public DataMap schoolData = new DataMap();
		public DataMap cloudData = new DataMap(); // This is for all server-related stuff

        /// <summary>
        /// Singleton Getter
        /// </summary>
        public static DataStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataStore();
                }
                return instance;
            }
        }
    
	    /// <summary>
	    /// Private deconstructor
	    /// </summary>
	    private DataStore()
        {
        }
    }
}
