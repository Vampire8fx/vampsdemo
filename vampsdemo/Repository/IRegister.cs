using System.Collections.Generic;
using vampsdemo.Models;
using vampsdemo.Models.Data_model;

namespace vampsdemo.Repository
{
    internal interface IRegister
    {
        IEnumerable<user_details> GetUserDetails();
        user_details GetUserId(int id);
        void Insert(user_details user);
        void AddUserToRole(user_role role);
        void Updateuser(user_details user);
        user_details validUser(string email , string password);
        string roles();
        void Delete(int id);
        void Deleterole(int id);     
         void save();
    }
}
