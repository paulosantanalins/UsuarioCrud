namespace Cadastro.Domain.PrestadorRoot.Dto
{
    public enum EmpresasGrupoEnum
    {
        Sunrising = 13,
        Callere = 11,
        OrbitallAtendimento = 20,
        OrbitallServicos = 17
    }

    public static class EmpresaGrupoTransferencia
    {
        public static bool VerificarEmpresaNaoPodeTransferir(this int idEmpresaGrupo)
        {
            return idEmpresaGrupo == EmpresasGrupoEnum.Sunrising.GetHashCode() ||
                   idEmpresaGrupo == EmpresasGrupoEnum.Callere.GetHashCode() ||
                   idEmpresaGrupo == EmpresasGrupoEnum.OrbitallAtendimento.GetHashCode() ||
                   idEmpresaGrupo == EmpresasGrupoEnum.OrbitallServicos.GetHashCode();
        }
    }
}
