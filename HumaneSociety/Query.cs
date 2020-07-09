﻿using System;

namespace HumaneSociety
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Runtime.InteropServices.ComTypes;

    public static class Query
    {
        internal static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;

            // submit changes
            db.SubmitChanges();
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }

        //// TODO Items: ////

        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation.ToLower())
            {
                case "add":
                    AddNewEmployee(employee);
                    break;
                case "update":
                    UpdateEmployee(employee);
                    break;
                case "read":
                    GetEmployee(employee);
                    break;
                case "remove":
                    RemoveEmployee(employee);
                    break;
                default:
                    UserInterface.DisplayUserOptions("Input not recognized please try again.");
                    break;
            }
            throw new NotImplementedException();
        }

        internal static void AddNewEmployee(Employee employee)
        {

            db.Employees.InsertOnSubmit(employee);

            db.SubmitChanges();
        }

        internal static void UpdateEmployee(Employee employeeWithUpdates)
        {
            // find corresponding Client from Db
            Employee employeeFromDb = null;

            try
            {
                employeeFromDb = db.Employees.Where(e => e.EmployeeId == employeeWithUpdates.EmployeeId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No employees have a EmployeeId that matches the Employee passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            employeeFromDb.FirstName = employeeWithUpdates.FirstName;
            employeeFromDb.LastName = employeeWithUpdates.LastName;
            employeeFromDb.UserName = employeeWithUpdates.UserName;
            employeeFromDb.Password = employeeWithUpdates.Password;
            employeeFromDb.EmployeeNumber = employeeWithUpdates.EmployeeNumber;
            employeeFromDb.Email = employeeWithUpdates.Email;


            // submit changes
            db.SubmitChanges();
        }

        internal static Employee GetEmployee(Employee employee)
        {
            try
            {
                employee = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).Single();

            }
            catch (Exception)
            {
                Console.WriteLine("No employees found.");
                throw;
            }

            return employee;
        }

        internal static void RemoveEmployee(Employee employee)
        {
            try
            {
                employee = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No employees have a EmployeeId that matches the Employee passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            db.Employees.DeleteOnSubmit(employee);
            db.SubmitChanges();
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal foundAnimal;
            try
            {
                foundAnimal = db.Animals.Where(i => i.AnimalId == id).Single();
                return foundAnimal;
            }
            catch (Exception)
            {
                Console.WriteLine("No animal found.");
                throw;
            }
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            var query = (from animal in db.Animals
                         where animal.AnimalId == animalId
                         select animal).Single();

            foreach (var update in updates)
            {
                switch (update.Key)
                {
                    case 1:
                        query.CategoryId = GetCategoryId(update.Value);
                        break;
                    case 2:
                        query.Name = update.Value;
                        break;
                    case 3:
                        query.Age = Int32.Parse(update.Value);
                        break;
                    case 4:
                        query.Demeanor = update.Value;
                        break;
                    case 5:
                        query.KidFriendly = bool.Parse(update.Value);
                        break;
                    case 6:
                        query.PetFriendly = bool.Parse(update.Value);
                        break;
                    case 7:
                        query.Weight = Int32.Parse(update.Value);
                        break;
                   

                }
            }
            db.SubmitChanges();
        }

        /*1. Category", "2. Name", "3. Age", "4. Demeanor", "5. Kid friendly", 
         * "6. Pet friendly", "7. Weight", "8. Finished*/
        internal static void RemoveAnimal(Animal animal)
        {
            try
            {
                animal = db.Animals.Where(a => a.AnimalId == animal.AnimalId).Single();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("No animals have a AnimalId that matches the Animal passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }

            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }

        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> searchedAnimals = db.Animals;
            
            foreach (var update in updates)
            {
                    switch (update.Key)
                {
                    case 1:
                        var categoryId = GetCategoryId(update.Value);
                        searchedAnimals = searchedAnimals.Where(a => a.CategoryId == categoryId);
                        break;
                    case 2:
                        searchedAnimals = searchedAnimals.Where(a => a.Name == update.Value);
                        break;
                    case 3:
                        searchedAnimals = searchedAnimals.Where(a => a.Age == Int32.Parse(update.Value));
                        break;
                    case 4:
                        searchedAnimals = searchedAnimals.Where(a => a.Demeanor == update.Value);
                        break;
                    case 5:
                        searchedAnimals = searchedAnimals.Where(a => a.KidFriendly == bool.Parse(update.Value));
                        break;
                    case 6:
                        searchedAnimals = searchedAnimals.Where(a => a.PetFriendly == bool.Parse(update.Value));
                        break;
                    case 7:
                        searchedAnimals = searchedAnimals.Where(a => a.Weight == Int32.Parse(update.Value));
                        break;
                    case 8:
                        searchedAnimals = searchedAnimals.Where(a => a.AnimalId == Int32.Parse(update.Value));
                        break;
                    }
            }
            return searchedAnimals;
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            var categoryID = db.Categories.Where(c => c.Name == categoryName).FirstOrDefault();
            return categoryID.CategoryId;
        }

        internal static Room GetRoom(int animalID)
        {
            var roomId = db.Rooms.Where(r => r.AnimalId == animalID).FirstOrDefault();
            return roomId;
        }

        internal static int GetDietPlanId(string dietPlanName)
        {
            var dietPlanId = db.DietPlans.Where(d => d.Name == dietPlanName).FirstOrDefault();
            return dietPlanId.DietPlanId;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            var adoption = new Adoption();
            adoption.AnimalId = animal.AnimalId;
            adoption.ClientId = client.ClientId;
            adoption.ApprovalStatus = "Pending";
            adoption.AdoptionFee = 75;
            adoption.PaymentCollected = true;
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            IQueryable<Adoption> adoption = db.Adoptions.Where(a => a.ApprovalStatus == "Pending");
            return adoption;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            var foundAdoptions = db.Adoptions.Where(a => a.AnimalId == adoption.AnimalId && a.ClientId == adoption.ClientId).Single();
            var updateAnimal = db.Animals.Where(x => x.AnimalId == adoption.AnimalId).Single();
            var updateRoom = db.Rooms.Where(r => r.AnimalId == adoption.AnimalId).Single();
            if (isAdopted)
            {
                foundAdoptions.ApprovalStatus = "Accepted";
                updateAnimal.AdoptionStatus = "Adopted";
                updateRoom.AnimalId = null;
            }
            else
            {
                foundAdoptions.ApprovalStatus = "Denied";
            }
            db.SubmitChanges();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            var  foundAdoptions = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).Single();
            db.Adoptions.DeleteOnSubmit(foundAdoptions);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            IQueryable<AnimalShot> animalShot = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId);
            return animalShot;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            var foundShot = db.Shots.Where(s => s.Name == shotName).FirstOrDefault();
            var newAnimalShot = new AnimalShot();

            newAnimalShot.AnimalId = animal.AnimalId;
            newAnimalShot.ShotId = foundShot.ShotId;
            newAnimalShot.DateReceived = DateTime.Today;

        }

        internal static void AnimalsFromCSV() //"C:/users/file.csv
        {
            string path = "D:\\Source\\repos\\HumaneSociety\\animals.csv";

            Animal newAnimal = new Animal();
            string[] csv = File.ReadAllLines(path);//"Fido,13,Male,Dog Food,Friendly,True"

            foreach (string item in csv)
            {
                string[] data = item.Split(',');

                newAnimal.Name = data[0];
                newAnimal.Weight = Int32.Parse(data[1]);
                newAnimal.Age = Int32.Parse(data[2]);
                newAnimal.Demeanor = data[3];
                newAnimal.KidFriendly = bool.Parse(data[4]);
                newAnimal.PetFriendly = bool.Parse(data[5]);
                newAnimal.Gender = data[6];
                newAnimal.AdoptionStatus = data[7];
                newAnimal.CategoryId = Int32.Parse(data[8]);
                newAnimal.DietPlanId = Int32.Parse(data[9]);
                newAnimal.EmployeeId = Int32.Parse(data[10]);
                            

            db.Animals.InsertOnSubmit(newAnimal);
            db.SubmitChanges();
            }

       }
        
    
    }
}



//var query = from line in csv
//            let data = line.Split(',')
//            select new
//            {
//                newAnimal.Name = data[0],
//                Weight = data[1],
//                Age = data[2],
//                Demeanor = data[3],
//                KidFriendly = data[4],
//                PetFriendly = data[5],
//                Gender = data[6],
//                AdoptionStatus = data[7],
//                CategoryId = data[8],
//                DietPlan = data[9],
//                EmployeeId = data[10]
//            };





//string[] csvlines = File.ReadAllLines(Filname);
//var query = from csvline in csvlines
//            let data = csvline.Split(',')
//            select new
//            {
//                ID = data[0],
//                FirstNumber = data[1],
//                SecondNumber = data[2],
//                ThirdNumber = data[3]
//            };
