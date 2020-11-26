/*
Copyright(c) 2020, Stefan Wagner, Aarhus University, Aarhus, Denmark
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of Aarhus University nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.
    * All source code, academic papers, technical respots, web pages or using
      or citing this source code must cite the following papers: 
      1) "The Healthcare Equipment Usage and Context Data (HEUCOD) Standard"
      2) "Healthcare Equipment Usage and Context Data (HEUCOD) Referece Implementation" 
      3) "The OpenCare Framework", all by S. Wagner and J. Miranda.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED.IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenCare.EVODAY
{
    public class Utils
    {

        static readonly string nspace = "OpenCare.EVODAY";


        public static bool IsAdverseEvent(BasicEvent ev)
        {
            if (ev.SensorType != null && ev.SensorType.Contains(".AE.")) 
                return true; 
            else return false;
        }

        /// <summary>
        /// This is the EVODAY hash function that converts an EVODAY type into an int hash code
        /// As a non-mandatory feature, we will enumarate all EVODAT types with this hashcode
        /// which makes it possible to use the short hand notation of the EVODAY event types 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int HashFunction(string s)
        {
            int total = 0;
            char[] c;

            c = s.ToCharArray();

            // Summing up all the ASCII values  
            // of each alphabet in the string 
            for (int k = 0; k <= c.GetUpperBound(0); k++)
                total += (int)c[k];

            total += (int)c[0] * 1000;
                        
            return total;
        }


        /// <summary>
        /// This will convert the event hashcode to the event type.
        /// The use of the event hashcode is not mandatory - but a recommendation in HEUDCOD
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static string ConvertEventHashCodeToEventType(int hash)
        {

            string result = null;

            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                        //where t.IsClass && t.Namespace == nspace
                    where t.IsClass && t.Namespace.Contains(nspace)
                    select t;
            q.ToList().ForEach(t => {
                if (HashFunction(t.Name) == hash) 
                { 
                    result = t.Name; 
                    return; 
                } 
            
            });

            return result;

        }

        /// <summary>
        /// This will convert the event hashcode to the event type.
        /// The use of the event hashcode is not mandatory - but a recommendation in HEUDCOD
        /// </summary>
        /// <returns>A string representation of all the hash and corresponding types</returns>
        public static string CreateListHashOnly()
        {

            string result = null;

            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                        // where t.IsClass && t.Namespace == nspace
                    where t.IsClass && t.Namespace.Contains(nspace)
                    select t;
            
            q.ToList().ForEach(t => {
                result += String.Format("{0}{2}", HashFunction(nspace+"."+t.Name), t.Name, System.Environment.NewLine);
            });

            return result;

        }
        /// <summary>
        /// This will convert the event hashcode to the event type.
        /// The use of the event hashcode is not mandatory - but a recommendation in HEUDCOD
        /// </summary>
        /// <returns>A string representation of all the hash and corresponding types</returns>
        public static string CreateListOfHashcodeAndTypeValuePairs()
        {

            string result = null;

            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                        //where t.IsClass && t.Namespace == nspace
                    where t.IsClass && t.Namespace.Contains(nspace)
                    select t;

            q.ToList().ForEach(t => {
                result += String.Format("{0} {1} {2}", HashFunction(nspace+"."+t.Name), t.Name, System.Environment.NewLine);
            });

            return result;

        }

        /// <summary>
        /// Converts to UNIX time
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns>the datetime as a long in UNIX Epoch format</returns>
        public static long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(datetime - sTime).TotalSeconds;
        }

        /// <summary>
        /// Convert Unix time value to a DateTime object.
        /// </summary>
        /// <param name="unixtime">The Unix time stamp you want to convert to DateTime.</param>
        /// <returns>Returns a DateTime object that represents value of the Unix time.</returns>
        public static DateTime UnixTimeToDateTime(long? unixtime)
        {
            if (unixtime > 9999999999)
                unixtime = unixtime / 1000;
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

            if (unixtime != null)
            {
                dtDateTime = dtDateTime.AddSeconds((long)unixtime).ToLocalTime();
            }
            return dtDateTime;
        }


        public static string GetNamePart(string s)
        {
            return s.Substring(s.LastIndexOf(".") + 1);
            
        }
    }
}
