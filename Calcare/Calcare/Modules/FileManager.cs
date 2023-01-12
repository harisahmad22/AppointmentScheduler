/*
    The goal of this class is to be the only class that knows about the file system.
    It'll handle all the "database" operations, like creating, deleting, moving, etc.

    The Alberta Health Care number is the primary key for the "database".
*/
using Newtonsoft.Json.Linq;

namespace Calcare.Services
{
    public class FileManager {
        private static Boolean _Initialized = false;
        private static JObject _CalcareDatabase = new JObject(); 
        private static JObject _UserDatabase = new JObject();
        private static JObject _PatientDatabase = new JObject();
        private static JObject _AppointmentDatabase = new JObject();

        private static readonly string _calcareJson = @"
        {
            ""notices"": {

            }
        }";
        private static readonly string _userJson = @"
        {
            ""sarah"": {
                ""name"": ""Dr. Sarah"",
                ""password"": ""password"",
                ""clinic"": ""Best Clinic!"",
                ""email"": ""sarah@bestclinic.com"",
                ""permissions"": ""user"",
                ""shortcuts"": {
                    ""1"": """",
                    ""2"": """",
                    ""3"": """",
                    ""4"": """",
                    ""5"": """",
                    ""6"": """",
                    ""7"": """",
                    ""8"": """",
                    ""9"": """"
                }
            }
        }";
        private static readonly string _patientJson = @"
        {
            ""12345-6789"": {
                ""name"": ""John Doe"",
                ""dob"": ""01/01/1970"",
                ""address"": ""123 Main St, Calgary, AB, T2P 2P2"",
                ""email"": ""johndoe@gmail.com"",
                ""phone"": ""403-456-7890"",
                ""appointments"": {
                    ""2022-12-29"": {
                        ""time"": ""9:00"",
                        ""doctor"": ""Dr. Gerald Jinx Mouse"",
                        ""length"": 30
                    },
                    ""2022-12-31"": {
                        ""time"": ""13:00"",
                        ""doctor"": ""Dr. Gerald Jinx Mouse"",
                        ""length"": 30
                    }
                },
                ""notes"": {}
            },
            ""12345-6788"": {
                ""name"": ""Jane Doe"",
                ""dob"": ""01/01/1970"",
                ""address"": ""321 Main St, Calgary, AB, T2P 2P2"",
                ""email"": ""johndoe@gmail.com"",
                ""phone"": ""403-456-7899"",
                ""appointments"": {
                    ""2022-12-31"": {
                        ""time"": ""13:30"",
                        ""doctor"": ""Dr. Gerald Jinx Mouse"",
                        ""length"": 30
                    }
                },
                ""notes"": {}
            }
        }";
        private static readonly string _appointmentJson = @"
        {
            ""2022-12-29"": {
                ""9:00"": {
                    ""doctor"": ""Dr. Gerald Jinx Mouse"",
                    ""ahn"": ""12345-6789"",
                    ""length"": 30
                }
            },
            ""2022-12-31"": {
                ""13:00"": {
                    ""doctor"": ""Dr. Gerald Jinx Mouse"",
                    ""ahn"": ""12345-6789"",
                    ""length"": 30
                },
                ""13:30"": {
                    ""doctor"": ""Dr. Gerald Jinx Mouse"",
                    ""ahn"": ""12345-6788"",
                    ""length"": 30
                }
            }
        }"; 

        public FileManager() {
            if (_Initialized) { return; } // Don't initialize twice.
            _Initialized = true;

            // Print out all the files in the current directory.
            Console.WriteLine("Files in the current directory:");
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory())) {
                // Print out the contents of the file.
                Console.WriteLine("Contents of " + file + ":");
                Console.WriteLine(File.ReadAllText(file));
            }

            // Read the data from the files, and store it in some data structures.
            Console.WriteLine("Preparing to read data from files.");
            _CalcareDatabase = JObject.Parse(_calcareJson);
            _UserDatabase = JObject.Parse(_userJson);
            _PatientDatabase = JObject.Parse(_patientJson);
            _AppointmentDatabase = JObject.Parse(_appointmentJson);
            Console.WriteLine("Data read from files.");
        }

        ~FileManager() {

        }

        public Boolean PatientExists(string AlbertaHealthCareNumber) {
            return _PatientDatabase.ContainsKey(AlbertaHealthCareNumber);
        }

        public string GetPatientName(string AlbertaHealthCareNumber) {
            if (!_PatientDatabase.ContainsKey(AlbertaHealthCareNumber)) { return ""; } // Return an empty struct if the patient doesn't exist.

            return _PatientDatabase[AlbertaHealthCareNumber]?["name"].ToString();
        }

        public PatientData? GetPatientData(string AlbertaHealthCareNumber) {
            if (!_PatientDatabase.ContainsKey(AlbertaHealthCareNumber)) { return null; } // Return an empty struct if the patient doesn't exist.

            // Patient exists, populate the struct with the data. (The keys in the JSON file are the same as the struct's fields.)
            PatientData patientData = new PatientData(); // Create a deep copy of the data, so we don't have to worry about the data being modified.
            patientData.AHN = AlbertaHealthCareNumber;
            patientData.Name = _PatientDatabase[AlbertaHealthCareNumber]?["name"].ToString();
            patientData.DOB = _PatientDatabase[AlbertaHealthCareNumber]?["dob"].ToString();
            patientData.Address = _PatientDatabase[AlbertaHealthCareNumber]?["address"].ToString();
            patientData.Email = _PatientDatabase[AlbertaHealthCareNumber]?["email"].ToString();
            patientData.PhoneNumber = _PatientDatabase[AlbertaHealthCareNumber]?["phone"].ToString();
            
            patientData.Appointments = new List<AppointmentData>(); 
            if (_PatientDatabase[AlbertaHealthCareNumber]?["appointments"] != null) {
                foreach (KeyValuePair<string, JToken?> appointment in _PatientDatabase[AlbertaHealthCareNumber]?["appointments"] as JObject) {
                    AppointmentData appointmentData = new AppointmentData();
                    appointmentData.PatientAHN = AlbertaHealthCareNumber;
                    appointmentData.AppointmentStartTime = DateTime.Parse(appointment.Key + " " + appointment.Value?["time"].ToString());
                    appointmentData.AppointmentEndTime = appointmentData.AppointmentStartTime.AddMinutes((int) appointment.Value?["length"]);
                    appointmentData.Doctor = appointment.Value["doctor"].ToString();
                    appointmentData.Length = (int) appointment.Value["length"];
                    patientData.Appointments.Add(appointmentData);
                }
            }
            
            return patientData;
        }

        public void SetPatientData(string AlbertaHealthCareNumber, PatientData patientData) {
            if (!_PatientDatabase.ContainsKey(AlbertaHealthCareNumber)) {
                _PatientDatabase.Add(AlbertaHealthCareNumber, new JObject());
            }

            // Update the patient's data.
            _PatientDatabase[AlbertaHealthCareNumber]["name"] = patientData.Name;
            _PatientDatabase[AlbertaHealthCareNumber]["dob"] = patientData.DOB;
            _PatientDatabase[AlbertaHealthCareNumber]["address"] = patientData.Address;
            _PatientDatabase[AlbertaHealthCareNumber]["email"] = patientData.Email;
            _PatientDatabase[AlbertaHealthCareNumber]["phone"] = patientData.PhoneNumber;
        }
        
        public void DeletePatientData(string AlbertaHealthCareNumber) {
            if (!_PatientDatabase.ContainsKey(AlbertaHealthCareNumber)) { return; } // Return if the patient doesn't exist.

            // Patient exists, delete them.
            _PatientDatabase.Remove(AlbertaHealthCareNumber);
        }

        public Dictionary<string, JToken> GetAllAppointments() {
            Dictionary<string, JToken> appointments = new Dictionary<string, JToken>();

            // Loop through the _AppointmentDatabase, and add each appointment to the list.
            foreach (KeyValuePair<string, JToken?> appointment in _AppointmentDatabase) {
                Console.WriteLine($"Key: {appointment.Key} Value: {appointment.Value}"); // NOTE: Going to leave this in, just to show that appointments are being added to the list.
                
                // Stick the appointment.Value into appointments with appointment.Key being the key.
                appointments.Add(appointment.Key, appointment.Value);
            }

            return appointments;
        }

        public Boolean BookAppointment(AppointmentData appointmentData) {
            if (!_PatientDatabase.ContainsKey(appointmentData.PatientAHN)) { return false; } // Return if the patient doesn't exist.
            if (appointmentData.AppointmentEndTime.Hour >= 17 && appointmentData.AppointmentEndTime.Minute > 0) { return false; } // Return if it's out of working hours. (5pm)

            // Keys
            string appointmentDate = appointmentData.AppointmentStartTime.ToString("yyyy-MM-dd");
            string appointmentTime = appointmentData.AppointmentStartTime.ToString("HH:mm");

            // Patient exists, create the appointment for the specified date.
            _AppointmentDatabase[appointmentDate] = _AppointmentDatabase[appointmentDate] ?? new JObject();
            if (_AppointmentDatabase[appointmentDate][appointmentTime] != null) { return false; } 
            for (int i = 0; i < (appointmentData.Length / 15); i++)
            {
                string time = appointmentData.AppointmentStartTime.AddMinutes(i * 15).ToString("HH:mm");
                if (_AppointmentDatabase[appointmentDate][time] != null) { return false; }
            } 

            _AppointmentDatabase[appointmentDate][appointmentTime] = new JObject();
            _AppointmentDatabase[appointmentDate][appointmentTime]["doctor"] = appointmentData.Doctor;
            _AppointmentDatabase[appointmentDate][appointmentTime]["ahn"] = appointmentData.PatientAHN;
            _AppointmentDatabase[appointmentDate][appointmentTime]["length"] = appointmentData.Length;

            // Sort the appointments by the date. (So they're in chronological order within _AppointmentDatabase.)
            var sortedAppointments = JObject.Parse(_AppointmentDatabase.ToString()).Children().Cast<JProperty>().OrderBy(jp => jp.Name);
            _AppointmentDatabase = new JObject(sortedAppointments.ToArray());

            // Stick the appointment into the patient's data.
            _PatientDatabase[appointmentData.PatientAHN]["appointments"] = _PatientDatabase[appointmentData.PatientAHN]["appointments"] ?? new JObject();
            _PatientDatabase[appointmentData.PatientAHN]["appointments"][appointmentDate] = _PatientDatabase[appointmentData.PatientAHN]["appointments"][appointmentDate] ?? new JObject();
            _PatientDatabase[appointmentData.PatientAHN]["appointments"][appointmentDate]["time"] = appointmentData.AppointmentStartTime.ToString("HH:mm");
            _PatientDatabase[appointmentData.PatientAHN]["appointments"][appointmentDate]["doctor"] = appointmentData.Doctor;
            _PatientDatabase[appointmentData.PatientAHN]["appointments"][appointmentDate]["length"] = appointmentData.Length;

            // Sort the patient's appointments by the date. (So they're in chronological order within _PatientDatabase.)
            var sortedPatientAppointments = JObject.Parse(_PatientDatabase[appointmentData.PatientAHN]["appointments"].ToString()).Children().Cast<JProperty>().OrderBy(jp => jp.Name);
            _PatientDatabase[appointmentData.PatientAHN]["appointments"] = new JObject(sortedPatientAppointments.ToArray());

            return true;
        }

        public Boolean DeleteAppointment(string date, string time, string AlbertaHealthCareNumber) {
            if (!_PatientDatabase.ContainsKey(AlbertaHealthCareNumber)) { return false; } // Return if the patient doesn't exist.
            if (_AppointmentDatabase[date][time] == null) { return false;  }

            // Delete the JObject from _AppointmentDatabase for the appointment.
            _AppointmentDatabase[date].Children().Cast<JProperty>().Where(jp => jp.Name == time).ToList().ForEach(jp => jp.Remove());

            // Sort the appointments by the date. (So they're in chronological order within _AppointmentDatabase.)
            var sortedAppointments = JObject.Parse(_AppointmentDatabase.ToString()).Children().Cast<JProperty>().OrderBy(jp => jp.Name);
            _AppointmentDatabase = new JObject(sortedAppointments.ToArray());

            // Remove the appointment.
            _PatientDatabase[AlbertaHealthCareNumber]["appointments"].Children().Cast<JProperty>().Where(jp => jp.Name == date).ToList().ForEach(jp => jp.Remove());

            // Sort the patient's appointments by the date. (So they're in chronological order within _PatientDatabase.)
            var sortedPatientAppointments = JObject.Parse(_PatientDatabase[AlbertaHealthCareNumber]["appointments"].ToString()).Children().Cast<JProperty>().OrderBy(jp => jp.Name);
            _PatientDatabase[AlbertaHealthCareNumber]["appointments"] = new JObject(sortedPatientAppointments.ToArray());

            return true;
        }

        public Boolean StaffExists(string id)
        {
            return _UserDatabase.ContainsKey(id);
        }

        public StaffData GetStaffData(string id)
        {
            if (!_UserDatabase.ContainsKey(id)) { return null; } // Return an empty struct if the patient doesn't exist.

            // Patient exists, populate the struct with the data. (The keys in the JSON file are the same as the struct's fields.)
            StaffData staffdata = new StaffData(); // Create a deep copy of the data, so we don't have to worry about the data being modified.
            staffdata.Userid = id;
            staffdata.Name = _UserDatabase[id]?["name"].ToString();
            staffdata.Email = _UserDatabase[id]?["email"].ToString();
            staffdata.Clinic = _UserDatabase[id]?["clinic"].ToString();
            staffdata.Password = _UserDatabase[id]?["password"].ToString();
            return staffdata;
        }

        public void SetStaffData(string id, StaffData staffdata)
        {
            // TODO: Figure out if this is the best approach.
            // Create the patient if they don't exist.
            if (!_UserDatabase.ContainsKey(id))
            {
                _UserDatabase.Add(id, new JObject());
            }

            // Update the patient's data.
            _UserDatabase[id]["name"] = staffdata.Name;
            _UserDatabase[id]["email"] = staffdata.Email;
            _UserDatabase[id]["password"] = staffdata.Password;
            _UserDatabase[id]["clinic"] = staffdata.Clinic;          
        }
    }
}