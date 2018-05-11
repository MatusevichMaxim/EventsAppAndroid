using System;
using EventApp.Helpers;

namespace EventApp.Model
{
    public class ManagerModel
    {
        public int ManagerId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public ManagersStatus Status { get; set; }
    }
}
