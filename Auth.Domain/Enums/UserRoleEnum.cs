using System.ComponentModel;

namespace Auth.Domain.Enums;

public enum UserRoleEnum
{
    [Description("Administrador")]
    Administrador = 1,

    [Description("Usuário comum")]
    Comum = 2,

    [Description("Suporte")]
    Suporte = 3
}