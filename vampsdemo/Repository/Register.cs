using Antlr.Runtime.Tree;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using vampsdemo.Models;
using vampsdemo.Models.Data_model;

namespace vampsdemo.Repository
{
    public class Register : IRegister
    {
        private neonmamEntities _vamps;
        public Register(neonmamEntities vamps)
        {
            this._vamps = vamps;
        }
        public IEnumerable<user_details> GetUserDetails()
        {
            return _vamps.user_details.ToList();
        }
        public user_details GetUserId(int id)
        {
            return _vamps.user_details.Find(id);
        }
        public void Insert(user_details user)
        {
            _vamps.user_details.Add(user);

        }
        public void AddUserToRole(user_role role)
        {
            _vamps.user_role.Add(role);
        }
        public void Updateuser(user_details user)
        {
            if (user.user_id > 0)
            {
                _vamps.Entry(user).State = EntityState.Modified;
            }
        }
        public user_details validUser(string email, string password)
        {
            var validate = /*_vamps.user_details.Where(x => x.email.Equals(email) &&
                                                     x.password.Equals(password)).ToList();*/
                (from m in _vamps.user_details
                 where
                m.email == email && m.password == password
                 select m).FirstOrDefault();
            return validate;
        }
        public string roles()
        {
            var getroles = (from user in _vamps.user_details
                            join roll in _vamps.user_role on user.user_id
                            equals roll.user_id
                            join mrole in _vamps.role_master
                            on roll.role_master_id equals mrole.role_master_id
                            where user.email == user.email
                            select mrole.role_type).SingleOrDefault();
            return getroles;
        }
        public void Delete(int id)
        {
            user_details user = _vamps.user_details.Find(id);
            _vamps.user_details.Remove(user);
        }
        public void Deleterole(int id)
        {
            user_role role = _vamps.user_role.SingleOrDefault(m => m.user_id == id);
            _vamps.user_role.Remove(role);
        }
        public void save()
        {
            _vamps.SaveChanges();
        }
    }
}