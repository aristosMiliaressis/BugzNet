using BugzNet.Core.SharedKernel;

namespace BugzNet.Core.ValueObjects
{
    public class Bug : ValueObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool CanFly { get; set; }
        public bool Bites { get; set; }
        public string Picture { get; set; }

        public Bug(string name, string desc, bool flies, bool bites, string pic)
        {
            Name = name;
            Description = desc;
            CanFly = flies;
            Bites = bites;
            Picture = pic;
        }

        public Bug() { }

        public static Bug CreateNew(Bug bug)
        {
            return new Bug(bug.Name, bug.Description, bug.CanFly, bug.Bites, bug.Picture);
        }
    }
}
