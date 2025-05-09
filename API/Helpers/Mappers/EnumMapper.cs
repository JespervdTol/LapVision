using ContractsRole = Contracts.Enums.UserRole;
using ModelRole = Model.Enums.UserRole;

namespace API.Helpers.Mappers
{
    public static class EnumMapper
    {
        public static ModelRole ToModel(this ContractsRole role)
            => (ModelRole)(int)role;

        public static ContractsRole ToDto(this ModelRole role)
            => (ContractsRole)(int)role;
    }
}