using System.ComponentModel;

namespace NetCoreApi.Crosscutting.Enums
{
    public enum FamilyRoles
    {
        [Description("Father")]
        Father = 1,

        [Description("Mother")]
        Mother = 2,

        [Description("Son")]
        Son = 3,

        [Description("Daughter")]
        Daughter = 4
    }
}
