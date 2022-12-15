using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace vampsdemo.Models.Data_model
{
    public class userdbContect :DbContext
    {
        public userdbContect() : base("neonmamEntities")
        {
        }
        public DbSet<user_details> UserDetail
        {
            get; set;
        }
        public DbSet<user_role> UserRole
        {
            get; set;
        }
        public DbSet<role_master> Rolemaster
        {
            get; set;
        }

    }
}