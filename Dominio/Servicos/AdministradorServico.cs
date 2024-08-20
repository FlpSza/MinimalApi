using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimalApi.Dominio.Entidades;
using minimalApi.Dominio.Interfaces;
using minimalApi.Infraestrutura.Db;

namespace minimalApi.Dominio.Servicos
{
    public class AdministradorServico : IAdministradorServicos
    {
        private readonly DbContexto _contexto;
        public AdministradorServico(DbContexto contexto)
        {
            _contexto = contexto;
        }
        public Administrador Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
            return adm;
        }

        public object Login(DTOs.VeiculoDTO veiculoDTO)
        {
            throw new NotImplementedException();
        }
    }
}